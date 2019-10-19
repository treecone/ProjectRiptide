using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public GameObject chunk;
    public string region;
    public Vector2 center;

    public Chunk(GameObject chunk, string region, Vector2 center)
    {
        this.chunk = chunk;
        this.region = region;
        this.center = center;
    }
}
