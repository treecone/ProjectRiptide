using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryMethods : MonoBehaviour
{
    #region Fields

    private Item _activeItem = null;            //set it automatically to null, closing UI resets to null as well
    private Recipe _activeRecipe = null;        //set it automatically to null, closing UI resets to null as well

    private int trashAmount = 1;

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
    private bool[] _exposed = new bool[25];
    #endregion
    #region InventoryUI
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
    #region MarketUI
    [SerializeField]
    private TextMeshProUGUI _marketTrash;
    [SerializeField]
    private TextMeshProUGUI _marketName;
    [SerializeField]
    private TextMeshProUGUI _itemDescriptionMarket;
    [SerializeField]
    private TextMeshProUGUI _itemCostMarket;
    [SerializeField]
    private GameObject[] _sortButtonsMarket;
    [SerializeField]
    private TextMeshProUGUI _sellName;
    #endregion
    #region CraftingUI
    //fields for crafting
    [SerializeField]
    private TextMeshProUGUI _craftingName;
    [SerializeField]
    private TextMeshProUGUI[] _statsCrafting;
    [SerializeField]
    private TextMeshProUGUI _activeAbilityCrafting;
    [SerializeField]
    private TextMeshProUGUI _passiveAbilityCrafting;
    [SerializeField]
    private GameObject[] _neededItemsCrafting;
    [SerializeField]
    private Button _craftAndUpgrade;
    [SerializeField]
    private Sprite[] _craftButtonImages; //holds greyed out as well
    [SerializeField]
    private Sprite[] _upgradeButtonImages; //holds greyed out/maxed
    [SerializeField]
    private TextMeshProUGUI _repairShip;
    [SerializeField]
    private GameObject[] _sortButtonsCrafting;
    [SerializeField]
    private GameObject[] _sortButtonsEquipping;
    [SerializeField]
    private Image[] _equippedImages;
    [SerializeField]
    private GameObject _didCraft;
    [SerializeField]
    private TextMeshProUGUI _equippingText;
    [SerializeField]
    private GameObject   _equippingImage;
    [SerializeField]
    private ActiveAbilities _activeAbilities;
    #endregion
    #region VaultUI
    [SerializeField]
    private TextMeshProUGUI _vaultName;
    [SerializeField]
    private TextMeshProUGUI _vaultDescription;
    [SerializeField]
    private TextMeshProUGUI _vaultCost;
    [SerializeField]
    private TextMeshProUGUI _vaultTrash;
    [SerializeField]
    private TextMeshProUGUI _vaultTrashLine;
    #endregion
    #region Scripts
    [SerializeField]
    private UIAnimMethods _uiAnimMethods;
    [SerializeField]
    private ChunkLoader _chunkLoader;
    [SerializeField]
    private InputManager _inputManagerScript;
    [SerializeField]
    private ShipMovement _shipMovement;
    #endregion

    #endregion

    public void Start()
    {
        _soundSlider.onValueChanged.AddListener(delegate { ChangeSoundImage(); });
        _volumeSlider.onValueChanged.AddListener(delegate { ChangeVolumeImage(); });

        //set all to false
        for (int i = 0; i < 25; i++)
        {
            _exposed[i] = false;
        }
        int chunk = (int)((_chunkLoader.StartingChunk.x - 1) + (_chunkLoader.StartingChunk.y - 1) * 5);
        _exposed[chunk] = true;
        _mapImages[25].enabled = true;
        //calculate where it should be on the map
        _uiAnimMethods.ExposeChunk(_mapImages[chunk]);
    }

    #region Shared Methods
    /// <summary>
    /// Selects active item for inventory/vault/market
    /// </summary>
    /// <param name="gObj"></param>
    public void SelectItem(GameObject gObj)
    {
        if (gObj.GetComponent<InventorySlot>().item != null && gObj.GetComponent<InventorySlot>().item.Name != "Null")
        {
            _activeItem = gObj.GetComponent<InventorySlot>().item;
            SoundManager.instance.PlaySound(_activeItem.InventoryTapSound);
        }
    }

    public void SelectEquipment(GameObject gObj)
    {
        if (gObj.GetComponent<EquipmentSlot>().equipment != null && gObj.GetComponent<EquipmentSlot>().equipment.Name != "Null")
        {
            _activeItem = gObj.GetComponent<EquipmentSlot>().equipment;
        }
    }

    /// <summary>
    /// Selects both recipe and item for crafting
    /// </summary>
    /// <param name="gObj">Recipe Slot</param>
    public void SelectRecipe(GameObject gObj)
    {
        _activeRecipe = gObj.GetComponent<RecipeSlot>().Recipe;
        _activeItem = ItemDB.Instance.FindItem(_activeRecipe.result);
    }

    /// <summary>
    /// resets active item when inventory is closed
    /// called during closing inventory only
    /// </summary>
    public void ResetActiveItem()
    {
        _activeItem = null;
    }

    public void ResetActiveRecipe()
    {
        _activeItem = null;
        _activeRecipe = null;
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

    public void OpenPopUp(GameObject popup)
    {
        if (_activeItem != null)
        {
            popup.SetActive(true);
        }
    }

    #endregion

    #region Inventory Methods
    /// <summary>
    /// Chooses inventory slot
    /// </summary>
    /// <param name="inventorySlot"></param>
    public void ChooseInventoryItem()
    {
        if (_activeItem != null && _activeItem.Name != "Null")
        {
            //automatically set this to 0
            _trashField.SetText("1");
            trashAmount = 1;
            _itemName.SetText(_activeItem.Name);
            _itemDescription.SetText(_activeItem.Description);
            _itemCost.SetText(_activeItem.Value.ToString());
            _trashName.SetText("Are you sure you want to throw out " + _activeItem.Name + "?");
        }
    }
    /// <summary>
    /// changes trash number
    /// </summary>
    /// <param name="num">change number in TextMeshPro</param>
    public void ChangeNumber(int numIncrease)
    {
        //if active item exists
        if (_activeItem != null && _activeItem.Name != "NullItem")
        {
            trashAmount += numIncrease;
            //check for above and below minimum
            if (trashAmount >= _activeItem.Amount)
            {
                trashAmount = _activeItem.Amount;
            }
            else if (trashAmount < 1)
            {
                trashAmount = 1;
            }
            _trashField.SetText(trashAmount.ToString());
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
        if (_activeItem.Amount == 0)
        {
            _activeItem = null;
            _marketName.SetText("");
            _itemDescriptionMarket.SetText("");
            _itemCostMarket.SetText("");
        }
        //checks if null item
        if (_activeItem != null)
        {
            Item saved = _activeItem;

            PlayerInventory.Instance.RemoveItem(saved.Name, trashAmount);
            _trashField.SetText("1");
            trashAmount = 1;
        }
    }

    #endregion

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
    /// <summary>
    /// Changes sound image based on slider level
    /// </summary>
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
    /// <summary>
    /// Changes volume image based on slider level
    /// </summary>
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
        int chunk = 0;
        if (_chunkLoader != null)
        {
            chunk = (int)((_chunkLoader.CurrentChunkPosition.y - 1) + (_chunkLoader.CurrentChunkPosition.x - 1) * 5);
        }
        else
        {
            Debug.Log("[Inventory Methods] Could not find chunk.");
            chunk = 0;
        }

        //reset map
        for (int i = 0; i < 25; i++)
        {
            if (_exposed[i] == false)
            {
                _uiAnimMethods.LeaveHiddenChunk(_mapImages[i]);
            }
            else if (_exposed[i] == true)
            {
                _uiAnimMethods.LeaveExposedChunk(_mapImages[i]);
            }
        }

        //calculate what chunk player is in
        if (_exposed[chunk] == false)
        {
            _mapImages[25].enabled = false; //disable player
            _uiAnimMethods.InHiddenChunk(_mapImages[chunk]);
        }
        else
        {
            _mapImages[25].enabled = true;
            //calculate where it should be on the map
            _uiAnimMethods.InExposedChunk(_mapImages[chunk]);
        }
    }
    //Exposes map (shipwreck)
    public void ExposeMap()
    {
        //finds chunk of player, remove check later?
        int chunk = 0;
        if (_chunkLoader != null)
        {
            chunk = (int)((_chunkLoader.CurrentChunkPosition.y - 1) + (_chunkLoader.CurrentChunkPosition.x - 1) * 5);
        }
        else
        {
            Debug.Log("[Inventory Methods] Could not find chunk.");
            chunk = 0;
        }
        _exposed[chunk] = true;
        _mapImages[25].enabled = true;
        //calculate where it should be on the map
        _uiAnimMethods.ExposeChunk(_mapImages[chunk]);
    }

    /// <summary>
    /// Returns if a chunk has been exposed
    /// </summary>
    /// <param name="x">x pos of chunk</param>
    /// <param name="y">y pos of chunk</param>
    /// <returns>If chunk has been exposed</returns>
    public bool ChunkExposed(int x, int y)
    {
        return _exposed[(x - 1) + ((y - 1) * 5)];
    }
    #endregion

    #region Market Methods
    public void ResetMarketDetails()
    {
        _marketTrash.SetText("1");
        trashAmount = 1;
        _marketName.SetText("");
        _itemDescriptionMarket.SetText("");
        _itemCostMarket.SetText("");
    }

    /// <summary>
    /// Chooses inventory slot
    /// </summary>
    /// <param name="inventorySlot"></param>
    public void ChooseMarketItem()
    {
        if (_activeItem != null)
        {
            //automatically set this to 0
            _marketTrash.SetText("1");
            trashAmount = 1;
            _marketName.SetText(_activeItem.Name);
            _itemDescriptionMarket.SetText(_activeItem.Description);
            _itemCostMarket.SetText(_activeItem.Value.ToString());
            _sellName.SetText("Are you sure you want to sell " + _activeItem.Name + "?");
        }
        else
        {
            _marketTrash.SetText("1");
            trashAmount = 1;
            _marketName.SetText("");
            _itemDescriptionMarket.SetText("");
            _itemCostMarket.SetText("");
        }
    }

    /// <summary>
    /// Chooses button for market, 3 buttons
    /// </summary>
    /// <param name="num"></param>
    public void ChooseButtonMarket(int num)
    {
        for (int i = 0; i < 3; i++)
        {
            if (_sortButtonsMarket[i].GetComponent<Image>().color == Color.white)
            {
                _uiAnimMethods.ResetButton(_sortButtonsMarket[i]);
            }
            if (i == num)
            {
                _uiAnimMethods.ChooseButton(_sortButtonsMarket[i]);
            }
        }
    }

    /// <summary>
    /// changes trash number
    /// </summary>
    /// <param name="num">change number in TextMeshPro</param>
    public void ChangeNumberMarket(int num)
    {
        //if active item exists
        if (_activeItem != null)
        {
            trashAmount += num;
            //check for above and below minimum
            if (trashAmount >= _activeItem.Amount)
            {
                trashAmount = _activeItem.Amount;
            }
            else if (trashAmount < 1)
            {
                trashAmount = 1;
            }
            _marketTrash.SetText(trashAmount.ToString());
        }
        else
        {
            Debug.Log("No Item");
        }
    }

    /// <summary>
    /// Sells item or buys depending on bool
    /// </summary>
    /// <param name="sell">sell for true, buy for false</param>
    public void SellItem(bool sell)
    {
        if (_activeItem != null)
        {
            //sells item
            if (sell)
            {
                PlayerInventory.Instance.TotalGold += _activeItem.Value * trashAmount;
                PlayerInventory.Instance.RemoveItem(_activeItem.Name, trashAmount);
            }
            //buys item
            else
            {
                if (PlayerInventory.Instance.TotalGold < _activeItem.Value * trashAmount)
                {
                    //sets it to max gold amount
                    trashAmount = PlayerInventory.Instance.TotalGold / _activeItem.Value;
                    //just double checking
                    _marketTrash.SetText(trashAmount.ToString());
                }
                PlayerInventory.Instance.TotalGold -= _activeItem.Value * trashAmount;
                PlayerInventory.Instance.AddItem(_activeItem.Name, trashAmount);
                PortManager.LastPortVisited.RemoveItem(_activeItem.Name, trashAmount);
            }
            if (_activeItem.Amount == 0)
            {
                _activeItem = null;
                _marketName.SetText("");
                _itemDescriptionMarket.SetText("");
                _itemCostMarket.SetText("");
            }
            _marketTrash.SetText("1");
            trashAmount = 1;
        }

    }

    public void ChangeToBuy()
    {
        if (_activeItem != null)
        {
            _sellName.SetText("Are you sure you want to buy " + _activeItem.Name + "?");
        }
    }

    #endregion

    #region Craft Methods
    /// <summary>
    /// expands crafting menu
    /// </summary>
    /// <param name="isCraft"></param>
    public void ExpandCraft()
    {
        if (_activeRecipe != null)
        {
            _craftingName.SetText(_activeItem.Name);
            //stats
            for (int i = 0; i < 4; i++)
            {
                if (_activeItem.Upgrades.Count > i)
                {
                    float upgradeVal = _activeItem.Upgrades[i].upgradeValue;
                    if (upgradeVal < 0)
                    {
                        _statsCrafting[i].color = Color.red;
                        _statsCrafting[i].SetText(_activeItem.Upgrades[i].upgradeType.ToString() + " " + _activeItem.Upgrades[i].upgradeValue);
                    }
                    else
                    {
                        _statsCrafting[i].color = Color.green;
                        _statsCrafting[i].SetText(_activeItem.Upgrades[i].upgradeType.ToString() + " +" + _activeItem.Upgrades[i].upgradeValue);
                    }
                }
                else
                {
                    _statsCrafting[i].SetText("");
                }
            }
            //abilities
            if (_activeItem.PassiveText != null && _activeItem.PassiveText != "")
            {
                _activeAbilityCrafting.SetText("Active: " + _activeItem.ActiveText);
                _passiveAbilityCrafting.SetText("Passive: " + _activeItem.PassiveText);
            }
            else if (_activeItem.ActiveText != null && _activeItem.ActiveText != "")
            {
                _activeAbilityCrafting.SetText("Active: " + _activeItem.ActiveText);
                _passiveAbilityCrafting.SetText("");
            }
            else
            {
                _activeAbilityCrafting.SetText("");
                _passiveAbilityCrafting.SetText("");
            }
            //ingredients
            for (int i = 0; i < 5; i++)
            {
                if (_activeRecipe.ingredients.Count > i)
                {
                    Item ingredient = ItemDB.Instance.FindItem(_activeRecipe.ingredients[i]);
                    _neededItemsCrafting[i].SetActive(true);
                    _neededItemsCrafting[i].transform.GetChild(0).GetComponent<Image>().sprite = ingredient.Icon;
                    _neededItemsCrafting[i].transform.GetChild(2).GetComponent<TextMeshProUGUI>().SetText(ingredient.Name);
                    _neededItemsCrafting[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText("{0}/{1}", PlayerInventory.Instance.CountOf(ingredient.Name), _activeRecipe.ingredientAmounts[i]);
                }
                else
                {
                    _neededItemsCrafting[i].SetActive(false);
                }
            }
            //crafting/upgrading button
            if (_activeItem.Rarity == 1)   
            {
                if (Crafting.Instance.CanCraft(_activeRecipe))
                {
                    _craftAndUpgrade.GetComponent<Image>().sprite = _craftButtonImages[1];
                }
                else
                {
                    _craftAndUpgrade.GetComponent<Image>().sprite = _craftButtonImages[0];
                }
            }
            else
            {
                if (Crafting.Instance.CanCraft(_activeRecipe))
                {
                    _craftAndUpgrade.GetComponent<Image>().sprite = _upgradeButtonImages[1];
                }
                else
                {
                    _craftAndUpgrade.GetComponent<Image>().sprite = _upgradeButtonImages[0];
                }
            }
        }
        else
        {
            _craftingName.SetText("");
            for (int i = 0; i < 4; i++)
            {
                _statsCrafting[i].SetText("");
            }
            _activeAbilityCrafting.SetText("");
            _passiveAbilityCrafting.SetText("");
            for (int i = 0; i < 5; i++)
            {
                _neededItemsCrafting[i].SetActive(false);
            }
            _craftAndUpgrade.GetComponent<Image>().sprite = _craftButtonImages[0];
        }
    }

    //Use this to equip based on selected item
    public void Equip()
    {
        PlayerInventory.Instance.SetEquipped(_activeItem);
        switch (_activeItem.Category)
        {
            case ItemCategory.Cannon:
                _activeAbilities.SetActiveSkill(1, (SkillType)_activeItem.ActiveAbilityID);
                break;
            case ItemCategory.Hull:
                _activeAbilities.SetActiveSkill(2, (SkillType)_activeItem.ActiveAbilityID);
                break;
            case ItemCategory.Sails:
                _activeAbilities.SetActiveSkill(0, (SkillType)_activeItem.ActiveAbilityID);
                break;
            case ItemCategory.Ship:
                _shipMovement.ChangeShipClass(_activeItem.ShipPrefab);
                break;
        }
    }

    //use this to craft based on selected item
    public void Craft()
    {
        if (_activeRecipe != null)
        {
            if (Crafting.Instance.CanCraft(_activeRecipe))
            {
                _equippingText.SetText("You have crafted a " + _activeRecipe.name + ".\nEquip now?");
                //rarity
                if (_activeItem.Rarity == 1)
                {
                    _equippingImage.transform.GetChild(0).GetComponent<Image>().color = Color.white;
                }
                else if (_activeItem.Rarity == 2)
                {
                    _equippingImage.transform.GetChild(0).GetComponent<Image>().color = new Color32(27, 150, 71, 255); //green
                }
                else if (_activeItem.Rarity == 3)
                {
                    _equippingImage.transform.GetChild(0).GetComponent<Image>().color = new Color32(253, 185, 63, 255);    //gold
                }
                else
                {
                    _equippingImage.transform.GetChild(0).GetComponent<Image>().color = new Color32(137, 77, 158, 255);   //purple
                }
                _equippingImage.transform.GetChild(1).GetComponent<Image>().sprite = _activeItem.Icon;
                Crafting.Instance.Craft(_activeRecipe);
                _didCraft.SetActive(true);
                _activeRecipe = null;
            }
            else
            {
                Debug.Log("Cannot Craft");
            }

        }
        else
        {
            Debug.Log("No Item");
        }
    }

    //choose button sorting
    public void ChooseButtonSort(int num)
    {
        for (int i = 0; i < 6; i++)
        {
            if (_sortButtonsCrafting[i].GetComponent<Image>().color == Color.white)
            {
                _uiAnimMethods.ResetButton(_sortButtonsCrafting[i]);
            }
            if (i == num)
            {
                _uiAnimMethods.ChooseButton(_sortButtonsCrafting[i]);
            }
        }
    }
    #endregion

    #region Equip Methods
    public void RepairShip(PlayerHealth player)
    {
        int goldAmount = System.Convert.ToInt32(_repairShip.text);
        int totalGold = PlayerInventory.Instance.totalGold;
        if (totalGold >= goldAmount)
        {
            totalGold -= goldAmount;
            player.AddHealth(player.MaxHealth - player.Health);
        }
        else
        {
            float percentage = (player.MaxHealth - player.Health) * (totalGold/goldAmount);
            totalGold = 0;

            player.AddHealth(percentage);
        }

        PlayerInventory.Instance.totalGold = totalGold;
    }

    //choose button sorting
    public void ChooseEquipButtonSort(int num)
    {
        for (int i = 0; i < 6; i++)
        {
            if (_sortButtonsEquipping[i].GetComponent<Image>().color == Color.white)
            {
                _uiAnimMethods.ResetButton(_sortButtonsEquipping[i]);
            }
            if (i == num)
            {
                _uiAnimMethods.ChooseButton(_sortButtonsEquipping[i]);
            }
        }
    }

    #endregion

    #region Vault Methods
    public void ResetVaultDetails()
    {
        _vaultTrash.SetText("1");
        trashAmount = 1;
        _vaultName.SetText("");
        _vaultDescription.SetText("");
        _vaultCost.SetText("");
    }
    
    /// <summary>
    /// Chooses inventory slot
    /// </summary>
    /// <param name="inventorySlot"></param>
    public void ChooseVaultItem()
    {
        if (_activeItem != null && _activeItem.Name != "Null")
        {
            _vaultTrash.SetText("1");
            trashAmount = 1;
            _vaultName.SetText(_activeItem.Name);
            _vaultDescription.SetText(_activeItem.Description);
            _vaultCost.SetText(_activeItem.Value.ToString());
        }
        else
        {
            _vaultTrash.SetText("1");
            trashAmount = 1;
            _vaultName.SetText("");
            _vaultDescription.SetText("");
            _vaultCost.SetText("");
        }
    }
    /// <summary>
    /// changes trash number in vault
    /// </summary>
    /// <param name="num">change number in TextMeshPro</param>
    public void ChangeVaultNumber(int num)
    {
        //if active item exists
        if (_activeItem != null)
        {
            trashAmount += num;
            //check for above and below minimum
            if (trashAmount >= _activeItem.Amount)
            {
                trashAmount = _activeItem.Amount;
            }
            else if (trashAmount < 1)
            {
                trashAmount = 1;
            }
            _vaultTrash.SetText(trashAmount.ToString());
        }
        else
        {
            Debug.Log("No Item");
        }
    }
    /// <summary>
    /// trashes the item in vault, checks number amounts
    /// </summary>
    public void TrashInventoryItem()
    {
        //checks if null item
        if (_activeItem != null && _activeItem.Name != "Null")
        {
            Item saved = _activeItem;

            if (trashAmount >= _activeItem.Amount)
            {
                ResetActiveItem();
            }
            PlayerInventory.Instance.RemoveItem(saved.Name, trashAmount);
        }
    }
    public void TrashVaultItem()
    {
        //checks if null item
        if (_activeItem != null && _activeItem.Name != "Null")
        {
            PlayerVault.Instance.RemoveItem(_activeItem.Name, trashAmount);
        }
    }
    public void AddToShip()
    {
        if (_activeItem != null && _activeItem.Name != "Null")
        {
            PlayerInventory.Instance.AddItem(_activeItem.Name, trashAmount);
            PlayerVault.Instance.RemoveItem(_activeItem.Name, trashAmount);

            if (_activeItem.Amount == 0)
            {
                _activeItem = null;
                _vaultTrash.SetText("1");
                trashAmount = 1;
                _vaultName.SetText("");
                _vaultDescription.SetText("");
                _vaultCost.SetText("");
            }
        }
    }

    public void AddToVault()
    {
        if (_activeItem != null && _activeItem.Name != "Null")
        {
            int savedAmount = _activeItem.Amount;

            Debug.Log(_activeItem.Name + _activeItem.Amount);
            PlayerVault.Instance.AddItem(_activeItem.Name, trashAmount);
            PlayerInventory.Instance.RemoveItem(_activeItem.Name, trashAmount);

            if (_activeItem.Amount == 0)
            {
                _activeItem = null;
                _vaultTrash.SetText("1");
                trashAmount = 1;
                _vaultName.SetText("");
                _vaultDescription.SetText("");
                _vaultCost.SetText("");
            }
        }
    }

    public void ChangeToMove()
    {
        if (_activeItem != null)
        {
            _vaultTrashLine.SetText("Move " + _activeItem.Name + " to: ");
        }
    }
    #endregion
}
