using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenFishFlock : Enemy
{
    [SerializeField]
    protected List<ChickenFish> _chickenFlock;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        //Set parameters
        _enemyType = EnemyType.ChickenFlock;
        _speed = 0.7f;
        _health = 50;
        _maxHealth = 50;
        _timeBetween = 5.0;
        _timeCurrent = _timeBetween;
        _startPos = transform.position;
        _wanderRadius = 45.0f;
        _hostileRadius = 10.0f;
        _passiveRadius = 20.0f;
        _maxRadius = 100.0f;
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
        _pushMult = 1.0f;
        //_HostileAI = HostilePandateeRunAway;
        //_PassiveAI = PassivePandateeWander;

        //Setup health bar
        _healthBar.SetMaxHealth(_maxHealth);
        _healthBar.UpdateHealth(_health);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    /// <summary>
    /// Moves flock according to flocking algorithm
    /// </summary>
    protected void MoveFlock()
    {
        for(int i = 0; i < _chickenFlock.Count; i++)
        {
            //Find closest chicken to current chicken
            Vector3 closest = Vector3.zero;
            float dist = 99999;
            for(int j = 0; j < _chickenFlock.Count; i++)
            {
                if(j == i)
                {
                    continue;
                }

                //Check if current chicken is closer then found chicken
                float currDist = Vector3.SqrMagnitude(_chickenFlock[i].Position - _chickenFlock[j].Position);
                if(currDist < dist)
                {
                    dist = currDist;
                    closest = _chickenFlock[j].Position;
                }
            }

            _chickenFlock[i].Alignment(_velocity);
        }
    }
}
