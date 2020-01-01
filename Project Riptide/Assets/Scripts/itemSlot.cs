using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlot
{
    public ItemSlot(Item item, int amount, GameObject theSlot)
    {
        this.item = item;
        this.amount = amount;
        this.theSlotObject = theSlot;
    }

    public void UpdateSlotVisuals() //Updates the image and amount text
    {
        theSlotObject.transform.Find("Item Image").GetComponent<Image>().sprite = item.sprite;
        theSlotObject.transform.Find("AmountText").GetComponent<TextMeshProUGUI>().text = amount.ToString();
    }

    public void Clear()
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
