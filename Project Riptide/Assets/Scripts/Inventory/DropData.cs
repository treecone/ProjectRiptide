using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using LitJson;

public class DropData : MonoBehaviour
{
    public static DropData instance;

    private JsonData _dropTableData;

    private Dictionary<string, List<Drop>> _dropDict;

    private void Awake()
    {
        instance = this;
        _dropDict = new Dictionary<string, List<Drop>>();
        _dropTableData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/Resources/Inventory/Droptable.json"));
        for (int i = 0; i < _dropTableData.Count; i++)
        {
            string name = _dropTableData[i]["name"].ToString();
            List<Drop> drops = new List<Drop>();
            JsonData singleDropData = _dropTableData[i]["drops"];
            for (int j = 0; j < singleDropData.Count; j++)
            {
                drops.Add(new Drop(singleDropData[j]));
            }

            _dropDict[name] = drops;
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
        foreach (Drop drop in _dropDict[dropType])
        {
            if(Random.Range(0.0f, 1.0f) < drop.chance)
            {
                Item item = ItemDatabase.instance.FindItem(drop.itemSlug);
                item.Amount = Random.Range(drop.minCount, drop.maxCount + 1);
                items.Add(item);
            }
        }
        return items;
    }
    
}

public class Drop
{
    public float chance;
    public int minCount;
    public int maxCount;
    public string itemSlug;

    public Drop(JsonData jsonData)
    {
        chance = float.Parse(jsonData["chance"].ToString());
        minCount = int.Parse(jsonData["minCount"].ToString());
        maxCount = int.Parse(jsonData["maxCount"].ToString());
        itemSlug = jsonData["itemSlug"].ToString();
    }
}