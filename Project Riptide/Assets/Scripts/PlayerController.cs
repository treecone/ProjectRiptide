using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	private InputManager inputManager; 
	private Rigidbody rb;

	public float speed = 1f;
    public float rotationSpeed = 0f;
    public float targetVelocity;
    public float currentVelocity;
    private float totalRotation;
    private float currentRotation;
    private float turningTime = 0f;
    private float turnSpeed = .001f;

    private Quaternion from;
    private Quaternion to;

	private Camera camera;
	private Vector3 newPosition;

    //The vectors defined by the points current touch position and touch visuals position.
    public Vector2 startTouchVector;
    public Vector2 currentTouchVector;

	// Start is called before the first frame update
	void Start()
    {
        inputManager = GameObject.Find("Canvas").GetComponent<InputManager>();
		rb = GetComponent<Rigidbody>();
		camera = Camera.main;
        startTouchVector = Vector2.zero;
        currentTouchVector = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        //If the player is swiping.
        if(inputManager.currentlySwiping)
		{
            //Set the current vector now.
            currentTouchVector = inputManager.currentTouchPos - (Vector2)(inputManager.touchVisual.GetComponent<RectTransform>().anchoredPosition);
            //If this is the first frame of touching, set the start touch vector.
            if(startTouchVector == Vector2.zero)
            {
                startTouchVector = currentTouchVector;
            }
            //Only update velocity if the player hasn't made the touch vector shorter than what it began as.
            if(currentTouchVector.magnitude>=100)
            {
                //Set the target velocity.
                targetVelocity = currentTouchVector.magnitude/100f - 1f;
                if(targetVelocity>6.6f)
                {
                    targetVelocity = 6.6f;
                }
                //The total rotation needed.
                totalRotation = Vector2.SignedAngle(currentTouchVector,startTouchVector);
                //Rotate the boat.
                transform.Rotate(0, rotationSpeed, 0, Space.Self);
                //Increase the current anglular distance traveled by the boat.
                currentRotation += rotationSpeed;
                //determine the sign of rotation.
                float sign = (totalRotation > 0)? 1f : -1f;
                //Increase the speed of rotation.
                rotationSpeed += .002f * sign;             
                //The ship has completed its rotation, so reset the neccessary components.
                if(Mathf.Abs(currentRotation) >= Mathf.Abs(totalRotation))
                {
                    startTouchVector = currentTouchVector;
                    currentRotation = 0;
                    totalRotation = 0;
                    rotationSpeed = 0;
                }


                /*turningTime += Time.deltaTime * turnSpeed;
                float yRot = transform.rotation.ToEuler().y;
                Vector2 currentRotation = new Vector2(Mathf.Cos(yRot)*-1f, Mathf.Sin(yRot));
                float goalRotation = Vector2.SignedAngle(currentTouchVector,currentRotation) * Mathf.Rad2Deg; // How much the ship needs to turn total.
                from = transform.rotation;
                to = Quaternion.AngleAxis(goalRotation, Vector3.up) * from;
                transform.rotation = Quaternion.Lerp(from, to , turningTime);
                */
            }
			
        }
        //If the player is not swiping reset the touch vectors.
        else
        {
            currentTouchVector = Vector2.zero;
            startTouchVector = Vector2.zero;
        }
        //Update the ships velocity according to what its target velocity is set at.   
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
