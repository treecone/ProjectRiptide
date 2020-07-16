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

    #endregion


    void Start()
    {
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

        goldTextMeshes = new List<TextMeshProUGUI>();
        foreach(GameObject g in goldTexts)
        {
            goldTextMeshes.Add(g.GetComponent<TextMeshProUGUI>());
        }

    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tilde))
        {
            UpdateInventoryVisuals();
        }
    }

    public void UpdateInventoryVisuals()
    {
        foreach (List<InventorySlot> inventory in inventorySlots)
        {
            for(int i = 0; i < inventory.Count; i++)
            {
                inventory[i].item = PlayerInventory.Instance.items[i];
                inventory[i].UpdateSlotVisuals();
            }
        }
        foreach (List<InventorySlot> vault in vaultSlots)
        {
            for (int i = 0; i < vault.Count; i++)
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




}
