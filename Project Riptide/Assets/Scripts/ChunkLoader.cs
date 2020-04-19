using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Region
{
    CHINA,
    NONE
};

public class ChunkLoader : MonoBehaviour
{
    public Region[,] map = new Region[,]{ {Region.NONE, Region.NONE, Region.NONE, Region.NONE, Region.NONE,},             // Blueprint for how to layout the world.
                                          {Region.NONE, Region.CHINA, Region.CHINA, Region.CHINA, Region.NONE},                                    
                                          {Region.NONE, Region.CHINA, Region.CHINA, Region.CHINA, Region.NONE},       
                                          {Region.NONE, Region.CHINA, Region.CHINA, Region.CHINA, Region.NONE },
                                          {Region.NONE, Region.NONE, Region.NONE, Region.NONE, Region.NONE,}};

    public Chunk[,] chunks;  // List of all the chunk prefabs
    public List<Chunk> visibleChunks; // A dynamic list of all chunks visible to the player.
    private const float _CHUNKSIDELENGTH = 100; // Base length of each chunk.
    public GameObject ship; // Player
    public Vector2 currentChunk; // The array coordinates of the chunk, not real world coordinates!!
    public Region currentRegion;
    private bool displayedAllChunks;
    public bool showAllChunks;

    // Start is called before the first frame update
    void Start()
    {
        currentRegion = Region.CHINA;
        chunks = new Chunk[map.GetLength(0), map.GetLength(1)];
        ship = GameObject.FindGameObjectWithTag("Player");
        visibleChunks = new List<Chunk>();
        showAllChunks = false;
        displayedAllChunks = false;
        int chinaCount = 1;
        int noneCount = 1;

        // Iterate through map, which acts as a blueprint for the layout of the world.
        for (int z = 0; z < map.GetLength(0); z++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                // Get which region this chunk is a part of.
                Region r = map[x, z];
                string pathName = "Chunks";
                // Take a loot at wich region this chunk is in.
                switch (r)
                {
                    // Set the chunks pathname to the next china chunk.
                    case Region.CHINA:
                        {
                            pathName = "Chunks/china/" + nameof(Region.CHINA).ToLower() + (chinaCount++);
                            break;
                        }
                    // Set the chunks pathname to the next "none" chunk
                    case Region.NONE:
                        {
                            pathName = "Chunks/none/" + nameof(Region.NONE).ToLower() + (noneCount++);
                            break;
                        }
                }
                Chunk c = new Chunk(Instantiate(Resources.Load<GameObject>(pathName), new Vector3(x * _CHUNKSIDELENGTH, 0, z * _CHUNKSIDELENGTH), Quaternion.identity), map[x, z], new Vector2(x * _CHUNKSIDELENGTH, z * _CHUNKSIDELENGTH));
                chunks[x, z] = c;
                chunks[x, z].chunk.SetActive(false);
            }
        }
        //Chunk the player starts in.
        chunks[1,1].chunk.SetActive(true);
        visibleChunks.Add(chunks[1, 1]);
        currentChunk = new Vector2(1, 1);
        // Physially move the player to the center of that chunk.
        ship.transform.position = new Vector3(_CHUNKSIDELENGTH * currentChunk.x, 1f, _CHUNKSIDELENGTH * currentChunk.y);

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
        // Display all the chunks
        if (showAllChunks)
        {
            if (!displayedAllChunks)
            {
                // Iterate through the 2D array of chunks.
                for (int i = 0; i < chunks.GetLength(0); i++)
                {
                    for (int j = 0; j < chunks.GetLength(1); j++)
                    {
                        // Chunk is not visible, make it visible.
                        if (!visibleChunks.Contains(chunks[i, j]))
                        {
                            visibleChunks.Add(chunks[i, j]);
                            chunks[i, j].chunk.SetActive(true);
                        }
                    }
                }
                displayedAllChunks = true;
            }
            return;
        }
        // Hide all the chunks.
        else if(!showAllChunks && displayedAllChunks)
        {
            // Iterate through the 2D array of chunks.
            for (int i = 0; i < chunks.GetLength(0); i++)
            {
                for (int j = 0; j < chunks.GetLength(1); j++)
                {
                    chunks[i, j].chunk.SetActive(false);
                    visibleChunks.Remove(chunks[i, j]);
                }
            }
            displayedAllChunks = false;
            return;
        }
        
        for (int x = (int)(currentChunk.x - 1); x < (int)(currentChunk.x + 2); x++)
        {
            for (int z = (int)(currentChunk.y - 1); z < (int)(currentChunk.y + 2); z++)
            {
                Rect chunkBounds = new Rect((x - .5f) * _CHUNKSIDELENGTH, (z - .5f) * _CHUNKSIDELENGTH, _CHUNKSIDELENGTH, _CHUNKSIDELENGTH);
                // Chunk is valid.
                if (x >= 0 && z >= 0 && x < map.GetLength(0) && z <map.GetLength(1) && chunks[x,z] != null)
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
