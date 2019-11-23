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

    /// <summary>
    /// Constructs a chunk object
    /// </summary>
    /// <param name="chunk"> The chunk itself</param>
    /// <param name="region"> The name of the region</param>
    /// <param name="center"> Where the center of the chunk is located</param>
    public Chunk(GameObject chunk, Region region, Vector2 center)
    {
        this.chunk = chunk;
        this.region = region;
        this.center = center;
    }
}
