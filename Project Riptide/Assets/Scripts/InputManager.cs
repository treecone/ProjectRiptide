using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    //Number of buttons currently being touched on screen, not sure if we'll need this variable but for debug
    public int buttonsPressed;

    //---------Swiping-------------------------------
    public bool currentlySwiping;
    private Vector2 startTouchPos;
    public Vector2 currentTouchPos;
    public float minTouchDistance;

    // ------------GameObjects and visuals-----------
    private GameObject touchVisual;
    private GameObject touchVisualCursor;

    //---------Multi Tapping-------------------------
    private float multiTapTimer;
    public int tapCounter;
 

    void Start()
    {
        touchVisual = gameObject.transform.Find("TouchVisual").gameObject;
        touchVisualCursor = gameObject.transform.Find("TouchVisualCursor").gameObject;
        multiTapTimer = 0;
        Vector2 startTouchPos = Vector2.zero;
        Vector2 currentTouchPos = Vector2.zero;
    }

    void Update()
    {
        DetectingMobileInput();
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
                        touchVisual.GetComponent<RectTransform>().anchoredPosition = startTouchPos;
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
}
