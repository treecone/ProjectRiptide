﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ClamOpenState { Dragon = 0, Bird = 1, WaterSpout = 2}; 
public enum ClamAnim { Open = 2, Close = 3, SpeedScale = 4};
public enum ClamAttackState { Opened = 1, TentacleBurst = 2, TentacleTripleBurst = 3, OpenAttack = 4, TentacleCircle = 5, TentacleTrack = 6, TentacleLine = 7 }

public partial class ClamBoss : Enemy
{
    protected ClamOpenState _openState;
    protected float _lineOffset;
    protected Vector3 _lineForward;
    protected float _speedScale;

    [SerializeField]
    protected GameObject _tentaclePrefab;
    [SerializeField]
    protected GameObject _puppetCanvas;
    [SerializeField]
    protected Image _puppet;
    [SerializeField]
    protected Sprite[] _puppetSprites;
    [SerializeField]
    protected GameObject _waterSpoutUpPrefab;
    [SerializeField]
    protected GameObject _waterSpoutDownPrefab;
    [SerializeField]
    protected GameObject _rockPrefab;
    [SerializeField]
    protected GameObject _stickPrefab;
    [SerializeField]
    protected GameObject _dragonSmokePrefab;
    [SerializeField]
    protected ParticleSystem _darknessParticles;

    protected ParticleSystem _waterSpoutUp;
    protected ParticleSystem _waterSpoutDown;
    protected List<ParticleSystem> _dragonSmokeParticles = new List<ParticleSystem>();

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        //Set parameters
        _enemyType = EnemyType.ClamBoss;
        _speed = EnemyConfig.Instance.ClamBoss.Base.Speed;
        _health = EnemyConfig.Instance.ClamBoss.Base.MaxHealth;
        _maxHealth = EnemyConfig.Instance.ClamBoss.Base.MaxHealth;
        _timeBetween = 5.0;
        _timeCurrent = _timeBetween;
        _startPos = transform.position;
        _wanderRadius = EnemyConfig.Instance.ClamBoss.Base.WanderRadius;
        _hostileRadius = EnemyConfig.Instance.ClamBoss.Base.HostileRadius;
        _passiveRadius = EnemyConfig.Instance.ClamBoss.Base.PassiveRadius;
        _maxRadius = EnemyConfig.Instance.ClamBoss.Base.MaxRadius;
        _specialCooldown = new float[8] { 1.0f, 0.0f, 0.0f, 0.0f, 5.0f, 0.0f, 0.0f, 0.0f };
        _activeStates = new bool[4] { false, false, false, false };
        _animParm = new int[5] {
                    Animator.StringToHash("die"),
                    Animator.StringToHash("velocity"),
                    Animator.StringToHash("open"),
                    Animator.StringToHash("close"),
                    Animator.StringToHash("speedScale")};
        _playerCollision = false;
        _isRaming = false;
        _ramingDamage = 20;
        _pushMult = EnemyConfig.Instance.ClamBoss.Base.PushMult;
        _HostileAI = HostileClamBoss;
        _PassiveAI = PassiveDoNothing;

        //Setup health bar
        _healthBar.SetMaxHealth(_maxHealth);
        _healthBar.UpdateHealth(_health);

        _darknessParticles.Stop();

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
        if(_activeStates[(int)ClamAttackState.Opened])
        {
            _puppetCanvas.transform.rotation = new Quaternion(_camera.transform.rotation.x, _camera.transform.rotation.y, _camera.transform.rotation.z, _camera.transform.rotation.w);
        }

        //Calculate speed scale for attacks
        _speedScale = 0.5f + _health / _maxHealth * 0.5f;
        _animator.SetFloat(_animParm[(int)ClamAnim.SpeedScale], 1 / _speedScale);
        ParticleSystem.EmissionModule emission = _darknessParticles.emission;
        emission.rateOverTime = 100 / _speedScale - 95;
        base.Update();
    }

    /// <summary>
    /// Toggles activity of puppet canvas
    /// </summary>
    /// <param name="on">Toggle on or off</param>
    protected void SetPuppetCanvas(bool on)
    {
        if (on)
        {
            _puppetCanvas.SetActive(true);
            _puppet.sprite = _puppetSprites[(int)_openState];
        }
        else
        {
            _puppetCanvas.SetActive(false);
        }
    }

    /// <summary>
    /// Deals damage to the player overtime from waterspout
    /// </summary>
    /// <param name="hitbox"></param>
    protected void DealWaterSpoutDamage(GameObject other)
    {
        //If the collision was with another hitbox
        if (other.CompareTag("Hitbox"))
        {
            Hitbox hitbox = other.GetComponent<Hitbox>();
            //If the collision was with a player hurtbox
            if (hitbox.Type == HitboxType.PlayerHurtbox && hitbox.AttachedObject.CompareTag("Player"))
            {
                //Player takes damage
                hitbox.AttachedObject.GetComponent<PlayerHealth>().TakeDamage(EnemyConfig.Instance.ClamBoss.OpenAttack.WaterSpoutAttack.DamagePerSecond * Time.deltaTime, false);
            }
        }
    }

    /// <summary>
    /// Add poison effect to player when dragon breath hitbox is triggered
    /// </summary>
    /// <param name="player"></param>
    protected void DealDragonPoison(GameObject player)
    {
        if(player.tag == "Player")
        {
            //Deal 2 damage per second for 5 seconds
            player.GetComponent<StatusEffects>().AddStatus(StatusType.Poison, 5.0f, EnemyConfig.Instance.ClamBoss.OpenAttack.DragonAttack.PoisonDamage);
        }
    }

    /// <summary>
    /// Called when clam turns hostile
    /// Turn on particles
    /// </summary>
    protected override void OnHostile()
    {
        _darknessParticles.Play();
        base.OnHostile();
    }

    /// <summary>
    /// Called when clam turns passive
    /// Turn off particles
    /// </summary>
    protected override void OnPassive()
    {
        _darknessParticles.Stop();
        SetPuppetCanvas(false);
        if (_health == _maxHealth)
        {
            _canvas.transform.GetChild(0).gameObject.SetActive(false);
        }
        //Only reset states that aren't triggers
        for (int i = 0; i < 2; i++)
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
        //Keep monster passive for 5 seconds at least
        _passiveCooldown = 5.0f;
    }

    /// <summary>
    /// Stop particles on death
    /// </summary>
    protected override void OnDeath()
    {
        _darknessParticles.Stop();
        SetPuppetCanvas(false);
        base.OnDeath();
    }
}
