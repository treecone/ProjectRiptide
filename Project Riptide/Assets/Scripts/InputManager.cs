using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    //Number of buttons currently being touched on screen, not sure if we'll need this variable
    public int buttonsPressed;

    public float minTouchDistance;

    private GameObject touchVisual;
    private GameObject touchVisualCursor;

    private Vector2 startTouchPos;
    private Vector2 currentTouchPos;


    void Start()
    {
        touchVisual = gameObject.transform.Find("TouchVisual").gameObject;
        touchVisualCursor = gameObject.transform.Find("TouchVisualCursor").gameObject;

        Vector2 startTouchPos = Vector2.zero;
        Vector2 currentTouchPos = Vector2.zero;
    }

    void Update()
    {
        DetectingMobileInput();
    }

    void DetectingMobileInput () //This method is responsiable for handing swipe and tapping
    {
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

                        break;

                    case (TouchPhase.Moved):
                        currentTouchPos = new Vector3(touch.position.x - gameObject.GetComponent<RectTransform>().rect.width / 2, touch.position.y - gameObject.GetComponent<RectTransform>().rect.height / 2, 0);
                        touchVisualCursor.GetComponent<RectTransform>().anchoredPosition = currentTouchPos;
                        if (Mathf.Abs(currentTouchPos.x - startTouchPos.x) > minTouchDistance || Mathf.Abs(currentTouchPos.y - startTouchPos.y) > minTouchDistance) //If the player has dragged a certain distanceS
                        {
                            Debug.Log("Player Is swipping!");
                        }
                        break;

                    case (TouchPhase.Ended):
                        touchVisual.GetComponent<Image>().enabled = false;
                        touchVisualCursor.GetComponent<Image>().enabled = false;
                        break;
                }
                MovementInput();
            }
        }
        else //No touches on screen
        {
            buttonsPressed = 0;
        }
    }

    //Returns a axis for the movement class out of 360 degrees in world z coordiantes
    private float MovementInput ()
    {
        return 5f;
    }
}
