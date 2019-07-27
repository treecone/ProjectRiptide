using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortManager : MonoBehaviour
{
	private GameObject player;
	public GameObject PortUI;
	public GameObject GameManager;
	private PortUIManager UIManager; 

    // Start is called before the first frame update
    void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player");
		UIManager = GameManager.GetComponent<PortUIManager>();
	}

    // Update is called once per frame
    void Update()
    {
		//If the escape timer is zero and the player is near the port, then disable player movement
		if (UIManager.EscapeTimer <= 0 && Vector3.Distance(player.transform.position, transform.position) < 1.5)
		{
			player.GetComponent<PlayerController>().enabled = false;
			PortUI.SetActive(true);
			player.GetComponent<Rigidbody>().velocity = Vector3.zero;
			player.transform.position = gameObject.transform.position;
		}
	}
}
