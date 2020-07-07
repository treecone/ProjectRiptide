using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{
    //need to refactor
    #region Fields
    public List<GameObject> inventorySlots;

    private List<Item> equipment;
    [SerializeField]
    private GameObject goldText;
    private TextMeshProUGUI goldTextMesh;
    public Upgrades shipUpgradeScript;
    #endregion
    
    
    void Start()
    {
        
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tilde))
        {
            UpdateInventoryVisuals();
        }
    }

    private void UpdateInventoryVisuals()
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            inventorySlots[i].GetComponent<InventorySlot>().item = PlayerInventory.Instance.items[i];
            inventorySlots[i].GetComponent<InventorySlot>().UpdateSlotVisuals();
        }
        if (goldTextMesh == null)
        {
            goldTextMesh = goldText.GetComponent<TextMeshProUGUI>();
        }
        goldTextMesh.text = "" + PlayerInventory.Instance.totalGold;
    }
}
