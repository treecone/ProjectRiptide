using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Caden Messenger, Mira Antolovich
/// 4/6/2019
/// </summary>
public enum EnemyState { Passive, Hostile }
public delegate void AI();
public delegate bool MonsterAction(ref float time);
public delegate Vector3 GetVector();
public delegate void GiveVector(Vector3 vec);
public delegate void GiveFloat(float f);
public delegate void DeleteHostile(GameObject g);
public enum EnemyType { FirstEnemy = 0, KoiBoss = 1, DefensiveEnemy = 2, PassiveEnemy = 3, RockCrab = 4, SeaSheep = 5,
    FlowerFrog = 6, ClamBoss = 7, Pandatee = 8, ChickenFlock = 9}
public enum Anim { Die = 0, Velocity = 1};


public partial class Enemy : Physics
{
    //public fields
    [SerializeField]
    protected GameObject _projectile;
    [SerializeField]
    protected GameObject _healthBarObject;
    [SerializeField]
    protected GameObject _hitbox;
    protected Camera _camera;

    //Health fields
    protected HealthBar _healthBar;
    protected float _health;
    protected float _maxHealth;

    protected EnemyType _enemyType;
    protected EnemyState _state;
    public EnemyState State => _state;

    //player's distance from enemy
    protected float _playerDistance;
    //monsters distance from start position
    protected float _enemyDistance;
    //Radius to trigger hostile AI
    protected float _hostileRadius;
    //Radius to trigger passive AI
    protected float _passiveRadius;
    //Fields for AI
    protected float _speed;
    protected Vector3 _destination;
    protected Quaternion _lookRotation;
    protected double _timeBetween;
    protected double _timeCurrent;
    protected Vector3 _startPos;
    protected Vector3 _gravity;
    protected float _wanderRadius;
    protected float _maxRadius;
    protected float _passiveCooldown;
    protected float _hostileCooldown;
    protected float _pushMult = 1.0f;
    protected float[] _specialCooldown;
    protected bool[] _activeStates;
    protected bool _playerCollision;
    protected bool _obsticalCollision;
    protected bool _isRaming;
    protected bool _isInvincible;
    protected bool _inKnockback = false;
    protected float _initalPos;
    protected float _currTime = 0.0f;
    protected int _ramingDamage;
    protected AI _HostileAI;
    protected AI _PassiveAI;
    protected List<GameObject> _hitboxes;
    protected List<GameObject> _hurtboxes;
    protected Queue<MonsterAction> _actionQueue;
    protected Transform _detectPosition;
    protected GetVector PlayerPosition;
    protected GetVector PlayerVelocity;
    protected GiveVector SendKnockback;
    protected GiveFloat SendFriction;

    //Animation
    protected Animator _animator;
    protected int[] _animParm;

    //Death
    protected bool _dying = false;
    protected int _deathAnim;
    protected float _deathTimer;

    //Fields for collision detection
    [SerializeField]
    protected float _lengthMult;
    [SerializeField]
    protected float _widthMult;
    [SerializeField]
    protected float _heightMult;

    protected float _halfView = 55.0f;
    protected float _viewRange = 20.0f;
    protected Vector3 _widthVector;

    protected float _rotationalVeloctiy = 0.5f;

    public float Health => _health;
    public bool IsInvincible => _isInvincible;

    protected Vector2 _enemyStartingChunk;
    public Vector2 EnemyStartingChunk
    {
        get { return _enemyStartingChunk; }
        set { _enemyStartingChunk = value; }
    }

    protected Vector2 _enemyStartingPosition;
    public Vector2 EnemyStartingPosition
    {
        get { return _enemyStartingPosition; }
        set { _enemyStartingPosition = value; }
    }

    protected int _enemyID;
    public int EnemyID
    {
        get { return _enemyID; }
        set { _enemyID = value; }
    }

    protected bool _readyToDelete;
    public bool ReadyToDelete
    {
        get { return _readyToDelete; }
        set { _readyToDelete = value; }
    }

    // Events:
    public event DeleteHostile delete;

