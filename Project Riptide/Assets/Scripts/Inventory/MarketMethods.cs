﻿using System.Collections;
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
    public void SellItem()
    {
        //increase gold
        int amount = System.Convert.ToInt32(_sellBuyField.text);
        PlayerInventory.Instance.totalGold += _activeItem.Value * amount;
        UpdateGold();
        //save and delete item
        Item saved = _activeItem;
        if (amount >= _activeItem.Amount)
        {
            ResetActiveItem();
        }
        PlayerInventory.Instance.RemoveItem(saved.Name, amount);
    }

    /// <summary>
    /// update gold for inventory and vault inventory
    /// </summary>
    public void UpdateGold()
    {
        _totalGoldMarket.SetText("{0}", PlayerInventory.Instance.totalGold);
        Debug.Log(PlayerInventory.Instance.totalGold);
        //_inventoryVault.TotalGold = _inventoryShip.TotalGold;
    }

    /// <summary>
    /// Used to set values of buttons to emphasize ship, vault, or buying
    /// </summary>



}
