using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryMethods : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI _trashField;
    [SerializeField]
    private InputManager _inputManagerScript;
    [SerializeField]
    private TextMeshProUGUI _itemName;
    [SerializeField]
    private TextMeshProUGUI _itemDescription;

    private Item _activeItem = null;            //set it automatically to null, closing inventory resets to null as well
    
    /// <summary>
    /// changes trash number
    /// </summary>
    /// <param name="num">change number in TextMeshPro</param>
    public void ChangeNumber(int num)
    {
        int amount = System.Convert.ToInt32(_trashField.text);

        //if active item exists
        /*
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
            _trashField.SetText("{0}", amount);
        }
        */
        amount += num;
        _trashField.SetText("{0}", amount);

        Debug.Log("E");
    }

    /// <summary>
    /// Sets time scale to 0, may add animation here later
    /// called when opening 
    /// </summary>
    public void PauseGame()
    {
        _inputManagerScript.enabled = false;
        Time.timeScale = 0.0f;
    }

    /// <summary>
    /// Sets time scale to 1, may add animation here later
    /// </summary>
    public void UnpauseGame()
    {
        _inputManagerScript.enabled = true;
        Time.timeScale = 1.0f;
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
        _trashField.SetText("0");
    }


    public void ChooseItem(InventorySlot inventorySlot)
    {
        _activeItem = inventorySlot.item;
        _trashField.SetText("0");
        Debug.Log("Clicked on " + _activeItem.Name);
        _itemName.SetText(_activeItem.Name);
        _itemDescription.SetText(_activeItem.Description);
    }



}
