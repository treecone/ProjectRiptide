using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortManager : MonoBehaviour
{
	public GameObject player;
	public GameObject PortUI;
	public PortUIManager UIManager; 

    // Start is called before the first frame update
    void Start()
	{
		if(player == null)
			player = GameObject.FindGameObjectWithTag("Player");
	}

    // Update is called once per frame
    void Update()
    {
		//If the escape timer is zero and the player is near the port, then disable player movement
		if (UIManager.EscapeTimer <= 0 && Vector3.Distance(player.transform.position, transform.position) < 1.5)
		{
			player.GetComponent<ShipMovementScript>().enabled = false;
			PortUI.SetActive(true);
			player.GetComponent<Rigidbody>().velocity = Vector3.zero;
			player.transform.position = gameObject.transform.position;
		}
	}
}
