using System.Collections;
using System.Collections.Generic;
using TMPro;
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

	public GameObject ScrollContent;

	public GameObject ListboxItem;

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
		player.GetComponent<ShipMovementScript>().enabled = true;
	}

	public void ItemSelected(GameObject obj)
	{
		foreach (Transform child in ScrollContent.transform)
		{
			Destroy(child.gameObject);
		}
		switch (obj.name)
		{
			case "Cannon":
				ScrollContent.GetComponent<RectTransform>();

				for (int i = 1; i <= 5; i++)
				{
					GameObject cannonItem = Instantiate(ListboxItem, ScrollContent.transform);
					cannonItem.GetComponentInChildren<Image>().color = Color.red;
					cannonItem.GetComponentInChildren<TextMeshProUGUI>().text = "Cannon " + i;
				}
				break;
			case "Sail":
				for (int i = 1; i <= 5; i++)
				{
					GameObject sailItem = Instantiate(ListboxItem, ScrollContent.transform);
					sailItem.GetComponentInChildren<Image>().color = Color.blue;
					sailItem.GetComponentInChildren<TextMeshProUGUI>().text = "Sail " + i;
				}
				break;
		}
	}
}
