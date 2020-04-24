using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tetromino : MonoBehaviour
{
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

    private float fallTimer = 0;                        // Countdown timer for the fall speed
    private float verticalTimer = 0;                    // Countdown timer for the vertical speed
    private float horizontalTimer = 0;                  // Countdown timer for the horizontal speed
    private float buttonDownWaitTimerHorizontal = 0;    // How long to wait before the tetromino recognizes that the left or right arrow is being held down
    private float buttonDownWaitTimerVertical = 0;      // How long to wait before the tetromino recognizes that the down arrow is being held down

    private bool movedImmediateHorizontal;
    private bool movedImmediateVertical;

    private int individualScore;
    private float individualScoreTime;

    // Variables for Touch movement
    private Vector2 previousUnitPosition = Vector2.zero;
    private Vector2 direction = Vector2.zero;
    private bool moved;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        individualScore = Game.Instance.MaxIndividualScore;
    }

    private void Update()
    {
        if (Game.Instance.IsGameOver || Game.Instance.IsPaused)
            return;

        CheckUserInput();
        UpdateIndividualScore();
    }

    /// <summary>
    /// Updates the individual score
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

                if (Mathf.Abs(touch.position.x - previousUnitPosition.x) >= Game.Instance.TouchSensitivityHorizontal && direction.x < 0 && touch.deltaPosition.y > -10 && touch.deltaPosition.y < 10)
                {
                    // Move Left
                    MoveLeft();
                    previousUnitPosition = touch.position;
                    moved = true;
                }
                else if (Mathf.Abs(touch.position.x - previousUnitPosition.x) >= Game.Instance.TouchSensitivityHorizontal && direction.x > 0 && touch.deltaPosition.y > -10 && touch.deltaPosition.y < 10)
                {
                    // Move Right
                    MoveRight();
                    previousUnitPosition = touch.position;
                    moved = true;
                }
                else if (Mathf.Abs(touch.position.y - previousUnitPosition.y) >= Game.Instance.TouchSensitivityVertical && direction.y < 0 && touch.deltaPosition.x > -10 && touch.deltaPosition.x < 10)
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

        if (Time.time - fallTimer >= Game.Instance.FallSpeed)
            MoveDown();

