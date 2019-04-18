using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ItemDatabase : MonoBehaviour
{
    private List<Item> allItemsList;
    private string defaultPath = Application.dataPath + "Assets/Resources/Items";

    void Awake()
    {
        allItemsList = new List<Item>();
        CreateListFromJson();
    }

    void Update()
    {
        
    }

    void CreateListFromJson ()
    {
        if(File.Exists (defaultPath))
        {
            Item[] tempItems = JsonHelper.FromJson<Item>(defaultPath);
            foreach (Item i in tempItems)
            {
                allItemsList.Add(new Item(i.Id, i.ItemName, i.Description, i.Stackable));
            }
        }
        else
        {
            Debug.LogError("There is no items Json in the defualt path!");
        }
    }
}

public class Item : MonoBehaviour
{
    public Item(int id, string itemName, string description, bool stackable)
    {
        Id = id;
        ItemName = itemName;
        Description = description;
        Stackable = stackable;
    }

    public int Id; //These are public, and thus not secure...
    public string ItemName;
    public string Description;
    public bool Stackable;
}

//https://stackoverflow.com/questions/36239705/serialize-and-deserialize-json-and-json-array-in-unity

//This class helps by allowing you to serialze and deserialize array's from Json files
public static class JsonHelper
{

    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = UnityEngine.JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return UnityEngine.JsonUtility.ToJson(wrapper);
    }

    [SerializeField] //This was SERILIZABLE not serField
    private class Wrapper<T>
    {
        public T[] Items;
    }
}
