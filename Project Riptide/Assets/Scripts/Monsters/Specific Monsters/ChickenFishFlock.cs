using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChickenFishAnim { Fly = 2, Swim = 3};

public partial class ChickenFishFlock : Enemy
{
    [SerializeField]
    protected List<ChickenFish> _chickenFlock;

    protected int _attackingChickenID = -1;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        //Set parameters
        _enemyType = EnemyType.ChickenFlock;
        _speed = EnemyConfig.Instance.ChickenFish.Base.Speed;
        _health = EnemyConfig.Instance.ChickenFish.Base.MaxHealth;
        _maxHealth = EnemyConfig.Instance.ChickenFish.Base.MaxHealth;
        _timeBetween = 5.0;
        _timeCurrent = _timeBetween;
        _startPos = transform.position;
        _wanderRadius = EnemyConfig.Instance.ChickenFish.Base.WanderRadius;
        _hostileRadius = EnemyConfig.Instance.ChickenFish.Base.HostileRadius;
        _passiveRadius = EnemyConfig.Instance.ChickenFish.Base.PassiveRadius;
        _maxRadius = EnemyConfig.Instance.ChickenFish.Base.MaxRadius;
        _specialCooldown = new float[2] { 5.0f, 0.0f };
        _activeStates = new bool[1] { false };
        _animParm = new int[4] {
                    Animator.StringToHash("die"),
                    Animator.StringToHash("velocity"),
                    Animator.StringToHash("fly"),
                    Animator.StringToHash("swim")};
        _playerCollision = false;
        _isRaming = false;
        _ramingDamage = 20;
        _pushMult = EnemyConfig.Instance.ChickenFish.Base.PushMult;
        _HostileAI = HostileChickenFish;
        _PassiveAI = PassiveWanderRadius;

        //Setup health bar
        _healthBar.SetMaxHealth(_maxHealth);
        _healthBar.UpdateHealth(_health);

        //Set up hitboxes
        foreach (Hitbox hitbox in GetComponentsInChildren<Hitbox>())
        {
            hitbox.OnTrigger += HitboxTriggered;
        }

        for (int i = 0; i < _chickenFlock.Count; i++)
        {
            _chickenFlock[i].MinY = transform.position.y - 1f;
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (!_dying && !_frozen)
        {
            MoveFlock();
        }
        base.Update();
    }

    /// <summary>
    /// Moves flock according to flocking algorithm
    /// </summary>
    protected void MoveFlock()
    {
        for(int i = 0; i < _chickenFlock.Count; i++)
        {
            if(i == _attackingChickenID)
            {
                continue;
            }

            if (_playerDistance > _maxRadius)
            {
                _chickenFlock[i].StopMotion();
                continue;
            }
            else
            {
                //Find closest chicken to current chicken
                Vector3 closest = Vector3.zero;
                float dist = 99999;
                for (int j = 0; j < _chickenFlock.Count; j++)
                {
                    if (j == i)
                    {
                        continue;
                    }

                    //Check if current chicken is closer then found chicken
                    float currDist = Vector3.SqrMagnitude(_chickenFlock[i].Position - _chickenFlock[j].Position);
                    if (currDist < dist)
                    {
                        dist = currDist;
                        closest = _chickenFlock[j].Position;
                    }
                }

                _chickenFlock[i].Alignment(_velocity);
                _chickenFlock[i].Cohesion(transform.position);
                _chickenFlock[i].Seperation(closest);
                _chickenFlock[i].MoveUpAndDown();
                _chickenFlock[i].FlockerAnimator.SetFloat(_animParm[(int)Anim.Velocity], _chickenFlock[i].Velocity.sqrMagnitude);
            }
        }
    }

    /// <summary>
    /// Kill chicken flock on death
    /// </summary>
    protected override void OnDeath()
    {
        //Kill all chickens in the flock
        for (int i = 0; i < _chickenFlock.Count; i++)
        {
            _chickenFlock[i].StopMotion();
            _chickenFlock[i].FlockerAnimator.SetTrigger(_animParm[(int)Anim.Die]);
        }
        base.OnDeath();
    }

    /// <summary>
    /// On Freeze, freeze all the children chickens
    /// </summary>
    protected override void OnFreeze()
    {
        for (int i = 0; i < _chickenFlock.Count; i++)
        {
            _chickenFlock[i].Frozen = true;
        }
        base.OnFreeze();
    }

    /// <summary>
    /// On unfreeze, unfreeze all the children chickens
    /// </summary>
    protected override void OnUnfreeze()
    {
        for (int i = 0; i < _chickenFlock.Count; i++)
        {
            _chickenFlock[i].Frozen = false;
        }
        base.OnUnfreeze();
    }
}
