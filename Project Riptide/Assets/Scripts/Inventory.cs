using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{
    public List<GameObject> inventorySlots;
    private ItemDatabase theDatabase;
    void Start()
    {
        inventorySlots = new List<GameObject>();
        theDatabase = GameObject.FindWithTag("GameManager").GetComponent<ItemDatabase>();
    }

    //This allows you to add a item using the name/slug or the ID
    
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
        if (Input.GetKey(KeyCode.J))
        {
            GameObject lootable = Instantiate(Resources.Load("Inventory/Lootable"), new Vector3(Random.Range(0,5), Random.Range(0, 5), Random.Range(0, 5)), Quaternion.identity) as GameObject;
            lootable.GetComponent<Lootable>().itemStored = theDatabase.GetRandomItem();
            lootable.GetComponent<Lootable>().lightColor = theDatabase.rarityColors[lootable.GetComponent<Lootable>().itemStored.rarity];
        }
    }

    public void AddItem(string itemName, int amountToAdd)
    {
        int amountToAddTemp = amountToAdd;
        Item itemToAdd = theDatabase.FindItem(itemName);
        for (int i = 0; i < inventorySlots.Count; i++) //Checking to see if it can add the item to a existing slot
        {
            ItemSlot slot = inventorySlots[i].GetComponent<ItemSlot>();
            if (slot.item.name == itemToAdd.name && slot.item.amount != slot.item.maxAmount) //Another item with room has been found, does it have room
            {
                if (slot.item.amount + amountToAdd <= slot.item.maxAmount)
                {
                    slot.item.amount += amountToAdd;
                    slot.UpdateSlotVisuals();
                    return; //Item is completely in the inventory now, end
                }
                else //amount to add is too much, split it up
                {
                    int subtractionAmount = slot.item.maxAmount - slot.item.amount;
                    slot.item.amount = slot.item.maxAmount;
                    amountToAddTemp -= subtractionAmount;
                    slot.UpdateSlotVisuals();
                }
            }
        }

        //Adding new item to slot, no previous items were found
        GameObject theNewSlot = Instantiate(Resources.Load("inventory/InventorySlot"), gameObject.transform.Find("InventoryScrollRect").Find("Inventory Panel").transform) as GameObject;
        theNewSlot.GetComponent<ItemSlot>().item = itemToAdd;
        theNewSlot.GetComponent<ItemSlot>().item.amount = amountToAddTemp;
        inventorySlots.Add(theNewSlot);
        theNewSlot.GetComponent<ItemSlot>().UpdateSlotVisuals();
    }

    public bool RemoveItem (string itemName, int amount)
    {
        if(inventorySlots.Count == 0) { Debug.LogWarning("Nothing in inventory, nothing to delete!"); return false; }
        Item itemToAdd = theDatabase.FindItem(itemName);
        for (int i = inventorySlots.Count-1; i > -1; i--) //Finding the slot with the item, starts from the bottom up
        {
            ItemSlot slot = inventorySlots[i].GetComponent<ItemSlot>();
            if (slot.item.name == itemToAdd.name)
            {
                if (slot.item.amount <= amount)
                {
                    int newAmount = amount - slot.item.amount;
                    inventorySlots.Remove(slot.gameObject);
                    slot.Clear();
                    RemoveItem(itemName, newAmount); //Recursive
                }
                else
                {
                    slot.item.amount -= amount;
                    slot.UpdateSlotVisuals();
                }
                return true;
            }
        }
        Debug.LogWarning("[Inventory] When removing + " + itemToAdd.name + ", no items of that type were found in the inventory!");
        return false;
    }

    public void SwapInventories (string itemName, int amount, Inventory otherInventory) //Not sure this works right now
    {
        if(RemoveItem(itemName, amount))
        {
            otherInventory.AddItem(itemName, amount);
        }
    }
}
