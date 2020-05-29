using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{
    //need to refactor
    #region Fields
    public List<Item> items;
    public List<GameObject> inventorySlots;
    private ItemDatabase _itemDatabase;
    public Upgrades shipUpgradeScript;
    private ItemDatabase _theDatabase;
    private int _inventoryIndex = 0;
    #endregion

    void Start()
    {
        items = new List<Item>();
        _itemDatabase = GameObject.FindWithTag("GameManager").GetComponent<ItemDatabase>();
    }


    /// <summary>
    /// Gets the item at index i in the inventory
    /// </summary>
    /// <param name="i">The index to get the item from</param>
    /// <returns>The item at index i</returns>
    /// <exception cref="System.IndexOutOfRangeException">Thrown when i is out of the range of the inventory</exception>
    public Item this[int i]
    {
        get
        {
            if(i >= inventorySlots.Count || i < 0)
            {
                throw new System.IndexOutOfRangeException("That inventory does not contain that index.");
            }
            return inventorySlots[i].GetComponent<ItemSlot>().item;
        }
    }
    public void UpdateTooltip()
    {

    }

    public void Update()
    {

        if (Input.GetKey(KeyCode.C))
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
        if (Input.GetKeyDown(KeyCode.N))
        {
            AddItem("silksails", 1);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            AddItem("grapeshot", 1);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("b");
            AddItem("healthyhull", 1);
        }
        if (Input.GetKey(KeyCode.J))
        {
            GameObject lootable = Instantiate(Resources.Load("Inventory/Lootable"), new Vector3(Random.Range(0, 5), Random.Range(0, 5), Random.Range(0, 5)), Quaternion.identity) as GameObject;
            lootable.GetComponent<Lootable>().itemStored = _itemDatabase.GetRandomItem();
            lootable.GetComponent<Lootable>().lightColor = _itemDatabase.rarityColors[lootable.GetComponent<Lootable>().itemStored.Rarity];
        }
    }

    /// <summary>
    /// Adds items to the inventory
    /// </summary>
    /// <param name="itemName">The name of the item to be added</param>
    /// <param name="amountToAdd">The amount of that item to be added</param>
    public void AddItem(string itemName, int amountToAdd)
    {
        Queue<int> tempClearSlots = new Queue<int>();
        int amountToAddTemp = amountToAdd;
        Item itemToAdd = _itemDatabase.FindItem(itemName);
        Debug.Log("Adding item " + itemToAdd.Name);
        for (int i = 0; i < inventorySlots.Count; i++) //Checking to see if it can add the item to a existing slot
        {
            ItemSlot slot = inventorySlots[i].GetComponent<ItemSlot>();
            if (slot.item.Name == itemToAdd.Name && slot.item.Amount != itemToAdd.MaxAmount) //A similiar item with room has been found, does it have room for all the items being added
            {
                if (slot.item.Amount + amountToAdd <= slot.item.MaxAmount)
                {
                    slot.item.Amount += amountToAdd;
                    slot.UpdateSlotVisuals();
                    return; //Item is completely in the inventory now, end
                }
                else //amount to add is too much, split it up
                {
                    slot.item.Amount = slot.item.MaxAmount;
                    amountToAddTemp -= (items[i].MaxAmount - items[i].Amount);
                    slot.UpdateSlotVisuals();
                }
            }
            else if (slot.item.Name == null)
            {
                //Slots is empty, store it temp
                tempClearSlots.Enqueue(i);
            }
        }

        while(amountToAddTemp > 0) //There are still items left over, need to make some new items in slots
        {
            if(tempClearSlots.Count > 0)
            {
                int itemSlotNumber = tempClearSlots.Dequeue();
                ItemSlot theSlot = inventorySlots[itemSlotNumber].GetComponent<ItemSlot>();
                if (itemToAdd.MaxAmount >= amountToAddTemp)
                {
                    //Everything fits
                    theSlot.item = itemToAdd;
                    theSlot.item.Amount = amountToAddTemp;
                    amountToAddTemp = 0;
                }
                else
                {
                    theSlot.item = itemToAdd;
                    theSlot.item.Amount = theSlot.item.MaxAmount;
                    amountToAddTemp -= (theSlot.item.MaxAmount - theSlot.item.Amount);
                }
                theSlot.UpdateSlotVisuals();
            }
            else
            {
                Debug.LogWarning("[Inventory] There is no space left for the rest of the items!");
                return;
            }
        }
        /*
        //Adding new item to slot, no previous items were found
        GameObject newSlot = Instantiate(Resources.Load("inventory/InventorySlot"), gameObject.transform.Find("InventoryScrollRect").Find("Inventory Panel").transform) as GameObject;
        newSlot.GetComponent<ItemSlot>().item = itemToAdd;
        newSlot.GetComponent<ItemSlot>().item.Amount = amountToAddTemp;
        inventorySlots.Add(newSlot);
        
        items.Add(itemToAdd);
        items[items.Count - 1].Amount = amountToAddTemp;

        newSlot.GetComponent<ItemSlot>().UpdateSlotVisuals();
        */

        shipUpgradeScript.Recalculate();
    }

    /// <summary>
    /// Removes items from the inventory
    /// </summary>
    /// <param name="itemName">The name of the item to be removed</param>
    /// <param name="amount">The amount of the item to be removed</param>
    /// <returns>true if the given amount of item was succesfully removed, false otherwise</returns>
    public bool RemoveItem(string itemName, int amount)
    {
        if(items.Count == 0) { Debug.LogWarning("Nothing in inventory, nothing to delete!"); return false; }
        Item itemToRemove = _itemDatabase.FindItem(itemName);
        for (int i = items.Count - 1; i > -1; i--) //Finding the slot with the item, starts from the bottom up
        {
            ItemSlot slot = inventorySlots[i].GetComponent<ItemSlot>();
            if (items[i].Name == itemToRemove.Name)
            {
                if (items[i].Amount <= amount)
                {
                    int newAmount = amount - items[i].Amount;
                    inventorySlots.Remove(slot.gameObject);
                    items.RemoveAt(i);
                    slot.Clear();
                    RemoveItem(itemName, newAmount); //Recursive
                }
                else
                {
                    items[i].Amount -= amount;
                    slot.UpdateSlotVisuals();
                }
                return true;
            }
        }
        Debug.LogWarning("[Inventory] When removing " + amount + " of " + itemToRemove.Name + ", not enough items of that type were found in the inventory!");
        shipUpgradeScript.Recalculate();
        return false;
    }

    public void SwapInventories(string itemName, int amount, Inventory otherInventory) //Not sure this works right now
    {
        if (RemoveItem(itemName, amount))
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
    /// Gets the total amount of an item in the inventory, regardless of how it is stacked
    /// </summary>
    /// <param name="itemName">The name of the item you want to count</param>
    /// <returns>The amount of the item in the inventory</returns>
    public int CountOf(string itemName)
    {
        int count = 0;
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].Name == itemName)
            {
                count += items[i].Amount;
            }
        }
        return count;
    }

}
