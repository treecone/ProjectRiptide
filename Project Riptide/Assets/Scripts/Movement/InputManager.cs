using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
	private Camera _camera;

    private bool _startedMove = false;
	//REFACTORED VARIABLES ONLY BELOW HERE

	//-----References-----
	private GameObject _ship;
	private ShipMovement _movementScript;
	private CannonFire _cannonFireScript;
	private RectTransform _iconPoint;
    private RectTransform _iconBase;
    private RectTransform _canvasRect;
    private const float MAX_ICON_DIST = 500.0f;

	//-----Multiple touches-----
	private List<TouchData> _currentTouches;
    [SerializeField]
	private bool _mobile;
	private float _doubleClickCheck;
	private bool _clickOne;

	//-----Config values-----

	/// <summary>
	/// The minimum displacement a swipe must have to be considered a swipe
	/// </summary>
	private const float MIN_SWIPE_DISPLACEMENT = 100;

	/// <summary>
	/// The minimum speed a swipe must have to be considered a swipe
	/// </summary>
	private const float MIN_SWIPE_SPEED = 1000;

	/// <summary>
	/// The maximum time a tap can be held down and still be considered a tap
	/// </summary>
	private const float MAX_TAP_DURATION = 0.5f;

	private static Vector3 ScreenCorrect;
    private Vector2 _screenScale;

    private Vector2 _clickStartPosition;
    private Vector2 _clickCurrentPosition;
    private float _clickDuration;

    void Awake()
	{
		_camera = Camera.main;
        ScreenCorrect = new Vector2(Screen.width / 2.0f, Screen.height / 2.0f);
        _canvasRect = GameObject.Find("Canvas").GetComponent<RectTransform>();
        _screenScale = new Vector2((_canvasRect.rect.width / Screen.width), (_canvasRect.rect.height / Screen.height));
		_ship = GameObject.FindWithTag("Player");
		_movementScript = _ship.GetComponent<ShipMovement>();
		_cannonFireScript = _ship.GetComponent<CannonFire>();
		_currentTouches = new List<TouchData>();
	    _iconPoint = GameObject.Find("InputIcon").GetComponent<RectTransform>();
        _iconBase = GameObject.Find("InputBase").GetComponent<RectTransform>();
	}

	void Update()
	{
        //Quit if pressing escape
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        //Take input depending on device
		if (_mobile)
			TakeMobileInput();
		else
			TakeKeyboardInput();
		_doubleClickCheck += Time.deltaTime;
	}

	void HandleTouch(TouchData t)
	{
		//Debug.Log("Touch released: Duration - " + t.Duration + "   Displacement - " + t.Displacement.magnitude + "   Velocity - " + t.Velocity.magnitude);
		if (t.Velocity.magnitude > MIN_SWIPE_SPEED && t.Displacement.magnitude > MIN_SWIPE_DISPLACEMENT)
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
		else if (t.Duration > MAX_TAP_DURATION)
		{
			//All behavior for when a tap and hold is completed

		}
		else
		{
			//All behavior for when a tap is completed
			if (_clickOne && _doubleClickCheck < 0.45f) //double click
			{
				_clickOne = false;
				_cannonFireScript.Fire("both");
			}
			else if (!_clickOne)
			{
				_clickOne = true;
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
				_currentTouches.Add(new TouchData(t));
			}
		}
		//Update all touches that are currently down
		for (int i = 0; i < _currentTouches.Count; i++)
		{
			TouchData t = _currentTouches[i];
			t.Update(Input.touches);

			//if the touch has just ended, remove it from the list and perform whatever behavior is appropriate for that touch
			if (t.phase == TouchPhase.Ended)
			{
				_currentTouches.Remove(t);
				i--;
				//HandleTouch(t);
				_doubleClickCheck = 0;
				continue;
			}

            //If double click is still being tested for
            if (_doubleClickCheck <= 0.8)
            {
                //Increment time of touch
                t.time += Time.deltaTime;
                //If touch displacment is big enough and at least some time has passed, stop looking for double tap
                if (t.Displacement.magnitude > 50f && t.time > 0.1f)
                    _doubleClickCheck = 0.9f;
            }

            //If no longer checking for double click
            //Treat touch for movement
            if (_doubleClickCheck > 0.8f)
			{
                //If move just started
                if(!t.startedMove)
                {
                    //Set position of move icon base
                    if(_iconBase != null)
                        _iconBase.anchoredPosition = t.Position;
                    _clickStartPosition = t.Position;
                    t.startedMove = true;
                }

                //Set position of move icon
                SetPointIcon(t.Position);

                //Get direction to move the player in
				Vector3 pos = GetTarget(t.Position);
				print(pos - _ship.transform.position);
				_movementScript.TargetDirection = pos - _ship.transform.position;
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

    /// <summary>
    /// Takes keyboard input from player
    /// </summary>
	void TakeKeyboardInput()
	{
        //Mouse initally pressed
		if (Input.GetMouseButtonDown(0)) //mouse down
		{
            //Set start position of click
			_clickStartPosition = Input.mousePosition - ScreenCorrect;
			_clickCurrentPosition = _clickStartPosition;

			_clickDuration = 0;
		}
        //Mouse is being held
		else if (Input.GetMouseButton(0)) //mouse held
		{
			_clickDuration += Time.deltaTime;
            _clickCurrentPosition = Input.mousePosition - ScreenCorrect;
            Vector2 clickDisplacement = _clickCurrentPosition - _clickStartPosition;
            //If click has moved enough and enough time has passed, stop checking for double click
            if (clickDisplacement.magnitude > 50f && _clickDuration > 0.1f)
                _doubleClickCheck = 0.9f;
            
            //If click is not a double click, handle it as movement
            if (_doubleClickCheck > 0.8f)
			{
                //If movement just started
                if(!_startedMove)
                {
                    //Set position of icon base
                    _clickStartPosition = Input.mousePosition - ScreenCorrect;
                    if (_iconBase != null)
                    {
                        _iconBase.anchoredPosition = _clickStartPosition;
                    }
                    _startedMove = true;
                }
                //_screenScale = new Vector2((_canvasRect.rect.width / Screen.width), (_canvasRect.rect.height / Screen.height));
                _clickCurrentPosition = (Input.mousePosition - ScreenCorrect) * _screenScale;

                //Pet position of movement icon
                if (_iconPoint != null)
                    SetPointIcon(_clickCurrentPosition);

                //Get direction of movement for player
				Vector3 pos = GetTarget(_clickCurrentPosition);
				_movementScript.TargetDirection = pos - _ship.transform.position;
			}
		}
        //If mouse is released
		else if (Input.GetMouseButtonUp(0)) //mouse up 
		{
            _startedMove = false;
			Vector2 clickDisplacement = _clickCurrentPosition - _clickStartPosition;
			Vector2 clickVelocity = clickDisplacement / _clickDuration;
            //Check for swipe
			if (clickVelocity.magnitude > MIN_SWIPE_SPEED && clickDisplacement.magnitude > MIN_SWIPE_DISPLACEMENT) //swipe behavior
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
			else if (_clickDuration > MAX_TAP_DURATION) //click and hold behavior
			{

			}
            //Check for click
			else //click behavior
			{
				//If double click, fire
				if (_clickOne && _doubleClickCheck < 0.45f) //double click
				{
					_clickOne = false;
					_cannonFireScript.Fire("right", GetFireTarget(Input.mousePosition - ScreenCorrect) - _ship.transform.position);
				}
                //If first click, remember
				else if (!_clickOne)
				{
					_clickOne = true;
				}
			}
			_doubleClickCheck = 0;
		}
		else if (_clickCurrentPosition != null && _doubleClickCheck > 1f)
		{
			Vector3 pos = GetTarget(_clickCurrentPosition);
			_movementScript.TargetDirection = pos - _ship.transform.position;
            _clickOne = false;
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
        Vector2 distVec = input - _clickStartPosition;
        //Get distance
        float dist = distVec.magnitude;
        distVec.Normalize();
        distVec *= 20.0f;

        //Find the location to move player towards based on player's location
        Vector3 targetPos = _ship.transform.position + new Vector3(-distVec.y, 0, distVec.x);

        //Set speed scale based on how far click is from starting click
        if (dist > MAX_ICON_DIST)
            _movementScript.SpeedScale = 1.0f;
        else
            _movementScript.SpeedScale = dist / MAX_ICON_DIST;
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
        Vector2 distVec = input - _iconBase.anchoredPosition;
        //Get distance
        float dist = distVec.magnitude;
        distVec.Normalize();
        distVec *= 20.0f;

        //Find the location to move player towards based on player's location
        Vector3 targetPos = _ship.transform.position + new Vector3(-distVec.y, 0, distVec.x);

        return targetPos;
    }

    /// <summary>
    /// Sets the position of the movement icon
    /// </summary>
    /// <param name="pos">Position of click</param>
    void SetPointIcon(Vector2 pos)
    {
        //Find distance of click from starting click
        float dist = Vector2.Distance(pos, _clickStartPosition);
        //If distance is less than max icon distance, set icon to pos
        if (dist <= MAX_ICON_DIST)
            _iconPoint.anchoredPosition = pos;
        //Else, find point on circle to place icon
        else
        {
            Vector2 distVec = pos - _clickStartPosition;
            distVec.Normalize();
            distVec *= MAX_ICON_DIST;
            _iconPoint.anchoredPosition = _clickStartPosition + distVec;
        }
    }
}


