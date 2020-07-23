using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using LitJson;

[CreateAssetMenu(menuName = "Custom Assets/Singletons/DropManager")]
[UnityEditor.InitializeOnLoad]
public class DropManager : ScriptableObject
{
    public static DropManager _instance;

    public static DropManager Instance
    {
        get
        {
            if (!_instance)
                _instance = Resources.LoadAll<DropManager>("ScriptableObjectInstances")[0];
            return _instance;
        }
    }
    

    private Dictionary<string, DropData> _dropDict;

    void OnEnable()
    {
        _instance = this;
        LoadDrops();
    }//

    private void LoadDrops()
    {
        _dropDict = new Dictionary<string, DropData>();
        DropData[] drops = Resources.LoadAll<DropData>("ScriptableObjectInstances");
        for(int i = 0; i < drops.Length; i++)
        {
            _dropDict[drops[i].name] = drops[i];
        }
    }
    public List<Item> GetDrops(string dropType)
    {
        List<Item> items = new List<Item>();
        foreach (Drop drop in _dropDict[dropType].drops)
        {
            if(Random.Range(0.0f, 1.0f) < drop.chance)
            {
                Item item = ItemDB.Instance.FindItem(drop.itemSlug);
                item.Amount = Random.Range(drop.minCount, drop.maxCount + 1);
                items.Add(item);
            }
        }
        return items;
    }
    
}