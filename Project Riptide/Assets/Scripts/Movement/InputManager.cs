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

    [SerializeField]
    private GameObject _shotIndicator;

	//-----References-----
	private GameObject _ship;
	private ShipMovement _movementScript;
	private CannonFire _cannonFireScript;
	private RectTransform _iconPoint;
    private RectTransform _iconBase;
    private RectTransform _canvasRect;
    private const float MAX_ICON_DIST = 500.0f;
    private const float MAX_ARROW_LENGTH = 6 * (MAX_ICON_DIST / 500.0f);


	//-----Multiple touches-----

	//-----Config values-----

	private static Vector3 ScreenCorrect;
    private Vector2 _screenScale;

    private Vector2 _clickStartPosition;
    private Vector2 _clickCurrentPosition;
    private float _clickDuration;
    private const float MAX_FAST_CLICK_DURATION = 0.4f;

    [SerializeField]
    private bool _autoFire = false;

    private float _fireRate = 0.5f;
    private float _currFireTime = 0.0f;

    private float _halfView = 20.0f;
    private float _viewRange = 30.0f;

    void Awake()
	{
		_camera = Camera.main;
        ScreenCorrect = new Vector2(Screen.width / 2.0f, Screen.height / 2.0f);
        _canvasRect = GameObject.Find("Canvas").GetComponent<RectTransform>();
        _screenScale = new Vector2((_canvasRect.rect.width / Screen.width), (_canvasRect.rect.height / Screen.height));
		_ship = GameObject.FindWithTag("Player");
		_movementScript = _ship.GetComponent<ShipMovement>();
		_cannonFireScript = _ship.GetComponent<CannonFire>();
	    _iconPoint = GameObject.Find("InputIcon").GetComponent<RectTransform>();
        _iconBase = GameObject.Find("InputBase").GetComponent<RectTransform>();
	}

	void Update()
	{
        //Quit if pressing escape
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

	    TakeKeyboardInput();

        if (_autoFire)
        {
            Enemy enemy;
            enemy = CheckEnemy(_ship.transform.right);
            if (enemy != null && !enemy.IsDying)
                AutoFire(enemy, 15.0f);
            enemy = CheckEnemy(-_ship.transform.right);
            if (enemy != null && !enemy.IsDying)
                AutoFire(enemy, -15.0f);
        }

        _currFireTime += Time.deltaTime;

		if (Input.GetKeyDown(KeyCode.I)) //This is temp and also bad, remove later
			GameObject.Find("Canvas").transform.Find("Inventory").gameObject.SetActive(!GameObject.Find("Canvas").transform.Find("Inventory").gameObject.activeSelf);
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
            _clickStartPosition = (Input.mousePosition - ScreenCorrect) * _screenScale;
            _clickCurrentPosition = _clickStartPosition;

            _clickDuration = 0;
        }
        //Mouse is being held
        else if (Input.GetMouseButton(0)) //mouse held
        {
            _clickDuration += Time.deltaTime;
            _clickCurrentPosition = (Input.mousePosition - ScreenCorrect) * _screenScale;
            Vector2 clickDisplacement = _clickCurrentPosition - _clickStartPosition;
            //If click has moved enough and enough time has passed, stop checking for double click
            if (clickDisplacement.magnitude > 50f && _clickDuration > 0.15f)
                _clickDuration = 0.9f;

            //If click is not a double click, handle it as movement
            if (_clickDuration > 0.8f)
            {
                //If movement just started
                if (!_startedMove)
                {
                    //Set position of icon base
                    _clickStartPosition = (Input.mousePosition - ScreenCorrect) * _screenScale;
                    if (_iconBase != null)
                    {
                        _iconBase.anchoredPosition = _clickStartPosition;
                    }
                    _startedMove = true;
                }
                _screenScale = new Vector2((_canvasRect.rect.width / Screen.width), (_canvasRect.rect.height / Screen.height));
                _clickCurrentPosition = (Input.mousePosition - ScreenCorrect) * _screenScale;

                //Pet position of movement icon
                if (_iconPoint != null)
                {
                    //SetPointIcon(_clickCurrentPosition);
                    SetArrowIcon(_clickCurrentPosition);
                }

                //Get direction of movement for player
                Vector3 pos = GetTarget(_clickCurrentPosition);
                _movementScript.TargetDirection = pos - _ship.transform.position;
            }
        }
        //If mouse is released
        else if (Input.GetMouseButtonUp(0)) //mouse up 
        {
            _startedMove = false;
            if (!_autoFire && _clickDuration < MAX_FAST_CLICK_DURATION) //double click
            {
                //_clickOne = false;
                if (_currFireTime >= _fireRate)
                {
                    Debug.DrawRay(_ship.transform.position, GetFireTarget((Input.mousePosition - ScreenCorrect) * _screenScale) - _ship.transform.position, Color.red, 5.0f);
                    float angle = _cannonFireScript.Fire(GetFireTarget((Input.mousePosition - ScreenCorrect) * _screenScale) - _ship.transform.position, 0);
                    GameObject indicator = Instantiate(_shotIndicator, _iconBase.transform.position, Quaternion.identity, _canvasRect.gameObject.transform);
                    indicator.transform.localRotation = Quaternion.Euler(0, 0, -(_ship.transform.eulerAngles.y + 90) + angle);
                    _currFireTime = 0.0f;
                }
            }
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
    /// Sets the position of the arrow movement icon
    /// </summary>
    /// <param name="pos">Position of click</param>
    void SetArrowIcon(Vector2 pos)
    {
        //Find distance of click from starting click
        float dist = Vector2.Distance(pos, _clickStartPosition);
        //If distance is less than max icon distance, set icon to pos
        if (dist > MAX_ICON_DIST)
        {
            dist = MAX_ICON_DIST;
        }

        //Set position of arrow
        _iconPoint.anchoredPosition = _clickStartPosition;

        //Find rotation for arrow
        Vector3 diff = pos - _clickStartPosition;
        float angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg - 90;
        _iconPoint.localRotation = Quaternion.Euler(0, 0, angle);

        //Scale arrow based on distance from center
        _iconPoint.localScale = new Vector3(_iconPoint.localScale.x, MAX_ARROW_LENGTH * (dist / MAX_ICON_DIST), _iconPoint.localScale.z);
    }

    /// <summary>
    /// Resets input values on being disabled
    /// </summary>
    private void OnDisable()
    {
        _startedMove = false;
    }

    /// <summary>
    /// Checks for an enemy to fire at by using ray casts
    /// </summary>
    /// <param name="targetDir">Direction to check for enemy</param>
    /// <returns>Enemy found, null if none</returns>
    private Enemy CheckEnemy(Vector3 targetDir)
    {
        RaycastHit hit = new RaycastHit();
        Vector3 detectPosition = _ship.transform.position;
        targetDir.Normalize();

        for (int i = 0; i <= _halfView; i += 4)
        {
            //Debug.DrawRay(detectPosition, Quaternion.AngleAxis(i, Vector3.up) * targetDir * _viewRange, Color.red);
            //Debug.DrawRay(detectPosition, Quaternion.AngleAxis(-i, Vector3.up) * targetDir * _viewRange, Color.red);
            if (UnityEngine.Physics.Raycast(detectPosition, Quaternion.AngleAxis(i, Vector3.up) * targetDir, out hit, _viewRange))
            {
                if (hit.collider.gameObject.tag == "Hitbox")
                {
                    return hit.collider.gameObject.GetComponent<Hitbox>().AttachedObject.GetComponent<Enemy>();
                }
            }
            if (UnityEngine.Physics.Raycast(detectPosition, Quaternion.AngleAxis(-i, Vector3.up) * targetDir, out hit, _viewRange))
            {
                if (hit.collider.gameObject.tag == "Hitbox")
                {
                    return hit.collider.gameObject.GetComponent<Hitbox>().AttachedObject.GetComponent<Enemy>();
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Automatically fire a shot towards the enemy
    /// </summary>
    /// <param name="enemy">Enemy to fire at</param>
    /// <param name="offset">Offset of ship fire angle</param>
    private void AutoFire(Enemy enemy, float offset)
    {
        if (_currFireTime >= _fireRate)
        {
            Vector3 diff = (enemy.transform.position - _ship.transform.position).normalized;
            _cannonFireScript.Fire(new Vector3(diff.x, 0, diff.z), offset);
            _currFireTime = 0.0f;
        }
    }
}


