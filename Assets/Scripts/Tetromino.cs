using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Tetromino : MonoBehaviour
{
    private float fall = 0;
    [SerializeField]
    private float fallSpeed = 1;
    [SerializeField]
    private bool allowRotation = true;
    [SerializeField]
    private bool limitRotation;
    private Game game;

    private void Start()
    {
        game = FindObjectOfType<Game>();
    }

    private void Update()
    {
        CheckUserInput();
    }

    private void CheckUserInput()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.position += new Vector3(1, 0);

            if (CheckIsValidPosition())
                game.UpdateGrid(this);
            else
                transform.position += new Vector3(-1, 0);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.position += new Vector3(-1, 0);

            if (CheckIsValidPosition())
                game.UpdateGrid(this);
            else
                transform.position += new Vector3(1, 0);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
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

                if (CheckIsValidPosition())
                    game.UpdateGrid(this);
                else
                {
                    int toMove = GetUnitsToMove();

                    if (toMove > 0 || toMove < 0)
                        transform.position += new Vector3(toMove, 0);
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
        else if (Time.time - fall > (Input.GetKey(KeyCode.DownArrow) ? fallSpeed / 10 : fallSpeed))
        {
            transform.position += new Vector3(0, -1);

            if (CheckIsValidPosition())
                game.UpdateGrid(this);
            else
            {
                transform.position += new Vector3(0, 1);

                game.DeleteRow();
                enabled = false;

                game.SpawnNextTetromino();
            }

            fall = Time.time;
        }
    }

    private bool CheckIsValidPosition()
    {
        foreach (Transform mino in transform)
        {
            Vector2 pos = game.Round(mino.position);

            if (!game.CheckIsInsideGrid(pos))
                return false;

            if (game.GetTransformAtGridPosition(pos) != null && game.GetTransformAtGridPosition(pos).parent != transform)
                return false;
        }

        return true;
    }

    private int GetUnitsToMove()
    {
        foreach (Transform mino in transform)
        {
            Vector3 roundedPos = game.Round(mino.position);

            if (roundedPos.x < 0)
                return (int)roundedPos.x * -1;
            else if (roundedPos.x >= Game.GridWidth)
                return ((int)roundedPos.x - 9) * -1;
        }

        return 0;
    }
}
