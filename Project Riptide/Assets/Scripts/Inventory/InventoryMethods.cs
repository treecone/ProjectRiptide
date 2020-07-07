using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryMethods : MonoBehaviour
{
    #region Fields
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
    private TextMeshProUGUI _itemCost;
    [SerializeField]
    private TextMeshProUGUI _totalGoldInventory;
    [SerializeField]
    private Slider _volumeSlider;
    [SerializeField]
    private Slider _soundSlider;
    [SerializeField]
    private Inventory _inventory;

    [SerializeField]
    private Image _soundImage;
    [SerializeField]
    private Image _volumeImage;
    [SerializeField]
    private Sprite[] _soundSprites;
    [SerializeField]
    private Sprite[] _volumeSprites;

    private float soundValue = .5f;
    private float volumeValue = .5f;

    private Item _activeItem = null;            //set it automatically to null, closing inventory resets to null as well
    #endregion

    public void Start()
    {
        _soundSlider.onValueChanged.AddListener(delegate { ChangeSoundImage(); });
        _volumeSlider.onValueChanged.AddListener(delegate { ChangeVolumeImage(); });
    }

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
    /// called when opening UI
    /// </summary>
    public void PauseGame()
    {
        _inputManagerScript.enabled = false;
        Time.timeScale = 0.0f;
    }

    /// <summary>
    /// Sets time scale to 0, resets movement
    /// called when opening Port Menu
    /// </summary>
    public void PauseMarketGame()
    {
        _inputManagerScript.ResetMovement();
        _inputManagerScript.enabled = false;
        Time.timeScale = 0.0f;
    }

    /// <summary>
    /// Sets time scale to 1, may add animation here later
    /// </summary>
    public void UnpauseGame()
    {
        Time.timeScale = 1f;
        Invoke("Unpause", .5f);
    }
    //for invoking
    public void Unpause()
    {
        _inputManagerScript.enabled = true;
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
        _itemCost.SetText("{0}", _activeItem.Value);
        //{0} did not work here
        _trashName.SetText("Are you sure you want to throw out " + _activeItem.Name + "?");
    }

    /// <summary>
    /// trashes the item in inventory, checks number amounts
    /// </summary>
    public void TrashItem()
    {
        //checks if null item
        if (_activeItem.Id != 0)
        {
            int amount = System.Convert.ToInt32(_trashField.text);

            Item saved = _activeItem;

            if (amount >= _activeItem.Amount)
            {
                ResetActiveItem();
            }

            PlayerInventory.Instance.RemoveItem(saved.Name, amount);

        }
    }

    /// <summary>
    /// Resets sound/volume to default value
    /// </summary>
    public void ResetSound()
    {
        _volumeSlider.value = .5f;
        _soundSlider.value = .5f;
    }

    /// <summary>
    /// Press volume, switch between the two
    /// </summary>
    public void PressVolume()
    {
        //if it is 0, reset it
        if (_volumeSlider.value == 0)
        {
            _volumeSlider.value = volumeValue;
        }
        //save value, sets 0
        else
        {
            volumeValue = _volumeSlider.value;
            _volumeSlider.value = 0;
        }
    }
    /// <summary>
    /// Press sound, switch between the two
    /// </summary>
    public void PressSound()
    {
        //if it is 0, reset it
        if (_soundSlider.value == 0)
        {
            _soundSlider.value = soundValue;
        }
        //save value, sets 0
        else
        {
            soundValue = _soundSlider.value;
            _soundSlider.value = 0;
        }
    }

    public void ChangeSoundImage()
    {
        if (_soundSlider.value == 0)
        {
            _soundImage.sprite = _soundSprites[0];
        }
        else if (_soundSlider.value <= .33f)
        {
            _soundImage.GetComponent<Image>().sprite = _soundSprites[1];
        }
        else if (_soundSlider.value <= .66f)
        {
            _soundImage.GetComponent<Image>().sprite = _soundSprites[2];
        }
        else
        {
            _soundImage.GetComponent<Image>().sprite = _soundSprites[3];
        }
    }

    public void ChangeVolumeImage()
    {
        if (_volumeSlider.value == 0)
        {
            _volumeImage.sprite = _volumeSprites[0];
        }
        else if (_volumeSlider.value <= .33f)
        {
            _volumeImage.GetComponent<Image>().sprite = _volumeSprites[1];
        }
        else if (_volumeSlider.value <= .66f)
        {
            _volumeImage.GetComponent<Image>().sprite = _volumeSprites[2];
        }
        else
        {
            _volumeImage.GetComponent<Image>().sprite = _volumeSprites[3];
        }
    }


    public void UpdateGold()
    {
        _totalGoldInventory.SetText("{0}", PlayerInventory.Instance.totalGold);
    }

}
