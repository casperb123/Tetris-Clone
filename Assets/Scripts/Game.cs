﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

[Serializable]
public class Game : MonoBehaviour
{
    public static Game Instance;
    public static bool StartingAtLevelZero;
    public static int StartingLevel;
    public static SavedGame SaveGame;
    public static bool SaveGameChanged;

    [Header("Game Settings")]
    public int GridWidth = 10;
    public int GridHeight = 20;
    [SerializeField]
    private Transform nextTetrominoTransform;
    [SerializeField]
    private Transform savedTetrominoTransform;
    [SerializeField]
    private int maxSwaps = 2;
    public int MaxIndividualScore = 100;
    public string Name;

    [Header("Touch Settings")]
    public int TouchSensitivityHorizontal = 8;
    public int TouchSensitivityVertical = 4;

    [Header("UI Settings")]
    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private TextMeshProUGUI levelText;
    [SerializeField]
    private TextMeshProUGUI linesText;
    [SerializeField]
    private TextMeshProUGUI playerText;
    [SerializeField]
    private GameObject playMenu;
    [SerializeField]
    private TMP_InputField nameField;

    [Header("Score Settings")]
    [SerializeField]
    private int scoreOneLine = 40;
    [SerializeField]
    private int scoreTwoLines = 100;
    [SerializeField]
    private int scoreThreeLines = 300;
    [SerializeField]
    private int scoreFourLines = 1200;

    [Header("Audio Settings")]
    [SerializeField]
    private AudioClip clearLineSound;
    [SerializeField]
    private AudioClip clearFourLinesSound;
    [SerializeField]
    private AudioClip buttonClick;

    [Header("Speed Settings")]
    public float FallSpeed = 1;             // The speed at which the tetromino will fall if the down arrow isn't being held down
    public float VerticalSpeed = .1f;       // The speed at which the tetromino will move when the down arrow is held down
    public float HorizontalSpeed = .1f;     // The speed at which the tetromino will move when the left or right arrow is held down
    public float ButtonDownWaitMax = .2f;   // How long to wait before the tetromino recognizes that a button is being held down

    [Header("General Settings")]
    [SerializeField]
    private Dialog dialog;

    [HideInInspector]
    public int CurrentScore;
    [HideInInspector]
    public int CurrentLevel;
    [HideInInspector]
    public int TotalLinesCleared;

    [HideInInspector]
    public List<SavedHighscore> HighScores;
    private int currentSwaps;
    [HideInInspector]
    public bool IsPaused;
    [HideInInspector]
    public bool IsGameOver;

    public Transform[,] Grid;

    private Transform tetrominos;
    private int numberOfRowsThisTurn;
    private AudioSource audioSource;
    private AudioSource buttonAudioSource;

    [HideInInspector]
    public GameObject NextTetromino;
    [HideInInspector]
    public GameObject CurrentTetromino;
    [HideInInspector]
    public GameObject SavedTetromino;
    [HideInInspector]
    public GameObject GhostTetromino = null;
    private Vector2 spawnPosition;

    private bool gameStarted;
    public SavedOptions Options;
    public List<SavedControl> Controls { get; private set; }
    private bool highScoreSaved;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Application.quitting += () => UpdateHighscores();

        audioSource = GetComponents<AudioSource>().FirstOrDefault(x => x.clip.name == "gameloop");
        buttonAudioSource = GetComponents<AudioSource>().FirstOrDefault(x => x.clip.name == "buttonclick");
        Options = SaveSystem.GetOptions();
        GetControls();
        Grid = new Transform[GridWidth, GridHeight];
        spawnPosition = new Vector2(GridWidth / 2, GridHeight);

        CurrentScore = 0;
        CurrentLevel = StartingLevel;
        HighScores = SaveSystem.GetHighscores();

        tetrominos = GameObject.Find("Tetrominos").transform;

