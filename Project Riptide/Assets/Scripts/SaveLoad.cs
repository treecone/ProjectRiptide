﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.Text;
using System.IO;

public class SaveLoad : MonoBehaviour
{
    private static SaveLoad instance;
    public static SaveLoad Instance
    {
        get
        {
            return instance;
        }
    }
    private const string SAVE_FILE_NAME = "savedata.json";
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private bool _forceNewSave;

    private SaveData save;
    public SaveData Data
    {
        get
        {
            return save;
        }
    }
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        Load();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            Save();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            Load();
        }
    }

    private void Save()
    {
        string json_test = new SaveData(this).GetJson();
        StreamWriter sw = new StreamWriter(Application.persistentDataPath + "/" + SAVE_FILE_NAME);

        sw.Write(json_test);
        sw.Close();
        sw.Dispose();
        Debug.Log(Application.persistentDataPath + "/" + SAVE_FILE_NAME);
        Debug.Log(json_test);
    }
    private void Load()
    {
        if (_forceNewSave)
        {
            save = new SaveData();
        }
        else
        {
            save = SaveData.FromJson();
        }
        if(!save.newGame)
        {
            MusicManager.instance.SetVolume((float)save.musicVolume);
            SoundManager.instance.SetGlobalVolume((float)save.sfxVolume);
            player.GetComponent<PlayerHealth>().Health = (float)save.playerHealth;
            player.GetComponent<ShipMovement>().Position = save.playerLocation;
            player.GetComponent<ShipMovement>().Rotation = save.playerRotation;
            

        }
    }

    public void LoadInventory()
    {
        Debug.Log("num items loaded: " + save.inv_items.Count + " and eq " + save.inv_equipment.Count);
        PlayerInventory.Instance.LoadInventoryAndEquipment(save.inv_items, save.inv_equipment);
    }

    public void LoadPosition()
    {
        player.GetComponent<ShipMovement>().Position = save.playerLocation;

    }
    private void OnApplicationFocus(bool focus)
    {
        Debug.Log("OnApplicationFocus called with parameter " + focus);
        /*if (focus)
        {
            //load on refocus
            Load();
        } else
        {
            //save on loss of focus
            Save();
        }*/
        if(!focus)
        {
            Save();
        }
    }

    private void OnApplicationPause(bool pause)
    {
        Debug.Log("OnApplicationPause called with parameter " + pause);
    }

    private void OnApplicationQuit()
    {
        Debug.Log("OnApplicationQuit called");
    }
    public class SaveData
    {

        //floats are doubles here bc litjson likes that idk

        //newgame flag
        public bool newGame;
        //settings
        public double musicVolume;
        public double sfxVolume;

        //world info
        public double playerHealth;
        public Vector3 playerLocation;
        public Quaternion playerRotation;

        //inventory
        public List<Item> inv_items;
        public List<Item> inv_equipment;
        public SaveData()
        {
            newGame = true;
        }
        private SaveData(string jsonData)
        {
            newGame = false;
            JsonReader reader = new JsonReader(jsonData);
            while(reader.Read())
            {
                //Debug.Log("" + reader.Token + " -- " + reader.Value);
                if(reader.Token == JsonToken.PropertyName)
                {
                    switch (reader.Value)
                    {
                        case "musicVolume":
                            reader.Read();
                            musicVolume = double.Parse(reader.Value.ToString());
                            break;
                        case "sfxVolume":
                            reader.Read();
                            sfxVolume = double.Parse(reader.Value.ToString());
                            break;
                        case "playerHealth":
                            reader.Read();
                            playerHealth = double.Parse(reader.Value.ToString());
                            break;
                        case "playerLocation":
                            reader.Read();
                            reader.Read();
                            float x = float.Parse(reader.Value.ToString());
                            reader.Read();
                            float y = float.Parse(reader.Value.ToString());
                            reader.Read();
                            float z = float.Parse(reader.Value.ToString());
                            playerLocation = new Vector3(x, y, z);
                            break;
                        case "playerRotation":
                            reader.Read();
                            reader.Read();
                            float _x = float.Parse(reader.Value.ToString());
                            reader.Read();
                            float _y = float.Parse(reader.Value.ToString());
                            reader.Read();
                            float _z = float.Parse(reader.Value.ToString());
                            reader.Read();
                            float _w = float.Parse(reader.Value.ToString());
                            playerRotation = new Quaternion(_x, _y, _z, _w);
                            break;
                        case "items":
                            reader.Read(); //skip to arraystart--
                            List<Item> tempItems = new List<Item>();
                            while(true)
                            {
                                reader.Read();//skip to objectstart-- or arrayend--
                                if(reader.Token ==JsonToken.ArrayEnd)
                                {
                                    break;
                                }
                                reader.Read();//skip to propertyname--name
                                reader.Read();//skip to string--{name}
                                Item item = ItemDB.Instance.FindItem(reader.Value.ToString());
                                reader.Read();//skip to propertyname--amount
                                reader.Read();//skip to int--{amount}
                                item.Amount = (int)reader.Value;
                                tempItems.Add(item);
                                reader.Read();//skip to objectend--

                            }
                            inv_items = tempItems;
                            break;

                        case "equipment":
                            reader.Read(); //skip to arraystart--
                            List<Item> tempEquipment = new List<Item>();
                            while (true)
                            {
                                reader.Read();//skip to objectstart-- or arrayend--
                                if (reader.Token == JsonToken.ArrayEnd)
                                {
                                    break;
                                }
                                reader.Read();//skip to propertyname--name
                                reader.Read();//skip to string--{name}
                                Item item = ItemDB.Instance.FindItem(reader.Value.ToString());
                                reader.Read();//skip to propertyname--amount
                                reader.Read();//skip to int--{amount}
                                item.Amount = (int)reader.Value;
                                reader.Read();//skip to propertyname--equipped
                                reader.Read();//skip to bool-{equipped}
                                item.Equipped = (bool)reader.Value;
                                tempEquipment.Add(item);
                                reader.Read();//skip to objectend--

                            }
                            inv_equipment = tempEquipment;
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        public SaveData(SaveLoad sl)
        {
            newGame = false;
            musicVolume = MusicManager.instance.GetVolume();
            sfxVolume = SoundManager.instance.GetGlobalVolume();
            playerHealth = sl.player.GetComponent<PlayerHealth>().Health;
            playerLocation = sl.player.transform.position;
            playerRotation = sl.player.transform.rotation;
            inv_items = PlayerInventory.Instance.items;
            inv_equipment = PlayerInventory.Instance.equipment;
        }

        public string GetJson()
        {
            StringBuilder sb = new StringBuilder();
            JsonWriter writer = new JsonWriter(sb);

            writer.WriteObjectStart();
            #region musicVolume
            writer.WritePropertyName("musicVolume");
            writer.Write(musicVolume);
            #endregion

            #region sfxVolume
            writer.WritePropertyName("sfxVolume");
            writer.Write(sfxVolume);
            #endregion

            #region playerHealth
            writer.WritePropertyName("playerHealth");
            writer.Write(playerHealth);
            #endregion

            #region playerLocation
            writer.WritePropertyName("playerLocation");
            writer.WriteArrayStart();
            writer.Write(playerLocation.x);
            writer.Write(playerLocation.y);
            writer.Write(playerLocation.z);
            writer.WriteArrayEnd();
            #endregion

            #region playerRotation
            writer.WritePropertyName("playerRotation");
            writer.WriteArrayStart();
            writer.Write(playerRotation.x);
            writer.Write(playerRotation.y);
            writer.Write(playerRotation.z);
            writer.Write(playerRotation.w);
            writer.WriteArrayEnd();
            #endregion

            #region inventory
            writer.WritePropertyName("inventory");
            writer.WriteObjectStart();
            #region inv_items
            writer.WritePropertyName("items");
            writer.WriteArrayStart();
            foreach(Item i in inv_items)
            {
                writer.WriteObjectStart();
                writer.WritePropertyName("name");
                writer.Write(i.Name);
                writer.WritePropertyName("amount");
                writer.Write(i.Amount);
                writer.WriteObjectEnd();
            }
            writer.WriteArrayEnd();
            #endregion

            #region inv_equipment
            writer.WritePropertyName("equipment");
            writer.WriteArrayStart();
            foreach (Item i in inv_equipment)
            {
                writer.WriteObjectStart();
                writer.WritePropertyName("name");
                writer.Write(i.Name);
                writer.WritePropertyName("amount");
                writer.Write(i.Amount);
                writer.WritePropertyName("equipped");
                writer.Write(i.Equipped);
                writer.WriteObjectEnd();
            }
            writer.WriteArrayEnd();
            #endregion
            writer.WriteObjectEnd();
            #endregion
            writer.WriteObjectEnd();
            return sb.ToString();
        }

        public static SaveData FromJson()
        {
            if(File.Exists(Application.persistentDataPath + "/" + SAVE_FILE_NAME)) {

                StreamReader sr = new StreamReader(Application.persistentDataPath + "/" + SAVE_FILE_NAME);
                string jsonSaveData = sr.ReadToEnd();
                Debug.Log(jsonSaveData);
                return new SaveData(jsonSaveData);
            } else
            {
                return new SaveData();
            }
        }
    }
}
