using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom Assets/Singletons/PlayerInventory")]
public class PlayerInventory : ScriptableObject
{
    private static PlayerInventory _instance;

    public static PlayerInventory Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = Resources.LoadAll<PlayerInventory>("ScriptableObjectInstances")[0];
                //_instance.InitializeInventory();
            }
                
            return _instance;
        }
    }

    private void OnEnable()
    {
        _instance = this;
        InitializeInventory();
    }
    [SerializeField]
    private List<string> startingItemNames;
    public int numItems;
    public List<Item> items;
    public List<Item> equipment;
    public int totalGold;

    public int TotalGold
    {
        get
        {
            return totalGold;
        }
        set
        {
            totalGold = value;
            if(totalGold <= 0)
            {
                totalGold = 0;
            }
        }
    }
    public void InitializeInventory()
    {
        startingItemNames = new List<string>{ "galleon4", "goldscalesails4", "monkeyhull4", "bubblebeam4", "monkeyscrown1" };
        totalGold = 3000;
        items = new List<Item>();
        equipment = new List<Item>();
        for(int i = 0; i < numItems; i++)
        {
            items.Add(ItemDB.Instance.FindItem("null"));
        }
        foreach(string s in startingItemNames)
        {
            AddItem(s, 1);
        }
        for (int i = 0; i < equipment.Count; i++)
        {
            SetEquipped(equipment[i]);
        }
    }

    /// <summary>
    /// Adds items to the inventory
    /// </summary>
    /// <param name="itemName">The name of the item to be added</param>
    /// <param name="amountToAdd">The amount of that item to be added</param>
    public int AddItem(string itemName, int amountToAdd)
    {
        if (itemName == "gold" || itemName == "Gold")
        {
            totalGold += amountToAdd;
            return 0;
        }
        if (itemName == "null" || itemName == "Null")
        {
            return 0;
        }
        Queue<int> tempClearSlots = new Queue<int>();
        int amountToAddTemp = amountToAdd;
        Item itemToAdd = ItemDB.Instance.FindItem(itemName);
        if (itemToAdd.Category != ItemCategory.Material)
        {
            AddEquipment(itemToAdd);
            return 0;
        }
        //Debug.Log("Adding item " + itemName + " as " + itemToAdd.Name);
        for (int i = 0; i < items.Count; i++) //Checking to see if it can add the item to a existing slot
        {
            
            Item item = items[i];
            //Debug.Log("trying to add in slot " + i + ". slot currently has " + item.Amount + " " + item.Name + " out of a max of " + item.MaxAmount);
            if (item.Name == itemToAdd.Name && item.Amount != itemToAdd.MaxAmount) //A similiar item with room has been found, does it have room for all the items being added
            {
                //Debug.Log("slot " + i + " has same item and has room.");
                if (item.Amount + amountToAddTemp <= item.MaxAmount)
                {
                    //Debug.Log("can fit rest of items in this slot, finishing");
                    item.Amount += amountToAddTemp;
                    return 0; //Item is completely in the inventory now, end
                }
                else //amount to add is too much, split it up
                {
                    //Debug.Log("cannot fit all items in this slot, amountToAdd reduced from " + amountToAddTemp);
                    item.Amount = item.MaxAmount;
                    amountToAddTemp -= (item.MaxAmount - item.Amount);
                    //Debug.Log("to " + amountToAddTemp);
                }
            }
            else if (item.Slug == "null")
            {
                //Debug.Log("slot " + i + " is empty, enqueueing");
                //Slots is empty, store it temp
                tempClearSlots.Enqueue(i);
            }
            else if (item.Name == itemToAdd.Name)
            {
                //Debug.Log("slot has " + item.Name + ", but is full");
            }
        }

        while (amountToAddTemp > 0) //There are still items left over, need to make some new items in slots
        {
            if (tempClearSlots.Count > 0)
            {

                int itemSlotNumber = tempClearSlots.Dequeue();
                //Debug.Log("trying empty slot " + itemSlotNumber);
                Item item = items[itemSlotNumber];
                if (itemToAdd.MaxAmount >= amountToAddTemp)
                {
                    //Debug.Log("everything fits in slot " + itemSlotNumber);
                    //Everything fits
                    items[itemSlotNumber] = itemToAdd;
                    items[itemSlotNumber].Amount = amountToAddTemp;
                    amountToAddTemp = 0;
                }
                else
                {
                    //Debug.Log("slot " + itemSlotNumber + " is open, not everything fits");
                    items[itemSlotNumber] = new Item(itemToAdd);
                    items[itemSlotNumber].Amount = item.MaxAmount;
                    amountToAddTemp -= (item.MaxAmount - item.Amount);
                }
            }
            else
            {
                //Debug.LogWarning("[Inventory] There is no space left for the rest of the items!");
                return amountToAdd;
            }
        }
        return 0;
    }

    /// <summary>
    /// Removes items from the inventory
    /// </summary>
    /// <param name="itemName">The name of the item to be removed</param>
    /// <param name="amount">The amount of the item to be removed</param>
    /// <returns>true if the given amount of item was succesfully removed, false otherwise</returns>
    public bool RemoveItem(string itemName, int amount)
    {
        if (itemName == "gold" || itemName == "Gold")
        {
            if(amount > totalGold)
            {
                return false;
            } else
            {
                totalGold -= amount;
                return true;
            }
        }

        if (items.Count == 0)
        {
            Debug.LogWarning("Nothing in inventory, nothing to delete!");
            return false;
        }
        int remaining = amount;
        for (int i = items.Count - 1; i > -1; i--) //Finding the slot with the item, starts from the bottom up
        {
            if (items[i].Name == itemName || items[i].Slug == itemName)
            {
                if (items[i].Amount <= remaining)
                {
                    remaining -= items[i].Amount;
                    items[i].Amount = 0;
                    items[i] = ItemDB.Instance.FindItem("null");
                    if(remaining == 0)
                    {
                        return true;
                    }
                }
                else
                {
                    items[i].Amount -= remaining;
                    return true;
                }
            }
        }
        Debug.LogWarning("[Inventory] When removing " + amount + " of " + itemName + ", not enough items of that type were found in the inventory!");
        return false;
    }

    public List<Item> GetEquipmentOfCategory(ItemCategory category)
    {
        List<Item> equipmentInCategory = new List<Item>();
        for (int i = 0; i < equipment.Count; i++)
        {
            if (equipment[i].Category == category)
            {
                equipmentInCategory.Add(equipment[i]);
            }
        }
        return equipmentInCategory;
    }

    public Item GetEquippedItemOfCategory(ItemCategory category)
    {
        for (int i = 0; i < equipment.Count; i++)
        {
            if (equipment[i].Category == category && equipment[i].Equipped)
            {
                return equipment[i];
            }
        }
        return ItemDB.Instance.FindItem("null");
    }
    public bool AddEquipment(Item equipmentItem)
    {
        bool addItems = true;
        for (int i = 0; i < equipment.Count; i++)
        {
            if (equipment[i].Slug == equipmentItem.Slug)
            {
                addItems = false;
            }
        }
        if (addItems)
        {
            equipment.Add(equipmentItem);
        }
        return addItems;
    }

    public bool HasEquipment(string name)
    {
        for (int i = 0; i < equipment.Count; i++)
        {
            if (equipment[i].Slug == name || equipment[i].Name == name)
            {
                return true;
            }
        }
        return false;
    }

    public void SetEquipped(Item equipping)
    {
        ItemCategory category = equipping.Category;
       
        for(int i = 0; i < equipment.Count; i++)
        {
            if(equipment[i].Name == equipping.Name)
            {
                equipment[i].Equipped = true;
            }
            else
            {
                if(equipment[i].Category.Equals(category))
                {
                    equipment[i].Equipped = false;
                }
            }
        }
    }

    /// <summary>
    /// Gets the total amount of an item in the inventory, regardless of how it is stacked
    /// </summary>
    /// <param name="itemName">The name of the item you want to count</param>
    /// <returns>The amount of the item in the inventory</returns>
    public int CountOf(string itemName)
    {
        if(itemName == "gold" || itemName == "Gold")
        {
            return totalGold;
        }
        int count = 0;
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].Name == itemName || items[i].Slug == itemName)
            {
                count += items[i].Amount;
            }
        }
        for (int i = 0; i < PlayerVault.Instance.items.Count; i++)
        {
            if (PlayerVault.Instance.items[i].Name == itemName || PlayerVault.Instance.items[i].Slug == itemName)
            {
                count += PlayerVault.Instance.items[i].Amount;
            }
        }
        for (int i = 0; i < equipment.Count; i++)
        {
            if (equipment[i].Name == itemName || equipment[i].Slug == itemName)
            {
                count += equipment[i].Amount;
            }
        }
        return count;
    }

    /// <summary>
    /// returns item based on name
    /// </summary>
    /// <param name="itemName"></param>
    /// <returns></returns>
    public Item GetFromName(string itemName)
    {
        Item toReturn = null;
        for (int i = 0; i < items.Count; i++)
        {
            if (itemName == items[i].Name)
            {
                toReturn = items[i];
                break;
            }
        }
        return toReturn;
    }
}
