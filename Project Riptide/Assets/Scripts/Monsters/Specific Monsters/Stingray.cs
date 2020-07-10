using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StingrayAttackState { BoltAttack = 1, CrossZap = 2};

public partial class Stingray : Enemy
{
    [SerializeField]
    private ParticleSystem _electricChargeParticles;
    [SerializeField]
    private GameObject _electricBoltParticlesPrefab;
    [SerializeField]
    private GameObject _electricShockParticlesPrefab;
    private GameObject _electricBoltParticles;
    private GameObject _electricShockParticles;

    private bool _crossZapping;
    public bool CrossZapping
    {
        get { return _crossZapping; }
        set { _crossZapping = value; }
    }

    private bool _crossZapSetup;
    private float _crossZapTime;
    private bool _crossZapParent;
    private const float MAX_CROSS_ZAP_TIME = 10.0f;

    public bool CanCrossZap
    {
        get
        {
            if(!_activeStates[(int)AttackState.Active] && !IsDying)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    private Stingray _zapBuddy;
    public Stingray ZapBuddy
    {
        get { return _zapBuddy; }
        set { _zapBuddy = value; }
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        _electricChargeParticles.Stop();
        base.Start();

        //Set parameters
        _enemyType = EnemyType.Stingray;
        _speed = 1.0f;
        _health = 50;
        _maxHealth = 50;
        _timeBetween = 5.0;
        _timeCurrent = _timeBetween;
        _startPos = transform.position;
        _wanderRadius = 60.0f;
        _hostileRadius = 30.0f;
        _passiveRadius = 70.0f;
        _maxRadius = 240.0f;
        _specialCooldown = new float[5] { 5.0f, 0.0f, Random.Range(0, 2.0f), 0.0f, 0.0f };
        _activeStates = new bool[3] { false, false, false };
        _animParm = new int[2] {
                    Animator.StringToHash("die"),
                    Animator.StringToHash("velocity")};
        _playerCollision = false;
        _isRaming = false;
        _ramingDamage = 20;
        _pushMult = 1.0f;
        _HostileAI = HostileStingray;
        _PassiveAI = PassiveWanderRadius;

        //Setup health bar
        _healthBar.SetMaxHealth(_maxHealth);
        _healthBar.UpdateHealth(_health);

        //Set up hitboxes
        foreach (Hitbox hitbox in GetComponentsInChildren<Hitbox>())
        {
            hitbox.OnTrigger += HitboxTriggered;
            hitbox.OnStay += OnObsticalCollision;
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(10);
        }
        base.Update();
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        if(_electricChargeParticles.isPlaying)
        {
            _electricChargeParticles.Stop();
        }
        if(_electricBoltParticles != null)
        {
            _electricBoltParticles.GetComponentInChildren<ParticleSystem>().Stop();
        }
        if(_frozen && _zapBuddy != null)
        {
            _zapBuddy.GetComponent<StatusEffects>().RemoveStatus("BuddyStun");
        }
    }

    /// <summary>
    /// Checks to see if there is another stingray around to do a combined attack
    /// </summary>
    /// <param name="radius">Radius to check for stingray in</param>
    protected bool CheckForZapPartner(float radius)
    {
        foreach(Collider collider in UnityEngine.Physics.OverlapSphere(transform.position, radius))
        {
            if(collider.transform.gameObject.tag == "Hitbox")
            {
                //Check to see if stingray can do cross zap attack
                Stingray foundEnemy = collider.GetComponent<Hitbox>().AttachedObject.GetComponent<Stingray>();
                if(foundEnemy != null && foundEnemy.gameObject != gameObject && !foundEnemy.CrossZapping && foundEnemy.CanCrossZap && !_crossZapping)
                {
                    foundEnemy.CrossZapping = true;
                    _crossZapping = true;
                    _zapBuddy = foundEnemy;
                    _zapBuddy.ZapBuddy = this;
                    _crossZapParent = true;
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Deals damage to the player overtime from electricity
    /// </summary>
    /// <param name="hitbox"></param>
    protected void DealElectricDamage(GameObject other)
    {
        const float DAMAGE_PER_SECOND = 10.0f;

        //If the collision was with another hitbox
        if (other.CompareTag("Hitbox"))
        {
            Hitbox hitbox = other.GetComponent<Hitbox>();
            //If the collision was with a player hurtbox
            if (hitbox.Type == HitboxType.PlayerHurtbox && hitbox.AttachedObject.CompareTag("Player"))
            {
                //Player takes damage
                hitbox.AttachedObject.GetComponent<PlayerHealth>().TakeDamage(DAMAGE_PER_SECOND * Time.deltaTime);
            }
        }
    }

    /// <summary>
    /// Adds electric shock effect to the player
    /// </summary>
    /// <param name="other">Other hitbox</param>
    protected void AddElectricEffect(GameObject other)
    {
        if (other.CompareTag("Player"))
        {
            //Add shock effect
            _electricShockParticles = Instantiate(_electricShockParticlesPrefab, other.transform.position, other.transform.rotation, other.transform);
        }
    }

    /// <summary>
    /// Remove electric shock effect to the player
    /// </summary>
    /// <param name="other">Other hitbox</param>
    protected void RemoveElectricEffectOnExit(GameObject other)
    {
        if (_electricShockParticles != null)
        {
            if (other.CompareTag("Hitbox"))
            {
                Hitbox hitbox = other.GetComponent<Hitbox>();
                //If the collision was with a player hurtbox
                if (hitbox.Type == HitboxType.PlayerHurtbox && hitbox.AttachedObject.CompareTag("Player"))
                {
                    //Player takes damage
                    _electricShockParticles.GetComponentInChildren<ParticleSystem>().Stop();
                }
            }
        }
    }

    /// <summary>
    /// removes electric shock effect from player
    /// </summary>
    protected void RemoveElectricEffect()
    {
        if (_electricShockParticles != null)
        {
            _electricShockParticles.GetComponentInChildren<ParticleSystem>().Stop();
        }
    }

    protected override void OnFreeze()
    {
        if(_zapBuddy != null && !_zapBuddy._frozen)
        {
            _zapBuddy.GetComponent<StatusEffects>().AddStatus(StatusType.Stun, "BuddyStun", 9999.0f, 1.0f);
        }
        base.OnFreeze();
    }

    protected override void OnUnfreeze()
    {
        if(_zapBuddy != null)
        {
            _zapBuddy.GetComponent<StatusEffects>().RemoveStatus("BuddyStun");
        }
        base.OnUnfreeze();
    }
}
