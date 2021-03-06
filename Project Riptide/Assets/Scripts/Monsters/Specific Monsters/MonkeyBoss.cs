﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonkeyAnim { ScreechAttack = 2, ScreechAngry = 3, Return = 4, Idle = 5, Swipe = 6, Still = 7, Slam = 8}
public enum MonkeyAttackState { HandPush = 2, HandSwipe = 3, HandClap = 4, Protect = 5, Screech = 6, PushWave = 7, SlamWave = 8 }

public partial class MonkeyBoss : Enemy
{
    [SerializeField]
    private Physics _leftHand;
    [SerializeField]
    private Physics _rightHand;
    [SerializeField]
    private GameObject _screechParticles;
    [SerializeField]
    private GameObject _angryScreechParticles;
    [SerializeField]
    private GameObject _storm;
    [SerializeField]
    private GameObject _forwardWavePrefab;
    [SerializeField]
    private GameObject _circleWavePrefab;

    private Vector3 _leftHandStartPos;
    private Vector3 _rightHandStartPos;
    private Vector3 _stormStartPos;
    private float _rightHandReturnDist;
    private float _leftHandReturnDist;
    private bool _moveLeftWithBody;
    private bool _rotateLeftWithBody;
    private bool _moveRightWithBody;
    private bool _rotateRightWithBody;
    private Animator _leftHandAnimator;
    private Animator _rightHandAnimator;

    private const float RISE_HEIGHT = 10.0f;
    private const float RISE_TIME = 2.0f;

    private bool _rising;
    private bool _rose;
    private bool _playedScreechAnim;
    private bool _playedScreechParticles;

    private Vector3 _screechPos;

