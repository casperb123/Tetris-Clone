using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Tetromino : MonoBehaviour
{
    private float fallTimer = 0;                        // Countdown timer for the fall speed
    private float verticalTimer = 0;                    // Countdown timer for the vertical speed
    private float horizontalTimer = 0;                  // Countdown timer for the horizontal speed
    private float buttonDownWaitTimerHorizontal = 0;    // How long to wait before the tetromino recognizes that the left or right arrow is being held down
    private float buttonDownWaitTimerVertical = 0;      // How long to wait before the tetromino recognizes that the down arrow is being held down

    private bool movedImmediateHorizontal;
    private bool movedImmediateVertical;

    [Header("Rotation Settings")]
    [SerializeField]
    private bool allowRotation = true;                  // We use this to specofy if we want to allow the tetromino to rotate
    [SerializeField]
    private bool limitRotation;                         // This is used to limit the rotation of the tetromino to a 90 / -90 rotation (To / From)
    private Game game;

    private AudioSource audioSource;

    [Header("Audio Settings")]
    [SerializeField]
    private AudioClip moveSound;                        // Sound for when the tetromino is moved or rotated
    [SerializeField]
    private AudioClip landSound;                        // Sound for when the tetromino lands

    private int individualScore = 100;
    private float individualScoreTime;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        game = GameObject.Find("GameScript").GetComponent<Game>();
    }

    private void Update()
    {
        if (game.IsGameOver)
            return;

        CheckUserInput();
        UpdateIndividualScore();
    }

    /// <summary>
    /// Updates the individual score for when the tetromino lands
    /// </summary>
    private void UpdateIndividualScore()
    {
        if (individualScoreTime < 1)
            individualScoreTime += Time.deltaTime;
        else
        {
            individualScoreTime = 0;
            individualScore = Mathf.Max(individualScore - 10, 0);
        }
    }

    /// <summary>
    /// Checks the user input and moves the tetromino down
    /// </summary>
    private void CheckUserInput()
    {
        // This method checks the keys that the player can press to manipulate the position of the tetromino
        // The options here will be left, right, up and down
        // Left and right will move the tetromino 1 unit to the left or right
        // Down will move the tetromino 1 unit down
        // Up will rotate the tetromino

        if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow))
        {
            movedImmediateHorizontal = false;
            horizontalTimer = 0;
            buttonDownWaitTimerHorizontal = 0;
        }

        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            movedImmediateVertical = false;
            verticalTimer = 0;
            buttonDownWaitTimerVertical = 0;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            MoveRight();
        }
        
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            MoveLeft();
        }
        
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Rotate();
        }
        
        if (Input.GetKey(KeyCode.DownArrow) || Time.time - fallTimer >= game.FallSpeed)
        {
            MoveDown();
        }
    }

    /// <summary>
    /// Moves the tetromino to the left
    /// </summary>
    private void MoveLeft()
    {
        if (movedImmediateHorizontal)
        {
            if (buttonDownWaitTimerHorizontal < game.ButtonDownWaitMax)
            {
                buttonDownWaitTimerHorizontal += Time.deltaTime;
                return;
            }

            if (horizontalTimer < game.HorizontalSpeed)
            {
                horizontalTimer += Time.deltaTime;
                return;
            }
        }
        else
            movedImmediateHorizontal = true;

        horizontalTimer = 0;

        // First we attempt to move the tetromino to the left
        transform.position += Vector3.left;

        // We then check if the tetromino is at a valid position
        if (CheckIsValidPosition())
        {
            // if it is, we then call the UpdateGrid method which records this tetrominos new position
            game.UpdateGrid(this);
            PlayMoveAudio();
        }
        else
            // If it isn't we move the tetromino back to the right
            transform.position += Vector3.right;
    }

    /// <summary>
    /// Moves the tetromino to the right
    /// </summary>
    private void MoveRight()
    {
        if (movedImmediateHorizontal)
        {
            if (buttonDownWaitTimerHorizontal < game.ButtonDownWaitMax)
            {
                buttonDownWaitTimerHorizontal += Time.deltaTime;
                return;
            }

            if (horizontalTimer < game.HorizontalSpeed)
            {
                horizontalTimer += Time.deltaTime;
                return;
            }
        }
        else
            movedImmediateHorizontal = true;

        horizontalTimer = 0;

        // First we attempt to move the tetromino to the right
        transform.position += Vector3.right;

        // We then check if the tetromino is at a valid position
        if (CheckIsValidPosition())
        {
            // if it is, we then call the UpdateGrid method which records this tetrominos new position
            game.UpdateGrid(this);
            PlayMoveAudio();
        }
        else
            // If it isn't we move the tetromino back to the left
            transform.position += Vector3.left;
    }

    /// <summary>
    /// Moves the tetromino down
    /// </summary>
    private void MoveDown()
    {
        if (movedImmediateVertical)
        {
            if (buttonDownWaitTimerVertical < game.ButtonDownWaitMax)
            {
                buttonDownWaitTimerVertical += Time.deltaTime;
                return;
            }

            if (verticalTimer < game.VerticalSpeed)
            {
                verticalTimer += Time.deltaTime;
                return;
            }
        }
        else
            movedImmediateVertical = true;

        verticalTimer = 0;

        transform.position += Vector3.down;

        if (CheckIsValidPosition())
        {
            game.UpdateGrid(this);

            if (Input.GetKey(KeyCode.DownArrow))
                PlayMoveAudio();
        }
        else
        {
            transform.position += Vector3.up;
            game.DeleteRow();

            // Check if there are any minos above the grid
            if (game.CheckIsAboveGrid(this))
                game.GameOver();

            // Disables the script
            enabled = false;
            // Updates the individual score
            Game.CurrentScore += individualScore;
            // Spawn the next tetromino
            game.SpawnTetromino();
            // Plays the land audio
            PlayLandAudio();
        }

        fallTimer = Time.time;
    }

    /// <summary>
    /// Rotates the tetromino
    /// </summary>
    private void Rotate()
    {
        // The up key was pressed, lets first check if the tetromino is allowed to rotate
        if (allowRotation)
        {
            // If it is, we need to check if the rotation is limited to just back and forth
            if (limitRotation)
            {
                // If it is, we need to check what the current rotation is
                if (transform.rotation.eulerAngles.z >= 90)
                    // If it is at 90 then we know it was already rotated, so we rotate it back by -90
                    transform.Rotate(0, 0, -90);
                else
                    // If it isn't, then we rotate it by 90
                    transform.Rotate(0, 0, 90);
            }
            else
                // If it isn't, we rotate it by 90
                transform.Rotate(0, 0, 90);

            // Not we check if the tetromino is at a valid position after attempting a rotation
            if (CheckIsValidPosition())
            {
                // If the position is valid, we update the grid
                game.UpdateGrid(this);
                PlayMoveAudio();
            }
            else
            {
                // Checks if any of the minos in the tetromino is outside of the grid, if it is then it gets how many minos is outside
                Vector3 toMove = GetUnitsToMove();

                // Checks if the x or y position is not equal to 0
                if (toMove.x != 0 || toMove.y != 0 && !IsOtherMinoInTheWay())
                {
                    // If thats true then it moves the tetromino
                    transform.position += toMove;
                    game.UpdateGrid(this);
                    PlayMoveAudio();
                }
                else
                {
                    // if the x or y position is equal to 0 then we check if the tetromino is limited to rotation back and forth
                    if (limitRotation)
                    {
                        // If it's limited then we need to check what the current rotation is
                        if (transform.rotation.eulerAngles.z >= 90)
                            // If it's at 90 then we know it was already rotated, so we rotate it back by -90
                            transform.Rotate(0, 0, -90);
                        else
                            // If it isn't, we rotate it by 90
                            transform.Rotate(0, 0, 90);
                    }
                    else
                        // If it isn't limited then we rotate it by -90
                        transform.Rotate(0, 0, -90);
                }

            }
        }
    }

    /// <summary>
    /// Plays audio clip when the tetromino is moved or rotated
    /// </summary>
    private void PlayMoveAudio()
    {
        audioSource.PlayOneShot(moveSound);
    }

    /// <summary>
    /// Plays audio clip when the tetromino lands
    /// </summary>
    private void PlayLandAudio()
    {
        audioSource.PlayOneShot(landSound);
    }

    /// <summary>
    /// Checks if the position is valid
    /// </summary>
    /// <returns><c>true</c>, if the positions is valid, <c>false</c> otherwise</returns>
    private bool CheckIsValidPosition()
    {
        foreach (Transform mino in transform)
        {
            Vector2 pos = game.Round(mino.position);

            if (!game.CheckIsInsideGrid(pos))
                return false;

            //if (game.GetTransformAtGridPosition(pos) != null && game.GetTransformAtGridPosition(pos).parent != transform)
            //    return false;
            if (!IsPositionFree(pos))
                return false;
        }

        return true;
    }

    private bool IsPositionFree(Vector2 pos)
    {
        if (game.GetTransformAtGridPosition(pos) != null && game.GetTransformAtGridPosition(pos).parent != transform)
            return false;

        return true;
    }

    private bool IsOtherMinoInTheWay()
    {
        foreach (Transform mino in transform)
        {
            Vector2 pos = game.Round(mino.position);

            if (!IsPositionFree(pos))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Checks how many units the tetromino is outside of the grid
    /// </summary>
    /// <returns>The number of units the tetromino is outside</returns>
    private Vector3 GetUnitsToMove()
    {
        Vector3 positionToReturn = new Vector3();

        foreach (Transform mino in transform)
        {
            Vector3 roundedPos = game.Round(mino.position);

            if (roundedPos.x < 0)
            {
                int toMove = (int)roundedPos.x * -1;

                if (toMove > (int)positionToReturn.x)
                    positionToReturn.x = toMove;
            }
            else if (roundedPos.x >= Game.GridWidth)
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
}
