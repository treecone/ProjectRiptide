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

	public bool touchingLeft;
	public bool touchingRight;

	public Slider speedSlider;

	//REFACTORED VARIABLES ONLY BELOW HERE

	//-----References-----
	private GameObject ship;
	private ShipMovementScript movementScript;
	private CannonFireScript cannonFireScript;
	public RectTransform iconPoint;
	public GameObject IconPrefab;

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
		movementScript = ship.GetComponent<ShipMovementScript>();
		cannonFireScript = ship.GetComponent<CannonFireScript>();
		currentTouches = new List<TouchData>();
		if (iconPoint == null && GameObject.Find("InputIcon"))
		{
			iconPoint = GameObject.Find("InputIcon").GetComponent<RectTransform>();
		}
		else if (iconPoint == null)
		{
			iconPoint = Instantiate(IconPrefab, transform).GetComponent<RectTransform>();
		}
	}

	void Update()
	{
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
				cannonFireScript.Fire(FireType.Target, GetTarget(t.Position));
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
			}
			*/
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
				HandleTouch(t);
				doubleClickCheck = 0;
				continue;
			}

			if (doubleClickCheck > 0.8f)
			{
				iconPoint.anchoredPosition = t.Position;

				Vector3 pos = GetTarget(t.Position);
				print(pos - ship.transform.position);
				movementScript.TargetDirection = pos - ship.transform.position;
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
			print(Position);
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
	#region Keyboard Input (deprecated)

	Vector2 clickStartPosition;
	Vector2 clickCurrentPosition;
	float clickDuration;

	void TakeKeyboardInput()
	{
		if (Input.GetMouseButtonDown(0)) //mouse down
		{
			clickStartPosition = Input.mousePosition - screenCorrect;
			clickCurrentPosition = clickStartPosition;

			clickDuration = 0;
		}
		else if (Input.GetMouseButton(0)) //mouse held
		{
			clickDuration += Time.deltaTime;

			if (doubleClickCheck > 0.8f)
			{
				clickCurrentPosition = Input.mousePosition - screenCorrect;

				if (iconPoint != null)
					iconPoint.anchoredPosition = clickCurrentPosition;

				Vector3 pos = GetTarget(clickCurrentPosition);
				movementScript.TargetDirection = pos - ship.transform.position;
			}
		}
		else if (Input.GetMouseButtonUp(0)) //mouse up 
		{
			Vector2 clickDisplacement = clickCurrentPosition - clickStartPosition;
			Vector2 clickVelocity = clickDisplacement / clickDuration;
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
			else if (clickDuration > maxTapDuration) //click and hold behavior
			{

			}
			else //click behavior
			{
				//Debug.Log(clickCurrentPosition);
				if (clickOne && doubleClickCheck < 0.45f) //double click
				{
					clickOne = false;
					cannonFireScript.Fire(FireType.Target, GetTarget(clickCurrentPosition));
				}
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
		}
	}
	#endregion
	Vector3 GetTarget(Vector3 input)
	{
		// create ray from the camera and passing through the touch position:
		Ray ray = camera.ScreenPointToRay(input);

		// create a logical plane at this object's position and perpendicular to world Y:
		Plane plane = new Plane(Vector3.up, Vector3.zero);
		float distance = 0;
		// if plane hit...
		if (plane.Raycast(ray, out distance))
		{
			// get the point pos has the position in the plane you've touched
			return ray.GetPoint(distance) + new Vector3(-14.4f, 0, 5.9f);
		}
		return ship.transform.position;
	}
}


