using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    //Number of buttons currently being touched on screen, not sure if we'll need this variable but for debug
    public int buttonsPressed;

    public Camera camera;

    private float touchStartTime = 0;
    private Vector2 touchStartPosition = new Vector2();
    //---------Swiping-------------------------------
    public bool currentlySwiping;
    public Vector2 startTouchPos;
    public Vector2 currentTouchPos;

    public bool touchingLeft;
    public bool touchingRight;

    // ------------GameObjects and visuals-----------
    public GameObject touchVisual;
    private GameObject touchVisualCursor;
    
    public Slider speedSlider;

    //---------Multi Tapping-------------------------
    private float multiTapTimer;
    public int tapCounter;

    //REFACTORED VARIABLES ONLY BELOW HERE

    //-----References-----
    private GameObject ship;
    private ShipMovementScript movementScript;
    private CannonFireScript cannonFireScript;

    //-----Multiple touches-----
    private List<TouchData> currentTouches;
	public bool mobile = false;

    //-----Config values-----

    /// <summary>
    /// How close to the side of the screen a touch must be to cause rotation
    /// </summary>
    public float turnTouchArea;

    /// <summary>
    /// The minimum displacement a swipe must have to be considered a swipe
    /// </summary>
    public float minSwipeDisplacement = 100;

    /// <summary>
    /// The minimum speed a swipe must have to be considered a swipe
    /// </summary>
    public float minSwipeSpeed = 1000;

    /// <summary>
    /// The maximum time a tap can be held down and still be considered a tap
    /// </summary>
    public float maxTapDuration = 0.5f;

    void Start()
    {
        touchVisual = gameObject.transform.Find("TouchVisual").gameObject;
        touchVisualCursor = gameObject.transform.Find("TouchVisualCursor").gameObject;
        speedSlider = gameObject.transform.Find("ToggleSpeed").GetComponent<Slider>();
        multiTapTimer = 0;

        ship = GameObject.Find("/Ship");
        movementScript = ship.GetComponent<ShipMovementScript>();
        cannonFireScript = ship.GetComponent<CannonFireScript>();

        currentTouches = new List<TouchData>();
    }

    void Update()
    {
        if (mobile)
            TakeMobileInput();
        else
            DetectingKeyboardInput();
    }

    void DetectingMobileInput () //This method is responsiable for handing swipe and tapping
    {
        //Swiping --------------------------------------------------------------------------------------------------------
        if (Input.touches.Length > 0) //If there are 1 or more touches on screen
        {
            buttonsPressed = Input.touches.Length;
            if(Input.touches.Length == 1) //user is touching with a single finger
            {
                Touch touch = Input.GetTouch(0);
                Vector2 touchLocation = touch.position - new Vector2(Screen.width / 2, Screen.height / 2);
                switch (touch.phase) //What is the first touch currently doing
                {
                    case TouchPhase.Began:
                        touchVisual.GetComponent<Image>().enabled = true;
                        touchVisualCursor.GetComponent<Image>().enabled = true;

                        touchStartTime = Time.time;
                        touchStartPosition = touch.position;

                        Debug.Log("Touch down at " + touch.position + "Time: " + touchStartTime);
                        touchVisualCursor.GetComponent<RectTransform>().anchoredPosition = touchLocation;
                        touchVisual.GetComponent<RectTransform>().anchoredPosition = touchLocation;
                        /*
                        //The y rotation/orientation of the ship.
                        yRot = ship.GetComponent<Rigidbody>().transform.rotation.ToEuler().y * Mathf.Rad2Deg;
                        tapCounter += 1;*/
                        break;
                    case TouchPhase.Moved:
                    case TouchPhase.Stationary:
                        
                        touchVisualCursor.GetComponent<RectTransform>().anchoredPosition = touchLocation;
                        break;

                    case TouchPhase.Ended:
                        touchVisual.GetComponent<Image>().enabled = false;
                        touchVisualCursor.GetComponent<Image>().enabled = false;

                        Debug.Log("Touch up at " + touch.position);

                        float tapLength = Time.time - touchStartTime;
                        Vector2 tapDisplacement = touch.position - touchStartPosition;
                        
                        if((tapDisplacement / tapLength).magnitude > minSwipeSpeed && tapDisplacement.magnitude > minSwipeDisplacement)
                        {
                            Debug.Log("Swipe");
                            Debug.Log("Swipe speed: " + (tapDisplacement / tapLength).magnitude);
                            Debug.Log("Swipe displacement: " + tapDisplacement.magnitude);
                            string directionText = "Swipe Direction: ";
                            if(Math.Abs(tapDisplacement.x) > Math.Abs(tapDisplacement.y)) //if true, is swipe left or right
                            {
                                if(tapDisplacement.x > 0)
                                {
                                    directionText += "Right";
                                } else
                                {
                                    directionText += "Left";
                                }
                            } else //if false, is swipe up or down
                            {
                                if (tapDisplacement.y > 0)
                                {
                                    directionText += "Up";
                                }
                                else
                                {
                                    directionText += "Down";
                                }
                            }
                            Debug.Log(directionText);
                        } else if(tapLength < 0.5f)
                        {
                            Debug.Log("Tap " + tapLength);
                        } else
                        {
                            Debug.Log("Touch and Hold " + tapLength);
                        }
                        break;
                }
                //Detect touching left or right
                if (currentTouchPos.x > 100 && currentTouchPos.y > -800)
                {
                    touchingRight = true;
                    touchingLeft = false;
                }
                else if (currentTouchPos.x < -100 && currentTouchPos.y > -800)
                {
                    touchingRight = false;
                    touchingLeft = true;
                }
                else
                {
                    touchingLeft = false;
                    touchingRight = false;
                }
            }
        }
        else //No touches on screen
        {
            buttonsPressed = 0;
			currentlySwiping = false;
            touchingLeft = false;
            touchingRight = false;
		}
		//Multi Tapping ----------------------------------------------------------------------------------------------------------
		if(tapCounter > 0)
         {
            multiTapTimer += Time.deltaTime;
            if(multiTapTimer > 2.5f) //Manually change the timer length here!
            {
                if(tapCounter == 2)
                {
                    //DOUBLE TAP | DO SOMETHING?
                    Debug.Log("Double Tapped");
                }
                if (tapCounter == 3)
                {
                    //TRIPLE TAP | DO SOMETHING?
                    Debug.Log("Triple Tapped");
                }
                tapCounter = 0;
                multiTapTimer = 0;
            }
        }
    }

    
    void DetectingKeyboardInput()
    {
        //On first click on Mouse Button.
        if (Input.GetMouseButtonDown(0))
        {
            touchVisual.GetComponent<Image>().enabled = true;
            touchVisualCursor.GetComponent<Image>().enabled = true;
            startTouchPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            touchVisual.GetComponent<RectTransform>().anchoredPosition = startTouchPos;
            touchVisualCursor.GetComponent<RectTransform>().anchoredPosition = startTouchPos;
            tapCounter += 1;
        }
        //When the mouse button is held down.
        else if (Input.GetMouseButton(0))
        {
            currentTouchPos = new Vector3(Input.mousePosition.x - gameObject.GetComponent<RectTransform>().rect.width / 2, Input.mousePosition.y - gameObject.GetComponent<RectTransform>().rect.height / 2, 0);
            touchVisualCursor.GetComponent<RectTransform>().anchoredPosition = currentTouchPos;
            if (Mathf.Abs(currentTouchPos.x - startTouchPos.x) > minSwipeDisplacement || Mathf.Abs(currentTouchPos.y - startTouchPos.y) > minSwipeDisplacement) //If the player has dragged a certain distanceS
            {
                //Do stuff while swiping?
                currentlySwiping = true;
            }
            else
            {
                currentlySwiping = false;
            }
        }
        //The player has stopped presing the mouse button.
        else
        {
            touchVisual.GetComponent<Image>().enabled = false;
            touchVisualCursor.GetComponent<Image>().enabled = false;
            buttonsPressed = 0;
            currentlySwiping = false;
        }

        if (tapCounter > 0)
        {
            multiTapTimer += Time.deltaTime;
            if (multiTapTimer > 2.5f) //Manually change the timer length here!
            {
                if (tapCounter == 2)
                {
                    //DOUBLE TAP | DO SOMETHING?
                    Debug.Log("Double Tapped");
                }
                if (tapCounter == 3)
                {
                    //TRIPLE TAP | DO SOMETHING?
                    Debug.Log("Triple Tapped");
                }
                tapCounter = 0;
                multiTapTimer = 0;
            }
        }
    }

    void HandleTouch(TouchData t)
    {
        Debug.Log("Touch released: Duration - " + t.Duration + "   Displacement - " + t.Displacement.magnitude + "   Velocity - " + t.Velocity.magnitude);
        if(t.Velocity.magnitude > minSwipeSpeed && t.Displacement.magnitude > minSwipeDisplacement)
        {
            //All behavior for when a swipe is completed

            if(Math.Abs(t.Displacement.y) > Math.Abs(t.Displacement.x)) //the swipe is up or down
            {
                if(t.Displacement.y < 0) //swipe down
                {
                    movementScript.LinearVelocity -= movementScript.linearAcceleration;
                    Debug.Log("Swipe Down");
                } else //swipe up
                {
                    movementScript.LinearVelocity += movementScript.linearAcceleration;
                    Debug.Log("Swipe Up");
                }
            }

        } else if(t.Duration > maxTapDuration)
        {
            //All behavior for when a tap and hold is completed

        } else
        {
            //All behavior for when a tap is completed
            if (t.Position.x < turnTouchArea - Screen.width / 2) //tapped left side of screen
            {
                cannonFireScript.Fire("debugOneBig");
            }

            if (t.Position.x > Screen.width / 2 - turnTouchArea) //tapped right side of screen
            {
                cannonFireScript.Fire("debugTriShot");
            }
        }
    }

    void TakeMobileInput()
    {
        //Add any new touches to the touch list
        foreach(Touch t in Input.touches)
        {
            if(t.phase == TouchPhase.Began)
            {
                currentTouches.Add(new TouchData(t));
            }
        }
        bool turning = false;
        //Update all touches that are currently down
        for(int i = 0; i < currentTouches.Count; i++)
        {
            TouchData t = currentTouches[i];
            t.Update(Input.touches);

            //if the touch has just ended, remove it from the list and perform whatever behavior is appropriate for that touch
            if (t.phase == TouchPhase.Ended) 
            {
                currentTouches.Remove(t);
                i--;
                HandleTouch(t);
                continue;
            }

            if(t.Duration > maxTapDuration)
            {
                if (t.Position.x < turnTouchArea - Screen.width / 2) //turning left
                {
                    turning = true;
                    movementScript.RotationalVelocity -= movementScript.rotationalAcceleration;
                }

                if (t.Position.x > Screen.width / 2 - turnTouchArea) //turning right
                {
                    turning = true;
                    movementScript.RotationalVelocity += movementScript.rotationalAcceleration;
                }
            }
            
        }

        if(!turning) //ship is not turning, bring rotational velocity back to zero
        {

            movementScript.RotationalVelocity *= movementScript.rotationalDrag;
            if(Math.Abs(movementScript.RotationalVelocity) < movementScript.maxRotationalVelocity / 100)
            {
                movementScript.RotationalVelocity = 0;
            }
        }
    }

    private class TouchData
    {
        private Touch touch;
        private int index;
        private float duration;
        private Vector2 startPosition;

        public TouchPhase phase;

        public Vector2 Displacement
        {
            get
            {
                return Position - startPosition;
            }
        }

        public Vector2 Velocity
        {
            get
            {
                return Displacement / duration;
            }
        }

        public Vector2 Position
        {
            get
            {
                return touch.position - new Vector2(Screen.width / 2, Screen.height / 2);
            }
        }

        public float Duration
        {
            get
            {
                return duration;
            }
        }

        public TouchData(Touch touch)
        {
            this.touch = touch;
            index = touch.fingerId;
            duration = 0;
            startPosition = Position;
        }

        public void Update(Touch[] touches)
        {
            foreach (Touch t in touches)
            {
                if (t.fingerId == index)
                {
                    touch = t;
                }
            }
            duration += touch.deltaTime;
            phase = touch.phase;
        }
    }
}

