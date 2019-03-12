using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public int buttonsPressed;


    void Start()
    {
        
    }

    void Update()
    {
        DetectingMobileInput();
    }

    void DetectingMobileInput ()
    {
        if (Input.touches.Length > 0)
        {
            buttonsPressed = Input.touches.Length;
            if(Input.touches.Length == 1)
            {
                Debug.Log("Single Finger Position is (" + Input.touches[0].position + ")");
                gameObject.transform.Find("TouchVisual").GetComponent<RectTransform>().anchoredPosition = new Vector3(Input.touches[0].position.x - gameObject.GetComponent<RectTransform>().rect.width/2, Input.touches[0].position.y - gameObject.GetComponent<RectTransform>().rect.height / 2, 0);
                MovementInput();
            }
        }
        else
        {
            buttonsPressed = 0;
        }
    }

    private void MovementInput ()
    {
        
    }
}
