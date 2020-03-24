﻿using UnityEngine;

public class Tetromino : MonoBehaviour
{
    private Game game;
    private Options options;

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

    [Header("Position Settings")]
    public Vector3 CenterPosition;

    private AudioSource audioSource;

    [Header("Audio Settings")]
    [SerializeField]
    private AudioClip moveSound;                        // Sound for when the tetromino is moved or rotated
    [SerializeField]
    private AudioClip landSound;                        // Sound for when the tetromino lands

    private int individualScore;
    private float individualScoreTime;

    // Variables for Touch movement
    private Vector2 previousUnitPosition = Vector2.zero;
    private Vector2 direction = Vector2.zero;
    private bool moved;

    private void Start()
    {
        game = Game.Instance;
        options = Options.Instance;
        audioSource = GetComponent<AudioSource>();
        individualScore = game.MaxIndividualScore;
    }

    private void Update()
    {
        if (game.IsGameOver || game.IsPaused)
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

#if UNITY_IOS
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                previousUnitPosition = new Vector2(touch.position.x, touch.position.y);
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector2 touchDeltaPosition = touch.deltaPosition;
                direction = touchDeltaPosition.normalized;

                if (Mathf.Abs(touch.position.x - previousUnitPosition.x) >= game.TouchSensitivityHorizontal && direction.x < 0 && touch.deltaPosition.y > -10 && touch.deltaPosition.y < 10)
                {
                    // Move Left
                    MoveLeft();
                    previousUnitPosition = touch.position;
                    moved = true;
                }
                else if (Mathf.Abs(touch.position.x - previousUnitPosition.x) >= game.TouchSensitivityHorizontal && direction.x > 0 && touch.deltaPosition.y > -10 && touch.deltaPosition.y < 10)
                {
                    // Move Right
                    MoveRight();
                    previousUnitPosition = touch.position;
                    moved = true;
                }
                else if (Mathf.Abs(touch.position.y - previousUnitPosition.y) >= game.TouchSensitivityVertical && direction.y < 0 && touch.deltaPosition.x > -10 && touch.deltaPosition.x < 10)
                {
                    // Move Down
                    MoveDown();
                    previousUnitPosition = touch.position;
                    moved = true;
                }
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                if (!moved && touch.position.x > Screen.width / 4)
                {
                    Rotate();
                }

                moved = false;
            }
        }

        if (Time.time - fallTimer >= game.FallSpeed)
            MoveDown();

#else

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
            MoveRight();

        if (Input.GetKey(KeyCode.LeftArrow))
            MoveLeft();

        if (Input.GetKeyDown(KeyCode.UpArrow))
            Rotate();

        if (Input.GetKey(KeyCode.DownArrow) || Time.time - fallTimer >= game.FallSpeed)
            MoveDown();

        if (Input.GetKeyDown(KeyCode.LeftAlt))
            MoveToBottom();
#endif
    }

    /// <summary>
    /// Moves the tetromino to the left
    /// </summary>
    void MoveLeft()
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
        if (game.CheckIsValidPosition(transform))
        {
            // if it is, we then call the UpdateGrid method which records this tetrominos new position
            game.UpdateGrid(this);
            PlayMoveAudio();
            game.MoveGhostTetromino((int)transform.position.x, transform);
        }
        else
            // If it isn't we move the tetromino back to the right
            transform.position += Vector3.right;
    }

    /// <summary>
    /// Moves the tetromino to the right
    /// </summary>
    void MoveRight()
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

        transform.position += Vector3.right;

        if (game.CheckIsValidPosition(transform))
        {
            game.UpdateGrid(this);
            PlayMoveAudio();
            game.MoveGhostTetromino((int)transform.position.x, transform);
        }
        else
            transform.position += Vector3.left;
    }

    /// <summary>
    /// Moves the tetromino down
    /// </summary>
    void MoveDown()
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

        if (game.CheckIsValidPosition(transform))
        {
            game.UpdateGrid(this);
            game.MoveGhostTetromino((int)transform.position.x, transform);

            if (Input.GetKey(KeyCode.DownArrow))
                PlayMoveAudio();
        }
        else
        {
            transform.position += Vector3.up;
            game.DeleteRow();

            // Check if there are any minos above the grid
            if (game.CheckIsAboveGrid(this))
            {
                game.GameOverScene();
                return;
            }

            game.CurrentScore += individualScore;
            game.SpawnTetromino();
            PlayLandAudio();
            tag = "Untagged";
            enabled = false;
        }

        fallTimer = Time.time;
    }

    /// <summary>
    /// Moves the tetromino to the bottom
    /// </summary>
    void MoveToBottom()
    {
        transform.position = new Vector2(transform.position.x, 0);

        for (int i = 1; i < game.GridHeight; i++)
        {
            if (game.CheckIsValidPosition(transform, checkMinosAbove: true))
            {
                game.UpdateGrid(this);
                break;
            }

            transform.position = new Vector2(transform.position.x, i);

            if (game.CheckIsAboveGrid(this))
            {
                game.GameOverScene();
                return;
            }
        }

        Camera.main.GetComponent<ShakeBehaviour>().TriggerShake(.5f, .1f, 5);

        game.DeleteRow();
        game.CurrentScore += individualScore;
        game.SpawnTetromino();
        PlayLandAudio();
        tag = "Untagged";
        enabled = false;
    }

    /// <summary>
    /// Rotates the tetromino
    /// </summary>
    void Rotate()
    {
        if (allowRotation)
        {
            if (limitRotation)
            {
                if (transform.rotation.eulerAngles.z >= 90)
                    transform.Rotate(0, 0, -90);
                else
                    transform.Rotate(0, 0, 90);
            }
            else
                transform.Rotate(0, 0, 90);

            if (game.CheckIsValidPosition(transform))
            {
                game.UpdateGrid(this);
                PlayMoveAudio();

                game.RotateGhostTetromino(transform.rotation, transform);
            }
            else
            {
                Vector3 toMove = game.GetUnitsToMove(transform);

                if (toMove.x != 0 || toMove.y != 0)
                {
                    transform.position += toMove;
                    if (game.IsOtherMinoInTheWay(transform))
                    {
                        transform.position -= toMove;

                        if (limitRotation)
                        {
                            if (transform.rotation.eulerAngles.z >= 90)
                                transform.Rotate(0, 0, -90);
                            else
                                transform.Rotate(0, 0, 90);
                        }
                        else
                            transform.Rotate(0, 0, -90);

                        return;
                    }

                    game.UpdateGrid(this);
                    PlayMoveAudio();

                    game.RotateGhostTetromino(transform.rotation, transform);
                    game.MoveGhostTetromino((int)transform.position.x, transform);
                }
                else
                {
                    if (limitRotation)
                    {
                        if (transform.rotation.eulerAngles.z >= 90)
                            transform.Rotate(0, 0, -90);
                        else
                            transform.Rotate(0, 0, 90);
                    }
                    else
                        transform.Rotate(0, 0, -90);
                }

            }
        }
    }

    /// <summary>
    /// Plays audio clip when the tetromino is moved or rotated
    /// </summary>
    void PlayMoveAudio()
    {
        if (options.SoundEffects)
            audioSource.PlayOneShot(moveSound);
    }

    /// <summary>
    /// Plays audio clip when the tetromino lands
    /// </summary>
    void PlayLandAudio()
    {
        if (options.SoundEffects)
            audioSource.PlayOneShot(landSound);
    }
}
