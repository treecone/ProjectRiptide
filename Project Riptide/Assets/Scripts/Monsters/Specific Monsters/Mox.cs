using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum MoxAnim { SwimSpeed = 2}

public partial class Mox : Enemy
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        //Set parameters
        _enemyType = EnemyType.Mox;
        _speed = 0.8f;
        _health = 70;
        _maxHealth = 70;
        _timeBetween = 5.0;
        _timeCurrent = _timeBetween;
        _startPos = transform.position;
        _wanderRadius = 45.0f;
        _hostileRadius = 20.0f;
        _passiveRadius = 50.0f;
        _maxRadius = 240.0f;
        _specialCooldown = new float[1] { 5.0f };
        _activeStates = new bool[1] { false };
        _animParm = new int[3] {
                    Animator.StringToHash("die"),
                    Animator.StringToHash("velocity"),
                    Animator.StringToHash("swimSpeed")};
        _playerCollision = false;
        _isRaming = false;
        _ramingDamage = 30;
        _pushMult = 1.0f;
        _HostileAI = HostileMox;
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
        base.Update();
    }
}
