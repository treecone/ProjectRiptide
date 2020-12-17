using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum WaterdeerAnim { SwimSpeed = 2 }

public partial class Waterdeer : Enemy
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        //Set parameters
        _enemyType = EnemyType.Waterdeer;
        _speed = EnemyConfig.Instance.Waterdeer.Base.Speed;
        _health = EnemyConfig.Instance.Waterdeer.Base.MaxHealth;
        _maxHealth = EnemyConfig.Instance.Waterdeer.Base.MaxHealth;
        _timeBetween = 5.0;
        _timeCurrent = _timeBetween;
        _startPos = transform.position;
        _wanderRadius = EnemyConfig.Instance.Waterdeer.Base.WanderRadius;
        _hostileRadius = EnemyConfig.Instance.Waterdeer.Base.HostileRadius;
        _passiveRadius = EnemyConfig.Instance.Waterdeer.Base.PassiveRadius;
        _maxRadius = EnemyConfig.Instance.Waterdeer.Base.MaxRadius;
        _specialCooldown = new float[1] { 5.0f };
        _activeStates = new bool[1] { false };
        _animParm = new int[3] {
                    Animator.StringToHash("die"),
                    Animator.StringToHash("velocity"),
                    Animator.StringToHash("swimSpeed")};
        _playerCollision = false;
        _pushMult = EnemyConfig.Instance.Waterdeer.Base.PushMult;
        _isRaming = false;
        _ramingDamage = 20;
        _HostileAI = HostileWaterdeer;
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

    /// <summary>
    /// On hit, make other deer around hostile as well
    /// </summary>
    protected override void OnHit()
    {
        AgitateDeersInRadius(EnemyConfig.Instance.Waterdeer.HoardBuff.AgitateDistance);
        base.OnHit();
    }

    protected override void OnDeath()
    {
        SpeedupDeersInRadius(EnemyConfig.Instance.Waterdeer.HoardBuff.SpeedupDistance);
        base.OnDeath();
    }

    /// <summary>
    ///Turn deers in radius hostile
    /// </summary>
    /// <param name="radius">Radius to check for deer in</param>
    protected void AgitateDeersInRadius(float radius)
    {
        foreach (Collider collider in UnityEngine.Physics.OverlapSphere(transform.position, radius))
        {
            if (collider.transform.gameObject.tag == "Hitbox")
            {
                //Check to see if stingray can do cross zap attack
                Hitbox hitbox = collider.GetComponent<Hitbox>();
                if (hitbox.Type == HitboxType.EnemyHurtbox)
                {
                    Waterdeer foundEnemy = hitbox.AttachedObject.GetComponent<Waterdeer>();
                    if (foundEnemy != null && foundEnemy.gameObject != gameObject && foundEnemy.State == EnemyState.Passive)
                    {
                        foundEnemy.TriggerHostile();
                    }
                }
            }
        }
    }

    /// <summary>
    /// Give speed boost to deers in a radius
    /// </summary>
    /// <param name="radius">Radius to check for deer in</param>
    protected void SpeedupDeersInRadius(float radius)
    {
        foreach (Collider collider in UnityEngine.Physics.OverlapSphere(transform.position, radius))
        {
            if (collider.transform.gameObject.tag == "Hitbox")
            {
                //Check to see if stingray can do cross zap attack
                Waterdeer foundEnemy = collider.GetComponent<Hitbox>().AttachedObject.GetComponent<Waterdeer>();
                if (foundEnemy != null && foundEnemy.gameObject != gameObject)
                {
                    if (!foundEnemy.GetComponent<StatusEffects>().CheckStatus("DeerSpeedup"))
                    {
                        foundEnemy.GetComponent<StatusEffects>().AddStatus(StatusType.Speed, "DeerSpeedup", EnemyConfig.Instance.Waterdeer.HoardBuff.SpeedBuffDuration, EnemyConfig.Instance.Waterdeer.HoardBuff.SpeedBuffLevel);
                    }
                }
            }
        }
    }
}
