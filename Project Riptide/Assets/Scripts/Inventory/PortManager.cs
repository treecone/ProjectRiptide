using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortManager : MonoBehaviour
{
	private GameObject player;
	private GameObject PortUI;
	public bool inPort;

    // Start is called before the first frame update
    void Start()
	{
		if(player == null)
			player = GameObject.FindGameObjectWithTag("Player");
        PortUI = GameObject.Find("Canvas").transform.Find("PortMainMenu").gameObject;
        inPort = false;
	}

    // Update is called once per frame
    void Update()
    {
		//If the escape timer is zero and the player is near the port, then disable player movement
		if (!inPort && Vector3.Distance(player.transform.position, transform.position) < 10)
		{
            inPort = true;
			player.GetComponent<ShipMovement>().enabled = false;
			PortUI.SetActive(true);
			player.GetComponent<Rigidbody>().velocity = Vector3.zero;
		}
	}

    public void LeavePort ()
    {
        inPort = false;
        player.transform.position = gameObject.transform.position + this.transform.right * -20;
        player.GetComponent<ShipMovement>().enabled = true;
        PortUI.SetActive(false);
    }
}
