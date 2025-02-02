using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    private float lastFall = 0f;

    // Start is called before the first frame update
    void Start()
    {
        // Default position not valid? Then it's game over
        if (!IsValidBoard())
        {
            /* Debug.Log("GAME OVER");
            Destroy(gameObject); */
        }
    }

    // Update is called once per frame.
    // Implements all piece movements: right, left, rotate and down.
    void Update()
    {
        if (!enabled)
            return;

        // Move Left
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MovePiece(new Vector3(-1, 0, 0));
        }

        // Move Right
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MovePiece(new Vector3(1, 0, 0));
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
        if (Input.GetKey(KeyCode.DownArrow) || Time.time - lastFall >= 1)
        {
            MovePiece(new Vector3(0, -1, 0));
            lastFall = Time.time;
        }
    }

    void MovePiece(Vector3 direction)
    {
        transform.position += direction;
        Debug.Log($"Moviendo pieza a {transform.position}");
        if (IsValidBoard())
        {
            UpdateBoard();
        }
        else
        {
            transform.position -= direction; // Revertir el movimiento si no es válido
            Debug.Log("Movimiento inválido, revirtiendo...");

            // Si el movimiento inválido fue hacia abajo, la pieza se detiene
            if (direction == new Vector3(0, -1, 0))
            {
                Debug.Log("Pieza se detiene, activando bloques...");
                foreach (Transform child in transform)
                {
                    Vector2 v = Board.RoundVector2(child.position);
                    Debug.Log($"Activando bloque en posición {v}");
                    Board.ActivateBlock((int)v.x, (int)v.y);
                }

                FindFirstObjectByType<Spawner>().ActivateNextPiece();
                transform.position = new Vector3(-100, -100, 0); // Mover a una ubicación no visible
                gameObject.SetActive(false);
                Board.DeleteFullRows();
                enabled = false;
            }
        }
    }

    // Updates the board with the current position of the piece.
    void UpdateBoard()
    {
        Debug.Log("Actualizando el tablero con la posición actual de la pieza.");
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
    public bool IsValidBoard()
    {
        foreach (Transform child in transform)
        {
            Vector2 v = Board.RoundVector2(child.position);
            Debug.Log($"Verificando posición {v}");

            // Not inside Border?
            if (!Board.InsideBorder(v))
            {
                Debug.Log("Posición fuera de los límites.");
                return false;
            }

            // Block in grid cell (and not part of same group)?
            if (Board.grid[(int)v.x, (int)v.y] != null)
            {
                Debug.Log($"Bloque encontrado en posición {v}");
                if (Board.grid[(int)v.x, (int)v.y].transform.parent != transform)
                {
                    Debug.Log($"Bloque en posición {v} no es parte de la misma pieza.");
                    if (Board.grid[(int)v.x, (int)v.y].activeInHierarchy)
                    {
                        Debug.Log("Colisión detectada.");
                        return false;
                    }
                }
            }
        }
        return true;
    }
}
