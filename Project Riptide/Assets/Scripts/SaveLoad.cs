using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.Text;
using System.IO;

public class SaveLoad : MonoBehaviour
{
    private const string SAVE_FILE_NAME = "savedata.json";
    [SerializeField]
    private GameObject player;

    private SaveData save;
    // Start is called before the first frame update
    void Start()
    {
        
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
        save = SaveData.FromJson();
    }
    private void OnApplicationFocus(bool focus)
    {
        Debug.Log("OnApplicationFocus called with parameter " + focus);
        if (focus)
        {
            //load on refocus
            Load();
        } else
        {
            //save on loss of focus
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

        }
        private SaveData(string jsonData)
        {

        }
        public SaveData(SaveLoad sl)
        {
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
            writer.Write(playerRotation.w);
            writer.Write(playerRotation.x);
            writer.Write(playerRotation.y);
            writer.Write(playerRotation.z);
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
            StreamReader sr = new StreamReader(Application.persistentDataPath + "/" + SAVE_FILE_NAME);
            string jsonSaveData = sr.ReadToEnd();
            Debug.Log(jsonSaveData);
            return new SaveData(jsonSaveData);
        }
    }
}
