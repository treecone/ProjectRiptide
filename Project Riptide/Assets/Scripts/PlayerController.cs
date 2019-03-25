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

    // Start is called before the first frame update
    void Start()
    {
        inputManager = GameObject.Find("Canvas").GetComponent<InputManager>();
		rb = GetComponent<Rigidbody>();
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

			//Old Test code that I may use later
			//if(touchMove.x > 0)
			//	rb.velocity = Vector3.forward * speed;
			//else if(touchMove.x < 0)
			//	rb.velocity = Vector3.back * speed;
			//rb.rotation = Quaternion.FromToRotation(gameObject.transform.position, touchMove + gameObject.transform.position);
		}
    }
}
