using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortManager : MonoBehaviour
{
	private GameObject player;
	public GameObject PortUI;

	/// <summary>
	/// Used to give player time to move away from the port after exiting the port menu
	/// </summary>
	public float EscapeTimer {get; set; } 

    // Start is called before the first frame update
    void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player");
		EscapeTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
		//If the escape timer is zero and the player is near the port, then disable player movement
		if(EscapeTimer <= 0 && Vector3.Distance(player.transform.position, transform.position) < 0.5)
		{
			player.GetComponent<PlayerController>().enabled = false;
			PortUI.SetActive(true);
		}
		else
		{
			player.GetComponent<PlayerController>().enabled = false;
		}
		if(EscapeTimer > 0)
		{
			EscapeTimer -= Time.deltaTime;
		}
	}
}
