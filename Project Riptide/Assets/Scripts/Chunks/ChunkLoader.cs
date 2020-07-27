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
    OCEAN,
    OUT_OF_BOUNDS
};

public class ChunkLoader : MonoBehaviour
{
    private Dictionary<string, GameObject> _monsters;
    private List<GameObject> _enemies;

    private Chunk[,] _chunks;  // List of all the chunk prefabs
    private List<Chunk> _visibleChunks; // A dynamic list of all chunks visible to the player.
    private float _chunkSideLength = 800; // Base length of each chunk.
    [SerializeField]
    private GameObject _ship; // Player
    [SerializeField]
    private Vector2 _currentChunkPosition; // The array coordinates of the chunk, not real world coordinates!
    private bool _displayedAllChunks; // All the chunks are currently being displayed.
    [SerializeField]
    private bool _showAllChunks; // Change this value in the editor to make all chunks visible.

    // Used to determine if the player has transitioned regions.
    private string _previousRegion;
    [SerializeField]
    private string _currentRegion;
    [SerializeField]
    private GameObject _outOfBoundsUI;

    private TextMeshProUGUI _regionDisplay;

    private float _unloadTimer;

    private string[] _lines;
    [SerializeField]
    private int _xLen;
    [SerializeField]
    private int _zLen;
    [SerializeField]
    private int _yStartingChunk;
    [SerializeField]
    private int _xStartingChunk;
    [SerializeField]
    private float _worldScale = 1.0f;

    private Map _map;
    private Vector2 _topLeftChunkPos;

    public Vector2 CurrentChunkPosition
    {
        get { return _currentChunkPosition; }
    }

    public Vector2 StartingChunk
    {
        get { return new Vector2(_xStartingChunk, _yStartingChunk); }
    }

    public float ChunkLength
    {
        get { return _chunkSideLength; }
    }

    public int XSize
    {
        get { return _zLen - 2; }
    }

    public int YSize
    {
        get { return _xLen - 2; }
    }
    