    // Start is called before the first frame update
    protected override void Start()
    {
        _readyToDelete = false;

        _state = EnemyState.Passive;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        _playerDistance = Vector3.Distance(transform.position, player.transform.position);
        _healthBar = GetComponent<HealthBar>();
        _healthBarObject.SetActive(false);
        _hitboxes = new List<GameObject>();
        _actionQueue = new Queue<MonsterAction>();
        ShipMovement movement = player.GetComponent<ShipMovement>();
        PlayerPosition = movement.GetPosition;
        PlayerVelocity = movement.GetVelocity;
        SendKnockback = movement.TakeKnockback;
        SendFriction = movement.ApplyFriction;
        foreach (Hitbox hitbox in GetComponentsInChildren<Hitbox>())
        {
            hitbox.OnTrigger += HitboxTriggered;
            hitbox.OnStay += OnObsticalCollision;
        }
        _camera = Camera.main.GetComponent<Camera>();
        _animator = GetComponentInChildren<Animator>();

        _widthVector = new Vector3(_widthMult, 0, 0);
        _detectPosition = transform.GetChild(transform.childCount - 1);
        if(_detectPosition.tag != "Detect")
        {
            _detectPosition = null;
        }

        base.Start();
    }
    // Update is called once per frame
    protected override void Update()
    {
        if (!_dying)
        {
            //updates player position
            _playerDistance = Vector3.Distance(transform.position, PlayerPosition());
            _enemyDistance = Vector3.Distance(_startPos, transform.position);

            //checks for states
            switch (_state)
            {
                case EnemyState.Passive:
                    _PassiveAI();
                    //check for hostile behavior trigger event stuff -> if you get close enough, or shoot it
                    //also make sure enemy is not in a passive cooldown
                    if (_playerDistance < _hostileRadius && _passiveCooldown <= 0)
                    {
                        OnHostile();
                        _state = EnemyState.Hostile;
                    }
                    break;
                case EnemyState.Hostile:
                    _HostileAI();
                    //check for passive behavior trigger, if you get far enough away
                    if (_playerDistance >= _passiveRadius && _hostileCooldown <= 0)
                    {
                        OnPassive();
                        _state = EnemyState.Passive;
                    }
                    break;
            }

            //Make health bar face player
            _healthBarObject.transform.rotation = new Quaternion(_camera.transform.rotation.x, _camera.transform.rotation.y, _camera.transform.rotation.z, _camera.transform.rotation.w);

            if (_passiveCooldown > 0)
                _passiveCooldown -= Time.deltaTime;

            if (_hostileCooldown > 0)
                _hostileCooldown -= Time.deltaTime;

            SetHealthBarPosition();

            _playerCollision = false;
            _obsticalCollision = false;

            base.Update();
        }
        else
        {
            if(_deathTimer > 3.0f)
            {
                _readyToDelete = true;
            }
            _deathTimer += Time.deltaTime;
        }
    }

    /// <summary>
    /// Monster takes damage, if health is 0 they die
    /// </summary>
    /// <param name="damage">Amount of damage taken</param>
    public void TakeDamage(float damage)
    {
        if (_health > 0 && !_isInvincible)
        {
            _health -= damage;
            _healthBar.UpdateHealth(_health);
            if (_state == EnemyState.Passive && _passiveCooldown <= 0)
            {
                OnHostile();
                _state = EnemyState.Hostile;
            }
            if (_health <= 0)
            {
                _health = 0;
                if (_animator != null)
                {
                    _animator.SetTrigger(_animParm[(int)Anim.Die]);
                    _deathAnim = Animator.StringToHash("death");
                }
                _dying = true;
                _isInvincible = true;
                _deathTimer = 0;
                OnDeath();
            }
        }
    }

    /// <summary>
    /// Destorys the enemy and drops loot
    /// </summary>
    public virtual void DestroyEnemy()
    {
        //GameObject lootable = Instantiate(Resources.Load("Inventory/Lootable"), new Vector3(transform.position.x + Random.Range(-2.0f, 2.0f), transform.position.y, transform.position.z + Random.Range(-2.0f, 2.0f)), Quaternion.identity) as GameObject;
        //lootable.GetComponent<Lootable>().itemStored = GameObject.FindWithTag("GameManager").GetComponent<ItemDatabase>().FindItem("Carp Scale");
        //lootable.GetComponent<Lootable>().lightColor = GameObject.FindWithTag("GameManager").GetComponent<ItemDatabase>().rarityColors[lootable.GetComponent<Lootable>().itemStored.Rarity];
        //Kill monster
        //Destroy(gameObject);
    }

