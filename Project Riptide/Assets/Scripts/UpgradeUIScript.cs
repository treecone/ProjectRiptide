using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeUIScript : MonoBehaviour
{
	// Start is called before the first frame update
	void Start()
	{

	}

	void OnMouseOver()
	{
		if (Input.GetMouseButtonDown(0))
		{
			print("Clicked " + gameObject.name);
		}
	}

	void OnClick()
	{
		print("Clicked " + gameObject.name);

	}
}
