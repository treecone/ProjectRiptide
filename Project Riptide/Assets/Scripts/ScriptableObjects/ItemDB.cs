using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom Assets/ItemDB")]
public class ItemDB : ScriptableObject
{
    private static ItemDB _instance;

    public static ItemDB Instance
    {
        get
        {
            if(!_instance)
            {
                _instance = Resources.FindObjectsOfTypeAll<ItemDB>()[0];
                _instance.Setup();
            }
            if(!_instance)
            {
                _instance = CreateInstance<ItemDB>();
                _instance.Setup();
            }
            return _instance;
        }
    }

    [System.Serializable]
    private class ItemData
    {
        public string name;
        public string slug;
        [TextArea]
        public string description;
        [Range(1,4)]
        public int rarity = 1;
        [Range(1,200)]
        public int value;
        public Sprite icon;
        public int maxAmount;
        public List<Upgrade> upgrades;
    }

    [SerializeField]
    private List<Item> _items;

    [SerializeField]
    private List<ItemData> _ships = new List<ItemData>();
    [SerializeField]
    private List<ItemData> _sails = new List<ItemData>();
    [SerializeField]
    private List<ItemData> _hulls = new List<ItemData>();
    [SerializeField]
    private List<ItemData> _cannons = new List<ItemData>();
    [SerializeField]
    private List<ItemData> _trinkets = new List<ItemData>();
    [SerializeField]
    private List<ItemData> _materials = new List<ItemData>();

    public Item FindItem(string name)
    {
        for (int i = 0; i < _items.Count; i++)
        {
            if (_items[i].Name == name || _items[i].Slug == name.ToLower())
            {
                return new Item(_items[i]);
            }
        }
        //Returns the null item
        Debug.LogWarning("[Inventory] Item could not be found in the method FindItem()! Returning Null Item!");
        return new Item(_items[0]);
    }

    public void Setup()
    {
        _items = new List<Item>();

        ImportItemData(_materials, ItemCategory.Material, 0);
        ImportItemData(_materials, ItemCategory.Ship, 100);
        ImportItemData(_materials, ItemCategory.Sails, 200);
        ImportItemData(_materials, ItemCategory.Hull, 300);
        ImportItemData(_materials, ItemCategory.Cannon, 400);
        ImportItemData(_materials, ItemCategory.Trinket, 500);
    }

    private void ImportItemData(List<ItemData> data, ItemCategory category, int startId)
    {
        for(int i = 0; i < data.Count; i++)
        {
            List<Upgrade> upgradeList = new List<Upgrade>();
            foreach(Upgrade u in data[i].upgrades)
            {
                upgradeList.Add(u);
            }
            _items.Add(new Item(startId + i, data[i].name, data[i].description, data[i].rarity, data[i].value, data[i].slug, data[i].icon, 1, data[i].maxAmount, upgradeList, category));
        }
    }
}
