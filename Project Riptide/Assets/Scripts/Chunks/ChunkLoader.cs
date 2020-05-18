﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using LitJson;

// Represents the name of the region, followed by an underscore and any additional information if needed.
public enum Region
{
    CHINA,
    CHINA_KOI,
    CHINA_ISLAND1,
    CHINA_ISLAND2,
    CHINA_ISLAND3,
    OCEAN
};

public class ChunkLoader : MonoBehaviour
{
    public Region[,] map = new Region[,]{ {Region.OCEAN, Region.OCEAN, Region.OCEAN, Region.OCEAN, Region.OCEAN,},             // Blueprint for how to layout the world.
                                          {Region.OCEAN, Region.CHINA_ISLAND1, Region.CHINA_ISLAND1, Region.CHINA_ISLAND1, Region.OCEAN},                                    
                                          {Region.OCEAN, Region.CHINA_ISLAND1, Region.CHINA_KOI, Region.CHINA_ISLAND1, Region.OCEAN},       
                                          {Region.OCEAN, Region.CHINA_ISLAND1, Region.CHINA_ISLAND1, Region.CHINA_ISLAND1, Region.OCEAN},
                                          {Region.OCEAN, Region.OCEAN, Region.OCEAN, Region.OCEAN, Region.OCEAN}};

    public Chunk[,] chunks;  // List of all the chunk prefabs
    public List<Chunk> visibleChunks; // A dynamic list of all chunks visible to the player.
    private const float _CHUNKSIDELENGTH = 100; // Base length of each chunk.
    public GameObject ship; // Player
    public Vector2 currentChunkPosition; // The array coordinates of the chunk, not real world coordinates!!
    private bool displayedAllChunks; // All the chunks are currently being displayed.
    public bool showAllChunks; // Change this value in the editor to make all chunks visible.

    // Used to determine if the player has transitioned regions.
    public string previousRegion;
    public string currentRegion;

    private TextMeshProUGUI regionDisplay;

    // Start is called before the first frame update
    void Start()
    {
        currentRegion = "china";
        chunks = new Chunk[map.GetLength(0), map.GetLength(1)];
        ship = GameObject.FindGameObjectWithTag("Player");
        visibleChunks = new List<Chunk>();
        showAllChunks = false;
        displayedAllChunks = false;
        // Get the textmeshpro object so we can write to it.
        regionDisplay = GameObject.Find("Canvas").GetComponent<TextMeshProUGUI>();
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
                            pathName = "Chunks/china/" + nameof(Region.CHINA).ToLower();
                            break;
                        }
                         
