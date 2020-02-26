using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    private Transform tetrominos;
    private int numberOfRowsThisTurn = 0;

    [SerializeField]
    private Text scoreText;

    [SerializeField]
    private int scoreOneLine = 40;
    [SerializeField]
    private int scoreTwoLines = 100;
    [SerializeField]
    private int scoreThreeLines = 300;
    [SerializeField]
    private int scoreFourLines = 1200;

    public static int GridWidth = 10;
    public static int GridHeight = 20;

    public static int CurrentScore;

    public static Transform[,] grid = new Transform[GridWidth, GridHeight];

    private void Start()
    {
        tetrominos = GameObject.Find("Tetrominos").transform;
        SpawnNextTetromino();
    }

    private void Update()
    {
        UpdateScore();
        UpdateUI();
    }

    private void UpdateUI()
    {
        scoreText.text = CurrentScore.ToString();
    }

    public void UpdateScore()
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

    public void ClearedOneLine()
    {
        CurrentScore += scoreOneLine;
    }

    public void ClearedTwoLines()
    {
        CurrentScore += scoreTwoLines;
    }

    public void ClearedThreeLines()
    {
        CurrentScore += scoreThreeLines;
    }

    public void ClearedFourLines()
    {
        CurrentScore += scoreFourLines;
    }

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

    public void DeleteMinoAt(int y)
    {
        for (int x = 0; x < GridWidth; x++)
        {
            Destroy(grid[x, y].gameObject);
            grid[x, y] = null;
        }
    }

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

    public void MoveAllRowsDown(int y)
    {
        for (int i = y; i < GridHeight; i++)
        {
            MoveRowDown(i);
        }
    }

    public void DeleteRow()
    {
        for (int y = 0; y < GridHeight; y++)
        {
            if (IsFullRowAt(y))
            {
                DeleteMinoAt(y);
                MoveAllRowsDown(y + 1);
                y--;
            }
        }
    }

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

    public Transform GetTransformAtGridPosition(Vector2 pos)
    {
        if (pos.y > GridHeight - 1)
            return null;

        return grid[(int)pos.x, (int)pos.y];
    }

    public void SpawnNextTetromino()
    {
        GameObject nextTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), new Vector2(5, 20), Quaternion.identity, tetrominos);
    }

    public bool CheckIsInsideGrid(Vector2 pos)
    {
        return (int)pos.x >= 0 && (int)pos.x < GridWidth && (int)pos.y >= 0;
    }

    public Vector2 Round(Vector2 pos)
    {
        return new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
    }

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

    public void GameOver()
    {
        SceneManager.LoadScene("GameOver");
    }
}
