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
        _speed = EnemyConfig.Instance.Mox.Base.Speed;
        _health = EnemyConfig.Instance.Mox.Base.MaxHealth;
        _maxHealth = EnemyConfig.Instance.Mox.Base.MaxHealth;
        _timeBetween = 5.0;
        _timeCurrent = _timeBetween;
        _startPos = transform.position;
        _wanderRadius = EnemyConfig.Instance.Mox.Base.WanderRadius;
        _hostileRadius = EnemyConfig.Instance.Mox.Base.HostileRadius;
        _passiveRadius = EnemyConfig.Instance.Mox.Base.PassiveRadius;
        _maxRadius = EnemyConfig.Instance.Mox.Base.MaxRadius;
        _specialCooldown = new float[1] { 5.0f };
        _activeStates = new bool[1] { false };
        _animParm = new int[3] {
                    Animator.StringToHash("die"),
                    Animator.StringToHash("velocity"),
                    Animator.StringToHash("swimSpeed")};
        _playerCollision = false;
        _isRaming = false;
        _ramingDamage = 30;
        _pushMult = EnemyConfig.Instance.Mox.Base.PushMult;
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
