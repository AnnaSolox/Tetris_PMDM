using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    private float lastFall = 0f;
    private bool isActive = true;

    // Start is called before the first frame update
    void Start()
    {
        // Default position not valid? Then it's game over
        if (!IsValidBoard())
        {
            Debug.Log("GAME OVER");
            Destroy(gameObject);
        }
    }

    // Update is called once per frame.
    // Implements all piece movements: right, left, rotate and down.
    void Update()
    {
        if (!isActive)
            return;

        // Move Left
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MovePiece(Vector3.left);
        }

        // Move Right
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MovePiece(Vector3.right);
        }

        // Rotate (Key UpArrow)
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            transform.rotation *= Quaternion.Euler(0, 0, 90);
            if (IsValidBoard())
                UpdateBoard();
            else
                transform.rotation *= Quaternion.Euler(0, 0, -90);  // Revert the rotation
        }

        // Move Downwards and Fall (each second)
        if (Input.GetKey(KeyCode.DownArrow))
        {
            MovePiece(Vector3.down);
        }

        // Automatic falling
        if (Time.time - lastFall >= 1)
        {
            MovePiece(Vector3.down);
            lastFall = Time.time;
        }
    }

    void MovePiece(Vector3 direction)
    {
        transform.position += direction;

        if (IsValidBoard())
        {
            UpdateBoard();
        }
        else
        {
            transform.position -= direction; // Revertir el movimiento si no es válido

            // Si el movimiento inválido fue hacia abajo, la pieza se detiene
            if (direction == Vector3.down)
            {
                isActive = false;

                Board.DeleteFullRows();
                FindFirstObjectByType<Spawner>().SpawnNext();
            }
        }
    }

    // Updates the board with the current position of the piece.
    void UpdateBoard()
    {
        // First, loop over the Board and make current positions of the piece null.
        for (int y = 0; y < Board.h; y++)
        {
            for (int x = 0; x < Board.w; ++x)
            {
                if (Board.grid[x, y] != null && Board.grid[x, y].transform.parent == transform)
                {
                    Board.grid[x, y] = null;
                }
            }
        }

        // Then loop over the blocks of the current piece and add them to the Board.
        foreach (Transform child in transform)
        {
            Vector2 v = Board.RoundVector2(child.position);
            Board.grid[(int)v.x, (int)v.y] = child.gameObject;
        }

    }

    // Returns if the current position of the piece makes the board valid or not.
    bool IsValidBoard()
    {
        foreach (Transform child in transform)
        {
            Vector2 v = Board.RoundVector2(child.position);

            // Not inside Border?
            if (!Board.InsideBorder(v))
                return false;

            // Block in grid cell (and not part of same group)?
            if (Board.grid[(int)v.x, (int)v.y] != null &&
                Board.grid[(int)v.x, (int)v.y].transform.parent != transform)
                return false;
        }
        return true;
    }
}
