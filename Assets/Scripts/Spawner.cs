using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    //Pieces
    public GameObject[] piecePrefabs;
    private List<GameObject> piecesPool = new List<GameObject>();
    public GameObject blockPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Inicializando Spawner...");
        // Crear las piezas y desactivarlas
        foreach (GameObject prefab in piecePrefabs)
        {
            GameObject piece = Instantiate(prefab, transform.position, Quaternion.identity);
            piece.SetActive(false);
            piecesPool.Add(piece);
        }

        if (blockPrefab == null)
        {
            Debug.LogError("blockPrefab no asignado. Asegúrate de que blockPrefab esté asignado en el inspector.");
            return;
        }

        Board.InitializeGrid(blockPrefab);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SpawnNext()
    {
        //Random index
        int i = Random.Range(0, piecePrefabs.Length);

        //Spawn Group at current Position
        Instantiate(piecePrefabs[i], transform.position, Quaternion.identity);
    }

    public void ActivateNextPiece()
    {
        Debug.Log("Activando la siguiente pieza...");

        // Seleccionar una pieza aleatoria del pool
        int randomIndex = Random.Range(0, piecesPool.Count);
        GameObject piece = piecesPool[randomIndex];

        // Asegurarse de que la pieza seleccionada esté inactiva
        while (piece.activeInHierarchy)
        {
            randomIndex = Random.Range(0, piecesPool.Count);
            piece = piecesPool[randomIndex];
        }

        // Activar la pieza seleccionada
        piece.transform.position = new Vector3(5, 19, 0); // Ajusta la posición inicial si es necesario
        piece.SetActive(true);
        piece.GetComponent<Piece>().enabled = true;
    }
}
