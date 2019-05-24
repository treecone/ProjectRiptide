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
    public float targetVelocity;
    private float currentVelocity;

	private Camera camera;
	private Vector3 newPosition;

    //The vectors defined by the points current touch position and touch visuals position.
    public Vector2 previousTouchVector;
    public Vector2 currentTouchVector;

	// Start is called before the first frame update
	void Start()
    {
        inputManager = GameObject.Find("Canvas").GetComponent<InputManager>();
		rb = GetComponent<Rigidbody>();
		camera = Camera.main;
        previousTouchVector = Vector3.zero;
        currentTouchVector = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if(inputManager.currentlySwiping)
		{
			Vector2 touchPos = inputManager.currentTouchPos - inputManager.startTouchPos;
			touchMove = new Vector3(-touchPos.y, 0, touchPos.x);	//Convert 2D input into 3d movement
			touchMove.Normalize();
            //Set the current vector now.
            currentTouchVector = inputManager.currentTouchPos - (Vector2)(inputManager.touchVisual.GetComponent<RectTransform>().anchoredPosition);
            //Only update velocity if the player hasn't made the touch vector shorter than what it began as.
            if(currentTouchVector.magnitude>=100)
            {
                targetVelocity = currentTouchVector.magnitude/100f - 1f;
                if(targetVelocity>6.6f)
                {
                    targetVelocity = 6.6f;
                }
            }
			//If you have moved your finger from the start position even slightly.
            if(Mathf.Abs(touchPos.x) > 100f || Mathf.Abs(touchPos.y) > 100f)
            {
                //Rotate the ship.
                float goalRotation = Vector2.SignedAngle(currentTouchVector,previousTouchVector);
                transform.Rotate(0,goalRotation/1,0,Space.Self);
                //Change the ships velocity.   
                if(rb.velocity.magnitude < targetVelocity - .1f)
                {
                    currentVelocity += .01f;
                }
                else if(rb.velocity.magnitude > targetVelocity +.1f)
                {
                    currentVelocity -= .01f;
                }    
                rb.velocity = transform.forward * currentVelocity;
            }
        }
        else
        {
            currentTouchVector = Vector3.zero;
        }
	    previousTouchVector = currentTouchVector;
		/*else if(Input.GetMouseButtonDown(0))
		{
			Vector3 clickedPos = Camera.main.ViewportToWorldPoint(Input.mousePosition);
			print(clickedPos.ToString());
			//clickedPos = new Vector2(clickedPos.x, 0, clickedPos.y);
			//transform.position = clickedPos;
		}*/
	}
}
