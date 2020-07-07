using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using LitJson;

[CreateAssetMenu(menuName = "Custom Assets/Singletons/DropManager")]
public class DropManager : ScriptableObject
{
    public static DropManager _instance;

    public static DropManager Instance
    {
        get
        {
            return _instance;
        }
    }

    [SerializeField]
    private List<DropData> _drops;

    private Dictionary<string, DropData> _dropDict;

    private void OnEnable()
    {
        _instance = this;
        _dropDict = new Dictionary<string, DropData>();

        for(int i = 0; i < _drops.Count; i++)
        {
            _dropDict[_drops[i].name] = _drops[i];
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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