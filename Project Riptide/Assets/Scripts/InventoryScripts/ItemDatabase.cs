using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using LitJson;

//USING LITJSON
public class ItemDatabase : MonoBehaviour
{
    private string defaultPath;
    private string jsonString;
    private JsonData itemData;
    private List<Item> allItemsList;

    //Types ---------
    /*
     * Resources
     * Equipment
     */

    void Start()
    {
        defaultPath = Application.dataPath + "/Items.json";
        jsonString = File.ReadAllText(defaultPath);
        allItemsList = new List<Item>();
        System.Console.WriteLine(GetItem("Resources", "Stone").Description);
    }

    void Update()
    {
        Debug.Log(allItemsList[0].ItemName);
    }

    public Item GetItem(string type, string nameOfItem)
    {
        JsonData tempObject = new JsonData();
        if (File.Exists(defaultPath))
        {
            itemData = JsonMapper.ToObject(jsonString);
            for (int i = 0; i < itemData[type].Count; i++)
            {
                if(itemData[type][i][nameOfItem].ToString() == name)
                {
                    tempObject = itemData[type][i];
                }
            }

            if(itemData[type][0] != null)
            {
                return new Item((int)tempObject["id"], (string)tempObject["name"], (string)tempObject["description"], (bool)tempObject["stackable"], (int)tempObject["rarity"], (int)tempObject["value"]);
            }
            else
            {
                Debug.LogError("Item of that type or name could not be retreved!");
            }
        }
        else
        {
            Debug.LogError("File path to Items.json is wrong!");
        }
        return null;
    }
}

//https://forum.unity.com/threads/reading-and-writing-json-files-c-litjson-awfulmedia.351806/

public class Item : MonoBehaviour
{

    public int Id; //These are public, and thus not secure...
    public string ItemName;
    public string Description;
    public bool Stackable;
    public int Rarity;
    public int Value;

    public Item(int id, string itemName, string description, bool stackable, int rarity, int value)
    {
        Id = id;
        ItemName = itemName;
        Description = description;
        Stackable = stackable;
        Rarity = rarity;
        Value = value;
    }
}
