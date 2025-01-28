using UnityEngine;

public class Spawner : MonoBehaviour
{
    //Pieces
    public GameObject[] pieces;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnNext();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnNext()
    {
        //Random index
        int i = Random.Range(0, pieces.Length);

        //Spawn Group at current Position
        Instantiate(pieces[i], transform.position, Quaternion.identity);
    }
}
