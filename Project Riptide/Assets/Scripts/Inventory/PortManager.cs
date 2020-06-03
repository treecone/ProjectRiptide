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
        _portUI = _canvas.transform.GetChild(8).gameObject;
        _inPort = false;
	}

    // Update is called once per frame
    void Update()
    {
		//If the escape timer is zero and the player is near the port, then disable player movement
		if (!_inPort && Vector3.Distance(_player.transform.position, transform.position) < 10)
		{
            _inPort = true;
			_player.GetComponent<ShipMovement>().enabled = false;
			_portUI.SetActive(true);
			_player.GetComponent<Rigidbody>().velocity = Vector3.zero;
            _inventoryMethods.PauseGame();
		}
	}

    public void LeavePort ()
    {
        _inPort = false;
        _player.transform.position = gameObject.transform.position + this.transform.right * -20;
        _player.GetComponent<ShipMovement>().enabled = true;
        _portUI.SetActive(false);
    }
}
