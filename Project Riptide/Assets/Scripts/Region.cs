using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Region : MonoBehaviour
{
    public GameObject[,] chunks;
    public int size;
    private string region;
    public Vector2 startPos;
    private const float _CHUNKSIDELENGTH = 100;
    //The first component is the x, the second is the z;
    public Vector2 currentChunk;
    public GameObject ship;

    public Region(string region, int size, Vector2 startPos)
    {
        this.region = region;
        this.size = size;
        this.startPos = startPos;
        currentChunk = startPos + new Vector2(_CHUNKSIDELENGTH / 2, _CHUNKSIDELENGTH / 2);
        chunks = new GameObject[size, size];
        ship = GameObject.FindGameObjectWithTag("Player");

        int count = 1;
        for (int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                chunks[x, z] = Instantiate(Resources.Load<GameObject>("Level" + count),
                    new Vector3(x * _CHUNKSIDELENGTH, 0, z * _CHUNKSIDELENGTH), Quaternion.identity);
                count++;
                chunks[x, z].SetActive(false);
            }
        }
        
        chunks[0, 0].SetActive(true);
    }


    public float DistanceFromChunkCenter(int x, int z)
    {
        return Mathf.Sqrt(Mathf.Pow(ship.transform.position.x - (x * _CHUNKSIDELENGTH) , 2) 
            + Mathf.Pow(ship.transform.position.z - (z * _CHUNKSIDELENGTH),2));
    }
    /// <summary>
    /// Store the current chunk the ship is in, show that chunk, and also show chunks that are withing viewing distance of the ship.
    /// </summary>
    public void ShowRelevantChunks()
    {
        for (int x = (int)(currentChunk.x-1); x < (int)(currentChunk.x + 2); x++)
        {
            for (int z = (int)(currentChunk.y - 1); z < (int)(currentChunk.y + 2); z++)
            {
                Rect chunkBounds = new Rect((x - .5f) * _CHUNKSIDELENGTH, (z - .5f) * _CHUNKSIDELENGTH, _CHUNKSIDELENGTH, _CHUNKSIDELENGTH);
                // Chunk is valid.
                if (x >= startPos.x + _CHUNKSIDELENGTH /2 && z >= startPos.y + _CHUNKSIDELENGTH / 2 
                    && x < startPos.x + _CHUNKSIDELENGTH / 2 + size && z < startPos.y + _CHUNKSIDELENGTH / 2 + size )
                {
                    // Ship is in this chunk.
                    if (chunkBounds.Contains(new Vector2(ship.transform.position.x, ship.transform.position.z)))
                    {
                        chunks[x, z].SetActive(true);
                        currentChunk = new Vector2(x, z);
                    }
                    // Ship is withing viewing distance of this chunk.
                    else if (DistanceFromChunkCenter(x, z) < Mathf.Sqrt(2 * Mathf.Pow(_CHUNKSIDELENGTH / 2, 2)))
                    {
                        chunks[x, z].SetActive(true);
                    }
                    // Ship is not in this chunk, nor is within viewing distance of it.
                    else
                    {
                        chunks[x, z].SetActive(false);
                    }
                }
            }
        }
    }

}