    private MonkeyStormCloud _monkeyStormCloud;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        //Set parameters
        _enemyType = EnemyType.MonkeyBoss;
        _speed = EnemyConfig.Instance.MonkeyBoss.Base.Speed;
        _health = EnemyConfig.Instance.MonkeyBoss.Base.MaxHealth;
        _maxHealth = EnemyConfig.Instance.MonkeyBoss.Base.MaxHealth;
        _timeBetween = 5.0;
        _timeCurrent = _timeBetween;
        _startPos = transform.position;
        _wanderRadius = EnemyConfig.Instance.MonkeyBoss.Base.WanderRadius;
        _hostileRadius = EnemyConfig.Instance.MonkeyBoss.Base.HostileRadius;
        _passiveRadius = EnemyConfig.Instance.MonkeyBoss.Base.PassiveRadius;
        _maxRadius = EnemyConfig.Instance.MonkeyBoss.Base.MaxRadius;
        _specialCooldown = new float[9] { 5.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
        _activeStates = new bool[3] { false, false, false };
        _animParm = new int[9] {
                    Animator.StringToHash("die"),
                    Animator.StringToHash("velocity"),
                    Animator.StringToHash("screechAttack"),
                    Animator.StringToHash("screechAngry"),
                    Animator.StringToHash("return"),
                    Animator.StringToHash("idle"),
                    Animator.StringToHash("swipe"),
                    Animator.StringToHash("still"),
                    Animator.StringToHash("slam")};
        _playerCollision = false;
        _isRaming = false;
        _ramingDamage = 20;
        _pushMult = EnemyConfig.Instance.MonkeyBoss.Base.PushMult;
        _HostileAI = HostileMonkeyBoss;
        _PassiveAI = PassiveMonkeyBoss;

        //Setup health bar
        _healthBar.SetMaxHealth(_maxHealth);
        _healthBar.UpdateHealth(_health);

        Hitbox hurtbox = transform.Find("MainHurtbox").GetComponent<Hitbox>();
        hurtbox.OnTrigger += HitboxTriggered;
        hurtbox.OnStay += OnObsticalCollision;


        _leftHandStartPos = _leftHand.transform.localPosition;
        _rightHandStartPos = _rightHand.transform.localPosition;
        _moveLeftWithBody = true;
        _moveRightWithBody = true;
        _rotateLeftWithBody = true;
        _rotateRightWithBody = true;
        _leftHandAnimator = _leftHand.GetComponentInChildren<Animator>();
        _rightHandAnimator = _rightHand.GetComponentInChildren<Animator>();

        //Clear particles
        _screechPos = _screechParticles.transform.position;
        foreach (ParticleSystem particles in _screechParticles.GetComponentsInChildren<ParticleSystem>())
        {
            particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
        foreach (ParticleSystem particles in _angryScreechParticles.GetComponentsInChildren<ParticleSystem>())
        {
            particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        _monkeyStormCloud = _storm.GetComponent<MonkeyStormCloud>();
        _stormStartPos = _storm.transform.localPosition;
        _storm.SetActive(false);
    }

    // Update is called once per frame
    protected override void Update()
    {
        MoveHands();
        //Keep storm above monkey
        if(_storm.activeSelf)
        {
            _monkeyStormCloud.LocalPosition = _stormStartPos;
        }

        base.Update();
    }

    protected override void OnPassive()
    {
        _storm.SetActive(false);
        _rising = false;
        _rose = false;
        base.OnPassive();
    }

    protected override void OnDeath()
    {
        _storm.SetActive(false);
        _leftHand.StopMotion();
        _rightHand.StopMotion();
        _moveLeftWithBody = false;
        _moveRightWithBody = false;
        _leftHand.ApplyForce(Vector3.down * 50);
        _rightHand.ApplyForce(Vector3.down * 50);
        base.OnDeath();
    }

    /// <summary>
    /// Moves hands with body
    /// </summary>
    protected void MoveHands()
    {
        if(_moveRightWithBody)
        {
            _rightHand.LocalPosition = _rightHandStartPos;
        }

        if(_rotateRightWithBody)
        {
            _rightHand.Rotation = transform.rotation;
        }

        if(_moveLeftWithBody)
        {
            _leftHand.LocalPosition = _leftHandStartPos;
        }

        if(_rotateLeftWithBody)
        {
            _leftHand.Rotation = transform.rotation;
        }
    }

    /// <summary>
    /// Toggle screech particles on or off
    /// </summary>
    /// <param name="on"></param>
    protected void ToggleScreechParticles(bool on)
    {
        if(on)
        {
            foreach(ParticleSystem particles in _screechParticles.GetComponentsInChildren<ParticleSystem>())
            {
                particles.Play();
            }
        }
        else
        {
            foreach (ParticleSystem particles in _screechParticles.GetComponentsInChildren<ParticleSystem>())
            {
                particles.Stop();
            }
        }
    }

    /// <summary>
    /// Toggle screech particles on or off
    /// </summary>
    /// <param name="on"></param>
    protected void ToggleAngryScreechParticles(bool on)
    {
        if (on)
        {
            foreach (ParticleSystem particles in _angryScreechParticles.GetComponentsInChildren<ParticleSystem>())
            {
                particles.Play();
            }
        }
        else
        {
            foreach (ParticleSystem particles in _angryScreechParticles.GetComponentsInChildren<ParticleSystem>())
            {
                particles.Stop();
            }
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
    protected GameObject CreateRightHandHitbox(Vector3 position, Vector3 scale, HitboxType type, float damage, Vector2 launchAngle, float launchStrength)
    {
        GameObject temp = Instantiate(_hitbox, _rightHand.transform);
        temp.GetComponent<Hitbox>().SetHitbox(gameObject, position, scale, type, damage, launchAngle, launchStrength);
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
    /// <returns></returns>
    protected GameObject CreateLeftHandHitbox(Vector3 position, Vector3 scale, HitboxType type, float damage, Vector2 launchAngle, float launchStrength)
    {
        GameObject temp = Instantiate(_hitbox, _leftHand.transform);
        temp.GetComponent<Hitbox>().SetHitbox(gameObject, position, scale, type, damage, launchAngle, launchStrength);
        temp.GetComponent<Hitbox>().OnTrigger += HitboxTriggered;
        return temp;
    }

    /// <summary>
    /// Creates a telegraph attached to the monkey's right hand
    /// </summary>
    /// <param name="position">Position relative to hand</param>
    /// <param name="scale">Scale</param>
    /// <param name="rotation">Rotation relative to hand</param>
    /// <param name="telegraphType">Type of telegraph</param>
    /// <param name="parented">If parented</param>
    protected void CreateRightHandTelegraph(Vector3 position, Vector3 scale, Quaternion rotation, TelegraphType telegraphType, bool parented)
    {
        GameObject temp;
        temp = Instantiate(_telegraphPrefab[(int)telegraphType], _rightHand.transform.position, _rightHand.transform.rotation, _rightHand.transform);
        temp.transform.localPosition = position;
        temp.transform.localScale = scale;
        if (rotation != Quaternion.identity)
        {
            temp.transform.rotation = rotation;
        }
        if (!parented)
        {
            temp.transform.parent = null;
        }

        _telegraphs.Add(temp);
    }

    /// <summary>
    /// Creates a telegraph attached to the monkey's left hand
    /// </summary>
    /// <param name="position">Position relative to hand</param>
    /// <param name="scale">Scale</param>
    /// <param name="rotation">Rotation relative to hand</param>
    /// <param name="telegraphType">Type of telegraph</param>
    /// <param name="parented">If parented</param>
    protected void CreateLeftHandTelegraph(Vector3 position, Vector3 scale, Quaternion rotation, TelegraphType telegraphType, bool parented)
    {
        GameObject temp;
        temp = Instantiate(_telegraphPrefab[(int)telegraphType], _leftHand.transform.position, _leftHand.transform.rotation, _leftHand.transform);
        temp.transform.localPosition = position;
        temp.transform.localScale = scale;
        if (rotation != Quaternion.identity)
        {
            temp.transform.rotation = rotation;
        }
        if (!parented)
        {
            temp.transform.parent = null;
        }

        _telegraphs.Add(temp);
    }

    /// <summary>
    /// Resets right hand's position and rotation
    /// </summary>
    protected void ResetRightHand()
    {
        _rightHand.StopMotion();
        _rightHand.LocalPosition = _rightHandStartPos;
        _rightHand.Rotation = transform.rotation;
        _moveRightWithBody = true;
        _rotateRightWithBody = true;
    }

    /// <summary>
    /// Resets left hand's position and rotation
    /// </summary>
    protected void ResetLeftHand()
    {
        _leftHand.StopMotion();
        _leftHand.LocalPosition = _leftHandStartPos;
        _leftHand.Rotation = transform.rotation;
        _moveLeftWithBody = true;
        _rotateLeftWithBody = true;
    }
}
