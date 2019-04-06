using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	private InputManager inputManager; 
	private Rigidbody rb;

	public Vector3 touchMove;
	public float speed = 1f;
	public float rotationSpeed = 0.5f;

	private Camera camera;
	private Vector3 newPosition;

	// Start is called before the first frame update
	void Start()
    {
        inputManager = GameObject.Find("Canvas").GetComponent<InputManager>();
		rb = GetComponent<Rigidbody>();
		camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if(inputManager.currentlySwiping)
		{
			Vector2 touchPos = inputManager.currentTouchPos - inputManager.startTouchPos;
			touchMove = new Vector3(-touchPos.y, 0, touchPos.x);	//Convert 2D input into 3d movement
			touchMove.Normalize();
			
			//Apply a velocity to the ship
			rb.velocity = touchMove;

			//Look in the direction of movement
			gameObject.transform.LookAt(touchMove + gameObject.transform.position);
		}
		/*else if(Input.GetMouseButtonDown(0))
		{
			Vector3 clickedPos = Camera.main.ViewportToWorldPoint(Input.mousePosition);
			print(clickedPos.ToString());
			//clickedPos = new Vector2(clickedPos.x, 0, clickedPos.y);
			//transform.position = clickedPos;
		}*/
	}
}
