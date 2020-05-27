using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{
    public List<Item> items;
    public List<GameObject> inventorySlots;
    private ItemDatabase _itemDatabase;
    public Upgrades shipUpgradeScript;
    void Start()
    {
        items = new List<Item>();
        inventorySlots = new List<GameObject>();
        _itemDatabase = GameObject.FindWithTag("GameManager").GetComponent<ItemDatabase>();
    }
    
    
    public void UpdateTooltip ()
    {

    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            foreach(Transform child in transform)
            {
                child.gameObject.SetActive(!child.gameObject.activeSelf); //Turning on/off UI
            }
        }
        if (Input.GetKey(KeyCode.N))
        {
            AddItem("carpscale", 8);
        }
        if (Input.GetKey(KeyCode.M))
        {
            RemoveItem("nails", 8);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            AddItem("woodencannon", 1);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            AddItem("scalemailhull", 1);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            AddItem("silksails", 1);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            AddItem("grapeshot", 1);
        }
        if (Input.GetKey(KeyCode.J))
        {
            GameObject lootable = Instantiate(Resources.Load("Inventory/Lootable"), new Vector3(Random.Range(0,5), Random.Range(0, 5), Random.Range(0, 5)), Quaternion.identity) as GameObject;
            lootable.GetComponent<Lootable>().itemStored = _itemDatabase.GetRandomItem();
            lootable.GetComponent<Lootable>().lightColor = _itemDatabase.rarityColors[lootable.GetComponent<Lootable>().itemStored.rarity];
        }
    }

    /// <summary>
    /// Adds items to the inventory
    /// </summary>
    /// <param name="itemName">The name of the item to be added</param>
    /// <param name="amountToAdd">The amount of that item to be added</param>
    public void AddItem(string itemName, int amountToAdd)
    {
        int amountToAddTemp = amountToAdd;
        Item itemToAdd = _itemDatabase.FindItem(itemName);
        for (int i = 0; i < items.Count; i++) //Checking to see if it can add the item to a existing slot
        {
            ItemSlot slot = inventorySlots[i].GetComponent<ItemSlot>();
            if (items[i].name == itemToAdd.name && items[i].amount != items[i].maxAmount) //Another item with room has been found, does it have room
            {
                if (items[i].amount + amountToAdd <= items[i].maxAmount)
                {
                    items[i].amount += amountToAdd;
                    slot.item.amount += amountToAdd;
                    slot.UpdateSlotVisuals();
                    return; //Item is completely in the inventory now, end
                }
                else //amount to add is too much, split it up
                {
                    int subtractionAmount = items[i].maxAmount - items[i].amount;
                    items[i].amount = items[i].maxAmount;
                    slot.item.amount = slot.item.maxAmount;
                    amountToAddTemp -= subtractionAmount;
                    slot.UpdateSlotVisuals();
                }
            }
        }
        //Adding new item to slot, no previous items were found
        GameObject newSlot = Instantiate(Resources.Load("inventory/InventorySlot"), gameObject.transform.Find("InventoryScrollRect").Find("Inventory Panel").transform) as GameObject;
        newSlot.GetComponent<ItemSlot>().item = itemToAdd;
        newSlot.GetComponent<ItemSlot>().item.amount = amountToAddTemp;
        inventorySlots.Add(newSlot);

        items.Add(itemToAdd);
        items[items.Count - 1].amount = amountToAddTemp;

        newSlot.GetComponent<ItemSlot>().UpdateSlotVisuals();
        shipUpgradeScript.Recalculate();
    }

    /// <summary>
    /// Removes items from the inventory
    /// </summary>
    /// <param name="itemName">The name of the item to be removed</param>
    /// <param name="amount">The amount of the item to be removed</param>
    /// <returns>true if the given amount of item was succesfully removed, false otherwise</returns>
    public bool RemoveItem (string itemName, int amount)
    {
        if(items.Count == 0) { Debug.LogWarning("Nothing in inventory, nothing to delete!"); return false; }
        Item itemToRemove = _itemDatabase.FindItem(itemName);
        for (int i = items.Count-1; i > -1; i--) //Finding the slot with the item, starts from the bottom up
        {
            ItemSlot slot = inventorySlots[i].GetComponent<ItemSlot>();
            if (items[i].name == itemToRemove.name)
            {
                if (items[i].amount <= amount)
                {
                    int newAmount = amount - items[i].amount;
                    inventorySlots.Remove(slot.gameObject);
                    items.RemoveAt(i);
                    slot.Clear();
                    RemoveItem(itemName, newAmount); //Recursive
                }
                else
                {
                    items[i].amount -= amount;
                    slot.UpdateSlotVisuals();
                }
                return true;
            }
        }
        Debug.LogWarning("[Inventory] When removing " + amount + " of " + itemToRemove.name + ", not enough items of that type were found in the inventory!");
        shipUpgradeScript.Recalculate();
        return false;
    }

    public void SwapInventories (string itemName, int amount, Inventory otherInventory) //Not sure this works right now
    {
        if(RemoveItem(itemName, amount))
        {
            otherInventory.AddItem(itemName, amount);
        }
    }
    
    /// <value>
    /// Gets the size of the inventory in number of slots
    /// </value>
    public int Size
    {
        
        get
        {
            return items.Count;
        }
    }

    /// <summary>
    /// Indexer for the inventory
    /// </summary>
    /// <param name="i">The slot number of the item you are looking for</param>
    /// <returns>The item in that slot</returns>
    public Item this[int i]
    {
        get
        {
            return items[i];
        }
    }
    /// <summary>
    /// Gets the total amount of an item in the inventory, regardless of how it is stacked
    /// </summary>
    /// <param name="itemName">The name of the item you want to count</param>
    /// <returns>The amount of the item in the inventory</returns>
    public int CountOf(string itemName)
    {
        int count = 0;
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].name == itemName)
            {
                count += items[i].amount;
            }
        }
        return count;
    }
    
}
