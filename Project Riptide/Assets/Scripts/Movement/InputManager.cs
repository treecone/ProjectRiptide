using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
	
	public Vector3 tempOffset =  new Vector3(-14.4f, 0, 5.9f);
	//Number of buttons currently being touched on screen, not sure if we'll need this variable but for debug
	public int buttonsPressed;

	public Camera camera;

	public bool touchingLeft;
	public bool touchingRight;
    private bool startedMove = false;

	public Slider speedSlider;

	//REFACTORED VARIABLES ONLY BELOW HERE

	//-----References-----
	private GameObject ship;
	private ShipMovement movementScript;
	private CannonFire cannonFireScript;
	public RectTransform iconPoint;
	public GameObject IconPrefab;
    public RectTransform iconBase;
    private const float MAX_ICON_DIST = 500.0f;

	//-----Multiple touches-----
	private List<TouchData> currentTouches;
	public bool mobile;
	float doubleClickCheck;
	bool clickOne;

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

	private static Vector3 screenCorrect;

	void Awake()
	{
		camera = Camera.main;
        screenCorrect = new Vector2(Screen.width / 2, Screen.height / 2);
		ship = GameObject.FindWithTag("Player");
		movementScript = ship.GetComponent<ShipMovement>();
		cannonFireScript = ship.GetComponent<CannonFire>();
		currentTouches = new List<TouchData>();
		if (iconPoint == null && GameObject.Find("InputIcon"))
		{
			iconPoint = GameObject.Find("InputIcon").GetComponent<RectTransform>();
		}
		else if (iconPoint == null)
		{
			iconPoint = Instantiate(IconPrefab, transform).GetComponent<RectTransform>();
		}
        if (GameObject.Find("InputBase"))
        {
            iconBase = GameObject.Find("InputBase").GetComponent<RectTransform>();
        }
	}

	void Update()
	{
        //Quit if pressing escape
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        //Take input depending on device
		if (mobile)
			TakeMobileInput();
		else
			TakeKeyboardInput();
		doubleClickCheck += Time.deltaTime;
	}

	void HandleTouch(TouchData t)
	{
		//Debug.Log("Touch released: Duration - " + t.Duration + "   Displacement - " + t.Displacement.magnitude + "   Velocity - " + t.Velocity.magnitude);
		if (t.Velocity.magnitude > minSwipeSpeed && t.Displacement.magnitude > minSwipeDisplacement)
		{
			//All behavior for when a swipe is completed

			if (Math.Abs(t.Displacement.y) > Math.Abs(t.Displacement.x)) //the swipe is up or down
			{
				if (t.Displacement.y < 0) //swipe down
				{
					//movementScript.LinearVelocity -= movementScript.linearAcceleration;
					Debug.Log("Swipe Down");
				}
				else //swipe up
				{
					//movementScript.LinearVelocity += movementScript.linearAcceleration;
					Debug.Log("Swipe Up");
				}
			}

		}
		else if (t.Duration > maxTapDuration)
		{
			//All behavior for when a tap and hold is completed

		}
		else
		{
			//All behavior for when a tap is completed
			if (clickOne && doubleClickCheck < 0.45f) //double click
			{
				clickOne = false;
				cannonFireScript.Fire("both");
			}
			else if (!clickOne)
			{
				clickOne = true;
			}

			#region Deprecated Code
            /*
			if (t.Position.x < turnTouchArea - Screen.width / 2) //tapped left side of screen
			{
				cannonFireScript.Fire("debugOneBig");
			}

			if (t.Position.x > Screen.width / 2 - turnTouchArea) //tapped right side of screen
			{
				cannonFireScript.Fire("debugTriShot");
			}*/
			#endregion
		}
	}

	void TakeMobileInput()
	{
		//Add any new touches to the touch list
		foreach (Touch t in Input.touches)
		{
			if (t.phase == TouchPhase.Began)
			{
				currentTouches.Add(new TouchData(t));
			}
		}
		//Update all touches that are currently down
		for (int i = 0; i < currentTouches.Count; i++)
		{
			TouchData t = currentTouches[i];
			t.Update(Input.touches);

			//if the touch has just ended, remove it from the list and perform whatever behavior is appropriate for that touch
			if (t.phase == TouchPhase.Ended)
			{
				currentTouches.Remove(t);
				i--;
				//HandleTouch(t);
				doubleClickCheck = 0;
				continue;
			}

            //If double click is still being tested for
            if (doubleClickCheck <= 0.8)
            {
                //Increment time of touch
                t.time += Time.deltaTime;
                //If touch displacment is big enough and at least some time has passed, stop looking for double tap
                if (t.Displacement.magnitude > 50f && t.time > 0.1f)
                    doubleClickCheck = 0.9f;
            }

            //If no longer checking for double click
            //Treat touch for movement
            if (doubleClickCheck > 0.8f)
			{
                //If move just started
                if(!t.startedMove)
                {
                    //Set position of move icon base
                    if(iconBase != null)
                        iconBase.anchoredPosition = t.Position;
                    clickStartPosition = t.Position;
                    t.startedMove = true;
                }

                //Set position of move icon
                SetPointIcon(t.Position);

                //Get direction to move the player in
				Vector3 pos = GetTarget(t.Position);
				print(pos - ship.transform.position);
				movementScript.TargetDirection = pos - ship.transform.position;
			}
		}
	}

    /// <summary>
    /// Handles information related to mobile touches
    /// </summary>
	private class TouchData
	{
		private Touch touch;
		private int index;
		private float duration;
		private Vector2 startPosition;

		public TouchPhase phase;
        public bool startedMove;
        public float time = 0.0f;

        /// <summary>
        /// Displacement from starting touch
        /// </summary>
		public Vector2 Displacement
		{
			get
			{
				return Position - startPosition;
			}
		}

        /// <summary>
        /// Velocity of touch movement
        /// </summary>
		public Vector2 Velocity
		{
			get
			{
				return Displacement / duration;
			}
		}

        /// <summary>
        /// Position of the touch on the screen
        /// </summary>
		public Vector2 Position
		{
			get
			{
				return touch.position - new Vector2(Screen.width / 2, Screen.height / 2);
			}
		}

        /// <summary>
        /// Length of time touch is active
        /// </summary>
		public float Duration
		{
			get
			{
				return duration;
			}
		}

        /// <summary>
        /// Creates touch data from a touch
        /// </summary>
        /// <param name="touch">Touch input</param>
		public TouchData(Touch touch)
		{
			this.touch = touch;
			index = touch.fingerId;
			duration = 0;
			print(Position);
			startPosition = Position;
		}

        /// <summary>
        /// Update touch data
        /// </summary>
        /// <param name="touches"></param>
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

	Vector2 clickStartPosition;
	Vector2 clickCurrentPosition;
	float clickDuration;

    /// <summary>
    /// Takes keyboard input from player
    /// </summary>
	void TakeKeyboardInput()
	{
        //Mouse initally pressed
		if (Input.GetMouseButtonDown(0)) //mouse down
		{
            //Set start position of click
			clickStartPosition = Input.mousePosition - screenCorrect;
			clickCurrentPosition = clickStartPosition;

			clickDuration = 0;
		}
        //Mouse is being held
		else if (Input.GetMouseButton(0)) //mouse held
		{
			clickDuration += Time.deltaTime;
            clickCurrentPosition = Input.mousePosition - screenCorrect;
            Vector2 clickDisplacement = clickCurrentPosition - clickStartPosition;
            //If click has moved enough and enough time has passed, stop checking for double click
            if (clickDisplacement.magnitude > 50f && clickDuration > 0.1f)
                doubleClickCheck = 0.9f;
            
            //If click is not a double click, handle it as movement
            if (doubleClickCheck > 0.8f)
			{
                //If movement just started
                if(!startedMove)
                {
                    //Set position of icon base
                    clickStartPosition = Input.mousePosition - screenCorrect;
                    if (iconBase != null)
                    {
                        iconBase.anchoredPosition = clickStartPosition;
                    }
                    startedMove = true;
                }
				clickCurrentPosition = Input.mousePosition - screenCorrect;

                //Pet position of movement icon
                if (iconPoint != null)
                    SetPointIcon(clickCurrentPosition);

                //Get direction of movement for player
				Vector3 pos = GetTarget(clickCurrentPosition);
				movementScript.TargetDirection = pos - ship.transform.position;
			}
		}
        //If mouse is released
		else if (Input.GetMouseButtonUp(0)) //mouse up 
		{
            startedMove = false;
			Vector2 clickDisplacement = clickCurrentPosition - clickStartPosition;
			Vector2 clickVelocity = clickDisplacement / clickDuration;
            //Check for swipe
			if (clickVelocity.magnitude > minSwipeSpeed && clickDisplacement.magnitude > minSwipeDisplacement) //swipe behavior
			{
				if (Math.Abs(clickDisplacement.y) > Math.Abs(clickDisplacement.x)) //the swipe is up or down
				{
					if (clickDisplacement.y < 0) //swipe down
					{
						//Debug.Log("Swipe Down");
					}
					else //swipe up
					{
						//Debug.Log("Swipe Up");
					}
				}
			}
            //Check for click and hold
			else if (clickDuration > maxTapDuration) //click and hold behavior
			{

			}
            //Check for click
			else //click behavior
			{
				//If double click, fire
				if (clickOne && doubleClickCheck < 0.45f) //double click
				{
					clickOne = false;
					cannonFireScript.Fire("right", GetFireTarget(Input.mousePosition - screenCorrect) - ship.transform.position);
				}
                //If first click, remember
				else if (!clickOne)
				{
					clickOne = true;
				}
			}
			doubleClickCheck = 0;
		}
		else if (clickCurrentPosition != null && doubleClickCheck > 1f)
		{
			Vector3 pos = GetTarget(clickCurrentPosition);
			movementScript.TargetDirection = pos - ship.transform.position;
            clickOne = false;
		}
	}

    /// <summary>
    /// Finds direction to move player towards
    /// </summary>
    /// <param name="input">Position of click on screen</param>
    /// <returns>Direction to move player towards</returns>
	Vector3 GetTarget(Vector2 input)
	{
        //Find direction of input from click start pos
        Vector2 distVec = input - clickStartPosition;
        //Get distance
        float dist = distVec.magnitude;
        distVec.Normalize();
        distVec *= 20.0f;

        //Find the location to move player towards based on player's location
        Vector3 targetPos = ship.transform.position + new Vector3(-distVec.y, 0, distVec.x);

        //Set speed scale based on how far click is from starting click
        if (dist > MAX_ICON_DIST)
            movementScript.SpeedScale = 1.0f;
        else
            movementScript.SpeedScale = dist / MAX_ICON_DIST;
        return targetPos;
	}

    /// <summary>
    /// Finds direction to shoot towards based on click
    /// </summary>
    /// <param name="input">Click position on screen</param>
    /// <returns>Direction to fire towards</returns>
    Vector3 GetFireTarget(Vector2 input)
    {
        //Find direction of input from click start pos
        Vector2 distVec = input - iconBase.anchoredPosition;
        //Get distance
        float dist = distVec.magnitude;
        distVec.Normalize();
        distVec *= 20.0f;

        //Find the location to move player towards based on player's location
        Vector3 targetPos = ship.transform.position + new Vector3(-distVec.y, 0, distVec.x);

        return targetPos;
    }

    /// <summary>
    /// Sets the position of the movement icon
    /// </summary>
    /// <param name="pos">Position of click</param>
    void SetPointIcon(Vector2 pos)
    {
        //Find distance of click from starting click
        float dist = Vector2.Distance(pos, clickStartPosition);
        //If distance is less than max icon distance, set icon to pos
        if (dist <= MAX_ICON_DIST)
            iconPoint.anchoredPosition = pos;
        //Else, find point on circle to place icon
        else
        {
            Vector2 distVec = pos - clickStartPosition;
            distVec.Normalize();
            distVec *= MAX_ICON_DIST;
            iconPoint.anchoredPosition = clickStartPosition + distVec;
        }
    }
}


