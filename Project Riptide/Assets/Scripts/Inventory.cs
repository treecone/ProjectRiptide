using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{
    private List<ItemSlot> inventorySlots;
    private ItemDatabase theDatabase;
    public int inventorySlotSize;
    void Start()
    {
        inventorySlots = new List<ItemSlot>();
        theDatabase = GameObject.FindWithTag("GameManager").GetComponent<ItemDatabase>();
        ConstructInventory();
        AddItem("nullitem", 20);
    }

    //This allows you to add a item using the name/slug or the ID
    
    public void UpdateTooltip ()
    {

    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            foreach(Transform child in transform)
            {
                child.gameObject.SetActive(!child.gameObject.activeSelf);
            }
        }
        if (Input.GetKey(KeyCode.L))
        {
            AddItem("nullitem", 1);
        }
        if (Input.GetKey(KeyCode.K))
        {
            RemoveItem("nullitem", 1);
        }
        if (Input.GetKey(KeyCode.N))
        {
            AddItem("stone", 1);
        }
        if (Input.GetKey(KeyCode.M))
        {
            RemoveItem("Stone", 1);
        }
        if (Input.GetKey(KeyCode.J))
        {
            GameObject lootable = Instantiate(Resources.Load("Lootable"), new Vector3(Random.Range(0,5), Random.Range(0, 5), Random.Range(0, 5)), Quaternion.identity) as GameObject;
            lootable.GetComponent<Lootable>().itemStored = theDatabase.GetRandomItem();
            lootable.GetComponent<Lootable>().lightColor = theDatabase.rarityColors[lootable.GetComponent<Lootable>().itemStored.rarity];
        }
    }

    public void ConstructInventory () //Sets up the empty slots for an inventory
    {
        for(int i = 0; i < inventorySlotSize; i++)
        {
            GameObject theSlot = Instantiate(Resources.Load("Inventory/InventorySlot"), this.transform.GetChild(0).transform) as GameObject;
            theSlot.name = "InventorySlot_" + i;
            inventorySlots.Add(new ItemSlot(null, 0, theSlot));
        }
    }

    public void AddItem(string itemName, int amountToAdd)
    {
        int amountToAddTemp = amountToAdd;
        Item itemToAdd = theDatabase.FindItem(itemName);
        ItemSlot tempSlot = null;
        for (int i = 0; i < inventorySlots.Count; i++) //Checking to see if it can add the item to a existing slot
        {
            ItemSlot slot = inventorySlots[i];
            if (slot.item == itemToAdd) //Another item with room has been found, does it have room
            {
                if (slot.amount + amountToAdd <= slot.item.maxAmount)
                {
                    slot.amount += amountToAdd;
                    slot.UpdateSlotVisuals();
                    return; //Item is completely in the inventory now, end
                }
                else //amount to add is too much, split it up
                {
                    int subtractionAmount = slot.item.maxAmount - slot.amount;
                    slot.amount = slot.item.maxAmount;
                    amountToAddTemp -= subtractionAmount;
                    slot.UpdateSlotVisuals();
                }
            }
        }

        //Adding new item to slot
        foreach (ItemSlot slot in inventorySlots)
        {
            if (slot.item == null && tempSlot == null) //Trying to find a empty slot to place the new item
            {
                tempSlot = slot;
            }
        }
        if(tempSlot != null) //Updating the new slot
        {
            tempSlot.item = itemToAdd;
            tempSlot.amount = amountToAddTemp;
            tempSlot.UpdateSlotVisuals();
        }
        else //No slots are avaiable!!!
        {
            Debug.LogWarning("[Inventory] No slots avaiable for adding the item: " + itemToAdd.name);
        }
    }

    public bool RemoveItem (string itemName, int amount)
    {
        Item itemToAdd = theDatabase.FindItem(itemName);
        for (int i = inventorySlots.Count-1; i > -1; i--) //Finding the slot with the item, starts from the bottom up
        {
            ItemSlot slot = inventorySlots[i];
            if (slot.item != null)
            {
                if (slot.item.name == itemToAdd.name)
                {
                    if (slot.amount <= amount)
                    {
                        int newAmount = amount - slot.amount;
                        slot.Clear();
                        RemoveItem(itemName, newAmount); //Recursive
                    }
                    else
                    {
                        slot.amount -= amount;
                        slot.UpdateSlotVisuals();
                    }
                    return true;
                }
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

public class ItemSlot
{
    public ItemSlot(Item item, int amount, GameObject theSlot)
    {
        this.item = item;
        this.amount = amount;
        this.theSlotObject = theSlot;
    }

    public void UpdateSlotVisuals () //Updates the image and amount text
    {
        theSlotObject.transform.Find("Item Image").GetComponent<Image>().sprite = item.sprite;
        theSlotObject.transform.Find("AmountText").GetComponent<TextMeshProUGUI>().text = amount.ToString();
    }

    public void Clear ()
    {
        item = null;
        amount = 0;
        theSlotObject.transform.Find("Item Image").GetComponent<Image>().sprite = null;
        theSlotObject.transform.Find("AmountText").GetComponent<TextMeshProUGUI>().text = "";
    }

    public Item item { get; set; }
    public int amount { get; set; }
    public GameObject theSlotObject { get; set; }
}
