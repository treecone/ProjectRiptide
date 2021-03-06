﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CrabAnim { Jump = 2, Open = 3, Close = 4 };

public partial class RockCrab : Enemy
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        //Set parameters
        _enemyType = EnemyType.RockCrab;
        _speed = EnemyConfig.Instance.RockCrab.Base.Speed;
        _health = EnemyConfig.Instance.RockCrab.Base.MaxHealth;
        _maxHealth = EnemyConfig.Instance.RockCrab.Base.MaxHealth;
        _timeBetween = 5.0;
        _timeCurrent = _timeBetween;
        _startPos = transform.position;
        _wanderRadius = EnemyConfig.Instance.RockCrab.Base.WanderRadius;
        _hostileRadius = EnemyConfig.Instance.RockCrab.Base.HostileRadius;
        _passiveRadius = EnemyConfig.Instance.RockCrab.Base.PassiveRadius;
        _maxRadius = EnemyConfig.Instance.RockCrab.Base.MaxRadius;
        _specialCooldown = new float[1] { 5.0f };
        _activeStates = new bool[3] { false, false, false };
        _animParm = new int[5] {
                    Animator.StringToHash("die"),
                    Animator.StringToHash("velocity"),
                    Animator.StringToHash("jump"),
                    Animator.StringToHash("open"),
                    Animator.StringToHash("close")};
        _playerCollision = false;
        _isRaming = false;
        _ramingDamage = 20;
        _pushMult = EnemyConfig.Instance.RockCrab.Base.PushMult;
        _HostileAI = HostileRockCrab;
        _PassiveAI = PassiveRockCrab;

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
}
