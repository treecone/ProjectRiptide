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

    //This list holds all the items in the game after it's been constructed using the ConstructDatabase method
    private List<Item> database = new List<Item>();

    void Awake()
    {
        defaultPath = Application.dataPath + "/Resources/Items.json";
        jsonString = File.ReadAllText(defaultPath);
        itemData = JsonMapper.ToObject(jsonString);
        ConstructDatabase();
    }

    //This method is called in the begining of the game to take all the sprites from the Json we made into a dictonary of items
    //This can be changed later if we decide that we want to pull items from the json mid game
    void ConstructDatabase ()
    {
        for (int i = 0; i < itemData.Count; i++)
        {
            string nameTempString = itemData[i]["name"].ToString();
            //Checks to see if the item has a sprite in the Resouces folder, and if not uses the nullItem Sprite
            if(!Resources.Load<Sprite>("ItemSprites/" + nameTempString + "Sprite"))
            {
                database.Add(new Item((int)itemData[i]["id"], nameTempString, itemData[i]["description"].ToString(), (bool)itemData[i]["stackable"], (int)itemData[i]["rarity"], (int)itemData[i]["value"], itemData[i]["slug"].ToString(), Resources.Load<Sprite>("ItemSprites/" + nameTempString + "Sprite"), (int)itemData[i]["maxAmount"]));
                Debug.LogWarning("[Inventory] " + nameTempString + "Sprite was not found in resources!");
                //This usually means that we have yet to put the sprite for the item in the game

            }
            else
            {
                database.Add(new Item((int)itemData[i]["id"], nameTempString, itemData[i]["description"].ToString(), (bool)itemData[i]["stackable"], (int)itemData[i]["rarity"], (int)itemData[i]["value"], itemData[i]["slug"].ToString(), Resources.Load<Sprite>("ItemSprites/" + nameTempString + "Sprite"), (int)itemData[i]["maxAmount"]));
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
            if(database[i].name == name || database[i].slug == name.ToLower())
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
        for(int i = 0; i < itemData[type].Count; i++)
        {
            if(itemData[type][i]["name"].ToString () == name || itemData[type][i]["slug"].ToString () == name)
            {
                return itemData[type][i];
            }
        }
        return null;
    }

    //Refrences -------------------------------------------------------------------------------------------------------------------------------

    //https://forum.unity.com/threads/reading-and-writing-json-files-c-litjson-awfulmedia.351806/
}