    /// <summary>
    /// Resets enemy's hostile AI values
    /// </summary>
    protected void ResetHostile()
    {
        //reset states
        for (int i = 0; i < _activeStates.Length; i++)
        {
            _activeStates[i] = false;
        }
        //reset cooldowns
        for (int i = 0; i < _specialCooldown.Length; i++)
        {
            _specialCooldown[i] = 5.0f;
        }
        _isRaming = false;
        _inKnockback = false;
        _actionQueue.Clear();
        ClearHitboxes();
        _currTime = 0;
    }

    /// <summary>
    /// Called when a hitbox is triggered
    /// </summary>
    /// <param name="collision">GameObject that triggered hitbox</param>
    public void HitboxTriggered(GameObject collision)
    {
        if (collision.tag == "Obstical")
        {
            _obsticalCollision = true;
        }
        if (collision.tag == "Player")
        {
            _playerCollision = true;
        }

    }

    /// <summary>
    /// Creates a hitbox as a child of the enemy
    /// </summary>
    /// <param name="position">Position relative to enemy</param>
    /// <param name="scale">Size of hitbox</param>
    /// <param name="type">Type of hitbox</param>
    /// <param name="damage">Damage dealt by hitbox</param>
    /// <returns></returns>
    protected GameObject CreateHitbox(Vector3 position, Vector3 scale, HitboxType type, float damage)
    {
        GameObject temp = Instantiate(_hitbox, transform);
        temp.GetComponent<Hitbox>().SetHitbox(gameObject, position, scale, type, damage);
        temp.GetComponent<Hitbox>().OnTrigger += HitboxTriggered;
        return temp;
    }

    /// <summary>
    /// Creates a hitbox as a child of the enemy
    /// </summary>
    /// <param name="position">Position relative to enemy</param>
    /// <param name="scale">Size of hitbox</param>
    /// <param name="type">Type of hitbox</param>
    /// <param name="damage">Damage dealt by hitbox</param>
    /// <param name="launchAngle">Angle that hitbox will launch player</param>
    /// <param name="launchStrength">Strength at which player will be launched</param>
    /// <returns></returns>
    protected GameObject CreateHitbox(Vector3 position, Vector3 scale, HitboxType type, float damage, Vector2 launchAngle, float launchStrength)
    {
        GameObject temp = Instantiate(_hitbox, transform);
        temp.GetComponent<Hitbox>().SetHitbox(gameObject, position, scale, type, damage, launchAngle, launchStrength);
        temp.GetComponent<Hitbox>().OnTrigger += HitboxTriggered;
        return temp;
    }

    /// <summary>
    /// Spawns an enemy projectile
    /// </summary>
    /// <param name="position">Position relative to enemy</param>
    /// <param name="speed">Speed of projectile</param>
    /// <param name="damage">Damage projectile inflicts</param>
    /// <param name="maxLifeSpan">Max life span of projectile</param>
    /// <param name="movementPattern">Movement pattern of projectile</param>
    protected void SpawnProjectile(Vector3 position, float speed, int damage, float maxLifeSpan, MovementPattern movementPattern)
    {
        GameObject.Instantiate(_projectile,
            transform.position + transform.TransformVector(position),
            new Quaternion())
            .GetComponent<EnemyProjectile>().LoadProjectile(
            transform.TransformVector(position),
            0.75f,
            damage,
            maxLifeSpan,
            movementPattern);
    }

    /// <summary>
    /// Spawns an enemy projectile
    /// </summary>
    /// <param name="position">Position relative to enemy</param>
    /// <param name="speed">Speed of projectile</param>
    /// <param name="damage">Damage projectile inflicts</param>
    /// <param name="maxLifeSpan">Max life span of projectile</param>
    /// <param name="movementPattern">Movement pattern of projectile</param>
    /// <param name="launchAngle">Angle that hitbox will launch player</param>
    /// <param name="launchStrength">Strength at which player will be launched</param>
    protected void SpawnProjectile(Vector3 position, float speed, int damage, float maxLifeSpan, MovementPattern movementPattern, Vector2 launchAngle, float launchStrength)
    {
        GameObject.Instantiate(_projectile,
            transform.position + transform.TransformVector(position),
            new Quaternion())
            .GetComponent<EnemyProjectile>().LoadProjectile(
            transform.TransformVector(position),
            0.75f,
            damage,
            maxLifeSpan,
            movementPattern,
            launchAngle,
            launchStrength);
    }

