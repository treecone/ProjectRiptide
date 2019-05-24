﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    //Number of buttons currently being touched on screen, not sure if we'll need this variable but for debug
    public int buttonsPressed;

    //---------Swiping-------------------------------
    public bool currentlySwiping;
    public Vector2 startTouchPos;
    public Vector2 currentTouchPos;
    public float minTouchDistance;

    // ------------GameObjects and visuals-----------
    public GameObject touchVisual;
    private GameObject touchVisualCursor;
    private GameObject ship;

    //---------Multi Tapping-------------------------
    private float multiTapTimer;
    public int tapCounter;
 
	public bool mobile = true;

    public float yRot;

    void Start()
    {
        touchVisual = gameObject.transform.Find("TouchVisual").gameObject;
        touchVisualCursor = gameObject.transform.Find("TouchVisualCursor").gameObject;
        multiTapTimer = 0;
        Vector2 startTouchPos = Vector2.zero;
        Vector2 currentTouchPos = Vector2.zero;
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
                switch (touch.phase) //What is the first touch currently doing
                {
                    case (TouchPhase.Began):
                        touchVisual.GetComponent<Image>().enabled = true;
                        touchVisualCursor.GetComponent<Image>().enabled = true;
                        startTouchPos = new Vector3(touch.position.x - gameObject.GetComponent<RectTransform>().rect.width / 2, touch.position.y - gameObject.GetComponent<RectTransform>().rect.height / 2, 0);
                        //The offset of the touch visual.
                        Vector2 touchVisualOffset = Vector2.zero;
                        //The y rotation/orientation of the ship.
                        yRot = ship.GetComponent<Rigidbody>().transform.rotation.ToEuler().y * Mathf.Rad2Deg;
                        //Offset the touch cursor.
                        touchVisualOffset = new Vector2(Mathf.Cos(Mathf.Deg2Rad*yRot)*-1f, Mathf.Sin(Mathf.Deg2Rad*yRot));           
                        touchVisual.GetComponent<RectTransform>().anchoredPosition = startTouchPos + touchVisualOffset*100;
                        touchVisualCursor.GetComponent<RectTransform>().anchoredPosition = startTouchPos;
                        tapCounter += 1;
                        break;

                    case (TouchPhase.Moved):
                    case (TouchPhase.Stationary):
                        currentTouchPos = new Vector3(touch.position.x - gameObject.GetComponent<RectTransform>().rect.width / 2, touch.position.y - gameObject.GetComponent<RectTransform>().rect.height / 2, 0);
                        touchVisualCursor.GetComponent<RectTransform>().anchoredPosition = currentTouchPos;
                        if (Mathf.Abs(currentTouchPos.x - startTouchPos.x) > minTouchDistance || Mathf.Abs(currentTouchPos.y - startTouchPos.y) > minTouchDistance) //If the player has dragged a certain distanceS
                        {
                            //Do stuff while swiping?
                            currentlySwiping = true;
                        }
                        else
                        {
                            currentlySwiping = false;
                        }
                        break;

                    case (TouchPhase.Ended):
                        touchVisual.GetComponent<Image>().enabled = false;
                        touchVisualCursor.GetComponent<Image>().enabled = false;
                        break;
                }
            }
        }
        else //No touches on screen
        {
            buttonsPressed = 0;
			currentlySwiping = false;
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
            if (Mathf.Abs(currentTouchPos.x - startTouchPos.x) > minTouchDistance || Mathf.Abs(currentTouchPos.y - startTouchPos.y) > minTouchDistance) //If the player has dragged a certain distanceS
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