#else

        SavedControl moveLeft = Game.Instance.Controls.FirstOrDefault(x => x.ControlType == SavedControl.Type.MoveLeft);
        SavedControl moveRight = Game.Instance.Controls.FirstOrDefault(x => x.ControlType == SavedControl.Type.MoveRight);
        SavedControl moveDown = Game.Instance.Controls.FirstOrDefault(x => x.ControlType == SavedControl.Type.MoveDown);
        SavedControl rotate = Game.Instance.Controls.FirstOrDefault(x => x.ControlType == SavedControl.Type.Rotate);
        SavedControl moveToBottom = Game.Instance.Controls.FirstOrDefault(x => x.ControlType == SavedControl.Type.MoveToBottom);
        SavedControl save = Game.Instance.Controls.FirstOrDefault(x => x.ControlType == SavedControl.Type.SaveTetromino);

        if (Input.GetKeyUp(moveRight.Key) || Input.GetKeyUp(moveLeft.Key))
        {
            movedImmediateHorizontal = false;
            horizontalTimer = 0;
            buttonDownWaitTimerHorizontal = 0;
        }

        if (Input.GetKeyUp(moveDown.Key))
        {
            movedImmediateVertical = false;
            verticalTimer = 0;
            buttonDownWaitTimerVertical = 0;
        }

        if (Input.GetKey(moveRight.Key))
            MoveRight();

        if (Input.GetKey(moveLeft.Key))
            MoveLeft();

        if (Input.GetKeyDown(rotate.Key))
            Rotate();

        if (Input.GetKey(moveDown.Key) || Time.time - fallTimer >= Game.Instance.FallSpeed)
            MoveDown();

        if (Input.GetKeyDown(moveToBottom.Key))
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
            if (buttonDownWaitTimerHorizontal < Game.Instance.ButtonDownWaitMax)
            {
                buttonDownWaitTimerHorizontal += Time.deltaTime;
                return;
            }

            if (horizontalTimer < Game.Instance.HorizontalSpeed)
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
        if (Game.Instance.CheckIsValidPosition(transform))
        {
            // if it is, we then call the UpdateGrid method which records this tetrominos new position
            Game.Instance.UpdateGrid(this);
            PlayMoveAudio();
            Game.Instance.MoveGhostTetromino((int)transform.position.x, transform);

            Game.SaveGameChanged = true;
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
            if (buttonDownWaitTimerHorizontal < Game.Instance.ButtonDownWaitMax)
            {
                buttonDownWaitTimerHorizontal += Time.deltaTime;
                return;
            }

            if (horizontalTimer < Game.Instance.HorizontalSpeed)
            {
                horizontalTimer += Time.deltaTime;
                return;
            }
        }
        else
            movedImmediateHorizontal = true;

        horizontalTimer = 0;
        transform.position += Vector3.right;

        if (Game.Instance.CheckIsValidPosition(transform))
        {
            Game.Instance.UpdateGrid(this);
            PlayMoveAudio();
            Game.Instance.MoveGhostTetromino((int)transform.position.x, transform);

            Game.SaveGameChanged = true;
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
            if (buttonDownWaitTimerVertical < Game.Instance.ButtonDownWaitMax)
            {
                buttonDownWaitTimerVertical += Time.deltaTime;
                return;
            }

            if (verticalTimer < Game.Instance.VerticalSpeed)
            {
                verticalTimer += Time.deltaTime;
                return;
            }
        }
        else
            movedImmediateVertical = true;

        verticalTimer = 0;
        transform.position += Vector3.down;

        if (Game.Instance.CheckIsValidPosition(transform))
        {
            Game.Instance.UpdateGrid(this);
            Game.Instance.MoveGhostTetromino((int)transform.position.x, transform);

            if (Input.GetKey(KeyCode.DownArrow))
                PlayMoveAudio();
        }
        else
        {
            transform.position += Vector3.up;
            Game.Instance.DeleteRow();

            // Check if there are any minos above the grid
            if (Game.Instance.CheckIsAboveGrid(this))
            {
                Game.Instance.GameOverScene();
                return;
            }

            Game.Instance.CurrentScore += individualScore;
            Game.Instance.SpawnTetromino();
            PlayLandAudio();
            tag = "Untagged";
            enabled = false;
        }

        Game.SaveGameChanged = true;
        fallTimer = Time.time;
    }

    /// <summary>
    /// Moves the tetromino to the bottom
    /// </summary>
    void MoveToBottom()
    {
        transform.position = new Vector2(transform.position.x, 0);

        for (int i = 1; i < Game.Instance.GridHeight; i++)
        {
            if (Game.Instance.CheckIsValidPosition(transform, checkMinosAbove: true))
            {
                Game.Instance.UpdateGrid(this);
                break;
            }

            transform.position = new Vector2(transform.position.x, i);

            if (Game.Instance.CheckIsAboveGrid(this))
            {
                Game.Instance.GameOverScene();
                return;
            }
        }

        Camera.main.GetComponent<ShakeBehaviour>().TriggerShake(.5f, .1f, 5);

        Game.Instance.DeleteRow();
        Game.Instance.CurrentScore += individualScore;
        Game.Instance.SpawnTetromino();
        PlayLandAudio();
        tag = "Untagged";
        enabled = false;

        Game.SaveGameChanged = true;
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

            if (Game.Instance.CheckIsValidPosition(transform))
            {
                Game.Instance.UpdateGrid(this);
                PlayMoveAudio();
                Game.Instance.RotateGhostTetromino(transform.rotation, transform);

                Game.SaveGameChanged = true;
            }
            else
            {
                Vector3 toMove = Game.Instance.GetUnitsToMove(transform);

                if (toMove.x != 0 || toMove.y != 0)
                {
                    transform.position += toMove;
                    if (Game.Instance.IsOtherMinoInTheWay(transform))
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

                    Game.Instance.UpdateGrid(this);
                    PlayMoveAudio();

                    Game.Instance.RotateGhostTetromino(transform.rotation, transform);
                    Game.Instance.MoveGhostTetromino((int)transform.position.x, transform);

                    Game.SaveGameChanged = true;
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
        if (Game.Instance.Options.SoundEffects)
            audioSource.PlayOneShot(moveSound);
    }

    /// <summary>
    /// Plays audio clip when the tetromino lands
    /// </summary>
    void PlayLandAudio()
    {
        if (Game.Instance.Options.SoundEffects)
            audioSource.PlayOneShot(landSound);
    }
}
