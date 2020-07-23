using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{
    //need to refactor
    #region Fields

    [SerializeField]
    private List<GameObject> inventoryParents;
    [SerializeField]
    private List<GameObject> vaultParents;

    private List<List<InventorySlot>> inventorySlots;
    private List<List<InventorySlot>> vaultSlots;
    [SerializeField]
    private List<GameObject> goldTexts;
    private List<TextMeshProUGUI> goldTextMeshes;

    [SerializeField]
    private InventoryMethods _inventoryMethods;
    [SerializeField]
    private GameObject recipeParent;
    [SerializeField]
    private GameObject recipePrefab;
    private List<RecipeSlot> recipeSlots;

    [SerializeField]
    private GameObject _equipmentParent;
    [SerializeField]
    private GameObject _equipmentPrefab;
    private List<EquipmentSlot> equipmentSlots;

    #endregion


    void Start()
    {
        //inventory
        inventorySlots = new List<List<InventorySlot>>();
        foreach(GameObject inventoryParent in inventoryParents)
        {
            List<InventorySlot> slots = new List<InventorySlot>();
            foreach (Transform t in inventoryParent.transform)
            {
                if (t.gameObject.GetComponent<InventorySlot>() != null)
                {
                    slots.Add(t.gameObject.GetComponent<InventorySlot>());
                }
            }
            inventorySlots.Add(slots);
        }
        //vault
        vaultSlots = new List<List<InventorySlot>>();
        foreach (GameObject vaultParent in vaultParents)
        {
            List<InventorySlot> slots = new List<InventorySlot>();
            foreach (Transform t in vaultParent.transform)
            {
                if (t.gameObject.GetComponent<InventorySlot>() != null)
                {
                    slots.Add(t.gameObject.GetComponent<InventorySlot>());
                }
            }
            vaultSlots.Add(slots);
        }
        //gold meshes
        goldTextMeshes = new List<TextMeshProUGUI>();
        foreach(GameObject g in goldTexts)
        {
            goldTextMeshes.Add(g.GetComponent<TextMeshProUGUI>());
        }
        UpdateInventoryVisuals();

        //recipes
        recipeSlots = new List<RecipeSlot>();
        List<Recipe> recipes = Crafting.Instance.Recipes();
        for (int i = 0; i < Mathf.Min(recipes.Count, recipeSlots.Count); i++)
        {
            GameObject newRecipe = Instantiate(recipePrefab, recipeParent.transform);
            newRecipe.GetComponent<RecipeSlot>().Recipe = recipes[i];
            newRecipe.GetComponent<RecipeSlot>().itemResult = ItemDB.Instance.FindItem(recipes[i].result);
            newRecipe.GetComponent<Button>().onClick.AddListener(delegate { _inventoryMethods.SelectRecipe(newRecipe); });
            newRecipe.GetComponent<Button>().onClick.AddListener(_inventoryMethods.ExpandCraft);
            recipeSlots.Add(newRecipe.GetComponent<RecipeSlot>());
        }
        UpdateRecipeVisuals();

        //generate equipment prefab for currently owned items (base equipment)
        equipmentSlots = new List<EquipmentSlot>();
        for (int i = 0; i < Mathf.Min(equipmentSlots.Count,PlayerInventory.Instance.equipment.Count); i++)
        {
            GameObject newEquipment = Instantiate(_equipmentPrefab, _equipmentParent.transform);
            //assign item
            newEquipment.GetComponent<EquipmentSlot>().equipment = PlayerInventory.Instance.equipment[i];
            newEquipment.transform.GetChild(6).GetComponent<Button>().onClick.AddListener(delegate { _inventoryMethods.SelectItem(newEquipment); });
            newEquipment.transform.GetChild(6).GetComponent<Button>().onClick.AddListener(_inventoryMethods.Equip);
            equipmentSlots.Add(newEquipment.GetComponent<EquipmentSlot>());
        }
        //generates equipment prefab for all items
        for (int i = 0; i < recipes.Count; i++)
        {
            GameObject newEquipment = Instantiate(_equipmentPrefab, _equipmentParent.transform);
            //assign item
            newEquipment.GetComponent<EquipmentSlot>().equipment = ItemDB.Instance.FindItem(recipes[i].result);
            newEquipment.transform.GetChild(6).GetComponent<Button>().onClick.AddListener(delegate { _inventoryMethods.SelectItem(newEquipment); });
            newEquipment.transform.GetChild(6).GetComponent<Button>().onClick.AddListener(_inventoryMethods.Equip);
            equipmentSlots.Add(newEquipment.GetComponent<EquipmentSlot>());
        }
        UpdateEquipmentVisuals();
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tilde))
        {
            Debug.Log("Being Called");
            UpdateInventoryVisuals();
        }
    }

    public void UpdateInventoryVisuals()
    {
        foreach (List<InventorySlot> inventory in inventorySlots)
        {
            for(int i = 0; i < PlayerInventory.Instance.items.Count; i++)
            {
                inventory[i].item = PlayerInventory.Instance.items[i];
                inventory[i].UpdateSlotVisuals();
            }
        }
        foreach (List<InventorySlot> vault in vaultSlots)
        {
            for (int i = 0; i < PlayerVault.Instance.items.Count; i++)
            {
                vault[i].item = PlayerVault.Instance.items[i];
                vault[i].UpdateSlotVisuals();
            }
        }
        foreach(TextMeshProUGUI textMesh in goldTextMeshes)
        {
            textMesh.text = "" + PlayerInventory.Instance.totalGold;
        }
    }
    

    public void UpdateRecipeVisuals()
    {
        for (int i = 0; i < recipeParent.transform.childCount; i++)
        {
            //destroy the recipe slot if it has been crafted
            if (!Crafting.Instance.IsUncrafted(recipeSlots[i].Recipe))
            {
                Destroy(recipeSlots[i]);
                recipeSlots.RemoveAt(i);
            }
            recipeSlots[i].UpdateSlotVisuals();
        }
        SortCraftRecipes();
    }

    public void SortCraftRecipes()
    {
        for (int i = 0; i < recipeParent.transform.childCount; i++)
        {
            //if you can craft, it gets moved to top
            if (Crafting.Instance.CanCraft(recipeSlots[i].Recipe))  
            {
                recipeParent.transform.GetChild(i).SetAsFirstSibling();
            }
        }
    }

    public void SortRecipeByType(int sortNum)
    {
        if (sortNum == -1)
        {
            for (int i = 0; i < recipeParent.transform.childCount; i++)
            {
                recipeParent.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
        else
        {
            for (int i = 0; i < recipeParent.transform.childCount; i++)
            {
                //if you can craft, it gets moved to top
                if (recipeSlots[i].itemResult.Category != (ItemCategory)sortNum)
                {
                    recipeParent.transform.GetChild(i).gameObject.SetActive(false);
                }
                else
                {
                    recipeParent.transform.GetChild(i).gameObject.SetActive(true);
                }
            }
        }
    }

    /// <summary>
    /// Sort all equipment by type, deactivate if it does not fit category
    /// </summary>
    /// <param name="sortNum">-1 is all, rest is ItemCategory</param>
    public void SortByEquipment(int sortNum) 
    {
        if (sortNum == -1)
        {
            for (int i = 0; i < _equipmentParent.transform.childCount; i++)
            {
                _equipmentParent.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
        else
        {
            for (int i = 0; i < recipeParent.transform.childCount; i++)
            {
                //if you can craft, it gets moved to top
                if (equipmentSlots[i].equipment.Category != (ItemCategory)sortNum)
                {
                    _equipmentParent.transform.GetChild(i).gameObject.SetActive(false);
                }
                else
                {
                    _equipmentParent.transform.GetChild(i).gameObject.SetActive(true);
                }
            }
        }
    }

    /// <summary>
    /// goes through and enables and disables the equipment that is valid
    /// </summary>
    public void UpdateEquipmentVisuals()
    {
        foreach (EquipmentSlot equipment in equipmentSlots)
        {
            if (PlayerInventory.Instance.GetFromName(equipment.equipment.Name) == null)
            {
                equipment.gameObject.SetActive(false);
            }
            else
            {
                equipment.gameObject.SetActive(true);
            }
            equipment.UpdateSlotVisuals();
        }
    }

}
