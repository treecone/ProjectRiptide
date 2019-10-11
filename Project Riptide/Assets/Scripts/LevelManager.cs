using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public GameObject[,] chunks;
    private int size = 3;
    private const float _CHUNKSIDELENGTH = 100;
    //The first component is the x, the second is the z;
    public Vector2 currentChunk;
    public GameObject ship;


    private void Start()
    {
        currentChunk = new Vector2(0, 0);
        chunks = new GameObject[size, size];
        ship = GameObject.FindGameObjectWithTag("Player");

        int count = 1;
        for(int x = 0; x < size; x++)
        {
            for(int z = 0; z < size; z++)
            {
                chunks[x,z] = Instantiate(Resources.Load<GameObject>("Level" +count), new Vector3(x * _CHUNKSIDELENGTH, 0, z * _CHUNKSIDELENGTH), Quaternion.identity);
                count++;
                chunks[x, z].SetActive(false);
            }
        }

        chunks[0, 0].SetActive(true);
    }


    private float DistanceFromChunkCenter(int x, int z)
    {
        return Mathf.Sqrt(Mathf.Pow(ship.transform.position.x - (x * _CHUNKSIDELENGTH) , 2) + Mathf.Pow(ship.transform.position.z - (z * _CHUNKSIDELENGTH),2));
    }
    /// <summary>
    /// Determine which chunk the ship is in by checking the 8 surrounding chunks.
    /// </summary>
    /// <returns> A vector2 representing the x and z of the chunk.
    /// *Note that the chunk may not actually exist, so you need to check if it does.
    private Vector2 GetCurrentChunk()
    {
        for (int x = (int)(currentChunk.x-1); x < (int)(currentChunk.x + 2); x++)
        {
            for (int z = (int)(currentChunk.y - 1); z < (int)(currentChunk.y + 2); z++)
            {
                Rect chunkBounds = new Rect((x - .5f) * _CHUNKSIDELENGTH, (z - .5f) * _CHUNKSIDELENGTH, _CHUNKSIDELENGTH, _CHUNKSIDELENGTH);
                if (chunkBounds.Contains(new Vector2(ship.transform.position.x, ship.transform.position.z)))
                {
                    return new Vector2(x, z);
                }
            }
        }
        return new Vector2(-1,-1);
    }

    private void Update()
    {
        currentChunk = GetCurrentChunk();
        Debug.Log("" + currentChunk.x + ", " + currentChunk.y);
        for (int x = (int)(currentChunk.x - 1); x < (int)(currentChunk.x + 2); x++)
        {
            for (int z = (int)(currentChunk.y - 1); z < (int)(currentChunk.y + 2); z++)
            {
                //The chunk actually exists.
                if (x >= 0 && x < size && z >= 0 && z < size && !(x == currentChunk.x && z == currentChunk.y))
                {
                    if (DistanceFromChunkCenter(x, z) <= Mathf.Sqrt(Mathf.Pow(_CHUNKSIDELENGTH / 2, 2) + Mathf.Pow(_CHUNKSIDELENGTH / 2, 2)))
                    {
                        chunks[x, z].SetActive(true);
                    }
                    else
                    {
                        chunks[x, z].SetActive(false);
                    }
                }
            }
        }

    }
}
