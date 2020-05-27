using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using LitJson;

public class LootSpawner : MonoBehaviour
{
    private JsonData dropTableData;

    private int randomNumber;
    private string ItemString;
    private GameObject GameManager;

    void Start()
    {
        dropTableData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/Resources/Inventory/Droptable.json"));
    }

    public void DropItems (int id)
    {
        //100 For now, this should be greater then the length of the Droptable json length
        //Finding the right ID in the loot table
        for (int i = 0; i < 100; i++)
        {
            if((int)dropTableData[i]["id"] == id)
            {
                ItemString = (string)dropTableData[i]["roll"];
                break;
            }
        }

        //---------Actually determining and spawning --------------------------

        string[] seperatedDropString = ItemString.Split('|');
        randomNumber = Random.Range(0, 101);
        //%OutOf100|Amount|ItemName|
        for (int i = 1; i < 100; i++)
        {
            if(randomNumber > int.Parse(seperatedDropString[i * 3 - 3]))
            {
                GameObject theSpawnedObject = Instantiate(Resources.Load("Inventory/Lootable"), gameObject.transform.position, Quaternion.identity) as GameObject;
                theSpawnedObject.GetComponent<Lootable>().itemStored = GameManager.GetComponent<ItemDatabase>().FindItem(seperatedDropString[3 * i - 1]);
                theSpawnedObject.GetComponent<Lootable>().itemStored.Amount = int.Parse(seperatedDropString[i * 3 - 2]);
            }

            if(seperatedDropString[i * 3] == "#")
            {
                break;
            }
        }

    }
}
