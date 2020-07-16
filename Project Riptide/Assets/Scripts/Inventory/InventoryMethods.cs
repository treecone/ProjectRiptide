﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryMethods : MonoBehaviour
{
    #region Fields

    private Item _activeItem = null;            //set it automatically to null, closing UI resets to null as well

    #region SettingsUI
    [SerializeField]
    private Image _soundImage;
    [SerializeField]
    private Image _volumeImage;
    [SerializeField]
    private Sprite[] _soundSprites;
    [SerializeField]
    private Sprite[] _volumeSprites;
    [SerializeField]
    private Slider _volumeSlider;
    [SerializeField]
    private Slider _soundSlider;
    private float soundValue = .5f;
    private float volumeValue = .5f;
    #endregion
    #region MapUI
    [SerializeField]
    private Image[] _mapImages; //25th image is the player position marker
    #endregion
    #region Inventory
    [SerializeField]
    private TextMeshProUGUI _trashField;
    [SerializeField]
    private TextMeshProUGUI _itemName;
    [SerializeField]
    private TextMeshProUGUI _itemDescription;
    [SerializeField]
    private TextMeshProUGUI _trashName;
    [SerializeField]
    private TextMeshProUGUI _itemCost;
    #endregion
    #region CraftingUI
    //fields for crafting
    [SerializeField]
    private TextMeshProUGUI _craftingName;
    [SerializeField]
    private TextMeshProUGUI _stats1;
    [SerializeField]
    private TextMeshProUGUI _stats2;
    [SerializeField]
    private TextMeshProUGUI _stats3;
    [SerializeField]
    private TextMeshProUGUI _stats4;
    [SerializeField]
    private TextMeshProUGUI _activeAbility;
    [SerializeField]
    private TextMeshProUGUI _passiveAbility;
    [SerializeField]
    private GameObject _neededItems1;
    [SerializeField]
    private GameObject _neededItems2;
    [SerializeField]
    private GameObject _neededItems3;
    [SerializeField]
    private GameObject _neededItems4;
    [SerializeField]
    private Button _craftAndUpgrade;
    [SerializeField]
    private Sprite[] _craftButtonImages; //holds greyed out as well
    [SerializeField]
    private Sprite[] _upgradeButtonImages; //holds greyed out/maxed
    #endregion
    #region Scripts
    [SerializeField]
    private UIAnimMethods _uiAnimMethods;
    [SerializeField]
    private ChunkLoader _chunkLoader;
    [SerializeField]
    private Inventory _inventory;
    [SerializeField]
    private InputManager _inputManagerScript;
    #endregion


    #endregion

    public void Start()
    {
        _soundSlider.onValueChanged.AddListener(delegate { ChangeSoundImage(); });
        _volumeSlider.onValueChanged.AddListener(delegate { ChangeVolumeImage(); });
    }

    #region Shared Methods
    public void SelectItem(GameObject gObj)
    {
        _activeItem = gObj.GetComponent<InventorySlot>().item;
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
    /// Sets time scale to 0, may add animation here later
    /// called when opening UI
    /// </summary>
    public void PauseGame()
    {
        _inputManagerScript.enabled = false;
        Time.timeScale = 0.0f;
    }

    #endregion

    /// <summary>
    /// Chooses inventory slot
    /// </summary>
    /// <param name="inventorySlot"></param>
    public void ChooseInventoryItem(InventorySlot inventorySlot)
    {
        //automatically set this to 0
        _trashField.SetText("0");

        _activeItem = inventorySlot.item;
        Debug.Log("Clicked on " + _activeItem.Name);    
        _itemName.SetText(_activeItem.Name);
        _itemDescription.SetText(_activeItem.Description);
        _itemCost.SetText("" + _activeItem.Value);
        //{0} did not work here
        _trashName.SetText("Are you sure you want to throw out " + _activeItem.Name + "?");
    }
    /// <summary>
    /// changes trash number
    /// </summary>
    /// <param name="num">change number in TextMeshPro</param>
    public void ChangeNumber(int num)
    {
        int amount = System.Convert.ToInt32(_trashField.text);

        //if active item exists
        if (_activeItem != null || _activeItem.Name != "NullItem")
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
    /// trashes the item in inventory, checks number amounts
    /// </summary>
    public void TrashItem()
    {
        //checks if null item
        if (_activeItem.Name != null)
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

    #region SettingsMethods
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
    #endregion
    #region Map Methods
    public void UpdateMap()
    {
        //finds chunk of player, remove check later?
        Image chunk = _mapImages[0];
        if (_chunkLoader != null)
        {
            int chunkNum = (int)(_chunkLoader.CurrentChunkPosition.x * 5 + _chunkLoader.CurrentChunkPosition.y);
            chunk = _mapImages[chunkNum];
        }
        else
        {
            chunk = _mapImages[13];
        }
        //calculate what chunk player is in
        if (chunk.sprite.name == "SquareTeleBackground")
        {
            _mapImages[25].enabled = false;
            _uiAnimMethods.InHiddenChunk(chunk);
        }
        else
        {
            _mapImages[25].enabled = true;
            //calculate where it should be on the map
            _uiAnimMethods.InExposedChunk(chunk);
        }
    }
    //Exposes map (shipwreck)
    public void ExposeMap()
    {
        //finds chunk of player, remove check later?
        Image chunk = _mapImages[0];
        if (_chunkLoader != null)
        {
            int chunkNum = (int)(_chunkLoader.CurrentChunkPosition.x * 5 + _chunkLoader.CurrentChunkPosition.y);
            chunk = _mapImages[chunkNum];
        }
        else
        {
            chunk = _mapImages[13];
        }

        _mapImages[25].enabled = true;
        //calculate where it should be on the map
        _uiAnimMethods.ExposeChunk(chunk);
    }
    #endregion


    public void ExpandCraft(bool isCraft)
    {
        if (_activeItem != null || _activeItem.Name != "NullItem")
        {
            _craftingName.SetText(_activeItem.Name);
            //do stats here, set their colors
            _activeAbility.SetText("Active: ");
            _passiveAbility.SetText("Passive: ");
            //make method for crafting things
            if (isCraft)
            {
                _craftAndUpgrade.GetComponent<Image>().sprite = _craftButtonImages[0];
            }
            else
            {
                _craftAndUpgrade.GetComponent<Image>().sprite = _upgradeButtonImages[0];
            }
        }
    }


    //Use this to equip based on selected item
    public void Equip()
    {
        PlayerInventory.Instance.SetEquipped(_activeItem.Name);
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


}
