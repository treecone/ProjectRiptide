using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionManager : MonoBehaviour
{
    private const float _CHUNKSIDELENGTH = 100;
    public Dictionary<string, Region> regions;
    public string currentRegion;

    // Start is called before the first frame update
    void Start()
    {
        regions = new Dictionary<string, Region>();
        regions.Add("China", new Region("China", 3, new Vector2(-50, -50)));
        currentRegion = "China";
    }

    private void Update()
    {
        regions[currentRegion].ShowRelevantChunks();     

    }
}
