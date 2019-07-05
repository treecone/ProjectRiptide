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
    public float minSwipeDelta = 100;
    public float minSwipeSpeed = 1000;

    public bool touchingLeft;
    public bool touchingRight;

    // ------------GameObjects and visuals-----------
    public GameObject touchVisual;
    private GameObject touchVisualCursor;
    private GameObject ship;
    public Slider speedSlider;

    //---------Multi Tapping-------------------------
    private float multiTapTimer;
    public int tapCounter;
 
	public bool mobile = true;

    public float yRot;

    void Start()
    {
        touchVisual = gameObject.transform.Find("TouchVisual").gameObject;
        touchVisualCursor = gameObject.transform.Find("TouchVisualCursor").gameObject;
        speedSlider = gameObject.transform.Find("ToggleSpeed").GetComponent<Slider>();
        multiTapTimer = 0;
        ship = GameObject.Find("/Ship");
    }

    void Update()
    {
		if(mobile)
			DetectingMobileInput();
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
                        
                        if((tapDisplacement / tapLength).magnitude > minSwipeSpeed && tapDisplacement.magnitude > minSwipeDelta)
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
            if (Mathf.Abs(currentTouchPos.x - startTouchPos.x) > minSwipeDelta || Mathf.Abs(currentTouchPos.y - startTouchPos.y) > minSwipeDelta) //If the player has dragged a certain distanceS
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
}
