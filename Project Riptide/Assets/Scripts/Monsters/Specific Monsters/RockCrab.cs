using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CrabAnim { Jump = 2, Open = 3, Close = 4 };

public partial class RockCrab : Enemy
{
    #region Config Constants
    private const float ROCKCRAB_FLING_MAX_DISTANCE = 20.0f;
    private const float ROCKCRAB_FLING_COOLDOWN = 5.0f;
    private const float ROCKCRAB_FLING_CHARGE_TIME = 1.0f;
    private const float ROCKCRAB_FLING_DISTANCE = 15.0f;
    #endregion

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        //Set parameters
        _enemyType = EnemyType.RockCrab;
        _speed = 1.0f;
        _health = 50;
        _maxHealth = 50;
        _timeBetween = 5.0;
        _timeCurrent = _timeBetween;
        _startPos = transform.position;
        _wanderRadius = 45.0f;
        _hostileRadius = 10.0f;
        _passiveRadius = 50.0f;
        _maxRadius = 240.0f;
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
