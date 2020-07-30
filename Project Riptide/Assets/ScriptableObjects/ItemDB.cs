using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom Assets/Singletons/ItemDB")]
public class ItemDB : ScriptableObject
{
    private static ItemDB _instance;

    public static ItemDB Instance
    {
        get
        {
            if (!_instance)
                _instance = Resources.LoadAll<ItemDB>("ScriptableObjectInstances")[0];
            return _instance;
        }
    }

    void OnEnable()
    {
        _instance = this;
        Setup();
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
        [Range(1,1000)]
        public int value;
        public Sprite icon;
        public int maxAmount;
        public List<ListWrapper> upgrades;
        public string activeText;
        public string passiveText;
        public string shotType;
        public string inventoryTapSound;

        [System.Serializable]
        public class ListWrapper
        {
            public List<Upgrade> list;

            public Upgrade this[int key]
            {
                get
                {
                    return list[key];
                }
                set
                {
                    list[key] = value;
                }
            }

            public int Count
            {
                get
                {
                    return list.Count;  
                }
            }
        }
    }

    [SerializeField]
    private ItemData _nullItemData;
    private Item _nullItem;
    [Header("Do not edit this directly - put items in their sections")]
    [SerializeField]
    private List<Item> _items;

    [Header("Add items in their proper section here:")]
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
        if(name == "null" || name == "Null")
        {
            return new Item(_nullItem);
        }
        for (int i = 0; i < _items.Count; i++)
        {
            if (_items[i].Name == name || _items[i].Slug == name.ToLower())
            {
                return new Item(_items[i]);
            }
        }
        //Returns the null item
        Debug.LogWarning("[Inventory] Item could not be found in the method FindItem()! Returning Null Item!");
        return new Item(_nullItem);
    }

    public Item FindItemNoClone(string name)
    {
        if (name == "null" || name == "Null")
        {
            return _nullItem;
        }
        for (int i = 0; i < _items.Count; i++)
        {
            if (_items[i].Name == name || _items[i].Slug == name.ToLower())
            {
                return _items[i];
            }
        }
        //Returns the null item
        ///
        Debug.LogWarning("[Inventory] Item could not be found in the method FindItem()! Returning Null Item!");
        return new Item(_nullItem);
    }

    public void Setup()
    {
        _items = new List<Item>();
        _nullItem = new Item(-1, _nullItemData.name, _nullItemData.description, _nullItemData.rarity, _nullItemData.value, _nullItemData.slug, _nullItemData.icon, 1, _nullItemData.maxAmount, new List<Upgrade>(), ItemCategory.Material, _nullItemData.activeText, _nullItemData.passiveText, _nullItemData.shotType, _nullItemData.inventoryTapSound);
        ImportItemData(_materials, ItemCategory.Material, 0);
        ImportItemData(_ships, ItemCategory.Ship, 100);
        ImportItemData(_sails, ItemCategory.Sails, 200);
        ImportItemData(_hulls, ItemCategory.Hull, 300);
        ImportItemData(_cannons, ItemCategory.Cannon, 400);
        ImportItemData(_trinkets, ItemCategory.Trinket, 500);
    }

    private void ImportItemData(List<ItemData> data, ItemCategory category, int startId)
    {
        
        for(int i = 0; i < data.Count; i++)
        {
            if(category == ItemCategory.Material)
            {
                List<Upgrade> upgradeList = new List<Upgrade>();
                /*for (int j = 0; j < data[i].upgrades[0].Count; j++)
                {
                    upgradeList.Add(data[i].upgrades[0][j]);
                }*/
                _items.Add(new Item(startId + i, data[i].name, data[i].description, 1, data[i].value, data[i].slug, data[i].icon, 1, data[i].maxAmount, upgradeList, category, data[i].activeText, data[i].passiveText, data[i].shotType, data[i].inventoryTapSound));
            }
            else
            {
                for (int j = 0; j < data[i].upgrades.Count; j++)
                {
                    List<Upgrade> upgradeList = new List<Upgrade>();
                    for (int k = 0; k < data[i].upgrades[j].Count; k++)
                    {
                        upgradeList.Add(data[i].upgrades[j][k]);
                    }
                    if (j == 0)
                    {
                        _items.Add(new Item(startId + i * 4 + j, data[i].name, data[i].description, j+1, data[i].value, data[i].slug + (j + 1), data[i].icon, 1, data[i].maxAmount, upgradeList, category, data[i].activeText, data[i].passiveText, data[i].shotType, data[i].inventoryTapSound));
                    }
                    else
                    {
                        _items.Add(new Item(startId + i * 4 + j, data[i].name + " +" + (j), data[i].description, j+1, data[i].value, data[i].slug + (j + 1), data[i].icon, 1, data[i].maxAmount, upgradeList, category, data[i].activeText, data[i].passiveText, data[i].shotType, data[i].inventoryTapSound));

                    }
                }
            }
            
        }
    }
}
