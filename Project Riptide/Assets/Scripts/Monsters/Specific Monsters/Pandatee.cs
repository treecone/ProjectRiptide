using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PandateeAnim { Situp = 2, Dive = 3};
public enum PandateePassiveState { Eat = 1, Underwater = 3 };

public partial class Pandatee : Enemy
{
    protected bool _wentUnderwater = false;
    protected bool _isUnderwater = false;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        //Set parameters
        _enemyType = EnemyType.Pandatee;
        _speed = EnemyConfig.Instance.Pandatee.Base.Speed;
        _health = EnemyConfig.Instance.Pandatee.Base.MaxHealth;
        _maxHealth = EnemyConfig.Instance.Pandatee.Base.MaxHealth;
        _timeBetween = 5.0;
        _timeCurrent = _timeBetween;
        _startPos = transform.position;
        _wanderRadius = EnemyConfig.Instance.Pandatee.Base.WanderRadius;
        _hostileRadius = EnemyConfig.Instance.Pandatee.Base.HostileRadius;
        _passiveRadius = EnemyConfig.Instance.Pandatee.Base.PassiveRadius;
        _maxRadius = EnemyConfig.Instance.Pandatee.Base.MaxRadius;
        _specialCooldown = new float[4] { 5.0f, 0.0f, 0.0f, 0.0f };
        _activeStates = new bool[4] { false, false, false, false };
        _animParm = new int[4] {
                    Animator.StringToHash("die"),
                    Animator.StringToHash("velocity"),
                    Animator.StringToHash("situp"),
                    Animator.StringToHash("dive")};
        _playerCollision = false;
        _isRaming = false;
        _ramingDamage = 20;
        _pushMult = EnemyConfig.Instance.Pandatee.Base.PushMult;
        _HostileAI = HostilePandateeRunAway;
        _PassiveAI = PassivePandateeWander;

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

    protected override void OnHostile()
    {
        ResetHostile();
        base.OnHostile();
    }

    protected override void OnPassive()
    {
        base.OnPassive();
        _passiveCooldown = 1.0f;
    }
}
