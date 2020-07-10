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
                _instance = Resources.LoadAll<PlayerInventory>("ScriptableObjectInstances")[0];
            return _instance;
        }
    }

    private void OnEnable()
    {
        _instance = this;
        InitializeInventory();
    }
    public int numItems;
    public List<Item> items;
    public List<Item> equipment;
    public int totalGold;

    public void InitializeInventory()
    {
        totalGold = 0;
        items = new List<Item>();
        equipment = new List<Item>();
        for(int i = 0; i < numItems; i++)
        {
            items.Add(ItemDB.Instance.FindItem("null"));
        }
    }

    /// <summary>
    /// Adds items to the inventory
    /// </summary>
    /// <param name="itemName">The name of the item to be added</param>
    /// <param name="amountToAdd">The amount of that item to be added</param>
    public void AddItem(string itemName, int amountToAdd)
    {
        if (itemName == "gold" || itemName == "Gold")
        {
            totalGold += amountToAdd;
            return;
        }
        if (itemName == "null" || itemName == "Null")
        {
            return;
        }
        Queue<int> tempClearSlots = new Queue<int>();
        int amountToAddTemp = amountToAdd;
        Item itemToAdd = ItemDB.Instance.FindItem(itemName);
        if (itemToAdd.Category != ItemCategory.Material)
        {
            AddEquipment(itemToAdd);
            return;
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
                    return; //Item is completely in the inventory now, end
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
                return;
            }
        }


    }

    /// <summary>
    /// Removes items from the inventory
    /// </summary>
    /// <param name="itemName">The name of the item to be removed</param>
    /// <param name="amount">The amount of the item to be removed</param>
    /// <returns>true if the given amount of item was succesfully removed, false otherwise</returns>
    public bool RemoveItem(string itemName, int amount)
    {
        if (items.Count == 0)
        {
            Debug.LogWarning("Nothing in inventory, nothing to delete!");
            return false;
        }
        Item itemToRemove = ItemDB.Instance.FindItem(itemName);
        for (int i = items.Count - 1; i > -1; i--) //Finding the slot with the item, starts from the bottom up
        {
            Item item = items[i];
            if (items[i].Name == itemToRemove.Name)
            {
                if (items[i].Amount <= amount)
                {
                    int newAmount = amount - items[i].Amount;
                    item = ItemDB.Instance.FindItem("null");
                    RemoveItem(itemName, newAmount); //Recursive
                }
                else
                {
                    items[i].Amount -= amount;
                }
                return true;
            }
        }
        Debug.LogWarning("[Inventory] When removing " + amount + " of " + itemToRemove.Name + ", not enough items of that type were found in the inventory!");
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
            if (items[i].Name == itemName)
            {
                count += items[i].Amount;
            }
        }
        for (int i = 0; i < equipment.Count; i++)
        {
            if (equipment[i].Name == itemName)
            {
                count += equipment[i].Amount;
            }
        }
        return count;
    }
}
