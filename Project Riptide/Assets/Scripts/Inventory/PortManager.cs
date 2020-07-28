using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PortManager : MonoBehaviour
{
    private enum Size { Small, Medium, Large };

    [SerializeField]
    private Size portSize;

    public List<Item> _marketItems;
    public string name = "";
    public string tip = "";

    [SerializeField]
    private GameObject _player;
    [SerializeField]
    private InventoryMethods _inventoryMethods;
    [SerializeField]
    private GameObject _canvas;
    [SerializeField]
    private GameObject _portUI;
    [SerializeField]    //can remove this later
    private static bool _inPort;
    [SerializeField]
    private Button _leavePort;

    public static PortManager LastPortVisited;

    private void Start()
    {
        GenerateItems();
    }

    void Awake()
	{
        if (_player == null)
        {
            _player = GameObject.FindGameObjectWithTag("Player");
        }
        if (_canvas == null)
        {
            _canvas = GameObject.Find("Canvas");
        }
        if (_portUI == null)
        {
            _portUI = _canvas.transform.GetChild(8).gameObject;
        }
        
        if (_leavePort == null)
        {
            _leavePort = _portUI.transform.GetChild(0).GetChild(4).gameObject.GetComponent<Button>();
        }

        _leavePort.onClick.RemoveAllListeners();
        _leavePort.onClick.AddListener(LeavePort);

        _inventoryMethods = _canvas.GetComponent<InventoryMethods>();

        _inPort = false;
	}

    // Update is called once per frame
    void Update()
    {
		//If the escape timer is zero and the player is near the port, then disable player movement
		if (!_inPort && Vector3.SqrMagnitude(_player.transform.position - transform.position) < 100)
		{
            _inPort = true;
			_portUI.SetActive(true);
            //title
            _portUI.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().SetText(name);
            //tip
            _portUI.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().SetText("Tip: " + tip);
            _player.GetComponent<ShipMovement>().StopMotion();
            _inventoryMethods.PauseMarketGame();
            LastPortVisited = this;
        }
        
    }

    public void GenerateItems()
    {
        if (portSize == Size.Small)
        {
            _marketItems = DropManager.Instance.GetDrops("smallPort");
        }
        else if (portSize == Size.Medium)
        {
            _marketItems = DropManager.Instance.GetDrops("mediumPort");
        }
        else
        {
            _marketItems = DropManager.Instance.GetDrops("largePort");
        }

        /*
        for (int i = 0; i < _marketItems.Count; i++)
        {
            _marketItems[i].Value = (int)1.25*_marketItems[i].Value;
        }
        */
    }

    public void LeavePort()
    {
        _player.GetComponent<ShipMovement>().Position = new Vector3(LastPortVisited.transform.position.x, _player.transform.position.y, LastPortVisited.transform.position.z) - LastPortVisited.transform.right * 18.0f;
        //_player.GetComponent<ShipMovement>().Position += new Vector3(15.0f, 0, 15.0f);
        _inPort = false;
        _portUI.SetActive(false);
        _inventoryMethods.UnpauseGame();
    }


    public bool RemoveItem(string itemName, int amount)
    {
        if (_marketItems.Count == 0)
        {
            Debug.LogWarning("Nothing in inventory, nothing to delete!");
            return false;
        }
        int remaining = amount;
        for (int i = _marketItems.Count - 1; i > -1; i--) //Finding the slot with the item, starts from the bottom up
        {
            if (_marketItems[i].Name == itemName)
            {
                if (_marketItems[i].Amount <= remaining)
                {
                    remaining -= _marketItems[i].Amount;
                    _marketItems[i] = ItemDB.Instance.FindItem("null");
                    if (remaining == 0)
                    {
                        return true;
                    }
                }
                else
                {
                    _marketItems[i].Amount -= remaining;
                    return true;
                }
            }
        }
        Debug.LogWarning("[PortManager] When removing " + amount + " of " + itemName + ", not enough items of that type were found in the inventory!");
        return false;
    }

}
