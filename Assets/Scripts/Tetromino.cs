using UnityEngine;

public class Tetromino : MonoBehaviour
{
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

    private void Start()
    {
        options = GameObject.Find("PauseManager").GetComponent<Options>();
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
            MoveRight();

        if (Input.GetKey(KeyCode.LeftArrow))
            MoveLeft();

        if (Input.GetKeyDown(KeyCode.UpArrow))
            Rotate();

        if (Input.GetKey(KeyCode.DownArrow) || Time.time - fallTimer >= Game.Instance.FallSpeed)
            MoveDown();

        if (Input.GetKeyDown(KeyCode.LeftAlt))
            MoveToBottom();
    }

    /// <summary>
    /// Moves the tetromino to the left
    /// </summary>
    private void MoveLeft()
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
        }
        else
            transform.position += Vector3.left;
    }

    /// <summary>
    /// Moves the tetromino down
    /// </summary>
    private void MoveDown()
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
                Game.Instance.GameOver();
                return;
            }

            Game.Instance.CurrentScore += individualScore;
            Game.Instance.SpawnTetromino();
            PlayLandAudio();
            tag = "Untagged";
            enabled = false;
        }

        fallTimer = Time.time;
    }

    /// <summary>
    /// Moves the tetromino to the bottom
    /// </summary>
    private void MoveToBottom()
    {
        transform.position = new Vector2(transform.position.x, 0);

        for (int i = 1; i < Game.Instance.GridHeight; i++)
        {
            if (Game.Instance.CheckIsValidPosition(transform, movedToBottom: true))
            {
                Game.Instance.UpdateGrid(this);
                break;
            }

            transform.position = new Vector2(transform.position.x, i);

            if (Game.Instance.CheckIsAboveGrid(this))
            {
                Game.Instance.GameOver();
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
    }

    /// <summary>
    /// Rotates the tetromino
    /// </summary>
    private void Rotate()
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
    private void PlayMoveAudio()
    {
        if (options.SoundEffects)
            audioSource.PlayOneShot(moveSound);
    }

    /// <summary>
    /// Plays audio clip when the tetromino lands
    /// </summary>
    private void PlayLandAudio()
    {
        if (options.SoundEffects)
            audioSource.PlayOneShot(landSound);
    }
}
