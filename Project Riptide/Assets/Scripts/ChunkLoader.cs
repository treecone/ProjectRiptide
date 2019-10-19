using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkLoader : MonoBehaviour
{
    public Chunk[,] chunks;
    public List<Chunk> visibleChunks;
    private const float _CHUNKSIDELENGTH = 100;
    private const float _CHINASIZE = 3;
    private const int _WORLDSIZE = 3;
    public GameObject ship;
    public Vector2 currentChunk;

    // Start is called before the first frame update
    void Start()
    {
        chunks = new Chunk[_WORLDSIZE, _WORLDSIZE];
        ship = GameObject.FindGameObjectWithTag("Player");
        visibleChunks = new List<Chunk>();
        //Load China Chunks
        int count = 1;
        for (int x = 0; x < _CHINASIZE; x++)
        {
            for (int z = 0; z < _CHINASIZE; z++)
            {
                Chunk c = new Chunk(Instantiate(Resources.Load<GameObject>("Level" + count), new Vector3(x * _CHUNKSIDELENGTH, 0, z * _CHUNKSIDELENGTH), Quaternion.identity), "China", new Vector2(x * _CHUNKSIDELENGTH, z * _CHUNKSIDELENGTH));
                chunks[x, z] = c;
                chunks[x, z].chunk.SetActive(false);
                count++;
            }
        }
        //Chunk the player starts in.
        chunks[0, 0].chunk.SetActive(true);
        visibleChunks.Add(chunks[0, 0]);
        currentChunk = new Vector2(0, 0);

        /* TODO Load other regions chunks
         * 
         */
    }

    // Update is called once per frame
    void Update()
    {
        DisplayChunks();
    }
    /// <summary>
    /// Gets the distance between the ship and the specified chunks center.
    /// </summary>
    /// <param name="x"> The row of the array of chunks.</param>
    /// <param name="z"> The col of the array of chunks.</param>
    /// <returns> The distance between the ship and the chunks center.</returns>
    public float DistanceFromChunkCenter(int x, int z)
    {
        return Mathf.Sqrt(Mathf.Pow(ship.transform.position.x - (x * _CHUNKSIDELENGTH), 2)
            + Mathf.Pow(ship.transform.position.z - (z * _CHUNKSIDELENGTH), 2));
    }
    /// <summary>
    /// Updates the current chunk field to accurately represent which chunk the user is in.
    /// </summary>
    public void DisplayChunks()
    {
        for (int x = (int)(currentChunk.x - 1); x < (int)(currentChunk.x + 2); x++)
        {
            for (int z = (int)(currentChunk.y - 1); z < (int)(currentChunk.y + 2); z++)
            {
                Rect chunkBounds = new Rect((x - .5f) * _CHUNKSIDELENGTH, (z - .5f) * _CHUNKSIDELENGTH, _CHUNKSIDELENGTH, _CHUNKSIDELENGTH);
                // Chunk is valid.
                if (x >= 0 && z >= 0 && x < _WORLDSIZE && z <_WORLDSIZE && chunks[x,z] != null)
                {
                    bool close = (DistanceFromChunkCenter(x, z) < Mathf.Sqrt(2 * Mathf.Pow(_CHUNKSIDELENGTH / 2, 2)));
                    // Ship was in a different chunk and has now moved into this chunk.
                    if ((currentChunk.x != x || currentChunk.y != z) && chunkBounds.Contains(new Vector2(ship.transform.position.x, ship.transform.position.z)))
                    {
                        // Set current chunk to the chunk the ships in.
                        currentChunk = new Vector2(x, z);
                        chunks[x, z].chunk.SetActive(true);
                        visibleChunks.Add(chunks[x, z]);
                    }
                    // If its the current chunk, skip over it.
                    else if(currentChunk.x == x && currentChunk.y == z)
                    {
                        continue;
                    }
                    // Ship is within viewing distance of the chunk, and it is currently not being displayed.
                    else if (close && !visibleChunks.Contains(chunks[x, z]))
                    {
                        // Show the chunk.
                        chunks[x, z].chunk.SetActive(true);
                        visibleChunks.Add(chunks[x, z]);

                            chunks[x, z].chunk.SetActive(false);
                            visibleChunks.Remove(chunks[x, z]);
                    }
                    else if(!close && visibleChunks.Contains(chunks[x, z]))
                    {
                        chunks[x, z].chunk.SetActive(false);
                        visibleChunks.Remove(chunks[x, z]);
                    }
                }
            }
        }
    }
}
