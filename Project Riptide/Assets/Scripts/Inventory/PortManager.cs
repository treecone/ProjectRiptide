using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortManager : MonoBehaviour
{
    [SerializeField]
	private GameObject _player;
    [SerializeField]
    private InventoryMethods _inventoryMethods;
    [SerializeField]
    private int _portNumber;
    [SerializeField]
	private GameObject _canvas;
    [SerializeField]
    private GameObject _portUI;
    [SerializeField]    //can remove this later
	private bool _inPort;
    [SerializeField]
    private Button _leavePort;

    public bool InPort { get; set; }

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
            _leavePort = _portUI.transform.GetChild(4).gameObject.GetComponent<Button>();
        }
        _leavePort.onClick.AddListener(LeavePort);

        _inPort = false;
	}

    // Update is called once per frame
    void Update()
    {
		//If the escape timer is zero and the player is near the port, then disable player movement
		if (!_inPort && Vector3.Distance(_player.transform.position, transform.position) < 10)
		{
            _inPort = true;
			_portUI.SetActive(true);
            _player.GetComponent<ShipMovement>().StopMotion();
            _inventoryMethods.PauseGame();
        }
        
    }

    public void LeavePort()
    {
        _player.GetComponent<ShipMovement>().Position += new Vector3(5, 0, 5);
        _inPort = false;
        _portUI.SetActive(false);
        _inventoryMethods.UnpauseGame();
    }
}
