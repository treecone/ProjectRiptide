using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate bool EnemySort(Enemy re, Enemy le);

public class InputManager : MonoBehaviour
{
	private Camera _camera;

    private bool _startedMove = false;
    //REFACTORED VARIABLES ONLY BELOW HERE

    [SerializeField]
    private GameObject _shotIndicator;
    [SerializeField]
    private LineRenderer _lineIndicator;

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

	private Vector3 _screenCorrect;
    public Vector3 ScreenCorrect => _screenCorrect;
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
    private List<Enemy> _targetEnemies = new List<Enemy>();
    private Enemy _currEnemy;
    private Enemy _leftEnemy;
    private Enemy _rightEnemy;
    private bool _isRightEnemy;
    public EnemySort EnemyCompare;
    public List<Enemy> TargetEnemies => _targetEnemies;
    public float MaxCombatRange => MAX_COMBAT_RANGE;

    void Awake()
	{
		_camera = Camera.main;
        _cameraController = _camera.GetComponent<CameraController>();
        _screenCorrect = new Vector2(Screen.width / 2.0f, Screen.height / 2.0f);
        _canvasRect = GameObject.Find("Canvas").GetComponent<RectTransform>();
        _screenScale = new Vector2((_canvasRect.rect.width / Screen.width), (_canvasRect.rect.height / Screen.height));
		_ship = GameObject.FindWithTag("Player");
		_movementScript = _ship.GetComponent<ShipMovement>();
		_cannonFireScript = _ship.GetComponent<CannonFire>();
	    _iconPoint = GameObject.Find("InputIcon").GetComponent<RectTransform>();
        _iconBase = GameObject.Find("InputBase").GetComponent<RectTransform>();
        EnemyCompare = ClosestHostileEnemy;
	}

