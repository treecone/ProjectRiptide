using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
using System.IO;
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
    public GameObject koiPrefab;
    public GameObject rockCrabPrefab;
    public GameObject seaSheepPrefab;
    public GameObject flowerFrogPrefab;

    private Dictionary<string, GameObject> monsters;
    private List<GameObject> enemies;

    public Chunk[,] chunks;  // List of all the chunk prefabs
    private List<Chunk> visibleChunks; // A dynamic list of all chunks visible to the player.
    private const float _CHUNKSIDELENGTH = 100; // Base length of each chunk.
    public GameObject ship; // Player
    public Vector2 currentChunkPosition; // The array coordinates of the chunk, not real world coordinates!!
    private bool displayedAllChunks; // All the chunks are currently being displayed.
    public bool showAllChunks; // Change this value in the editor to make all chunks visible.

    // Used to determine if the player has transitioned regions.
    private string previousRegion;
    public string currentRegion;

    private TextMeshProUGUI regionDisplay;

    private float unloadTimer;

    private string[] _lines;
    public int _xLen;
    public int _zLen;

    // Start is called before the first frame update
    void Start()
    {
        chunks = new Chunk[_xLen, _zLen];
        SetUpMonsters();
        LoadWorld();
        unloadTimer = 0;
        currentRegion = "china";
        ship = GameObject.FindGameObjectWithTag("Player");
        visibleChunks = new List<Chunk>();
        showAllChunks = false;
        displayedAllChunks = false;
        // Get the textmeshpro object so we can write to it.
        regionDisplay = GameObject.Find("Canvas").GetComponent<TextMeshProUGUI>();

        chunks[1,1].chunk.SetActive(true);
        visibleChunks.Add(chunks[1, 1]);
        currentChunkPosition = new Vector2(1, 1);
        // Physially move the player to the center of that chunk.
        ship.transform.position = new Vector3(_CHUNKSIDELENGTH * currentChunkPosition.x, 1f, _CHUNKSIDELENGTH * currentChunkPosition.y);
        enemies = new List<GameObject>();
    }

    public void LoadWorld()
    {
        GameObject world = GameObject.Find("World");
        // Get every line from the world text file.
        TextAsset t = Resources.Load<TextAsset>("World");
        string text = t.text;
        string[] partsOfText = text.Split(' ');
        int indexOfLines = 0;
        _lines = new string[_xLen];
        // Loop through string and extract each lines worth of 
        for(int i = 0; i < _xLen; i++)
        {
            string line = "";
            for(int j = 0; j < _zLen; j++)
            {
                line += partsOfText[indexOfLines] + " ";
                indexOfLines++;
            }
            indexOfLines++;
            _lines[i] = line;
        }

        // March through each line of the file.
        for(int x = 0; x < _xLen; x++)
        {
            // Seperate the line into an array of chunk descriptors.
            string[] parts = _lines[x].Split(' ');
            // March through each chunk descriptor.
            for(int z = 0; z < _zLen; z++)
            {
                string regionText = parts[z];
                int index = regionText.IndexOf("<");
                string description = "";
                bool hasEnemies = false;
                int numEnemies = 0;
                List<GameObject> enemies = new List<GameObject>();
                if (index > -1)
                {
                    hasEnemies = true;
                    description = regionText.Substring(index);
                    regionText = regionText.Substring(0, index);
                    index = 0;
                }
                // Load in enemies as they are described in the text file.
                while(index > -1)
                {
                   // Debug.Log(description);
                    string[] details = description.Substring(index + 1, description.IndexOf('>') - index - 1).Split('|');
                    string enemyName = details[0];
                    numEnemies = Int32.Parse(details[1]);
                    for (int i = 2; i < 2 + numEnemies; i++)
                    {
                        Debug.Log(i - 2);
                        string coords = details[i];
                        float xCoord = float.Parse(coords.Substring(1, coords.IndexOf(",") - 1)) + (x - 0.5f) * _CHUNKSIDELENGTH;
                        float zCoord = float.Parse(coords.Substring(coords.IndexOf(",") + 1, coords.IndexOf(")") - coords.IndexOf(",") - 1))
                            + (z - 0.5f) * _CHUNKSIDELENGTH;
                        //Debug.Log(enemyName);
                        GameObject enemy = Instantiate(GetPrefabByName(enemyName), new Vector3(xCoord, 0, zCoord), Quaternion.identity);
                        enemy.SetActive(false);

                        // Cash the starting position of this enemy
                        enemy.GetComponent<Enemy>().EnemyStartingPosition = new Vector2(xCoord, zCoord);
                        enemy.GetComponent<Enemy>().EnemyStartingChunk = new Vector2(x, z);
                        enemy.GetComponent<Enemy>().EnemyID = i - 2;
                        enemy.transform.parent = world.transform;
                        // Store a reference in the list of enemies corresponding to this species.
                        enemies.Add(enemy);
                    }
                    // Look at the next portion of the string.
                    description = description.Substring(description.IndexOf(">") + 1);
                    // Get the index of the next detail if it exists, otherwise index becomes -1.
                    index = description.IndexOf("<");
                }
                string pathName = "Chunks";
                Region r = Region.OCEAN;
                bool hasMonster = false;

                switch (regionText)
                {
                    // Set the chunks pathname to the next china chunk.
                    case "CHINA":
                        {
                            pathName = "Chunks/china/china";
                            r = Region.CHINA;
                            break;
                        }

                    case "CHINA_KOI":
                        {
                            pathName = "Chunks/china/china_koi";
                            r = Region.CHINA_KOI;
                            hasMonster = true;
                            break;
                        }
                    case "CHINA_ISLAND1":
                        {
                            pathName = "Chunks/china/china_island1";
                            r = Region.CHINA_ISLAND1;
                            break;
                        }
                    case "CHINA_ISLAND2":
                        {
                            pathName = "Chunks/china/china_island2";
                            r = Region.CHINA_ISLAND2;
                            break;
                        }
                    case "CHINA_ISLAND3":
                        {
                            pathName = "Chunks/china/china_island3";
                            r = Region.CHINA_ISLAND3;
                            break;
                        }
                    // Set the chunks pathname to the next "none" chunk
                    case "OCEAN":
                        {
                            pathName = "Chunks/none/ocean";
                            r = Region.OCEAN;
                            break;
                        }
                }
                GameObject obj = Instantiate(Resources.Load<GameObject>(pathName), new Vector3(x * _CHUNKSIDELENGTH, 0, z * _CHUNKSIDELENGTH), Quaternion.identity);
                obj.transform.parent = world.transform;
                Chunk c = new Chunk(obj, r, new Vector2(x * _CHUNKSIDELENGTH, z * _CHUNKSIDELENGTH), enemies);
                c.hasMonster = hasMonster;
                c.hasEnemies = hasEnemies;
                chunks[x, z] = c;
                chunks[x, z].chunk.SetActive(false);
            }
        }
    }
    // Set up the dictionaries of monsters.
    public void SetUpMonsters()
    {
        monsters = new Dictionary<string, GameObject>();
        monsters.Add("koi", null);

        // Add more monsters

    }
    // Load a monster of the specified name at the specifed positon.
    public void LoadMonster(Vector2 position, Vector2 chunk, string name)
    {
        switch (name)
        {
            // Load the monster.
            case "koi":
                {
                    monsters[name] = Instantiate(koiPrefab, new Vector3(position.x, 0, position.y), Quaternion.identity);
                    monsters[name].GetComponent<Enemy>().EnemyStartingChunk = chunk;
                    break;
                }
        }
    }
    public void UnloadMonster(string name)
    {
        Destroy(monsters[name]);
        monsters[name] = null;
    }
    // March through each monster and unload the monster if.
    // 1) The player is more than 2 chunks away from the monster AND
    //      a. The monster is Passive.
    public void CheckUnloadMonster()
    {
        // List of monsters to unload.
        List<string> destroyMonsters = new List<string>();
        // Loop through each potential monster.
        foreach(KeyValuePair<string, GameObject> kvpair in monsters)
        {
            string name = kvpair.Key;
            GameObject monster = kvpair.Value;
            // Monster exists and is roaming...
            if (monster)
            {
                Vector2 start = monster.GetComponent<Enemy>().EnemyStartingChunk;
                bool playerCloseToMonster = Mathf.Sqrt(Mathf.Pow(ship.transform.position.x - monster.transform.position.x, 2) + Mathf.Pow(ship.transform.position.z - monster.transform.position.z, 2)) < 1 * Mathf.Sqrt(2 * Mathf.Pow(_CHUNKSIDELENGTH / 2, 2));
                Debug.Log((DistanceFromChunkCenter(monster, (int)start.x, (int)start.y)));
                Debug.Log(start);
                // Monster is passive, and player is in a different chunk than it was loaded in, delete the monster. OR Monster is passive and is in a chunk it wasn't spawned in.
                if ((!playerCloseToMonster && monster.GetComponent<Enemy>().State == EnemyState.Passive) || monster.GetComponent<Enemy>().ReadyToDelete)
                {
                    // Add this monster to the list of monsters to destroy.
                    destroyMonsters.Add(name);
                }
            }
        }
        // Unload each monster in this list.
        foreach(string n in destroyMonsters)
        {
            UnloadMonster(n);
        }

        // Check each smaller enemy and unload it if neccessary.
        for(int i = 0; i < enemies.Count; i++)
        {
            bool enemyIsFar = Mathf.Sqrt(Mathf.Pow(enemies[i].transform.position.x - ship.transform.position.x, 2) 
                + Mathf.Pow(enemies[i].transform.position.z - ship.transform.position.z, 2)) > 2 * Mathf.Sqrt(2 * Mathf.Pow(_CHUNKSIDELENGTH / 2, 2));
            if (enemyIsFar || enemies[i].GetComponent<Enemy>().ReadyToDelete)
            {
                // Get the origin chunk of the enemy.
                Vector2 originChunk = enemies[i].GetComponent<Enemy>().EnemyStartingChunk;
                // Mark this enemy as unloaded.
                chunks[(int)originChunk.x, (int)originChunk.y].enemieIsLoaded[enemies[i].GetComponent<Enemy>().EnemyID] = false;
                // Hide the enemy.
                enemies[i].SetActive(false);
                // Remove it from the list of enemies.
                enemies.RemoveAt(i);
                i--;
            }
        }
    }
    public void ResetEnemy(GameObject enemy)
    {
        Vector2 pos = enemy.GetComponent<Enemy>().EnemyStartingPosition;
        Debug.Log(pos);
        enemy.GetComponent<Enemy>().Position = new Vector3(pos.x, 0, pos.y);
        // other resetting stuffs
    }
    public void CheckLoadEnemies(Chunk c)
    {
        List<GameObject> e = c.enemies;
        // Look through each enemy and see who needs to be loaded in.
        for(int i = 0; i < e.Count; i++)
        {
            // Enemy is not loaded in.
            if (!c.enemieIsLoaded[i])
            {
                ResetEnemy(e[i]);
                e[i].SetActive(true);
                c.enemieIsLoaded[i] = true;
                enemies.Add(e[i]);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        previousRegion = currentRegion;
        DisplayChunks();
        //Determine if the players has entered a new region.
        //if (!currentRegion.Equals(previousRegion))
        //{
        //    // Display the text for a few seconds.
        //    StartCoroutine(DisplayRegion(currentRegion));
        //}

    }
    // Fade in and out for region switching.
    //private IEnumerator DisplayRegion(string newRegion)
    //{
    //    regionDisplay.text = "Now Entering " +newRegion.ToUpper();
    //    regionDisplay.enabled = true;
    //    Color originalColor = new Color(regionDisplay.color.r, regionDisplay.color.g, regionDisplay.color.b, 0);
    //    while(regionDisplay.color.a < 1.0f)
    //    {
    //        regionDisplay.color = new Color(regionDisplay.color.r, regionDisplay.color.g, regionDisplay.color.b, regionDisplay.color.a + (Time.deltaTime * 1));
    //        yield return null;
    //    }
    //    yield return new WaitForSeconds(2f);
    //    while (regionDisplay.color.a > 0f)
    //    {
    //        regionDisplay.color = new Color(regionDisplay.color.r, regionDisplay.color.g, regionDisplay.color.b, regionDisplay.color.a - (Time.deltaTime * 1));
    //        yield return null;
    //    }
    //    regionDisplay.enabled = false;
    //}
    /// <summary>
    /// Gets the distance between the ship and the specified chunks center.
    /// </summary>
    /// <param name="x"> The row of the array of chunks.</param>
    /// <param name="z"> The col of the array of chunks.</param>
    /// <returns> The distance between the ship and the chunks center.</returns>
    public float DistanceFromChunkCenter(GameObject g, int x, int z)
    {
        return Mathf.Sqrt(Mathf.Pow(g.transform.position.x - (x * _CHUNKSIDELENGTH), 2)
            + Mathf.Pow(g.transform.position.z - (z * _CHUNKSIDELENGTH), 2));
    }
    /// <summary>
    /// Updates the current chunk field to accurately represent which chunk the user is in.
    /// </summary>
    public void DisplayChunks()
    {
        if(!displayedAllChunks && showAllChunks)
        {
            for(int i = 0; i < chunks.Length; i++)
            {
                for(int j = 0; j < chunks.GetLength(1); j++)
                {
                    chunks[i, j].chunk.SetActive(true);
                }
            }
            displayedAllChunks = true;
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
                if (x >= 0 && z >= 0 && x < _xLen && z < _zLen && chunks[x, z] != null)
                {
                    // If the chunk is close enough to render.
                    bool close = DistanceFromChunkCenter(ship, x, z) < /*Mathf.Sqrt(2 * Mathf.Pow(_CHUNKSIDELENGTH / 2, 2))*/150f;
                    bool inVisibleChunks = visibleChunks.Contains(chunks[x, z]);
                    // Chunk is close enough to render so do so.
                    if (close && !inVisibleChunks)
                    {
                        // Set the chunk as active and add it to the list of active chunks.
                        chunks[x, z].chunk.SetActive(true);
                        visibleChunks.Add(chunks[x, z]);
                        // Get the name of the boss to add if there is supposed to be a boss in this chunk.
                        string monsterName = GetMonsterName(chunks[x, z].region);
                        // Monster is not yet loaded in this chunk
                        if (monsters.ContainsKey(monsterName) && monsters[monsterName] == null)
                        {
                            // Load in the monster.
                            LoadMonster(chunks[x, z].center, new Vector2(x,z), monsterName);
                        }
                        // There are enemies that are supposed to be in this chunk so see if they need to be loaded in...
                        if (chunks[x, z].hasEnemies)
                        {
                            CheckLoadEnemies(chunks[x, z]);
                        }
                    }
                    // Chunk is no longer within viewing distance of the ship, so unload it.
                    if (!close && inVisibleChunks)
                    {
                        // Deactivate this chunk
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
        unloadTimer += Time.deltaTime;
        if(unloadTimer > 1)
        {
            CheckUnloadMonster();
            unloadTimer = 0;
        }
    }
    // Given the region label, return as a string which region this chunk comes from.
    // Example CHINA_KOI would return "china"
    public string GetRegionName(Region region)
    {
        string name ="NOTHING";
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

    public string GetMonsterName(Region r)
    {
        switch (r)
        {
            case Region.CHINA_KOI:
                    return "koi";
        }
        return "NONE";
    }
    public GameObject GetPrefabByName(string s)
    {
        switch (s)
        {
            case "rockCrab":
                return rockCrabPrefab;
            case "seaSheep":
                return seaSheepPrefab;
            case "koi":
                return koiPrefab;
            case "flowerFrog":
                return flowerFrogPrefab;
        }
        return null;
    }
}
