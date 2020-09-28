using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CarpAnim { SwimSpeed = 2, Dive = 3, Shoot = 4, UAttack = 5 };
public enum KoiAttackState { TripleDash = 2, BubbleBlast = 4, UnderwaterAttack = 3, BubbleAttack = 3 }

public partial class KoiBoss : Enemy
{
    #region Config Constants
    //Constant values for Koi boss AI
    //Koi dash attack
    private const float KOI_DASH_ATTACK_MAX_DISTANCE = 13.0f; //Max distance to be able to use dash attack
    private const float KOI_DASH_ATTACK_COOLDOWN     = 6.0f;
    private const float KOI_DASH_ATTACK_CHARGE_TIME  = 1.5f; //Time charging dash
    private const float KOI_DASH_ATTACK_STALL_TIME   = 0.2f; //Time stalling after charging before using dash
    private const float KOI_DASH_ATTACK_DAMGAGE      = 20.0f; //Damage of dash attack
    private const float KOI_DASH_ATTACK_DISTANCE     = 30.0f; //If this is changed, telegraph will need to be change manually

    //Koi bubble blast attack
    private const float KOI_BUBBLE_BLAST_MAX_DISTANCE         = 20.0f;
    private const float KOI_BUBBLE_BLAST_COOLDOWN             = 8.0f;
    private const float KOI_BUBBLE_BLAST_TRANSITION_DOWN_TIME = 1.0f; //Time it takes koi to go underwater before bubble blast
    private const float KOI_BUBBLE_BLAST_TRANSITION_UP_TIME   = 1.0f; //Time it takes koi to go back above water before bubble blast
    private const float KOI_BUBBLE_BLAST_CHARGE_TIME          = 1.0f; //Time charging bubble blast
    private const float KOI_BUBBLE_BLAST_STALL_TIME           = 0.1f; //Time stalling after charging bubble blast
    private const float KOI_BUBBLE_BLAST_DAMAGE               = 15.0f;
    private const float KOI_BUBBLE_BLAST_RECHARGE_TIME        = 0.5f; //Time charging subsequent shots after first one
    private const float KOI_BUBBLE_BLAST_RESTALL_TIME         = 0.1f; //Stall time of subsequent shots

    //Koi bubble attack
    private const float KOI_BUBBLE_ATTACK_MIN_DISTANCE = 10.0f;
    private const float KOI_BUBBLE_ATTACK_COOLDOWN     = 3.0f;
    private const float KOI_BUBBLE_ATTACK_DAMAGE       = 10.0f;

    //Koi underwater dash
    private const float KOI_UNDERWATER_DASH_VULNERABILITY_TIME = 3.0f;

    //Koi underwater bubble blast
    private const float KOI_UNDERWATER_BUBBLE_BLAST_VULNERABILITY_TIME = 2.5f;

    //Koi underater attack
    private const float KOI_UNDERWATER_ATTACK_MAX_DISTANCE        = 15.0f;
    private const float KOI_UNDERWATER_ATTACK_COOLDOWN            = 8.0f;
    private const float KOI_UNDERWATER_ATTACK_FOLLOW_TIME         = 4.0f;
    private const float KOI_UNDERWATER_ATTACK_STALL_TIME          = 0.5f;
    private const float KOI_UNDERWATER_ATTACK_FOLLOW_SPEED        = 5.0f;
    private const float KOI_UNDERWATER_ATTACK_VULNERABILIITY_TIME = 3.0f;
    #endregion

    [SerializeField]
    private ParticleSystem _dashParticles;
    [SerializeField]
    private GameObject _bubbleBrothPrefab;
    [SerializeField]
    private float _waterLevel;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        //Set parameters
        _enemyType = EnemyType.KoiBoss;
        _speed = 1.0f;
        _health = 200;
        _maxHealth = 200;
        _timeBetween = 5.0;
        _timeCurrent = _timeBetween;
        _startPos = transform.position;
        _wanderRadius = 60.0f;
        _hostileRadius = 30.0f;
        _passiveRadius = 120.0f;
        _maxRadius = 240.0f;
        _specialCooldown = new float[5] { 5.0f, 0.0f, 0.0f, 0.0f, 0.0f };
        _activeStates = new bool[3] { false, false, false };
        _animParm = new int[6] {
                    Animator.StringToHash("die"),
                    Animator.StringToHash("velocity"),
                    Animator.StringToHash("swimSpeed"),
                    Animator.StringToHash("dive"),
                    Animator.StringToHash("shoot"),
                    Animator.StringToHash("uAttack")};
        _playerCollision = false;
        _isRaming = false;
        _ramingDamage = 20;
        _pushMult = 0.1f;
        _HostileAI = HostileKoiBoss;
        _PassiveAI = PassiveWanderRadius;

        _dashParticles.Stop();

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
        base.Update();
    }

    /// <summary>
    /// Spawns bubble broth at detect position of koi boss
    /// </summary>
    protected void SpawnBubbleBroth()
    {
        Instantiate(_bubbleBrothPrefab, new Vector3(_detectPosition.position.x, _waterLevel, _detectPosition.position.z), transform.rotation);
    }
}