	void Update()
	{
        //Quit if pressing escape
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

	    TakeKeyboardInput();

        //If there are enemies to target
        if(_targetEnemies.Count > 0 && !_combatMode)
        {
            //ACTIVATE COMBAT MODE
            _combatMode = true;
            _cameraController.ToggleCombatView(true);
            _lineIndicator.enabled = true;
        }

        if(_targetEnemies.Count == 0 && _combatMode)
        {
            //DEACTIVATE COMBAT MODE
            _combatMode = false;
            _cameraController.ToggleCombatView(false);
            _lineIndicator.enabled = false;
        }
        
        if(_combatMode)
        {
            _currFireTime += Time.deltaTime;

            _lineIndicator.SetPosition(0, Vector3.zero);
            _lineIndicator.transform.rotation = Quaternion.identity;
            bool fired = false;

            _rightEnemy = CheckTargetEnemy(_ship.transform.right);
            _leftEnemy = CheckTargetEnemy(-_ship.transform.right);

            //AUTO FIRE ON CURRENT ENEMY
            if (_rightEnemy != null && !_rightEnemy.IsInvincible && (_leftEnemy == null || _leftEnemy.IsInvincible || EnemyCompare(_rightEnemy, _leftEnemy)))
            {
                _currEnemy = _rightEnemy;
                _isRightEnemy = true;
            }
            else if (_leftEnemy != null && !_leftEnemy.IsInvincible)
            {
                _currEnemy = _leftEnemy;
                _isRightEnemy = false;
            }
            else
            {
                _currEnemy = null;
            }

            //If an enemy is found, fire towards that enemy
            if (_currEnemy != null)
            {
                if (_isRightEnemy && AutoFire(_currEnemy, 15.0f))
                {
                    fired = true;
                }
                else if (AutoFire(_currEnemy, -15.0f))
                {
                    fired = true;
                }
                Vector3 indVec = new Vector3(_currEnemy.Position.x - _ship.transform.position.x, 0, _currEnemy.Position.z - _ship.transform.position.z);
                _lineIndicator.SetPosition(1, indVec);
            }
            else
            {
                _lineIndicator.SetPosition(1, Vector3.zero);
            }

            //Reset
            if (fired)
            {
                _currFireTime = 0;
            }

            //CHECK IF A TARGET ENEMY IS OUT OF RANGE OR DEAD
            for(int i = 0; i < _targetEnemies.Count; i++)
            {
                if(_targetEnemies[i].IsDying || Vector3.SqrMagnitude(_ship.transform.position - _targetEnemies[i].Position) > MAX_COMBAT_RANGE * MAX_COMBAT_RANGE)
                {
                    _targetEnemies[i].SetTargetIndicator(false);
                    if (_targetEnemies[i].OffScreenIndicator != null)
                    {
                        Destroy(_targetEnemies[i].OffScreenIndicator);
                    }
                    _targetEnemies.RemoveAt(i);
                    i--;
                }
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
            _clickStartPosition = (Input.mousePosition - _screenCorrect) * _screenScale;
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
            _clickCurrentPosition = (Input.mousePosition - _screenCorrect) * _screenScale;
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
                    _clickStartPosition = (Input.mousePosition - _screenCorrect) * _screenScale;
                    if (_iconBase != null)
                    {
                        _iconBase.anchoredPosition = _clickStartPosition;
                    }
                    _startedMove = true;
                }
                _screenScale = new Vector2((_canvasRect.rect.width / Screen.width), (_canvasRect.rect.height / Screen.height));
                _clickCurrentPosition = (Input.mousePosition - _screenCorrect) * _screenScale;

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
                     _lineIndicator.enabled = true;
                }

                CheckEnemyTap();
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
    public Enemy CheckTargetEnemy(Vector3 targetDir)
    {
        Enemy foundEnemy = null;
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
                //If obstical is detected, stop checking hits
                if (hit.collider.gameObject.tag == "Obstical")
                {
                    break;
                }

                //Check if hit was from enemy
                if (hit.collider.gameObject.tag == "Hitbox")
                {
                    Enemy enemy = hit.collider.gameObject.GetComponent<Hitbox>().AttachedObject.GetComponent<Enemy>();
                    if (enemy != null && _targetEnemies.Contains(enemy) && !enemy.IsInvincible)
                    {
                        //If new enemy compares better, take that enemy instead
                        if (foundEnemy == null || EnemyCompare(enemy, foundEnemy))
                        {
                            foundEnemy = enemy;
                        }
                    }
                }
            }
            hits = UnityEngine.Physics.RaycastAll(detectPosition, Quaternion.AngleAxis(-i, Vector3.up) * targetDir, _viewRange);
            //Check each hit from raycast
            foreach (RaycastHit hit in hits)
            {
                //If obstical is detected, stop checking hits
                if (hit.collider.gameObject.tag == "Obstical")
                {
                    break;
                }

                //Check if hit was from enemy
                if (hit.collider.gameObject.tag == "Hitbox")
                {
                    Enemy enemy = hit.collider.gameObject.GetComponent<Hitbox>().AttachedObject.GetComponent<Enemy>();
                    if (enemy != null && _targetEnemies.Contains(enemy) && !enemy.IsInvincible)
                    {
                        //If new enemy compares better, take that enemy instead
                        if (foundEnemy == null || EnemyCompare(enemy, foundEnemy))
                        {
                            foundEnemy = enemy;
                        }
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

    /// <summary>
    /// Compares two enemys to find the closer one
    /// </summary>
    /// <param name="re">Right Enemy</param>
    /// <param name="le">Left Enemy</param>
    /// <returns>True if right enemy chosen</returns>
    private bool ClosestEnemy(Enemy re, Enemy le)
    {
        return Vector3.SqrMagnitude(re.Position - _ship.transform.position) < Vector3.SqrMagnitude(le.Position - _ship.transform.position);
    }

    /// <summary>
    /// Compares two enemys to find the closer hostile one
    /// </summary>
    /// <param name="re">Right Enemy</param>
    /// <param name="le">Left Enemy</param>
    /// <returns>True if right enemy chosen</returns>
    private bool ClosestHostileEnemy(Enemy re, Enemy le)
    {
        //If one enemy is passive and the other is hostile, the hostile enemy is taken
        if(re.State == EnemyState.Hostile && le.State == EnemyState.Passive)
        {
            return true;
        }
        else if(le.State == EnemyState.Hostile && re.State == EnemyState.Passive)
        {
            return false;
        }
        
        //If states are the same, take closest enemy
        return Vector3.SqrMagnitude(re.Position - _ship.transform.position) < Vector3.SqrMagnitude(le.Position - _ship.transform.position);
    }

    /// <summary>
    /// Checks to see if the player has tapped on an enemy
    /// </summary>
    private void CheckEnemyTap()
    {
        RaycastHit hit;
        if (UnityEngine.Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            if (hit.collider.gameObject.tag == "Hitbox")
            {
                Enemy enemy = hit.collider.gameObject.GetComponent<Hitbox>().AttachedObject.GetComponent<Enemy>();
                if(enemy != null && !_targetEnemies.Contains(enemy))
                {
                    //Check if enemy is close enough to target
                    if (Vector3.SqrMagnitude(enemy.Position - _ship.transform.position) < MAX_COMBAT_RANGE * MAX_COMBAT_RANGE)
                    {
                        //Target enemy
                        _targetEnemies.Add(enemy);
                        enemy.SetTargetIndicator(true);
                    }
                }
                else if(enemy != null)
                {
                    //Untarget enemy
                    if (enemy.OffScreenIndicator != null)
                    {
                        Destroy(enemy.OffScreenIndicator);
                    }
                    _targetEnemies.Remove(enemy);
                    enemy.SetTargetIndicator(false);
                }
            }
        }
    }
}


