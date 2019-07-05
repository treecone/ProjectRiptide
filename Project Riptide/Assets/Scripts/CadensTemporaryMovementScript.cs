﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CadensTemporaryMovementScript : MonoBehaviour
{
    private InputManager inputManager;
    private Rigidbody rb;

    private float speedLevel = 0f;
    private float rotationSpeed = 0f;
    public float rotationScale;
    private float targetVelocity;
    private float currentVelocity;
    private float totalRotation;
    private float currentRotation;
    private float turningTime = 0f;
    private float turnSpeed = .001f;
    private float timer;

    public bool readyForSwipe = true;

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
        Turning();
        SetSpeed();
    }


    /// <summary>
    /// Detect input from the user and turn the ship accordingly.
    /// </summary>
    public void Turning()
    {
        //Rotate the ship ccw.
        if (inputManager.touchingLeft)
        {
            rotationSpeed = rotationScale;
            transform.Rotate(0, rotationSpeed, 0, Space.Self);
        }
        //Rotate the ship cw
        else if (inputManager.touchingRight)
        {
            rotationSpeed = -rotationScale;
            transform.Rotate(0, rotationSpeed, 0, Space.Self);
        }
        //Reset rotation speed.
        else
        {
            rotationSpeed = 0;
        }
    }
    /// <summary>
    /// Detect swiping movement from the player and adjust the ship's speed accordingly
    /// </summary>
    public void SetSpeed()
    {
        //Check for swiping and eligibility of player swipe.
        if (inputManager.currentlySwiping && readyForSwipe)
        {
            //Swipe up -- increase speed level.
            if (inputManager.currentTouchPos.y > inputManager.startTouchPos.y + 25 && speedLevel < 2)
            {
                speedLevel++;
                targetVelocity = speedLevel * 2.5f;
                readyForSwipe = false;
                Debug.Log("Speed Up");
            }
            //Swipe down -- decrease speed level.
            else if (inputManager.startTouchPos.y > inputManager.currentTouchPos.y + 25 && speedLevel > 0)
            {
                speedLevel--;
                targetVelocity = speedLevel * 2.5f;
                readyForSwipe = false;
                Debug.Log("Slow Down");
            }
        }
        //Adjust current velocity of the ship.
        if (currentVelocity < targetVelocity)
        {
            currentVelocity += .05f;
        }
        else if (currentVelocity > targetVelocity)
        {
            currentVelocity -= .05f;
        }
        //Move the ship.
        rb.velocity = transform.forward * currentVelocity;
        //See if enough time has elapsed to allow swipe.
        timer += Time.deltaTime;
        if (timer > 1f)
        {
            readyForSwipe = true;
            timer = 0;
        }
    }
}
