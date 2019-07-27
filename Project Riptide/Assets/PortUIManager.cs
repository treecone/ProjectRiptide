using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortUIManager : MonoBehaviour
{
	private GameObject player;

	public GameObject PortUI;

	/// <summary>
	/// Used to give player time to move away from the port after exiting the port menu
	/// </summary>
	public float EscapeTimer { get; set; }

	// Start is called before the first frame update
	void Start()
    {
		player = GameObject.FindGameObjectWithTag("Player");
		PortUI.SetActive(false);
		EscapeTimer = 0;
	}

    // Update is called once per frame
    void Update()
    {
		if (EscapeTimer > 0)
		{
			EscapeTimer -= Time.deltaTime;
		}
	}

	public void ExitUI()
	{
		EscapeTimer = 5;
		PortUI.SetActive(false);
		player.GetComponent<PlayerController>().enabled = true;
	}
}
