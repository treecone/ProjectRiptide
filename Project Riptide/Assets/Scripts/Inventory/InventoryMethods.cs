using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
    [SerializeField]
    private TextMeshProUGUI _trashName;
    [SerializeField]
    private Slider _volumeSlider;
    [SerializeField]
    private Slider _soundSlider;

    const float soundValue = .5f;

    private Item _activeItem = null;            //set it automatically to null, closing inventory resets to null as well
    
    /// <summary>
    /// changes trash number
    /// </summary>
    /// <param name="num">change number in TextMeshPro</param>
    public void ChangeNumber(int num)
    {
        int amount = System.Convert.ToInt32(_trashField.text);

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
            _trashField.SetText("{0}", amount);
        }
        else
        {
            Debug.Log("No Item");
        }
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

    /// <summary>
    /// Chooses inventory slot
    /// </summary>
    /// <param name="inventorySlot"></param>
    public void ChooseItem(InventorySlot inventorySlot)
    {
        //automatically set this to 0
        _trashField.SetText("0");

        _activeItem = inventorySlot.item;
        Debug.Log("Clicked on " + _activeItem.Name);    
        _itemName.SetText(_activeItem.Name);
        _itemDescription.SetText(_activeItem.Description);
        //{0} did not work here
        _trashName.SetText("Are you sure you want to throw out " + _activeItem.Name + "?");
    }


    public void TrashItem(Inventory inventory)
    {
        int amount = System.Convert.ToInt32(_trashField.text);
        inventory.RemoveItem(_activeItem.Name, amount);
    }

    public void ResetSound()
    {
        _volumeSlider.value = soundValue;
        _soundSlider.value = soundValue;
    }

    


}
