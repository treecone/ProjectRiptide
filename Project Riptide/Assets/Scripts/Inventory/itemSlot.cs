using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public Item item;

    public ItemSlot(Item item)
    {
        this.item = item;
    }

    public void UpdateSlotVisuals() //Updates the image and amount text
    {
        gameObject.transform.Find("Icon").GetComponent<Image>().sprite = item.Icon;
        gameObject.transform.Find("AmountText").GetComponent<TextMeshProUGUI>().text = item.Amount.ToString();
    }

    public void Clear()
    {
        Destroy(gameObject);
    }
}
