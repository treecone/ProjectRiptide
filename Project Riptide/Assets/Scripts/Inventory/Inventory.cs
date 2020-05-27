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
    public Upgrades shipUpgradeScript;
    private ItemDatabase _theDatabase;
    #endregion

    void Start()
    {
        items = new List<Item>();
        inventorySlots = new List<GameObject>();
        _theDatabase = GameObject.FindWithTag("GameManager").GetComponent<ItemDatabase>();
    }


    public void UpdateTooltip()
    {

    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            foreach (Transform child in transform)
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
            GameObject lootable = Instantiate(Resources.Load("Inventory/Lootable"), new Vector3(Random.Range(0, 5), Random.Range(0, 5), Random.Range(0, 5)), Quaternion.identity) as GameObject;
            lootable.GetComponent<Lootable>().itemStored = _theDatabase.GetRandomItem();
            lootable.GetComponent<Lootable>().lightColor = _theDatabase.rarityColors[lootable.GetComponent<Lootable>().itemStored.Rarity];
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
        Item itemToAdd = _theDatabase.FindItem(itemName);
        for (int i = 0; i < items.Count; i++) //Checking to see if it can add the item to a existing slot
        {
            ItemSlot slot = inventorySlots[i].GetComponent<ItemSlot>();
            if (items[i].Name == itemToAdd.Name && items[i].Amount != items[i].MaxAmount) //Another item with room has been found, does it have room
            {
                if (items[i].Amount + amountToAdd <= items[i].MaxAmount)
                {
                    items[i].Amount += amountToAdd;
                    slot.item.Amount += amountToAdd;
                    slot.UpdateSlotVisuals();
                    return; //Item is completely in the inventory now, end
                }
                else //amount to add is too much, split it up
                {
                    int subtractionAmount = items[i].MaxAmount - items[i].Amount;
                    items[i].Amount = items[i].MaxAmount;
                    slot.item.Amount = slot.item.MaxAmount;
                    amountToAddTemp -= subtractionAmount;
                    slot.UpdateSlotVisuals();
                }
            }
        }
        //Adding new item to slot, no previous items were found
        GameObject newSlot = Instantiate(Resources.Load("inventory/InventorySlot"), gameObject.transform.Find("InventoryScrollRect").Find("Inventory Panel").transform) as GameObject;
        newSlot.GetComponent<ItemSlot>().item = itemToAdd;
        newSlot.GetComponent<ItemSlot>().item.Amount = amountToAddTemp;
        inventorySlots.Add(newSlot);

        items.Add(itemToAdd);
        items[items.Count - 1].Amount = amountToAddTemp;

        newSlot.GetComponent<ItemSlot>().UpdateSlotVisuals();
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
        if (items.Count == 0)
        {
            Debug.LogWarning("Nothing in inventory, nothing to delete!");
            return false;
        }
        Item itemToRemove = _theDatabase.FindItem(itemName);
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

    /// <summary>
    /// Gets the size of the inventory in slots
    /// </summary>
    /// <returns>The number of slots in the inventory</returns>
    public int Size
    {
        get
        {
            return items.Count;
        }
    }

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
            if (items[i].Name == itemName)
            {
                count += items[i].Amount;
            }
        }
        return count;
    }

}