        if (SaveGame != null)
        {
            if (Options.BackgroundMusic)
                audioSource.Play();

            CurrentScore = SaveGame.Score;
            TotalLinesCleared = SaveGame.Lines;
            Name = SaveGame.Name;

            foreach (SavedMino savedMino in SaveGame.Minos)
            {
                GameObject mino = Instantiate(GetMino(savedMino.Name), new Vector3(savedMino.PositionX, savedMino.PositionY), Quaternion.identity, tetrominos);
                Grid[savedMino.PositionX, savedMino.PositionY] = mino.transform;
            }

            SpawnTetromino(new Vector2(SaveGame.CurrentTetromino.PositionX, SaveGame.CurrentTetromino.PositionY), SaveGame.CurrentTetromino.RotationZ, SaveGame.CurrentTetromino.Name, SaveGame.NextTetromino.Name);

            if (SaveGame.SavedTetromino != null)
                SaveTetromino((GameObject)Resources.Load(GetTetromino(SaveGame.SavedTetromino.Name)), true);

            Time.timeScale = 1;
        }
        else
        {
            Time.timeScale = 0;
            playMenu.SetActive(true);
            nameField.Select();
        }
    }

    private void Update()
    {
        if (IsPaused)
            return;

        UpdateScore();
        UpdateUI();
        UpdateLevel();
        UpdateSpeed();
        CheckUserInput();
    }

    private string AddCommas(int number)
    {
        return string.Format(CultureInfo.InvariantCulture, "{0:n0}", number);
    }

    /// <summary>
    /// Gets the controls
    /// </summary>
    public void GetControls()
    {
        Controls = SaveSystem.GetControls();
    }

    /// <summary>
    /// Checks the users input
    /// </summary>
    private void CheckUserInput()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            GameObject tempTetromino = GameObject.FindGameObjectWithTag("CurrentTetromino");
            SaveTetromino(tempTetromino);
        }

        if (Input.GetKeyDown(KeyCode.Return) && playMenu.activeSelf && !dialog.IsOpen)
            Play();
    }

    public void Play()
    {
        buttonAudioSource.Play();
        if (string.IsNullOrWhiteSpace(nameField.text))
        {
            dialog.Open(Dialog.DialogType.Ok, "The name can't be empty or whitespace");
            return;
        }
        if (SaveSystem.IsNameTaken(nameField.text))
        {
            dialog.Open(Dialog.DialogType.Ok, $"The name <b>{nameField.text}</b> is already taken");
            return;
        }
        if (nameField.text.Length > 15)
        {
            dialog.Open(Dialog.DialogType.Ok, "The name can't contain more than 15 characters");
            return;
        }

        Name = nameField.text;
        playMenu.SetActive(false);
        Time.timeScale = 1;
        SpawnTetromino();

        if (Options.BackgroundMusic)
            audioSource.Play();
    }

    public void Back()
    {
        buttonAudioSource.Play();
        SceneManager.LoadScene("GameMenu");
    }

    /// <summary>
    /// Updates the current level according to the total lines cleared
    /// </summary>
    private void UpdateLevel()
    {
        if (StartingAtLevelZero || !StartingAtLevelZero && TotalLinesCleared / 10 > StartingLevel)
        {
            if (CurrentLevel <= 9)
                CurrentLevel = TotalLinesCleared / 10;
        }
    }

    /// <summary>
    /// Updates the speed according to the current level
    /// </summary>
    private void UpdateSpeed()
    {
        FallSpeed = 1 - (CurrentLevel * .1f);
    }

    /// <summary>
    /// Updates the UI
    /// </summary>
    private void UpdateUI()
    {
        if (string.IsNullOrWhiteSpace(Name))
            return;

        scoreText.text = AddCommas(CurrentScore);
        levelText.text = CurrentLevel.ToString();
        linesText.text = AddCommas(TotalLinesCleared);
        playerText.text = $"Player: {Name}";
    }

    /// <summary>
    /// Checks how many lines is cleared
    /// </summary>
    public void UpdateScore()
    {
        //if (CurrentScore > HighScore.Score)
        //    scoreText.color = Color.green;

        if (numberOfRowsThisTurn > 0)
        {
            if (numberOfRowsThisTurn == 1)
                ClearedOneLine();
            else if (numberOfRowsThisTurn == 2)
                ClearedTwoLines();
            else if (numberOfRowsThisTurn == 3)
                ClearedThreeLines();
            else if (numberOfRowsThisTurn == 4)
                ClearedFourLines();

            numberOfRowsThisTurn = 0;
        }
    }

    /// <summary>
    /// Updates the score
    /// </summary>
    private void ClearedOneLine()
    {
        CurrentScore += scoreOneLine + (CurrentLevel * 20);
        PlayClearLineSound();
        TotalLinesCleared++;
    }

    /// <summary>
    /// Updates the score
    /// </summary>
    private void ClearedTwoLines()
    {
        CurrentScore += scoreTwoLines + (CurrentLevel * 25);
        PlayClearLineSound();
        TotalLinesCleared += 2;
    }

    /// <summary>
    /// Updates the score
    /// </summary>
    private void ClearedThreeLines()
    {
        CurrentScore += scoreThreeLines + (CurrentLevel * 30);
        PlayClearLineSound();
        TotalLinesCleared += 3;
    }

    /// <summary>
    /// Updates the score
    /// </summary>
    private void ClearedFourLines()
    {
        CurrentScore += scoreFourLines + (CurrentLevel * 40);
        PlayClearFourLinesSound();
        TotalLinesCleared += 4;
    }

    /// <summary>
    /// Updates the current highscore
    /// </summary>
    public void UpdateHighscores()
    {
        if (string.IsNullOrWhiteSpace(Name) || CurrentScore == 0 || highScoreSaved || SaveGame is null)
            return;

        if (HighScores.Count == 0)
        {
            HighScores.Add(new SavedHighscore(Name, CurrentScore, SaveGame.Slot));
            highScoreSaved = true;
        }

        if (!highScoreSaved)
        {
            for (int i = 0; i < HighScores.Count; i++)
            {
                if (CurrentScore > HighScores[i].Score)
                {
                    SavedHighscore highscore = HighScores.FirstOrDefault(x => x.SaveSlot == SaveGame.Slot && x.Name == SaveGame.Name);

                    if (highscore != null)
                        highscore.Score = CurrentScore;
                    else
                    {
                        highscore = new SavedHighscore(Name, CurrentScore, SaveGame.Slot);
                        HighScores.Insert(i, highscore);
                        highScoreSaved = true;
                    }

                    break;
                }
            }

        }

        SaveSystem.SaveHighscores(HighScores);
    }

    /// <summary>
    /// Plays the clear line sound
    /// </summary>
    private void PlayClearLineSound()
    {
        if (Options.SoundEffects)
            audioSource.PlayOneShot(clearLineSound);
    }

    /// <summary>
    /// Plays the clear 4 lines sound
    /// </summary>
    private void PlayClearFourLinesSound()
    {
        if (Options.SoundEffects)
            audioSource.PlayOneShot(clearFourLinesSound);
    }

    /// <summary>
    /// Checks if the tetrominos position is valid
    /// </summary>
    /// <param name="tetromino">The tetromino to check</param>
    /// <param name="ignore">The tetromino to ignore</param>
    /// <returns><c>trye</c>, if the tetrominos position is valid, <c>false</c> otherwise</returns>
    public bool CheckIsValidPosition(Transform tetromino, Transform ignore = null, bool checkMinosAbove = false)
    {
        foreach (Transform mino in tetromino)
        {
            Vector2 pos = Round(mino.position);

            if (!CheckIsInsideGrid(pos))
                return false;

            if (ignore is null)
            {
                if (GetTransformAtGridPosition(pos) != null && GetTransformAtGridPosition(pos).parent != tetromino)
                    return false;

                if (checkMinosAbove)
                {
                    for (int y = (int)pos.y + 1; y < GridHeight; y++)
                    {
                        Vector2 upPos = Round(new Vector2(pos.x, y));

                        if (GetTransformAtGridPosition(upPos) != null && GetTransformAtGridPosition(upPos).parent != tetromino)
                            return false;
                    }
                }
            }
            else
            {
                if (GetTransformAtGridPosition(pos) != null && GetTransformAtGridPosition(pos).parent != tetromino && GetTransformAtGridPosition(pos).parent != ignore)
                    return false;

                for (int y = (int)pos.y + 1; y < GridHeight; y++)
                {
                    Vector2 upPos = Round(new Vector2(pos.x, y));

                    if (GetTransformAtGridPosition(upPos) != null && GetTransformAtGridPosition(upPos).parent != tetromino && GetTransformAtGridPosition(upPos).parent != ignore)
                        return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Checks if a tetromino is above the grid
    /// </summary>
    /// <param name="tetromino">The tetromino to check</param>
    /// <returns><c>true</c>, if the tetromino is above the grid, <c>false</c> otherwise</returns>
    public bool CheckIsAboveGrid(Tetromino tetromino)
    {
        for (int x = 0; x < GridWidth; x++)
        {
            foreach (Transform mino in tetromino.transform)
            {
                Vector2 pos = Round(mino.position);

                if (pos.y > GridHeight - 1)
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Checks if a row is full
    /// </summary>
    /// <param name="y">The row to check</param>
    /// <returns><c>true</c>, if the row is full, <c>false</c> otherwise</returns>
    public bool IsFullRowAt(int y)
    {
        // The parameter y, is the row we will iterate over in the grid array to check each x position for a transform
        for (int x = 0; x < GridWidth; x++)
        {
            // If we find a position that returns NULL instead of a transform, then we know that the row is not full
            if (Grid[x, y] == null)
                // So we return false
                return false;
        }

        // Since we found a full row, we increment the full row variable
        numberOfRowsThisTurn++;

        // If we iterated over the entire loop and didn't encounter any NULL positions, then we return true
        return true;
    }

    /// <summary>
    /// Deletes minos at a row
    /// </summary>
    /// <param name="y">The row to delete</param>
    public void DeleteMinosAt(int y)
    {
        for (int x = 0; x < GridWidth; x++)
        {
            Destroy(Grid[x, y].gameObject);
            Grid[x, y] = null;
        }
    }

    /// <summary>
    /// Moves a row down
    /// </summary>
    /// <param name="y">The row to move</param>
    public void MoveRowDown(int y)
    {
        for (int x = 0; x < GridWidth; x++)
        {
            if (Grid[x, y] != null)
            {
                Grid[x, y - 1] = Grid[x, y];
                Grid[x, y] = null;
                Grid[x, y - 1].position += new Vector3(0, -1);
            }
        }
    }

    /// <summary>
    /// Moves all rows down
    /// </summary>
    /// <param name="y">The row to stop at</param>
    public void MoveAllRowsDown(int y)
    {
        for (int i = y; i < GridHeight; i++)
        {
            MoveRowDown(i);
        }
    }

    /// <summary>
    /// Deletes the full rows
    /// </summary>
    public void DeleteRow()
    {
        for (int y = 0; y < GridHeight; y++)
        {
            if (IsFullRowAt(y))
            {
                DeleteMinosAt(y);
                MoveAllRowsDown(y + 1);
                y--;
            }
        }
    }

    /// <summary>
    /// Updates the grid with the tetromino position
    /// </summary>
    /// <param name="tetromino">The tetromino to update</param>
    public void UpdateGrid(Tetromino tetromino)
    {
        for (int y = 0; y < GridHeight; y++)
        {
            for (int x = 0; x < GridWidth; x++)
            {
                if (Grid[x, y] != null)
                {
                    if (Grid[x, y].parent == tetromino.transform)
                    {
                        Grid[x, y] = null;
                    }
                }
            }
        }

        foreach (Transform mino in tetromino.transform)
        {
            Vector2 pos = Round(mino.position);

            if (pos.y < GridHeight)
            {
                Grid[(int)pos.x, (int)pos.y] = mino;
            }
        }
    }

    /// <summary>
    /// Removes the tetromino from the grid
    /// </summary>
    /// <param name="tetromino">The tetromino to remove</param>
    public void RemoveFromGrid(Transform tetromino)
    {
        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 0; y < GridHeight; y++)
            {
                if (Grid[x, y] != null && Grid[x, y].parent == tetromino)
                    Grid[x, y] = null;
            }
        }
    }

    /// <summary>
    /// Gets the transform at a specific position in the grid
    /// </summary>
    /// <param name="pos">The position to get the transform from</param>
    /// <returns><c>Transform</c>, if one exists at the position in the grid, <c>null</c> otherwise</returns>
    public Transform GetTransformAtGridPosition(Vector2 pos)
    {
        if (pos.y > GridHeight - 1)
            return null;

        return Grid[(int)pos.x, (int)pos.y];
    }

    /// <summary>
    /// Checks how many units the tetromino is outside of the grid
    /// </summary>
    /// <param name="tetromino">The tetromino to check</param>
    /// <returns>The number of units the tetromino is outside</returns>
    public Vector3 GetUnitsToMove(Transform tetromino)
    {
        Vector3 positionToReturn = new Vector3();

        foreach (Transform mino in tetromino)
        {
            Vector3 roundedPos = Round(mino.position);

            if (roundedPos.x < 0)
            {
                int toMove = (int)roundedPos.x * -1;

                if (toMove > (int)positionToReturn.x)
                    positionToReturn.x = toMove;
            }
            else if (roundedPos.x >= GridWidth)
            {
                int toMove = ((int)roundedPos.x - 9) * -1;

                if (toMove < (int)positionToReturn.x)
                    positionToReturn.x = toMove;
            }

            if (roundedPos.y < 0)
            {
                int toMove = (int)roundedPos.y * -1;

                if (toMove > (int)positionToReturn.y)
                    positionToReturn.y = toMove;
            }
        }

        return positionToReturn;
    }

    /// <summary>
    /// Checks if the position is free
    /// </summary>
    /// <param name="pos">The position to check</param>
    /// <param name="tetromino">The tetromino to check</param>
    /// <param name="ignoreTag">The tag to be ignored</param>
    /// <returns><c>true</c>, is the position is free, <c>false</c> otherwise</returns>
    public bool IsPositionFree(Vector2 pos, Transform tetromino)
    {
        if (GetTransformAtGridPosition(pos) != null && GetTransformAtGridPosition(pos).parent != tetromino && !GetTransformAtGridPosition(pos).parent.CompareTag("CurrentTetromino"))
            return false;

        return true;
    }

    /// <summary>
    /// Checks if another mino is in the way
    /// </summary>
    /// <returns><c>true</c>, if another mino is in the way, <c>false</c> otherwise</returns>
    public bool IsOtherMinoInTheWay(Transform tetromino)
    {
        foreach (Transform mino in tetromino)
        {
            Vector2 pos = Round(mino.position);

            if (!IsPositionFree(pos, tetromino))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Spawns a random tetromino
    /// </summary>
    public bool SpawnTetromino(Vector2 position = default, int rotationZ = -1, string currentTetrominoName = null, string nextTetrominoName = null)
    {
        if (!gameStarted)
        {
            gameStarted = true;

            if (position.x == 0 && position.y == 0)
            {
                if (currentTetrominoName is null)
                    CurrentTetromino = (GameObject)Instantiate(Resources.Load(GetTetromino()), spawnPosition, Quaternion.identity, tetrominos);
                else
                    CurrentTetromino = (GameObject)Instantiate(Resources.Load(GetTetromino(currentTetrominoName)), spawnPosition, Quaternion.identity, tetrominos);
            }
            else
            {
                if (currentTetrominoName is null)
                    CurrentTetromino = (GameObject)Instantiate(Resources.Load(GetTetromino()), position, Quaternion.identity, tetrominos);
                else
                    CurrentTetromino = (GameObject)Instantiate(Resources.Load(GetTetromino(currentTetrominoName)), position, Quaternion.identity, tetrominos);
            }

            if (rotationZ != -1)
                CurrentTetromino.transform.rotation = Quaternion.Euler(0, 0, rotationZ);
        }
        else
        {
            CurrentTetromino = NextTetromino;
            CurrentTetromino.transform.SetParent(tetrominos);
            if (position.x == 0 && position.y == 0)
                CurrentTetromino.transform.position = spawnPosition;
            else
            {
                CurrentTetromino.transform.position = position;
                CurrentTetromino.transform.position += GetUnitsToMove(CurrentTetromino.transform);

                if (IsOtherMinoInTheWay(CurrentTetromino.transform))
                {
                    DestroyImmediate(CurrentTetromino);
                    return false;
                }
            }

            CurrentTetromino.GetComponent<Tetromino>().enabled = true;
        }

        CurrentTetromino.name = CurrentTetromino.name.Replace("(Clone)", "");
        CurrentTetromino.tag = "CurrentTetromino";

        SpawnNextTetromino(nextTetrominoName);
        SpawnGhostTetromino();
        currentSwaps = 0;
        return true;
    }

    /// <summary>
    /// Spawns the next tetromino
    /// </summary>
    /// <param name="tetrominoName">The tetromino to spawn</param>
    private void SpawnNextTetromino(string tetrominoName = null)
    {
        if (tetrominoName is null)
            NextTetromino = (GameObject)Instantiate(Resources.Load(GetTetromino()), nextTetrominoTransform, false);
        else
            NextTetromino = (GameObject)Instantiate(Resources.Load(GetTetromino(tetrominoName)), nextTetrominoTransform, false);

        NextTetromino.name = NextTetromino.name.Replace("(Clone)", "");
        NextTetromino.transform.localPosition = -NextTetromino.GetComponent<Tetromino>().CenterPosition;
        NextTetromino.GetComponent<Tetromino>().enabled = false;
    }

    /// <summary>
    /// Saves the current tetromino
    /// </summary>
    /// <param name="tetromino">The tetromino to save</param>
    /// <param name="loadedGame">If the current game is loaded from a saved game</param>
    private void SaveTetromino(GameObject tetromino, bool loadedGame = false)
    {
        if (!loadedGame)
        {
            if (currentSwaps == maxSwaps)
                return;

            currentSwaps++;
        }

        if (SavedTetromino != null)
        {
            // There is currently a tetromino being held
            GameObject tempSavedTetromino = GameObject.FindGameObjectWithTag("SavedTetromino");
            tempSavedTetromino.transform.SetParent(tetrominos);
            tempSavedTetromino.transform.position = tetromino.transform.position;
            tempSavedTetromino.transform.position += GetUnitsToMove(tempSavedTetromino.transform);

            if (IsOtherMinoInTheWay(tempSavedTetromino.transform))
            {
                tempSavedTetromino.transform.SetParent(savedTetrominoTransform);
                tempSavedTetromino.transform.localPosition = -tempSavedTetromino.GetComponent<Tetromino>().CenterPosition;
                return;
            }

            SavedTetromino = Instantiate(tetromino, savedTetrominoTransform, false);
            SavedTetromino.name = SavedTetromino.name.Replace("(Clone)", "");
            SavedTetromino.tag = "SavedTetromino";
            Tetromino savedTetrominoClass = SavedTetromino.GetComponent<Tetromino>();
            SavedTetromino.transform.localPosition = -savedTetrominoClass.CenterPosition;
            SavedTetromino.transform.rotation = Quaternion.identity;
            savedTetrominoClass.enabled = false;

            CurrentTetromino = Instantiate(tempSavedTetromino, tetrominos, true);
            CurrentTetromino.name = CurrentTetromino.name.Replace("(Clone)", "");
            CurrentTetromino.tag = "CurrentTetromino";
            CurrentTetromino.transform.SetParent(tetrominos);
            CurrentTetromino.transform.position = tetromino.transform.position + Vector3.up;
            CurrentTetromino.transform.position += GetUnitsToMove(CurrentTetromino.transform);
            CurrentTetromino.GetComponent<Tetromino>().enabled = true;

            RemoveFromGrid(tetromino.transform);

            DestroyImmediate(tetromino);
            DestroyImmediate(tempSavedTetromino);
        }
        else
        {
            // There is currently no tetromino being held
            GameObject currentTetromino = null;

            if (loadedGame)
                SavedTetromino = Instantiate(tetromino, savedTetrominoTransform, false);
            else
            {
                currentTetromino = GameObject.FindGameObjectWithTag("CurrentTetromino");
                SavedTetromino = Instantiate(currentTetromino, savedTetrominoTransform, false);
            }

            SavedTetromino.name = SavedTetromino.name.Replace("(Clone)", "");
            SavedTetromino.tag = "SavedTetromino";
            SavedTetromino.transform.rotation = Quaternion.identity;
            Tetromino savedTetrominoClass = SavedTetromino.GetComponent<Tetromino>();
            SavedTetromino.transform.localPosition = -savedTetrominoClass.CenterPosition;
            savedTetrominoClass.enabled = false;

            if (currentTetromino != null)
            {
                bool isValid = SpawnTetromino(currentTetromino.transform.position + Vector3.up);

                if (!isValid)
                {
                    DestroyImmediate(SavedTetromino);
                    SavedTetromino = null;
                    GameObject newCurrentTetromino = Instantiate(currentTetromino, currentTetromino.transform.position, currentTetromino.transform.rotation, tetrominos);
                    newCurrentTetromino.name = newCurrentTetromino.name.Replace("(Clone)", "");

                    RemoveFromGrid(currentTetromino.transform);
                    DestroyImmediate(currentTetromino);
                    return;
                }

                RemoveFromGrid(currentTetromino.transform);
                DestroyImmediate(currentTetromino);
            }
        }

        SpawnGhostTetromino();
        SaveGameChanged = true;
    }

    /// <summary>
    /// Spawns the ghost tetromino
    /// </summary>
    private void SpawnGhostTetromino()
    {
        DestroyImmediate(GhostTetromino);
        GhostTetromino = Instantiate(CurrentTetromino, CurrentTetromino.transform.position, CurrentTetromino.transform.rotation);
        GhostTetromino.name = "GhostTetromino";
        GhostTetromino.tag = "GhostTetromino";
        GhostTetromino.GetComponent<Tetromino>().enabled = false;

        foreach (Transform mino in GhostTetromino.transform)
        {
            Renderer renderer = mino.GetComponent<Renderer>();
            Color color = renderer.material.color;
            color.a = .5f;

            renderer.material.color = color;
        }

        MoveGhostTetromino((int)GhostTetromino.transform.position.x, CurrentTetromino.transform);
    }

    /// <summary>
    /// Checks if a position is inside the grid
    /// </summary>
    /// <param name="pos">The position to check</param>
    /// <returns><c>true</c>, if the position is inside, <c>false</c> otherwise</returns>
    public bool CheckIsInsideGrid(Vector2 pos)
    {
        return (int)pos.x >= 0 && (int)pos.x < GridWidth && (int)pos.y >= 0;
    }

    /// <summary>
    /// Rounds the x and y of the position
    /// </summary>
    /// <param name="pos">The position to round</param>
    /// <returns>The rounded position</returns>
    public Vector2 Round(Vector2 pos)
    {
        return new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
    }

    /// <summary>
    /// Moves the ghost tetromino
    /// </summary>
    /// <param name="x">The x position to move the tetromino to</param>
    /// <param name="tetromino">The tetromino to ignore</param>
    public void MoveGhostTetromino(int x, Transform tetromino)
    {
        GhostTetromino.transform.position = new Vector2(x, 0);

        for (int i = 1; i < GridHeight; i++)
        {
            if (CheckIsValidPosition(GhostTetromino.transform, tetromino))
                break;
            else
            {
                GhostTetromino.transform.position = new Vector2(x, i);

                if (GhostTetromino.transform.position.y > CurrentTetromino.transform.position.y)
                {
                    GhostTetromino.transform.position = CurrentTetromino.transform.position;
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Rotates the ghost tetromino
    /// </summary>
    /// <param name="rotation">The rotation to rotate it to</param>
    /// <param name="tetromino">The tetromino to ignore</param>
    public void RotateGhostTetromino(Quaternion rotation, Transform tetromino)
    {
        GhostTetromino.transform.rotation = rotation;
        GhostTetromino.transform.position += new Vector3(GetUnitsToMove(GhostTetromino.transform).x, 0);
        MoveGhostTetromino((int)GhostTetromino.transform.position.x, tetromino);
    }

    /// <summary>
    /// Gets a random tetromino
    /// </summary>
    /// <returns>Tetromino name</returns>
    private string GetTetromino(string name = default)
    {
        string tetrominoName = string.Empty;

        if (name == default)
        {
            int randomTetromino = Random.Range(1, 8);

            switch (randomTetromino)
            {
                case 1:
                    tetrominoName = "Tetromino_T";
                    break;
                case 2:
                    tetrominoName = "Tetromino_Long";
                    break;
                case 3:
                    tetrominoName = "Tetromino_Square";
                    break;
                case 4:
                    tetrominoName = "Tetromino_J";
                    break;
                case 5:
                    tetrominoName = "Tetromino_L";
                    break;
                case 6:
                    tetrominoName = "Tetromino_S";
                    break;
                case 7:
                    tetrominoName = "Tetromino_Z";
                    break;
            }
        }
        else
            tetrominoName = name;

        return $"Prefabs/{tetrominoName}";
    }

    /// <summary>
    /// Gets the mino prefab with the name
    /// </summary>
    /// <param name="name">The mino name</param>
    /// <returns>A mino prefab</returns>
    private GameObject GetMino(string name)
    {
        return (GameObject)Resources.Load($"Prefabs/{name}");
    }

    /// <summary>
    /// Pretty self explanatory
    /// </summary>
    public void GameOverScene()
    {
        UpdateHighscores();
        GameOver.Score = CurrentScore;
        GameOver.NewHighscore = highScoreSaved;

        SceneManager.LoadScene("GameOver");
    }
}
