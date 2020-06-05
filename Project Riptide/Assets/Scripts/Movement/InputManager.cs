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
    [SerializeField]
    private LineRenderer _leftIndicator;
    [SerializeField]
    private LineRenderer _rightIndicator;

	//-----References-----
	private GameObject _ship;
	private ShipMovement _movementScript;
	private CannonFire _cannonFireScript;
    private CameraController _cameraController;
	private RectTransform _iconPoint;
    private RectTransform _iconBase;
    private RectTransform _canvasRect;
    private const float MAX_ICON_DIST = 500.0f;
    private const float MAX_ICON_RECLICK_DIST = MAX_ICON_DIST + 100.0f;
    private const float MAX_ARROW_LENGTH = 6 * (MAX_ICON_DIST / 500.0f);


	//-----Multiple touches-----

	//-----Config values-----

	private static Vector3 ScreenCorrect;
    private Vector2 _screenScale;

    private Vector2 _clickStartPosition;
    private Vector2 _clickCurrentPosition;
    private float _clickDuration;
    private const float MAX_FAST_CLICK_DURATION = 0.4f;

    private float _fireRate = 0.5f;
    private float _currFireTime = 0.0f;

    private float _viewRange = 20.0f;

    private bool _combatMode = false;
    public bool InCombatMode => _combatMode;
    private const float MAX_COMBAT_RANGE = 50.0f;
    private Enemy _targetEnemy;
    private Enemy _leftEnemy;
    private Enemy _rightEnemy;

    void Awake()
	{
		_camera = Camera.main;
        _cameraController = _camera.GetComponent<CameraController>();
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

        if (_combatMode)
        {
            _currFireTime += Time.deltaTime;

            _rightIndicator.SetPosition(0, Vector3.zero);
            _leftIndicator.SetPosition(0, Vector3.zero);
            _rightIndicator.transform.rotation = Quaternion.identity;
            _leftIndicator.transform.rotation = Quaternion.identity;

            bool fired = false;
            //Check right side for enemies
            _rightEnemy = CheckEnemy(_ship.transform.right);
            if (_rightEnemy != null && !_rightEnemy.IsDying)
            {
                if(AutoFire(_rightEnemy, 15.0f))
                {
                    fired = true;
                }
                _rightIndicator.SetPosition(1, _rightEnemy.transform.position - _ship.transform.position);
            }
            else
            {
                _rightIndicator.SetPosition(1, Vector3.zero);
            }

            //Check left side for enemies
            _leftEnemy = CheckEnemy(-_ship.transform.right);
            if (_leftEnemy != null && !_leftEnemy.IsDying)
            {
                if(AutoFire(_leftEnemy, -15.0f))
                {
                    fired = true;
                }
                _leftIndicator.SetPosition(1, _leftEnemy.transform.position - _ship.transform.position);
            }
            else
            {
                _leftIndicator.SetPosition(1, Vector3.zero);
            }

            //Check to see if combat mode should be turned off
            if (_targetEnemy.IsDying || Vector3.SqrMagnitude(_ship.transform.position - _targetEnemy.transform.position) > MAX_COMBAT_RANGE * MAX_COMBAT_RANGE)
            {
                _targetEnemy = CheckEnemyInRadius(MAX_COMBAT_RANGE, true);

                if(_targetEnemy == null)
                {
                    _combatMode = false;
                    _cameraController.ToggleCombatView(false);
                    _leftIndicator.enabled = false;
                    _rightIndicator.enabled = false;
                }
            }

            if(fired)
            {
                _currFireTime = 0;
            }
        }
        else
        {
            _targetEnemy = CheckEnemyInRadius(_viewRange, false);
            //Set indicator if enemy is in range
            if (_targetEnemy != null && !_targetEnemy.IsDying)
            {
                _movementScript.IndicatorActive = true;
            }
            else
            {
                _movementScript.IndicatorActive = false;
            }
        }

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

            //If click is close enough to base, don't move base when move starts
            if(Vector3.SqrMagnitude(_iconBase.anchoredPosition - _clickStartPosition) <= MAX_ICON_RECLICK_DIST * MAX_ICON_RECLICK_DIST)
            {
                _startedMove = true;
            }

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
            if (_clickDuration < MAX_FAST_CLICK_DURATION) //double click
            {
                if (!_combatMode && _movementScript.IndicatorActive)
                {
                     _combatMode = true;
                     _cameraController.ToggleCombatView(true);
                     _movementScript.IndicatorActive = false;
                    _leftIndicator.enabled = true;
                    _rightIndicator.enabled = true;
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
        Vector2 distVec = input - _iconBase.anchoredPosition;
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
    /// Resets movement icons and stops the player's motion
    /// </summary>
    public void ResetMovement()
    {
        _iconBase.anchoredPosition = new Vector2(0, 100000);
        SetArrowIcon(_iconBase.anchoredPosition);
        _movementScript.TargetDirection = Vector3.zero;
        _movementScript.SpeedScale = 0;
        _movementScript.StopMotion();
    }

    /// <summary>
    /// Sets the position of the arrow movement icon
    /// </summary>
    /// <param name="pos">Position of click</param>
    void SetArrowIcon(Vector2 pos)
    {
        //Find distance of click from starting click
        float dist = Vector2.Distance(pos, _iconBase.anchoredPosition);
        //If distance is less than max icon distance, set icon to pos
        if (dist > MAX_ICON_DIST)
        {
            dist = MAX_ICON_DIST;
        }

        //Set position of arrow
        _iconPoint.anchoredPosition = _iconBase.anchoredPosition;

        //Find rotation for arrow
        Vector3 diff = pos - _iconBase.anchoredPosition;
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
    public Enemy CheckEnemy(Vector3 targetDir)
    {
        Enemy foundEnemy = null;
        float dist = 999999;
        RaycastHit[] hits;
        Vector3 detectPosition = _ship.transform.position;
        targetDir.Normalize();

        for (int i = 0; i <= _cannonFireScript.ShotAngle / 2; i += 4)
        {
            //Debug.DrawRay(detectPosition, Quaternion.AngleAxis(i, Vector3.up) * targetDir * _viewRange, Color.red);
            //Debug.DrawRay(detectPosition, Quaternion.AngleAxis(-i, Vector3.up) * targetDir * _viewRange, Color.red);
            hits = UnityEngine.Physics.RaycastAll(detectPosition, Quaternion.AngleAxis(i, Vector3.up) * targetDir, _viewRange);
            //Check each hit from raycast
            foreach (RaycastHit hit in hits)
            {
                //Check if hit was from enemy
                if (hit.collider.gameObject.tag == "Hitbox")
                {
                    Enemy enemy = hit.collider.gameObject.GetComponent<Hitbox>().AttachedObject.GetComponent<Enemy>();
                    if (enemy != null && !enemy.IsDying)
                    {
                        //Take only the closest enenmy
                        float enemyDist = Vector3.SqrMagnitude(_ship.transform.position - enemy.transform.position);
                        if (enemyDist < dist)
                        {
                            foundEnemy = enemy;
                            dist = enemyDist;
                        }
                    }
                }
            }
            hits = UnityEngine.Physics.RaycastAll(detectPosition, Quaternion.AngleAxis(-i, Vector3.up) * targetDir, _viewRange);
            //Check each hit from raycast
            foreach (RaycastHit hit in hits)
            {
                //Check if hit was from enemy
                if (hit.collider.gameObject.tag == "Hitbox")
                {
                    Enemy enemy = hit.collider.gameObject.GetComponent<Hitbox>().AttachedObject.GetComponent<Enemy>();
                    if (enemy != null && !enemy.IsDying)
                    {
                        //Take only the closest enenmy
                        float enemyDist = Vector3.SqrMagnitude(_ship.transform.position - enemy.transform.position);
                        if (enemyDist < dist)
                        {
                            foundEnemy = enemy;
                            dist = enemyDist;
                        }
                    }
                }
            }
        }

        return foundEnemy;
    }

    /// <summary>
    /// Finds the closest enemy in a radius around the player
    /// </summary>
    /// <param name="radius">Radius to check</param>
    /// <param name="checkHostile">Ensure monster is hostile</param>
    /// <returns>Enemy found, null if none</returns>
    private Enemy CheckEnemyInRadius(float radius, bool checkHostile)
    {
        Collider[] colliders;
        Enemy foundEnemy = null;
        float dist = 999999;
        colliders = UnityEngine.Physics.OverlapSphere(_ship.transform.position, radius);
        //Check all colliders found to find closest enemy
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.tag == "Hitbox")
            {
                Enemy enemy = collider.gameObject.GetComponent<Hitbox>().AttachedObject.GetComponent<Enemy>();
                if (enemy != null && !enemy.IsDying)
                {
                    //If check hostile is on, make sure enemy is not passive
                    if (checkHostile)
                    {
                        if (enemy.State == EnemyState.Passive)
                        {
                            continue;
                        }
                    }
                    float enemyDist = Vector3.SqrMagnitude(_ship.transform.position - enemy.transform.position);
                    if (enemyDist < dist)
                    {
                        foundEnemy = enemy;
                        dist = enemyDist;
                    }
                }
            }
        }
        return foundEnemy;
    }

    /// <summary>
    /// Automatically fire a shot towards the enemy
    /// </summary>
    /// <param name="enemy">Enemy to fire at</param>
    /// <param name="offset">Offset of ship fire angle</param>
    private bool AutoFire(Enemy enemy, float offset)
    {
        if (_currFireTime >= _fireRate)
        {
            Vector3 diff = (enemy.transform.position - _ship.transform.position).normalized;
            _cannonFireScript.Fire(new Vector3(diff.x, 0, diff.z), offset);
            return true;
        }
        return false;
    }
}


