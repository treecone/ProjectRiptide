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
        _speed = 0.8f;
        _health = 45;
        _maxHealth = 45;
        _timeBetween = 5.0;
        _timeCurrent = _timeBetween;
        _startPos = transform.position;
        _wanderRadius = 50.0f;
        _hostileRadius = 0.0f;
        _passiveRadius = 50.0f;
        _maxRadius = 140.0f;
        _specialCooldown = new float[1] { 5.0f };
        _activeStates = new bool[1] { false };
        _animParm = new int[3] {
                    Animator.StringToHash("die"),
                    Animator.StringToHash("velocity"),
                    Animator.StringToHash("swimSpeed")};
        _playerCollision = false;
        _pushMult = 1.0f;
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
        AgitateDeersInRadius(30.0f);
        base.OnHit();
    }

    protected override void OnDeath()
    {
        SpeedupDeersInRadius(40.0f);
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
                    foundEnemy.GetComponent<StatusEffects>().AddStatus(StatusType.MovementSpeed, 15.0f, 0.5f);
                }
            }
        }
    }
}
