using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom Assets/DropData")]
public class DropData : ScriptableObject
{
    public string name;
    public List<Drop> drops;
}

[System.Serializable]
public class Drop
{
    [Range(0,1)]
    public float chance;
    public int minCount;
    public int maxCount;
    public string itemSlug;
}