    // Start is called before the first frame update
    void Start()
    {
        _chunkSideLength *= _worldScale;
        _chunks = new Chunk[_xLen, _zLen];
        SetUpMonsters();
        _unloadTimer = 0;
        _currentRegion = "china";
        _ship = GameObject.FindGameObjectWithTag("Player");
        _visibleChunks = new List<Chunk>();
        _showAllChunks = false;
        _displayedAllChunks = false;
        // Get the textmeshpro object so we can write to it.
        _regionDisplay = GameObject.Find("Canvas").GetComponent<TextMeshProUGUI>();
        _currentChunkPosition = new Vector2(_yStartingChunk, _xStartingChunk);
        // Physially move the player to the center of that chunk.
        _ship.GetComponent<ShipMovement>().Position = new Vector3(_chunkSideLength * _currentChunkPosition.x + 100, 1f, _chunkSideLength * _currentChunkPosition.y + 50);
        _enemies = new List<GameObject>();
        LoadWorld();

        _chunks[_yStartingChunk, _xStartingChunk].chunk.SetActive(true);
        _visibleChunks.Add(_chunks[_yStartingChunk, _xStartingChunk]);
        if (_chunks[_yStartingChunk, _xStartingChunk].hasEnemies)
        {
            CheckLoadEnemies(_chunks[_yStartingChunk, _xStartingChunk]);
        }
        _map = GetComponent<Map>();
        _map.SetUpMap();
        _topLeftChunkPos = new Vector2(_chunks[1, 1].chunk.transform.position.z - (_chunkSideLength / 2), _chunks[1, 1].chunk.transform.position.x - (_chunkSideLength / 2));
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
                // The name of the chunk to load in.
                string regionText = parts[z];
                // If there is an extra specifier.
                int index = regionText.IndexOf("_");
                string region = "ocean";
                if(index > -1)
                {
                    region = regionText.Substring(0, index);
                }
                bool hasEnemies = false;
                List<GameObject> enemies = new List<GameObject>();
                string pathName = "";
                Region r = Region.OCEAN;
                // Determine which region this chunk is in.
                switch (region)
                {
                    case "china":
                        pathName = "Chunks/china/" + regionText;
                        r = Region.CHINA;
                        break;
                    case "ocean":
                        pathName = "Chunks/none/" + regionText;
                        r = Region.OCEAN;
                        break;
                    case "out-of-bounds":
                        pathName = "Chunks/out-of-bounds/" + regionText;
                        r = Region.OUT_OF_BOUNDS;
                        break;
                }
                // Make the chunk.
                GameObject obj = Instantiate(Resources.Load<GameObject>(pathName), new Vector3(x * _chunkSideLength, 0, z * _chunkSideLength), Quaternion.identity);
                obj.transform.localScale = obj.transform.localScale *= _worldScale;
                Transform unscaledEnviroment = obj.transform.Find("Unscaled Enviroment");
                if(unscaledEnviroment != null)
                {
                    for(int i = 0; i < unscaledEnviroment.childCount; i++)
                    {
                        unscaledEnviroment.GetChild(i).localScale = unscaledEnviroment.GetChild(i).localScale / _worldScale;
                    }
                }
                obj.transform.parent = world.transform;

                // There are monsters to load in.
                Transform spawnTransform = obj.transform.Find("Enemies");
                if (spawnTransform != null && spawnTransform.childCount > 0)
                {
                    hasEnemies = true;
                    spawnTransform.localScale = new Vector3(spawnTransform.localScale.x, spawnTransform.localScale.y / _worldScale, spawnTransform.localScale.z);
                    GameObject listOfSpawnPoints = spawnTransform.gameObject;
                    int numEnemies = listOfSpawnPoints.transform.childCount;
                    for(int i = 0; i < numEnemies; i++)
                    {
                        GameObject spawnPoint = listOfSpawnPoints.transform.GetChild(i).gameObject;
                        GameObject enemyPrefab = spawnPoint.GetComponent<SpawnPoint>().EnemyPrefab;
                        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
                        enemy.SetActive(false);

                        // Cash the starting position of this enemy
                        enemy.GetComponent<Enemy>().EnemyStartingPosition = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y, spawnPoint.transform.position.z);
                        enemy.GetComponent<Enemy>().EnemyStartingChunk = new Vector2(x, z);
                        enemy.GetComponent<Enemy>().EnemyID = i;
                        enemy.transform.parent = world.transform;
                        // Store a reference in the list of enemies corresponding to this species.
                        enemies.Add(enemy);
                    }
                }
                Chunk c = new Chunk(obj, r, new Vector2(x * _chunkSideLength, z * _chunkSideLength), enemies);
                c.hasEnemies = hasEnemies;
                _chunks[x, z] = c;
                _chunks[x, z].chunk.SetActive(false);
            }
        }
    }
    // Set up the dictionaries of monsters.
    public void SetUpMonsters()
    {
        _monsters = new Dictionary<string, GameObject>();
        _monsters.Add("koi", null);

        // Add more monsters

    }
    // Load a monster of the specified name at the specifed positon.
    /*public void LoadMonster(Vector2 position, Vector2 chunk, string name)
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
    }*/
    public void UnloadMonster(string name)
    {
        Destroy(_monsters[name]);
        _monsters[name] = null;
    }
    // March through each monster and unload the monster if.
    // 1) The player is more than 2 chunks away from the monster AND
    //      a. The monster is Passive.
    public void CheckUnloadMonster()
    {
        // List of monsters to unload.
        List<string> destroyMonsters = new List<string>();
        // Loop through each potential monster.
        foreach(KeyValuePair<string, GameObject> kvpair in _monsters)
        {
            string name = kvpair.Key;
            GameObject monster = kvpair.Value;
            // Monster exists and is roaming...
            if (monster)
            {
                Vector2 start = monster.GetComponent<Enemy>().EnemyStartingChunk;
                bool playerCloseToMonster = Mathf.Sqrt(Mathf.Pow(_ship.transform.position.x - monster.transform.position.x, 2) + Mathf.Pow(_ship.transform.position.z - monster.transform.position.z, 2)) < 1 * Mathf.Sqrt(2 * Mathf.Pow(_chunkSideLength / 2, 2));
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
        for(int i = 0; i < _enemies.Count; i++)
        {
            bool enemyIsFar = Mathf.Sqrt(Mathf.Pow(_enemies[i].transform.position.x - _ship.transform.position.x, 2) 
                + Mathf.Pow(_enemies[i].transform.position.z - _ship.transform.position.z, 2)) > 2 * Mathf.Sqrt(2 * Mathf.Pow(_chunkSideLength / 2, 2));
            if (enemyIsFar || _enemies[i].GetComponent<Enemy>().ReadyToDelete)
            {
                // Get the origin chunk of the enemy.
                Vector2 originChunk = _enemies[i].GetComponent<Enemy>().EnemyStartingChunk;
                // Mark this enemy as unloaded.
                _chunks[(int)originChunk.x, (int)originChunk.y].enemieIsLoaded[_enemies[i].GetComponent<Enemy>().EnemyID] = false;
                // Hide the enemy.
                _enemies[i].SetActive(false);
                // Remove it from the list of enemies.
                _enemies.RemoveAt(i);
                i--;
            }

        }
    }
    public void ResetEnemy(GameObject enemy)
    {
        Vector3 pos = enemy.GetComponent<Enemy>().EnemyStartingPosition;
        enemy.GetComponent<Enemy>().Position = new Vector3(pos.x, pos.y, pos.z);
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
                _enemies.Add(e[i]);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        _previousRegion = _currentRegion;
        DisplayChunks();
        if(_previousRegion != _currentRegion && _currentRegion == GetRegionName(Region.OUT_OF_BOUNDS))
        {
            _outOfBoundsUI.SetActive(true);
        }
        if(_previousRegion != _currentRegion && _previousRegion == GetRegionName(Region.OUT_OF_BOUNDS))
        {
            _outOfBoundsUI.SetActive(false);
        }
        //Determine if the players has entered a new region.
        /*if (!currentRegion.Equals(previousRegion))
        {
            // Display the text for a few seconds.
            StartCoroutine(DisplayRegion(currentRegion));
        }*/

    }
    // Fade in and out for region switching.
    /*private IEnumerator DisplayRegion(string newRegion)
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
    }*/
    /// <summary>
    /// Gets the distance between the ship and the specified chunks center.
    /// </summary>
    /// <param name="x"> The row of the array of chunks.</param>
    /// <param name="z"> The col of the array of chunks.</param>
    /// <returns> The distance between the ship and the chunks center.</returns>
    public float DistanceFromChunkCenter(GameObject g, int x, int z)
    {
        return Mathf.Sqrt(Mathf.Pow(g.transform.position.x - (x * _chunkSideLength), 2)
            + Mathf.Pow(g.transform.position.z - (z * _chunkSideLength), 2));
    }
    /// <summary>
    /// Updates the current chunk field to accurately represent which chunk the user is in.
    /// </summary>
    public void DisplayChunks()
    {
        if(!_displayedAllChunks && _showAllChunks)
        {
            for(int i = 0; i < _chunks.Length; i++)
            {
                for(int j = 0; j < _chunks.GetLength(1); j++)
                {
                    _chunks[i, j].chunk.SetActive(true);
                }
            }
            _displayedAllChunks = true;
            return;
        }
        // The current chunk position before the loop executes and potentially alters the current chunk positon.
        Vector2 stashedChunkPos = _currentChunkPosition;
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
                Rect chunkBounds = new Rect((x - .5f) * _chunkSideLength, (z - .5f) * _chunkSideLength, _chunkSideLength, _chunkSideLength);
                // Chunk is valid.
                if (x >= 0 && z >= 0 && x < _xLen && z < _zLen && _chunks[x, z] != null)
                {
                    // If the chunk is close enough to render.
                    bool close = DistanceFromChunkCenter(_ship, x, z) < _chunkSideLength + 30;
                    bool inVisibleChunks = _visibleChunks.Contains(_chunks[x, z]);
                    // Chunk is close enough to render so do so.
                    if (close && !inVisibleChunks)
                    {
                        // Set the chunk as active and add it to the list of active chunks.
                        _chunks[x, z].chunk.SetActive(true);
                        _visibleChunks.Add(_chunks[x, z]);
                        if (_chunks[x, z].hasEnemies)
                        {
                            CheckLoadEnemies(_chunks[x, z]);
                        }
                    }
                    // Chunk is no longer within viewing distance of the ship, so unload it.
                    if (!close && inVisibleChunks)
                    {
                        // Deactivate this chunk
                        _chunks[x, z].chunk.SetActive(false);
                        _visibleChunks.Remove(_chunks[x, z]);
                    }
                    // Ship was in a different chunk and has now moved into this chunk, make it the current chunk.
                    if ((stashedChunkPos.x != x || stashedChunkPos.y != z) && chunkBounds.Contains(new Vector2(_ship.transform.position.x, _ship.transform.position.z)))
                    {
                        // Set current chunk to the chunk the ships in.
                        _currentChunkPosition = new Vector2(x, z);
                        _currentRegion = GetRegionName(_chunks[x, z].region);
                        _map.UpdateCurrentChunk();

                    }
                }
            }
        }
        _unloadTimer += Time.deltaTime;
        if(_unloadTimer > 1)
        {
            CheckUnloadMonster();
            _unloadTimer = 0;
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
                name = "china";
                break;
            case Region.OCEAN:
                name = "ocean";
                break;
            case Region.OUT_OF_BOUNDS:
                name = "out-of-bounds";
                break;
        }
        return name;
    }

    /// <summary>
    /// Gets position of player relative to top left chunk
    /// </summary>
    /// <returns></returns>
    public Vector2 GetRelativePlayerPosition()
    {
        return new Vector2(_ship.transform.position.z - _topLeftChunkPos.x, _ship.transform.position.x - _topLeftChunkPos.y);
    }

    /// <summary>
    /// Gets y euler of player
    /// </summary>
    /// <returns></returns>
    public float GetPlayerYEuler()
    {
        return _ship.transform.localEulerAngles.y;
    }

    /*public GameObject GetPrefabByName(string s)
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
            case "pandaTree":
                return pandaTreePrefab;
            case "clam":
                return clamPrefab;
        }
        return null;
    }*/
}
