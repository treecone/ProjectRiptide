using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using LitJson;
using System;

//USING LITJSON
public class ItemDatabase : MonoBehaviour
{
    private string _defaultPath;
    private string _jsonString;
    private JsonData _itemData;

    public Color[] rarityColors;

    //This list holds all the items in the game after it's been constructed using the ConstructDatabase method
    private List<Item> database = new List<Item>();

    void Awake()
    {
        _defaultPath = Application.dataPath + "/Resources/Inventory/Items.json";
        _jsonString = File.ReadAllText(_defaultPath);
        _itemData = JsonMapper.ToObject(_jsonString);
        ConstructDatabase();
    }

    //This method is called in the begining of the game to take all the sprites from the Json we made into a dictonary of items
    //This can be changed later if we decide that we want to pull items from the json mid game
    private void ConstructDatabase ()
    {
        for (int i = 0; i < _itemData.Count; i++)
        {
            string nameTempString = _itemData[i]["name"].ToString();
            string slugTempString = _itemData[i]["slug"].ToString();

            //Parses the item's upgrades
            List<Upgrade> upgrades = new List<Upgrade>();
            for(int j = 0; j < _itemData[i]["upgrades"].Count; j++)
            {
                JsonData upgradeData = _itemData[i]["upgrades"][j];
                string name = (string)upgradeData["name"];
                Dictionary<string, float> upgradeInfo = new Dictionary<string, float>();
                foreach(string key in upgradeData["data"].Keys)
                {
                    upgradeInfo[key] = Convert.ToSingle(upgradeData["data"][key].ToString());
                }
                upgrades.Add(new Upgrade(name, upgradeInfo));
            }


            //Checks to see if the item has a sprite in the Resouces folder, and if not uses the nullItem Sprite
            if(!Resources.Load<Sprite>("Inventory/ItemSprites/" + slugTempString + "Sprite"))
            {
                database.Add(new Item((int)_itemData[i]["id"], nameTempString, _itemData[i]["description"].ToString(), (int)_itemData[i]["rarity"],
                    (int)_itemData[i]["value"], slugTempString, Resources.Load<Sprite>("Inventory/ItemSprites/nullitemSprite"),
                    (int)_itemData[i]["amount"], (int)_itemData[i]["maxAmount"], upgrades));
                Debug.LogWarning("[Inventory] " + nameTempString + "Sprite was not found in resources!");
                //This usually means that we have yet to put the sprite for the item in the game

            }
            else
            {
                //found the item!
                database.Add(new Item((int)_itemData[i]["id"], nameTempString, _itemData[i]["description"].ToString(), (int)_itemData[i]["rarity"],
                    (int)_itemData[i]["value"], slugTempString, Resources.Load<Sprite>("Inventory/ItemSprites/" + slugTempString + "Sprite"),
                    (int)_itemData[i]["amount"], (int)_itemData[i]["maxAmount"], upgrades));
            }
        }
    }

    //Sorts through the list "database" to find a certain item by name defined in the perams
    //Also allows slugs if the user mistypes the name in the peram
    // <Note> I could make this binary search or something faster if we run into problems
    public Item FindItem (string name)
    {
        for(int i = 0; i < database.Count; i++)
        {
            if(database[i].Name == name || database[i].Slug == name.ToLower())
            {
                return database[i];
            }
        }
        //Returns the null item
        Debug.LogWarning("[Inventory] Item could not be found in the method FindItem()! Returning Null Item!");
        return database[0];
    }

    //Gets a Jsondata file of a item from the json, don't recomend you use unless you need the jsondata file for it
    JsonData GetItem (string name, string type)
    {
        for(int i = 0; i < _itemData[type].Count; i++)
        {
            if(_itemData[type][i]["name"].ToString () == name || _itemData[type][i]["slug"].ToString () == name)
            {
                return _itemData[type][i];
            }
        }
        return null;
    }

    public Item GetRandomItem ()
    {
        return database[UnityEngine.Random.Range(0, database.Count)];
    }

    //Refrences -------------------------------------------------------------------------------------------------------------------------------

    //https://forum.unity.com/threads/reading-and-writing-json-files-c-litjson-awfulmedia.351806/
}