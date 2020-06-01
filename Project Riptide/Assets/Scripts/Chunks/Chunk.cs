using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Holds basic information about each chunk.
/// </summary>
public class Chunk
{
    public GameObject chunk;
    public Region region;
    public Vector2 center;
    public bool hasEnemies;
    public bool hasMonster;

    public List<GameObject> enemies = new List<GameObject>();
    public bool[] enemieIsLoaded;

    /// <summary>
    /// Constructs a chunk object
    /// </summary>
    /// <param name="chunk"> The chunk itself</param>
    /// <param name="region"> The name of the region</param>
    /// <param name="center"> Where the center of the chunk is located</param>
    public Chunk(GameObject chunk, Region region, Vector2 center, List<GameObject> enemies)
    {
        this.chunk = chunk;
        this.region = region;
        this.center = center;
        this.enemies = enemies;
        if (enemies.Count > 0)
            enemieIsLoaded = new bool[enemies.Count];
    }
}