                    case Region.CHINA_KOI:
                        {
                            pathName = "Chunks/china/" + nameof(Region.CHINA_KOI).ToLower();
                            break;
                        }
                    case Region.CHINA_ISLAND1:
                        {
                            pathName = "Chunks/china/" + nameof(Region.CHINA_ISLAND1).ToLower();
                            break;
                        }
                    // Set the chunks pathname to the next "none" chunk
                    case Region.OCEAN:
                        {
                            pathName = "Chunks/none/" + nameof(Region.OCEAN).ToLower();
                            break;
                        }
                }
                // Create a chunk object representing this chunk
                Chunk c = new Chunk(Instantiate(Resources.Load<GameObject>(pathName), new Vector3(x * _CHUNKSIDELENGTH, 0, z * _CHUNKSIDELENGTH), Quaternion.identity), map[x, z], new Vector2(x * _CHUNKSIDELENGTH, z * _CHUNKSIDELENGTH));
                chunks[x, z] = c;
                chunks[x, z].chunk.SetActive(false);
                Debug.Log(c);
            }
        }
        //Chunk the player starts in.
        chunks[1,1].chunk.SetActive(true);
        visibleChunks.Add(chunks[1, 1]);
        currentChunkPosition = new Vector2(1, 1);
        // Physially move the player to the center of that chunk.
        ship.transform.position = new Vector3(_CHUNKSIDELENGTH * currentChunkPosition.x, 1f, _CHUNKSIDELENGTH * currentChunkPosition.y);

    }

    // Update is called once per frame
    void Update()
    {
        previousRegion = currentRegion;
        DisplayChunks();
        //Determine if the players has entered a new region.
        if (!currentRegion.Equals(previousRegion))
        {
            // Display the text for a few seconds.
            StartCoroutine(DisplayRegion(currentRegion));
        }

    }
    // Fade in and out for region switching.
    private IEnumerator DisplayRegion(string newRegion)
    {
        regionDisplay.text = "Now Entering " +newRegion.ToUpper();
        regionDisplay.enabled = true;
        Color originalColor = new Color(regionDisplay.color.r, regionDisplay.color.g, regionDisplay.color.b, 0);
        while(regionDisplay.color.a < 1.0f)
        {
            regionDisplay.color = new Color(regionDisplay.color.r, regionDisplay.color.g, regionDisplay.color.b, regionDisplay.color.a + (Time.deltaTime * 1));
            yield return null;
        }
        yield return new WaitForSeconds(2f);
        while (regionDisplay.color.a > 0f)
        {
            regionDisplay.color = new Color(regionDisplay.color.r, regionDisplay.color.g, regionDisplay.color.b, regionDisplay.color.a - (Time.deltaTime * 1));
            yield return null;
        }
        regionDisplay.enabled = false;
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
        //// Display all the chunks
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
        else if (!showAllChunks && displayedAllChunks)
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
        // The current chunk position before the loop executes and potentially alters the current chunk positon.
        Vector2 stashedChunkPos = currentChunkPosition;
        // Check the 8 surrounding chunks.
        for (int x = (int)(stashedChunkPos.x - 1); x < (int)(stashedChunkPos.x + 2); x++)
        {
            for (int z = (int)(stashedChunkPos.y - 1); z < (int)(stashedChunkPos.y + 2); z++)
            {
                // This is the current chunk so skip over it.
                if (stashedChunkPos.x == x && stashedChunkPos.y == z)
                {
                    continue;
                }
                // The bounds of this chunk.
                Rect chunkBounds = new Rect((x - .5f) * _CHUNKSIDELENGTH, (z - .5f) * _CHUNKSIDELENGTH, _CHUNKSIDELENGTH, _CHUNKSIDELENGTH);
                // Chunk is valid.
                if (x >= 0 && z >= 0 && x < map.GetLength(0) && z < map.GetLength(1) && chunks[x,z] != null)
                {
                    // If the chunk is close enough to render.
                    bool close = (DistanceFromChunkCenter(x, z) < Mathf.Sqrt(2 * Mathf.Pow(_CHUNKSIDELENGTH / 2, 2)));
                    bool inVisibleChunks = visibleChunks.Contains(chunks[x, z]);
                    // Chunk is close enough to render so do so.
                    if (close && !inVisibleChunks)
                    {
                        chunks[x, z].chunk.SetActive(true);
                        visibleChunks.Add(chunks[x, z]);
                    }
                    // Chunk is no longer within viewing distance of the ship, so unload it.
                    if (!close && inVisibleChunks)
                    {
                        chunks[x, z].chunk.SetActive(false);
                        visibleChunks.Remove(chunks[x, z]);
                    }
                    // Ship was in a different chunk and has now moved into this chunk, make it the current chunk.
                    if ((stashedChunkPos.x != x || stashedChunkPos.y != z) && chunkBounds.Contains(new Vector2(ship.transform.position.x, ship.transform.position.z)))
                    {
                        // Set current chunk to the chunk the ships in.
                        currentChunkPosition = new Vector2(x, z);
                        currentRegion = GetRegionName(chunks[x, z].region);
                    }
                }
            }
        }
    }
    // Given the region label, return as a string which region this chunk comes from.
    // Example CHINA_KOI would return "china"
    public string GetRegionName(Region region)
    {
        string name ="";
        switch (region)
        {
            case Region.CHINA:
                {
                    name = "china";
                    break;
                }
            case Region.CHINA_KOI:
                {
                    name = "china";
                    break;
             
                }
            case Region.CHINA_ISLAND1:
                {
                    name = "china";
                    break;
                }
            case Region.OCEAN:
                {
                    name = "ocean";
                    break;
                }
        }
        return name;
    }

}