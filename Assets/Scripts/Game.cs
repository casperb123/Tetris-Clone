﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public static Game Instance;
    public int GridWidth = 10;
    public int GridHeight = 20;
    public Transform[,] grid;
    public int CurrentScore;
    public bool StartingAtLevelZero;
    public int StartingLevel;
    public bool IsPaused;

    private Transform tetrominos;
    private int currentLevel;
    private int totalLinesCleared;
    private int numberOfRowsThisTurn = 0;   // The number of rows cleared by a tetromino
    private AudioSource audioSource;

    [Header("UI Settings")]
    [SerializeField]
    private Text scoreText;
    [SerializeField]
    private Text levelText;
    [SerializeField]
    private Text linesText;
    [SerializeField]
    private GameObject gameOverPanel;

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

    private GameObject previewTetromino;
    private GameObject nextTetromino;

    private bool gameStarted;
    private int startingHighscore;

    [Header("Speed Settings")]
    public float FallSpeed = 1;             // The speed at which the tetromino will fall if the down arrow isn't being held down
    public float VerticalSpeed = .1f;       // The speed at which the tetromino will move when the down arrow is held down
    public float HorizontalSpeed = .1f;     // The speed at which the tetromino will move when the left or right arrow is held down
    public float ButtonDownWaitMax = .2f;   // How long to wait before the tetromino recognizes that a button is being held down

    [Header("Game Settings")]
    [SerializeField]
    private GameObject minoGridPrefab;
    public bool IsGameOver;

    private void Awake()
    {
        if (Instance is null)
            Instance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        grid = new Transform[GridWidth, GridHeight];

        CurrentScore = 0;
        currentLevel = StartingLevel;
        startingHighscore = PlayerPrefs.GetInt("highscore");

        GenerateGridWalls();
        audioSource = GetComponent<AudioSource>();
        tetrominos = GameObject.Find("Tetrominos").transform;
        SpawnTetromino();
    }

    private void Update()
    {
        if (IsPaused)
            return;

        UpdateScore();
        UpdateUI();
        UpdateLevel();
        UpdateSpeed();
    }

    private void GenerateGridWalls()
    {
        Transform grid = GameObject.Find("MinoGrid").transform;

        for (int x = -1; x < GridWidth + 1; x++)
        {
            Instantiate(minoGridPrefab, new Vector3(x, -1), Quaternion.identity, grid);
        }

        for (int y = 0; y < GridHeight; y++)
        {
            Instantiate(minoGridPrefab, new Vector3(-1, y), Quaternion.identity, grid);
        }

        for (int y = 0; y < GridHeight; y++)
        {
            Instantiate(minoGridPrefab, new Vector3(GridWidth, y), Quaternion.identity, grid);
        }
    }

    private void UpdateLevel()
    {
        if (StartingAtLevelZero || !StartingAtLevelZero && totalLinesCleared / 10 > StartingLevel)
        {
            if (currentLevel <= 9)
                currentLevel = totalLinesCleared / 10;
        }
    }

    private void UpdateSpeed()
    {
        FallSpeed = 1 - (currentLevel * .1f);
    }

    /// <summary>
    /// Updates the UI
    /// </summary>
    private void UpdateUI()
    {
        scoreText.text = CurrentScore.ToString();
        levelText.text = currentLevel.ToString();
        linesText.text = totalLinesCleared.ToString();
    }

    /// <summary>
    /// Checks how many lines is cleared
    /// </summary>
    public void UpdateScore()
    {
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
            UpdateHighscore();
        }
    }

    /// <summary>
    /// Updates the score
    /// </summary>
    private void ClearedOneLine()
    {
        CurrentScore += scoreOneLine + (currentLevel * 20);
        PlayClearLineSound();
        totalLinesCleared++;
    }

    /// <summary>
    /// Updates the score
    /// </summary>
    private void ClearedTwoLines()
    {
        CurrentScore += scoreTwoLines + (currentLevel * 25);
        PlayClearLineSound();
        totalLinesCleared += 2;
    }

    /// <summary>
    /// Updates the score
    /// </summary>
    private void ClearedThreeLines()
    {
        CurrentScore += scoreThreeLines + (currentLevel * 30);
        PlayClearLineSound();
        totalLinesCleared += 3;
    }

    /// <summary>
    /// Updates the score
    /// </summary>
    private void ClearedFourLines()
    {
        CurrentScore += scoreFourLines + (currentLevel * 40);
        PlayClearFourLinesSound();
        totalLinesCleared += 4;
    }

    public void UpdateHighscore()
    {
        if (CurrentScore > startingHighscore)
            PlayerPrefs.SetInt("highscore", CurrentScore);
    }

    /// <summary>
    /// Plays the clear line sound
    /// </summary>
    private void PlayClearLineSound()
    {
        audioSource.PlayOneShot(clearLineSound);
    }

    /// <summary>
    /// Plays the clear 4 lines sound
    /// </summary>
    private void PlayClearFourLinesSound()
    {
        audioSource.PlayOneShot(clearFourLinesSound);
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
            if (grid[x, y] == null)
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
            Destroy(grid[x, y].gameObject);
            grid[x, y] = null;
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
            if (grid[x, y] != null)
            {
                grid[x, y - 1] = grid[x, y];
                grid[x, y] = null;
                grid[x, y - 1].position += new Vector3(0, -1);
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
                if (grid[x, y] != null)
                {
                    if (grid[x, y].parent == tetromino.transform)
                    {
                        grid[x, y] = null;
                    }
                }
            }
        }

        foreach (Transform mino in tetromino.transform)
        {
            Vector2 pos = Round(mino.position);

            if (pos.y < GridHeight)
            {
                grid[(int)pos.x, (int)pos.y] = mino;
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

        return grid[(int)pos.x, (int)pos.y];
    }

    /// <summary>
    /// Spawns a random tetromino
    /// </summary>
    public void SpawnTetromino()
    {
        if (!gameStarted)
        {
            gameStarted = true;

            nextTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), new Vector2(GridWidth / 2, GridHeight), Quaternion.identity, tetrominos);

            previewTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), new Vector2(-7.5f, GridHeight / 2), Quaternion.identity);
            previewTetromino.GetComponent<Tetromino>().enabled = false;
        }
        else
        {
            previewTetromino.transform.localPosition = new Vector2(GridWidth / 2, GridHeight);
            nextTetromino = previewTetromino;
            nextTetromino.transform.SetParent(tetrominos);
            nextTetromino.GetComponent<Tetromino>().enabled = true;

            previewTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), new Vector2(-7.5f, GridHeight / 2), Quaternion.identity);
            previewTetromino.GetComponent<Tetromino>().enabled = false;
        }
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
    /// Gets a random tetromino
    /// </summary>
    /// <returns>Tetromino name</returns>
    private string GetRandomTetromino()
    {
        int randomTetromino = Random.Range(1, 8);
        string randomTetrominoName = string.Empty;

        switch (randomTetromino)
        {
            case 1:
                randomTetrominoName = "Tetromino_T";
                break;
            case 2:
                randomTetrominoName = "Tetromino_Long";
                break;
            case 3:
                randomTetrominoName = "Tetromino_Square";
                break;
            case 4:
                randomTetrominoName = "Tetromino_J";
                break;
            case 5:
                randomTetrominoName = "Tetromino_L";
                break;
            case 6:
                randomTetrominoName = "Tetromino_S";
                break;
            case 7:
                randomTetrominoName = "Tetromino_Z";
                break;
        }

        return $"Prefabs/{randomTetrominoName}";
    }

    /// <summary>
    /// Pretty self explanatory
    /// </summary>
    public void GameOver()
    {
        SceneManager.LoadScene("GameOver");
    }
}