    /// <summary>
    /// Clears all hitboxes off the enemy
    /// </summary>
    protected void ClearHitboxes()
    {
        for (int i = 0; i < _hitboxes.Count; i++)
        {
            GameObject.Destroy(_hitboxes[i]);
        }
        _hitboxes.Clear();
    }

    /// <summary>
    /// Returns enemy to inital position on Y axis
    /// </summary>
    protected void ReturnToInitalPosition()
    {

        _position = new Vector3(transform.position.x, _initalPos, transform.position.z);
    }

    /// <summary>
    /// Sets position of health bar above enemy
    /// </summary>
    protected void SetHealthBarPosition()
    {
        _healthBarObject.transform.position = new Vector3(transform.position.x, _heightMult + 1.5f * transform.localScale.y, transform.position.z);
    }

    /// <summary>
    /// Checks if there is an obstical in the enemy's path
    /// </summary>
    /// <returns>If enemy's path is interuptted</returns>
    protected bool CheckObstacle(Vector3 target)
    {
        RaycastHit[] hits;
        Vector3 detectPosition = transform.position;
        if (_detectPosition != null)
        {
            detectPosition = _detectPosition.position;
        }
        Vector3 targetDir = target - transform.position;
        targetDir.Normalize();
        
        for (int i = 0; i <= _halfView; i += 4)
        {
            Debug.DrawRay(detectPosition, Quaternion.AngleAxis(i, Vector3.up) * targetDir * _viewRange, Color.red);
            Debug.DrawRay(detectPosition, Quaternion.AngleAxis(-i, Vector3.up) * targetDir * _viewRange, Color.red);
            //Check right
            hits = UnityEngine.Physics.SphereCastAll(detectPosition, _widthMult, Quaternion.AngleAxis(i, Vector3.up) * targetDir, _viewRange);
            foreach(RaycastHit hit in hits)
            {
                //Make sure hit was not from their own hitbox
                if (!(hit.collider.tag == "Hitbox" && hit.collider.transform.parent.gameObject == gameObject))
                {
                    return true;
                }
            }

            //Check left
            hits = UnityEngine.Physics.SphereCastAll(detectPosition, _widthMult, Quaternion.AngleAxis(-i, Vector3.up) * targetDir, _viewRange);
            foreach (RaycastHit hit in hits)
            {
                //Make sure hit was not from their own hitbox
                if (!(hit.collider.tag == "Hitbox" && hit.collider.transform.parent.gameObject == gameObject))
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Find direction to avoid obstacle
    /// </summary>
    /// <returns>Direction to avoid obstacle</returns>
    protected Vector3 AvoidObstacle(Vector3 target)
    {
        //Debug.Log("Avoiding Obstacle");
        Vector3 dir = Vector3.zero;
        bool found = false;

        Vector3 detectPosition = transform.position;
        if(_detectPosition != null)
        {
            detectPosition = _detectPosition.position;
        }

        Vector3 targetDir = target - transform.position;
        targetDir.Normalize();
        RaycastHit hit;

        //Check 90 degrees for a path to avoid obstacle
        for (int i = 0; i <= 90; i += 4)
        {
            //Check right side for path
            if (!UnityEngine.Physics.SphereCast(detectPosition, _widthMult, Quaternion.AngleAxis(i, Vector3.up) * targetDir, out hit, _viewRange * 1.5f))
            {
                //Set direction if path is found
                dir = Quaternion.AngleAxis(i, Vector3.up) * targetDir;
                Debug.DrawLine(transform.position, transform.position + dir * _viewRange * 1.5f, Color.yellow);
                found = true;
            }
            //Check left side for path
            if (!UnityEngine.Physics.SphereCast(detectPosition, _widthMult, Quaternion.AngleAxis(-i, Vector3.up) * targetDir, out hit, _viewRange * 1.5f))
            {
                //Set direction if path is found
                dir = Quaternion.AngleAxis(-i, Vector3.up) * targetDir;
                Debug.DrawLine(transform.position, transform.position + dir * _viewRange * 1.5f, Color.yellow);
                found = true;
            }
            if (found)
                return dir;
        }

        return Quaternion.AngleAxis(90, Vector3.up) * transform.forward;
    }

    /// <summary>
    /// Called when inside an obstacle
    /// Move enemy after the obstacle
    /// </summary>
    /// <param name="obstical">GameObject colliding with</param>
    public void OnObsticalCollision(GameObject obstical)
    {
        if (obstical.tag == "Obstical")
        {
            StopHorizontalMotion();
            Vector3 backForce = transform.position - obstical.transform.position;
            backForce = new Vector3(backForce.x, 0, backForce.z);
            backForce.Normalize();
            backForce *= 200.0f;
            ApplyForce(backForce);
        }
        else if(obstical.tag == "Hitbox" && obstical.transform.parent.tag == "Enemy")
        {
            GameObject attached = obstical.GetComponent<Hitbox>().AttachedObject;
            if(attached != gameObject)
            {
                Vector3 backForce = transform.position - obstical.transform.position;
                backForce = new Vector3(backForce.x, 0, backForce.z);
                backForce.Normalize();
                backForce *= 5.0f * _pushMult;
                ApplyForce(backForce);
            }
        }
        else if(obstical.tag == "Hitbox" && obstical.transform.parent.tag == "Player")
        {
            Vector3 backForce = transform.position - obstical.transform.position;
            backForce = new Vector3(backForce.x, 0, backForce.z);
            backForce.Normalize();
            backForce *= 5.0f * _pushMult;
            ApplyForce(backForce);
        }
    }

    /// <summary>
    /// Take knockback from an outside source
    /// </summary>
    /// <param name="knockback">Knockback force</param>
    public void TakeKnockback(Vector3 knockback)
    {
        ApplyForce(knockback);
    }

    /// <summary>
    /// Rotates monster smoothly towards desired rotation
    /// </summary>
    /// <param name="desiredRotation">Desired Rotation</param>
    /// <param name="rotationalAcceleration">Rotational Acceleration</param>
    /// <param name="minRotationalVelocity">Minimum Rotational Velocity</param>
    /// <param name="maxRotationalVelocity">Maximum Rotational Velocity</param>
    protected void SetSmoothRotation(Quaternion desiredRotation, float rotationalAcceleration, float minRotationalVelocity, float maxRotationalVelocity)
    {
        //Rotate based on target location
        if (_rotation != desiredRotation)
        {
            //If rotation is close to desired location, slow down rotation
            if (Quaternion.Angle(_rotation, desiredRotation) < 45.0f)
            {
                _rotationalVeloctiy += _rotationalVeloctiy * -0.80f * Time.deltaTime;
                //Make sure rotation stay's above minium value
                if (_rotationalVeloctiy < minRotationalVelocity)
                    _rotationalVeloctiy = minRotationalVelocity;
            }
            //Else speed up rotation
            else
            {
                _rotationalVeloctiy += rotationalAcceleration * Time.deltaTime;
                //Make sure rotation stay's below maximum value
                if (_rotationalVeloctiy > maxRotationalVelocity)
                    _rotationalVeloctiy = maxRotationalVelocity;
            }

            //Update rotation
            _rotation = Quaternion.RotateTowards(_rotation, desiredRotation, _rotationalVeloctiy * 60 * Time.deltaTime);
        }
        //Reset velocity when not rotating
        else
            _rotationalVeloctiy = minRotationalVelocity;
    }

    /// <summary>
    /// Called when monster becomes passive
    /// </summary>
    protected virtual void OnPassive()
    {
        if (_health == _maxHealth)
        {
            _healthBarObject.SetActive(false);
        }
        ResetHostile();
        //Keep monster passive for 5 seconds at least
        _passiveCooldown = 5.0f;
    }
    
    /// <summary>
    /// Called when monster becomes hostile
    /// </summary>
    protected virtual void OnHostile()
    {
        _healthBarObject.SetActive(true);
    }

    /// <summary>
    /// Called when monster's death is triggered
    /// </summary>
    protected virtual void OnDeath()
    {

    }
}