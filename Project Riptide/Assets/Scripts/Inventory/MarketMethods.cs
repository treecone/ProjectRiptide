using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MarketMethods : MonoBehaviour
{
    #region Fields
    [SerializeField]
    private InventoryMethods _inventoryMethods;
    [SerializeField]
    private Inventory _inventoryVault;
    [SerializeField]
    private Inventory _inventoryShip;
    //private Inventory _inventoryShop;
    [SerializeField]
    private TextMeshProUGUI _sellBuyField;
    [SerializeField]
    private TextMeshProUGUI _itemName;
    [SerializeField]
    private TextMeshProUGUI _itemDescription;
    [SerializeField]
    private TextMeshProUGUI _itemCost;
    [SerializeField]
    private TextMeshProUGUI _totalGoldMarket;


    const float soundValue = .5f;

    private Item _activeItem = null;            //set it automatically to null, closing inventory resets to null as well
    #endregion

    /// <summary>
    /// changes trash number
    /// </summary>
    /// <param name="num">change number in TextMeshPro</param>
    public void ChangeNumber(int num)
    {
        int amount = System.Convert.ToInt32(_sellBuyField.text);

        //if active item exists
        if (_activeItem != null)
        {
            amount += num;
            //check for above and below minimum
            if (amount >= _activeItem.Amount)
            {
                amount = _activeItem.Amount;
            }
            else if (amount < 0)
            {
                amount = 0;
            }
            _sellBuyField.SetText("{0}", amount);
        }
        else
        {
            Debug.Log("No Item");
        }
    }

    /// <summary>
    /// resets active item when inventory is closed
    /// called during closing inventory only
    /// </summary>
    public void ResetActiveItem()
    {
        _activeItem = null;
        _itemName.SetText("");
        _itemDescription.SetText("");
        _itemCost.SetText("");
        _sellBuyField.SetText("0");
    }

    /// <summary>
    /// Chooses inventory slot
    /// </summary>
    /// <param name="inventorySlot"></param>
    public void ChooseItem(InventorySlot inventorySlot)
    {
        //automatically set this to 0
        _sellBuyField.SetText("0");
        _activeItem = inventorySlot.item;
        Debug.Log("Clicked on " + _activeItem.Name);
        _itemName.SetText(_activeItem.Name);
        _itemDescription.SetText(_activeItem.Description);
        _itemCost.SetText("{0}", _activeItem.Value);
    }

    /// <summary>
    /// Sells item based on amount * value, adds to gold
    /// </summary>
    /// <param name="inventory"></param>
    public void SellItem(Inventory inventory)
    {
        //increase gold
        int amount = System.Convert.ToInt32(_sellBuyField.text);
        inventory.TotalGold += _activeItem.Value * amount;
        UpdateGold();
        //save and delete item
        Item saved = _activeItem;
        if (amount >= _activeItem.Amount)
        {
            ResetActiveItem();
        }
        inventory.RemoveItem(saved.Name, amount);
    }

    /// <summary>
    /// update gold for inventory and vault inventory
    /// </summary>
    public void UpdateGold()
    {
        _totalGoldMarket.SetText("{0}", _inventoryShip.TotalGold);
        Debug.Log(_inventoryShip.TotalGold);
        _inventoryVault.TotalGold = _inventoryShip.TotalGold;
    }

    /// <summary>
    /// Used to set values of buttons to emphasize ship, vault, or buying
    /// </summary>
    public void ChooseButton(GameObject button)
    {
        button.GetComponent<RectTransform>().sizeDelta = new Vector2(180, 80);
        button.GetComponent<Image>().color = new Color32(132, 132, 132, 255);
        button.GetComponent<Button>().interactable = false;
    }

    /// <summary>
    /// Used to set values of buttons to reset ship, vault, or buying
    /// </summary>
    /// <param name="button"></param>
    public void ResetButton(GameObject button)
    {
        button.GetComponent<RectTransform>().sizeDelta = new Vector2(180, 50);
        button.GetComponent<Image>().color = new Color32(100, 100, 100, 255);
        button.GetComponent<Button>().interactable = true;
    }


}
