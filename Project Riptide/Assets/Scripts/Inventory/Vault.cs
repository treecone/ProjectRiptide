using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vault : Inventory
{
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

            InventorySlot slot = inventorySlots[i].GetComponent<InventorySlot>();
            Debug.Log("trying to add in slot " + i + ". slot currently has " + slot.item.Amount + " " + slot.item.Name + " out of a max of " + slot.item.MaxAmount);
            if (slot.item.Name == itemToAdd.Name && slot.item.Amount != itemToAdd.MaxAmount) //A similiar item with room has been found, does it have room for all the items being added
            {
                Debug.Log("slot " + i + " has same item and has room.");
                if (slot.item.Amount + amountToAddTemp <= slot.item.MaxAmount)
                {
                    Debug.Log("can fit rest of items in this slot, finishing");
                    slot.item.Amount += amountToAddTemp;
                    slot.UpdateSlotVisuals();
                    return; //Item is completely in the inventory now, end
                }
                else //amount to add is too much, split it up
                {
                    Debug.Log("cannot fit all items in this slot, amountToAdd reduced from " + amountToAddTemp);
                    slot.item.Amount = slot.item.MaxAmount;
                    amountToAddTemp -= (slot.item.MaxAmount - slot.item.Amount);
                    Debug.Log("to " + amountToAddTemp);
                    slot.UpdateSlotVisuals();
                }
            }
            else if (slot.item.Id == 0)
            {
                Debug.Log("slot " + i + " is empty, enqueueing");
                //Slots is empty, store it temp
                tempClearSlots.Enqueue(i);
            }
            else if (slot.item.Name == itemToAdd.Name)
            {
                Debug.Log("slot has " + slot.item.Name + ", but is full");
            }
        }

        while (amountToAddTemp > 0) //There are still items left over, need to make some new items in slots
        {
            if (tempClearSlots.Count > 0)
            {
                string debugStr = "trying empty slots ";
                foreach (int i in tempClearSlots)
                {
                    debugStr += i + ", ";
                }
                Debug.Log(debugStr);

                int itemSlotNumber = tempClearSlots.Dequeue();
                Debug.Log("trying empty slot " + itemSlotNumber);
                InventorySlot theSlot = inventorySlots[itemSlotNumber].GetComponent<InventorySlot>();
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
        //create a new itemSlot and add items to it

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

    }
}
