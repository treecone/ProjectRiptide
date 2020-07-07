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

        if (Input.GetKeyDown(KeyCode.C))
        {
            PlayerInventory.Instance.AddItem("carpscale", 8);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            PlayerInventory.Instance.AddItem("wood", 4);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            PlayerInventory.Instance.AddItem("nails", 8);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            PlayerInventory.Instance.RemoveItem("nails", 3);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            PlayerInventory.Instance.AddItem("woodencannon", 1);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            PlayerInventory.Instance.AddItem("scalemailhull", 1);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            PlayerInventory.Instance.AddItem("silksails", 1);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            PlayerInventory.Instance.AddItem("grapeshot", 1);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            PlayerInventory.Instance.AddItem("healthyhull", 1);
        }
    }

    private void UpdateInventoryVisuals(List<Item> items, int totalGold)
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            inventorySlots[i].GetComponent<InventorySlot>().item = items[i];
            inventorySlots[i].GetComponent<InventorySlot>().UpdateSlotVisuals();
        }
        if (goldTextMesh == null)
        {
            goldTextMesh = goldText.GetComponent<TextMeshProUGUI>();
        }
        goldTextMesh.text = "" + totalGold;
    }
}
