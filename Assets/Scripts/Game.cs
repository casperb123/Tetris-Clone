using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public static Game Instance;
    public static bool StartingAtLevelZero;
    public static int StartingLevel;

    public Transform[,] grid;

    private Transform tetrominos;
    private int currentLevel;
    private int totalLinesCleared;
    private int numberOfRowsThisTurn = 0;   // The number of rows cleared by a tetromino
    private AudioSource audioSource;

    private GameObject previewTetromino;
    private GameObject currentTetromino;
    private GameObject savedTetromino;
    [HideInInspector]
    public GameObject GhostTetromino = null;
    private Vector2 spawnPosition;

    private bool gameStarted;
    [HideInInspector]
    public int CurrentScore;
    private int startingHighScore;
    private int currentSwaps;
    [HideInInspector]
    public bool IsPaused;
    [HideInInspector]
    public bool IsGameOver;

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

    [Header("UI Settings")]
    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private TextMeshProUGUI levelText;
    [SerializeField]
    private TextMeshProUGUI linesText;

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

    [Header("Speed Settings")]
    public float FallSpeed = 1;             // The speed at which the tetromino will fall if the down arrow isn't being held down
    public float VerticalSpeed = .1f;       // The speed at which the tetromino will move when the down arrow is held down
    public float HorizontalSpeed = .1f;     // The speed at which the tetromino will move when the left or right arrow is held down
    public float ButtonDownWaitMax = .2f;   // How long to wait before the tetromino recognizes that a button is being held down

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Time.timeScale = 1;

        grid = new Transform[GridWidth, GridHeight];
        spawnPosition = new Vector2(GridWidth / 2, GridHeight);

        CurrentScore = 0;
        currentLevel = StartingLevel;
        startingHighScore = PlayerPrefs.GetInt("highscore");

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
        CheckUserInput();
    }

    private void CheckUserInput()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            GameObject tempTetromino = GameObject.FindGameObjectWithTag("CurrentTetromino");
            SaveTetromino(tempTetromino.transform);
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
        if (CurrentScore > startingHighScore)
            scoreText.color = Color.green;

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

            UpdateHighscore();
            numberOfRowsThisTurn = 0;
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
        if (CurrentScore > startingHighScore)
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
    /// Checks if the tetrominos position is valid
    /// </summary>
    /// <param name="tetromino">The tetromino to check</param>
    /// <param name="ignore">The tetromino to ignore</param>
    /// <returns><c>trye</c>, if the tetrominos position is valid, <c>false</c> otherwise</returns>
    public bool CheckIsValidPosition(Transform tetromino, Transform ignore = null)
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
    public bool SpawnTetromino(Vector2 pos = default)
    {
        if (!gameStarted)
        {
            gameStarted = true;

            if (pos.x == 0 && pos.y == 0)
                currentTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino()), spawnPosition, Quaternion.identity, tetrominos);
            else
                currentTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino()), pos, Quaternion.identity, tetrominos);

            currentTetromino.tag = "CurrentTetromino";

            previewTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino()), nextTetrominoTransform, false);
            previewTetromino.transform.localPosition = -previewTetromino.GetComponent<Tetromino>().CenterPosition;
            previewTetromino.GetComponent<Tetromino>().enabled = false;
        }
        else
        {
            currentTetromino = previewTetromino;
            currentTetromino.transform.SetParent(tetrominos);
            if (pos.x == 0 && pos.y == 0)
                currentTetromino.transform.position = spawnPosition;
            else
            {
                currentTetromino.transform.position = pos;
                currentTetromino.transform.position += GetUnitsToMove(currentTetromino.transform);

                if (IsOtherMinoInTheWay(currentTetromino.transform))
                {
                    DestroyImmediate(currentTetromino);
                    return false;
                }
            }
            currentTetromino.GetComponent<Tetromino>().enabled = true;
            currentTetromino.tag = "CurrentTetromino";

            previewTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino()), nextTetrominoTransform, false);
            previewTetromino.transform.localPosition = -previewTetromino.GetComponent<Tetromino>().CenterPosition;
            previewTetromino.GetComponent<Tetromino>().enabled = false;
        }

        SpawnGhostTetromino();
        currentSwaps = 0;
        return true;
    }

    private void SaveTetromino(Transform tetrominoTransform)
    {
        if (currentSwaps == maxSwaps)
            return;

        currentSwaps++;

        if (savedTetromino != null)
        {
            // There is currently a tetromino being held
            GameObject tempSavedTetromino = GameObject.FindGameObjectWithTag("SavedTetromino");
            tempSavedTetromino.transform.SetParent(tetrominos);
            tempSavedTetromino.transform.position = tetrominoTransform.position;
            tempSavedTetromino.transform.position += GetUnitsToMove(tempSavedTetromino.transform);

            if (IsOtherMinoInTheWay(tempSavedTetromino.transform))
            {
                tempSavedTetromino.transform.SetParent(savedTetrominoTransform);
                tempSavedTetromino.transform.localPosition = -tempSavedTetromino.GetComponent<Tetromino>().CenterPosition;
                return;
            }

            savedTetromino = Instantiate(tetrominoTransform.gameObject, savedTetrominoTransform, false);
            savedTetromino.tag = "SavedTetromino";
            Tetromino savedTetrominoClass = savedTetromino.GetComponent<Tetromino>();
            savedTetromino.transform.localPosition = -savedTetrominoClass.CenterPosition;
            savedTetromino.transform.rotation = Quaternion.identity;
            savedTetrominoClass.enabled = false;

            currentTetromino = Instantiate(tempSavedTetromino, tetrominos, true);
            currentTetromino.tag = "CurrentTetromino";
            currentTetromino.transform.SetParent(tetrominos);
            currentTetromino.transform.position = tetrominoTransform.position + Vector3.up;
            currentTetromino.transform.position += GetUnitsToMove(currentTetromino.transform);
            currentTetromino.GetComponent<Tetromino>().enabled = true;

            DestroyImmediate(tetrominoTransform.gameObject);
            DestroyImmediate(tempSavedTetromino);
        }
        else
        {
            // There is currently no tetromino being held
            GameObject currentTetromino = GameObject.FindGameObjectWithTag("CurrentTetromino");
            savedTetromino = Instantiate(currentTetromino, savedTetrominoTransform, false);
            savedTetromino.tag = "SavedTetromino";
            savedTetromino.transform.rotation = Quaternion.identity;
            Tetromino tetromino = savedTetromino.GetComponent<Tetromino>();
            savedTetromino.transform.localPosition = -tetromino.CenterPosition;
            tetromino.enabled = false;

            bool isValid = SpawnTetromino(currentTetromino.transform.position + Vector3.up);

            if (!isValid)
            {
                DestroyImmediate(savedTetromino);
                savedTetromino = null;
                Instantiate(currentTetromino, currentTetromino.transform.position, currentTetromino.transform.rotation, tetrominos);
                return;
            }

            DestroyImmediate(currentTetromino);
        }

        SpawnGhostTetromino();
    }

    private void SpawnGhostTetromino()
    {
        DestroyImmediate(GhostTetromino);
        GhostTetromino = Instantiate(currentTetromino, currentTetromino.transform.position, currentTetromino.transform.rotation);
        GhostTetromino.name = "GhostTetromino";
        GhostTetromino.GetComponent<Tetromino>().enabled = false;

        foreach (Transform mino in GhostTetromino.transform)
        {
            Renderer renderer = mino.GetComponent<Renderer>();
            Color color = renderer.material.color;
            color.a = .5f;

            renderer.material.color = color;
        }

        MoveGhostTetromino((int)GhostTetromino.transform.position.x, currentTetromino.transform);
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

    public void MoveGhostTetromino(int x, Transform tetromino)
    {
        GhostTetromino.transform.position = new Vector2(x, 0);

        while (!CheckIsValidPosition(GhostTetromino.transform, tetromino))
        {
            GhostTetromino.transform.position += Vector3.up;

            if (GhostTetromino.transform.position.y > currentTetromino.transform.position.y)
            {
                GhostTetromino.transform.position = currentTetromino.transform.position;
                break;
            }
        }
    }

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
        UpdateHighscore();
        SceneManager.LoadScene("GameOver");
    }
}
