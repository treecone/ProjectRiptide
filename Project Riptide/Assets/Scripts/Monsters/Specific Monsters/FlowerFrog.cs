﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FrogAnim { Attack = 2, Close = 3};
public enum FlowerFrogAttackState { Latched = 1 }

public partial class FlowerFrog : Enemy
{
    [SerializeField]
    protected LineRenderer _tounge;
    [SerializeField]
    protected bool _isPoison;

    protected const float LATCH_DAMAGE_CAP = 15;
    protected const float MAX_DRAG_DIST = 15.0f;
    protected const float MAX_LATCH_DIST = 25.0f;
    protected float _latchStartHealth = 0;
    protected StatusEffects _playerStatusEffects;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        //Set parameters
        _enemyType = EnemyType.FlowerFrog;
        _speed = EnemyConfig.Instance.FlowerFrog.Base.Speed;
        _health = EnemyConfig.Instance.FlowerFrog.Base.MaxHealth;
        _maxHealth = EnemyConfig.Instance.FlowerFrog.Base.MaxHealth;
        _timeBetween = 5.0;
        _timeCurrent = _timeBetween;
        _startPos = transform.position;
        _wanderRadius = EnemyConfig.Instance.FlowerFrog.Base.WanderRadius;
        _hostileRadius = EnemyConfig.Instance.FlowerFrog.Base.HostileRadius;
        _passiveRadius = EnemyConfig.Instance.FlowerFrog.Base.PassiveRadius;
        _maxRadius = EnemyConfig.Instance.FlowerFrog.Base.MaxRadius;
        _specialCooldown = new float[2] { 0.0f, 0.0f};
        _activeStates = new bool[2] { false, false };
        _animParm = new int[4] {
                    Animator.StringToHash("die"),
                    Animator.StringToHash("velocity"),
                    Animator.StringToHash("attack"),
                    Animator.StringToHash("close")};
        _playerCollision = false;
        _isRaming = false;
        _ramingDamage = 20;
        _pushMult = EnemyConfig.Instance.FlowerFrog.Base.PushMult;
        _HostileAI = HostileFlowerFrog;
        _PassiveAI = PassiveReturnToRadius;

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
        _tounge.transform.rotation = Quaternion.identity;
        base.Update();
        if(IsDying)
        {
            _tounge.SetPosition(1, Vector3.zero);
        }
    }

    /// <summary>
    /// Called when frog latches onto player, apply slow effect
    /// </summary>
    /// <param name="other"></param>
    protected void OnToungeLatch(GameObject other)
    {
        if(other.tag == "Player")
        {
            _playerStatusEffects = other.GetComponent<StatusEffects>();
            _playerStatusEffects.AddStatus(StatusType.Speed, "ToungeLatch" + _enemyID, 999999.0f, EnemyConfig.Instance.FlowerFrog.ToungeLatch.SlowDownEffect);
            if(_isPoison)
            {
                _playerStatusEffects.AddStatus(StatusType.Poison, "ToungePoison" + _enemyID, 999999.0f, EnemyConfig.Instance.FlowerFrog.ToungeLatch.PoisonDamage);
            }
        }
    }

    /// <summary>
    /// Removes slow effect when tounge detaches
    /// </summary>
    protected void OnToungeDetach()
    {
        if(_playerStatusEffects != null)
        {
            _playerStatusEffects.RemoveStatus("ToungeLatch" + _enemyID);
            if(_isPoison)
            {
                _playerStatusEffects.RemoveStatus("ToungePoison" + _enemyID);
            }
        }
    }

    /// <summary>
    /// On passive reset tounge position
    /// </summary>
    protected override void OnPassive()
    {
        _tounge.SetPosition(1, Vector3.zero);
        if(_activeStates[(int)FlowerFrogAttackState.Latched])
        {
            OnToungeDetach();
        }
        base.OnPassive();
    }

    protected override void OnDeath()
    {
        if (_activeStates[(int)FlowerFrogAttackState.Latched])
        {
            OnToungeDetach();
        }
        base.OnDeath();
    }
}
