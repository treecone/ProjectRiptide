/*This script contains actions used by various enemy classes
 * uses partial classes to declare actions
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Actions available to all enemys
public partial class Enemy : Physics
{
    //General method for making the moster follow the player
    //Usually should be used for enemy AI when not in an action
    protected void FollowPlayer()
    {
        Vector3 destination = Vector3.zero;
        //Check for obstacle
        if (CheckObstacle(new Vector3(PlayerPosition().x, transform.position.y, PlayerPosition().z)))
        {
            //Set destination to closest way to player that avoids obstacles
            destination = transform.position + AvoidObstacle(new Vector3(PlayerPosition().x, transform.position.y, PlayerPosition().z));
        }
        else
        {
            //Set destination to player
            destination = new Vector3(PlayerPosition().x, transform.position.y, PlayerPosition().z);
        }
        //Seek destination
        Vector3 netForce = Seek(destination) * (1 + _enemyUpgrades.masterUpgrade[StatusType.Speed]);
        netForce += new Vector3(transform.forward.x, 0, transform.forward.z).normalized * 2.0f * (1 + _enemyUpgrades.masterUpgrade[StatusType.Speed]);

        //Rotate in towards direction of velocity
        if (_velocity != Vector3.zero)
        {
            Quaternion desiredRotation = Quaternion.LookRotation(_velocity);
            SetSmoothRotation(desiredRotation, 1.0f, 0.5f, 2.0f);
            //rotation = Quaternion.RotateTowards(rotation, Quaternion.LookRotation(velocity), 4.0f);
        }
        //Debug.DrawLine(transform.position, transform.position + netForce, Color.red);
        ApplyForce(netForce);
    }

    //General method for making the moster flee a point
    //Usually should be used for enemy AI when not in an action
    protected void FleePoint(Vector3 point, float speed)
    {
        Vector3 destination = Vector3.zero;
        Vector3 avoidDirection = (transform.position - point) + transform.position;
        //Check for obstacle
        if (CheckObstacle(new Vector3(avoidDirection.x, transform.position.y, avoidDirection.z)))
        {
            //Set destination to closest way to player that avoids obstacles
            destination = transform.position + AvoidObstacle(new Vector3(avoidDirection.x, transform.position.y, avoidDirection.z));
        }
        else
        {
            //Set destination to player
            destination = new Vector3(avoidDirection.x, transform.position.y, avoidDirection.z);
        }
        //Seek destination
        Vector3 netForce = Seek(destination) * (1 + _enemyUpgrades.masterUpgrade[StatusType.Speed]);
        netForce += new Vector3(transform.forward.x, 0, transform.forward.z).normalized * 2.0f * (1 + _enemyUpgrades.masterUpgrade[StatusType.Speed]);

        //Rotate in towards direction of velocity
        if (_velocity != Vector3.zero)
        {
            Quaternion desiredRotation = Quaternion.LookRotation(_velocity);
            SetSmoothRotation(desiredRotation, 1.0f, 0.5f, 2.0f);
            //rotation = Quaternion.RotateTowards(rotation, Quaternion.LookRotation(velocity), 4.0f);
        }
        //Debug.DrawLine(transform.position, transform.position + netForce, Color.red);
        ApplyForce(netForce * speed);
    }

    /// <summary>
    /// General method for making the monster stay facing the player
    /// </summary>
    protected void LookAtPlayer()
    {
        _destination = new Vector3(PlayerPosition().x, transform.position.y, PlayerPosition().z);
        Quaternion desiredRotation = Quaternion.LookRotation(_destination - transform.position);
        SetSmoothRotation(desiredRotation, 1.0f, 0.5f, 3.0f);
    }

    /// <summary>
    /// Go through enemy's action queue and play each action
    /// Returns false when finished
    /// </summary>
    /// <returns></returns>
    protected bool DoActionQueue()
    {
        //Go through enmeies action queue
        if (_actionQueue.Peek()(ref _currTime))
        {
            _currTime += Time.deltaTime;
        }
        //If action is finished, go to next action
        //or end attack sequence
        else
        {
            _actionQueue.Dequeue();

            if (_actionQueue.Count != 0)
            {
                _currTime = 0;
            }
            else
            {
                _currTime = 0;
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Charges dash attack for 2 seconds
    /// </summary>
    /// <param name="time">Current time</param>
    /// <returns></returns>
    protected bool DashCharge(ref float time)
    {
        const float MAX_TIME = 2.0f;

        //Stop motion before charge
        if (time == 0)
        {
            StopMotion();
        }

        //Look towards player
        _destination = new Vector3(PlayerPosition().x, transform.position.y, PlayerPosition().z);
        _rotation = Quaternion.RotateTowards(_rotation, Quaternion.LookRotation(_destination - transform.position), 1.0f);

        if (time >= MAX_TIME)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Dashes for 1 second
    /// </summary>
    /// <param name="time">Current Time</param>
    /// <returns></returns>
    protected bool DashAttack(ref float time)
    {
        const float MAX_TIME = 1.0f;

        //Add hitbox at begining
        if (time == 0.0f)
        {
            _hitboxes.Add(CreateHitbox(Vector3.forward * 3.0f, new Vector3(1, 1, 1), HitboxType.EnemyHitbox, _ramingDamage));
        }

        if (!_inKnockback)
        {
            //If monster hits player, stop special
            if (_playerCollision || _obsticalCollision)
            {
                _inKnockback = true;
                if(_playerCollision)
                {
                    Vector3 knockback = PlayerPosition() - transform.position;
                    knockback.Normalize();
                    knockback *= 40.0f;
                    SendKnockback(knockback);
                }
                time = 0.7f;
            }

            //Move forwards
            ApplyConstantMoveForce(transform.forward, 20.0f * _speed, 1.0f);
        }
        //Do knockback if there was a hit
        else
        {
            //Move backwards
            ApplyConstantMoveForce(-transform.forward, 10.0f * _speed, 0.3f);
        }

        if (time >= MAX_TIME)
        {
            GameObject.Destroy(_hitboxes[_hitboxes.Count - 1]);
            _hitboxes.RemoveAt(_hitboxes.Count - 1);
            _inKnockback = false;
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Clears telegraphs from enemy as an action
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected bool ClearTelegraphsAction(ref float time)
    {
        ClearTelegraphs();
        return false;
    }
}

//Actions available to the koi boss
public partial class KoiBoss : Enemy
{
    /// <summary>
    /// Transition to charge attack by slowing down
    /// </summary>
    /// <param name="time">Current time</param>
    /// <returns></returns>
    protected bool KoiStopTransition(ref float time)
    {
        const float MAX_TIME = 0.25f;

        ApplyFriction(0.5f);

        if (time >= MAX_TIME)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Used for KoiBoss, Charges a dash for 1 second
    /// </summary>
    /// <param name="time">Current time</param>
    /// <returns></returns>
    protected bool KoiDashCharge(ref float time)
    {
        float MAX_TIME = EnemyConfig.Instance.KoiBoss.DashAttack.ChargeTime;
        float STALL_TIME = EnemyConfig.Instance.KoiBoss.DashAttack.StallTime;

        //Stop motion at begining of charge
        if (time == 0)
        {
            StopMotion();
            if (DoTelegraphs())
            {
                CreateTelegraph(new Vector3(0, _detectPosition.localPosition.y - 0.5f, (_lengthMult + 20f) / transform.localScale.z), new Vector3(_widthMult, 1, 32.0f / transform.localScale.z), Quaternion.identity, TelegraphType.Square, true);
            }
        }

        if (time <= MAX_TIME - STALL_TIME)
        {
            //Look towards player
            _destination = new Vector3(PlayerPosition().x, transform.position.y, PlayerPosition().z);
            Quaternion desiredRotation = Quaternion.LookRotation(_destination - transform.position);
            SetSmoothRotation(desiredRotation, 1.5f, 1.0f, 4.0f);
            //rotation = Quaternion.RotateTowards(rotation, Quaternion.LookRotation(destination - transform.position), 1.0f);
        }

        if (time >= MAX_TIME)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Used for KoiBoss, Dashes at player for 1 second
    /// </summary>
    /// <param name="time">Current Time</param>
    /// <returns></returns>
    protected bool KoiDashAttack(ref float time)
    {
        const float MAX_TIME = 1.0f;

        //Add hitbox and start dash
        if (time == 0.0f)
        {
            _hitboxes.Add(CreateHitbox(Vector3.forward * 2.2f + Vector3.up * 0.2f, new Vector3(1, 1, 1), HitboxType.EnemyHitbox, EnemyConfig.Instance.KoiBoss.DashAttack.Distance, Vector2.zero, 2000));
            ApplyMoveForce(transform.forward, EnemyConfig.Instance.KoiBoss.DashAttack.Distance, 1.0f);
            _animator.SetFloat(_animParm[(int)CarpAnim.SwimSpeed], 2.0f);
            if(DoTelegraphs())
            {
                _telegraphs[0].transform.parent = null;
                ClearTelegraphs();
            }
            _dashParticles.Play();
        }

        if (!_inKnockback)
        {
            if (_playerCollision || _obsticalCollision)
            {
                //Go into knockback
                StopMotion();
                _inKnockback = true;
                time = 0.7f;
                ApplyMoveForce(-transform.forward, 2.0f * _speed, 0.3f);
            }
            //ApplyFriction(0.25f);
        }
        //Do knockback if there was a hit
        else
        {
            //Do nothing
        }

        _animator.SetFloat(_animParm[(int)Anim.Velocity], _velocity.sqrMagnitude);

        if (time >= MAX_TIME)
        {
            GameObject.Destroy(_hitboxes[_hitboxes.Count - 1]);
            _hitboxes.RemoveAt(_hitboxes.Count - 1);
            _inKnockback = false;
            StopMotion();
            _animator.SetFloat(_animParm[(int)CarpAnim.SwimSpeed], 1.0f);
            _dashParticles.Stop();
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Used for KoiBoss, charges bubble blast attack for 2 seconds
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected bool KoiBubbleBlastTransitionDown(ref float time)
    {
        float MAX_TIME = EnemyConfig.Instance.KoiBoss.BubbleBlast.TransitionDownTime;

        if (time == 0)
        {
            StopMotion();
            _animator.SetTrigger(_animParm[(int)CarpAnim.Dive]);
            PlaySplash();
            SpawnBubbleBroth();
            _animator.SetFloat(_animParm[(int)Anim.Velocity], 0);
        }

        ApplyConstantMoveForce(Vector3.down, 3.0f * transform.localScale.y, MAX_TIME);

        if (time >= MAX_TIME)
        {
            _animator.ResetTrigger(_animParm[(int)CarpAnim.Dive]);
            StopMotion();
            return false;
        }

        return true;
    }

    /// <summary>
    /// Used for KoiBoss, move back up after bubble blast attack
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected bool KoiBubbleBlastTransitionUp(ref float time)
    {
        float MAX_TIME = EnemyConfig.Instance.KoiBoss.BubbleBlast.TransitionUpTime;

        if (time == 0)
        {
            StopMotion();
            _animator.SetFloat(_animParm[(int)Anim.Velocity], 0);
            _animator.SetTrigger(_animParm[(int)CarpAnim.Shoot]);
        }

        ApplyConstantMoveForce(Vector3.up, 3.0f * transform.localScale.y, MAX_TIME);

        if (time >= MAX_TIME)
        {
            ReturnToInitalPosition();
            _animator.ResetTrigger(_animParm[(int)CarpAnim.Shoot]);
            StopMotion();
            return false;
        }

        return true;
    }

    /// <summary>
    /// Used for KoiBoss, charges bubble blast attack for 2 seconds
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected bool KoiBubbleBlastCharge(ref float time)
    {
        float MAX_TIME = EnemyConfig.Instance.KoiBoss.BubbleBlast.ChargeTime;
        float STALL_TIME = EnemyConfig.Instance.KoiBoss.BubbleBlast.StallTime;

        if (time < MAX_TIME - STALL_TIME)
        {
            //Stop motion at start
            if (time == 0)
            {
                StopMotion();
                if(DoTelegraphs())
                {
                    CreateTelegraph(new Vector3(0, 0, (_lengthMult + 30f) / transform.localScale.z), new Vector3(2.0f, 1, 62.0f / transform.localScale.z), Quaternion.identity, TelegraphType.Square, true);
                }
            }

            //Look towards player
            _destination = new Vector3(PlayerPosition().x, transform.position.y, PlayerPosition().z);
            Quaternion desiredRotation = Quaternion.LookRotation(_destination - transform.position);
            SetSmoothRotation(desiredRotation, 1.0f, 0.5f, 3.0f);
            //rotation = Quaternion.RotateTowards(rotation, Quaternion.LookRotation(destination - transform.position), 1.0f);
            ApplyFriction(0.99f);
        }

        if (time >= MAX_TIME)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Used for KoiBoss, sends out 7 projectiles in an arc
    /// </summary>
    /// <param name="time">Current time</param>
    /// <returns></returns>
    protected bool KoiBubbleBlastAttack(ref float time)
    {
        if(DoTelegraphs())
        {
            _telegraphs[0].transform.parent = null;
            ClearTelegraphs();
        }
        //Spawn projectiles
        SpawnProjectile(new Vector3(0, 0, (5 * _lengthMult / 6)), new Vector3(1, 1, 1), 0.5f, EnemyConfig.Instance.KoiBoss.BubbleBlast.Damage, 1.0f, MovementPattern.Forward, Vector2.zero, 200);
        SpawnProjectile(new Vector3(-0.10f, 0, (5 * _lengthMult / 6) - 0.25f), new Vector3(1, 1, 1), 0.5f, EnemyConfig.Instance.KoiBoss.BubbleBlast.Damage, 1.0f, MovementPattern.Forward, Vector2.zero, 200);
        SpawnProjectile(new Vector3(-0.25f, 0, (5 * _lengthMult / 6) - 0.75f), new Vector3(1, 1, 1), 0.5f, EnemyConfig.Instance.KoiBoss.BubbleBlast.Damage, 1.0f, MovementPattern.Forward, Vector2.zero, 200);
        SpawnProjectile(new Vector3(-0.50f, 0, (5 * _lengthMult / 6) - 1.50f), new Vector3(1, 1, 1), 0.5f, EnemyConfig.Instance.KoiBoss.BubbleBlast.Damage, 1.0f, MovementPattern.Forward, Vector2.zero, 200);
        SpawnProjectile(new Vector3(0.10f, 0, (5 * _lengthMult / 6) - 0.25f), new Vector3(1, 1, 1), 0.5f, EnemyConfig.Instance.KoiBoss.BubbleBlast.Damage, 1.0f, MovementPattern.Forward, Vector2.zero, 200);
        SpawnProjectile(new Vector3(0.25f, 0, (5 * _lengthMult / 6) - 0.75f), new Vector3(1, 1, 1), 0.5f, EnemyConfig.Instance.KoiBoss.BubbleBlast.Damage, 1.0f, MovementPattern.Forward, Vector2.zero, 200);
        SpawnProjectile(new Vector3(0.50f, 0, (5 * _lengthMult / 6) - 1.50f), new Vector3(1, 1, 1), 0.5f, EnemyConfig.Instance.KoiBoss.BubbleBlast.Damage, 1.0f, MovementPattern.Forward, Vector2.zero, 200);

        return false;
    }

    protected bool KoiBubbleBlastBig(ref float time)
    {
        //Spawn projectiles
        SpawnProjectile(new Vector3(0, 0, _lengthMult / 3f), new Vector3(4, 4, 4), 0.4f, EnemyConfig.Instance.KoiBoss.BubbleBlast.Damage, 1.2f, MovementPattern.Forward, Vector2.zero, 2000);

        return false;
    }

    protected bool KoiBubbleBlastBigCharge(ref float time)
    {
        float MAX_TIME = EnemyConfig.Instance.KoiBoss.BubbleBlast.RechargeTime;
        float STALL_TIME = EnemyConfig.Instance.KoiBoss.BubbleBlast.ChargeTime;

        if (time < MAX_TIME - STALL_TIME)
        {
            //Look towards player
            _destination = new Vector3(PlayerPosition().x, transform.position.y, PlayerPosition().z);
            Quaternion desiredRotation = Quaternion.LookRotation(_destination - transform.position);
            SetSmoothRotation(desiredRotation, 1.0f, 0.5f, 3.0f);
            //rotation = Quaternion.RotateTowards(rotation, Quaternion.LookRotation(destination - transform.position), 1.0f);
            ApplyFriction(0.99f);
        }

        if (time >= MAX_TIME)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Used for KoiBoss, sends out one projectile towards player
    /// </summary>
    /// <param name="time">Current time</param>
    /// <returns></returns>
    protected bool KoiBubbleAttack(ref float time)
    {
        SpawnProjectile(new Vector3(0, 0, 5 * _lengthMult / 6), new Vector3(1,1,1), 1.0f, EnemyConfig.Instance.KoiBoss.BubbleAttack.Damage, 3.0f, MovementPattern.Forward, Vector2.zero, 200);
        CreateTelegraph(new Vector3(0, 0, (_lengthMult + 30) / transform.localScale.z), new Vector3(0.5f, 1, 62f / transform.localScale.z), Quaternion.identity, TelegraphType.Square, false);

        if(time != 0)
        {
            ClearTelegraphs();
            return false;
        }
        return true;
    }

    /// <summary>
    /// Used for KoiBoss, Koi dashes three times at player going above and bellow the water
    /// </summary>
    /// <param name="time">Current time</param>
    /// <returns></returns>
    protected bool KoiUnderwaterDash(ref float time)
    {
        const float MAX_TIME = 1.0f;

        //Start dashing
        if (time == 0.0f)
        {
            _hitboxes.Add(CreateHitbox(Vector3.forward * 2.2f + Vector3.up * 0.2f, new Vector3(1, 1, 1), HitboxType.EnemyHitbox, EnemyConfig.Instance.KoiBoss.DashAttack.Damage, Vector2.zero, 2000));
            _gravity = ApplyArcForce(transform.forward, EnemyConfig.Instance.KoiBoss.DashAttack.Distance, 2f * transform.localScale.y, 1.0f);
            if (DoTelegraphs())
            {
                _telegraphs[0].transform.parent = null;
                ClearTelegraphs();
            }
            _dashParticles.Play();
        }

        if (!_inKnockback)
        {
            //If monster hits player, stop special
            if (_playerCollision || _obsticalCollision)
            {
                _inKnockback = true;
                _velocity.x = 0;
                _velocity.z = 0;
                ApplyMoveForce(-transform.forward, 2.0f * _speed, 0.3f);
            }
            ApplyForce(_gravity);
        }
        //Do knockback if there was a hit
        else
        {
            ApplyForce(_gravity);

            //Stop moving fish if it goes below its original height
            if (transform.position.y <= _initalPos)
            {
                time = MAX_TIME;
            }
        }

        _animator.SetFloat(_animParm[(int)Anim.Velocity], _velocity.sqrMagnitude);

        if (time >= MAX_TIME)
        {
            StopMotion();
            GameObject.Destroy(_hitboxes[_hitboxes.Count - 1]);
            _hitboxes.RemoveAt(_hitboxes.Count - 1);
            ReturnToInitalPosition();
            _inKnockback = false;
            _dashParticles.Stop();
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Used for KoiBoss, Koi comes above water for player to get a chance
    /// to attack, then goes back under.
    /// </summary>
    /// <param name="time">Current Time</param>
    /// <returns></returns>
    protected bool KoiUnderwaterDashReturn(ref float time)
    {
        float MAX_TIME = EnemyConfig.Instance.KoiBoss.UnderwaterDash.VulnerabilityTime + 1.0f;
        float STALL_TIME = EnemyConfig.Instance.KoiBoss.UnderwaterDash.VulnerabilityTime;

        //Move fish above water
        if (time < MAX_TIME - STALL_TIME)
        {
            ApplyConstantMoveForce(Vector3.up, 1.0f * transform.localScale.y, 1.0f);
        }
        else if (time < MAX_TIME)
        {
            StopMotion();
            //Do nothing, give player chance to attack
        }
        else if (time < MAX_TIME + 0.5f)
        {
            _animator.SetTrigger(_animParm[(int)CarpAnim.Dive]);
            PlaySplash();
            SpawnBubbleBroth();
            time = MAX_TIME + 0.5f;
        }
        //Move fish back underwater
        else if (transform.position.y >= _initalPos)
        {
            ApplyConstantMoveForce(Vector3.down, 1.0f * transform.localScale.y, 1.0f);
        }
        else
        {
            StopMotion();
            ReturnToInitalPosition();
            return false;
        }

        return true;
    }

    /// <summary>
    /// Used for KoiBoss, moves Koi above water to prepare for 
    /// bubble blast attack
    /// </summary>
    /// <param name="time">Current Time</param>
    /// <returns></returns>
    protected bool KoiBubbleBlastUnderwaterCharge(ref float time)
    {
        float MAX_TIME = EnemyConfig.Instance.KoiBoss.BubbleBlast.ChargeTime;

        if (time == 0)
        {
            StopMotion();
            _animator.SetFloat(_animParm[(int)Anim.Velocity], 0);
            _animator.SetTrigger(_animParm[(int)CarpAnim.Shoot]);
        }

        //Move fish out of water
        ApplyConstantMoveForce(Vector3.up, 1.0f * transform.localScale.y, MAX_TIME);

        if (time >= MAX_TIME)
        {
            _position = new Vector3(_position.x, _startPos.y, _position.z);
            StopMotion();
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Used for KoiBoss, returns koi underwater after bubble blast attack
    /// Also gives player a chance to attack for 1.5 seconds
    /// </summary>
    /// <param name="time">Current time</param>
    /// <returns></returns>
    protected bool KoiBubbleBlastReturn(ref float time)
    {
        float MAX_TIME = EnemyConfig.Instance.KoiBoss.UnderwaterBubbleBlast.VulnerabilityTime + 0.5f;
        float STALL_TIME = EnemyConfig.Instance.KoiBoss.UnderwaterBubbleBlast.VulnerabilityTime;

        if (time < STALL_TIME)
        {
            //Do nothing, give player chance to attack
        }
        else if (time >= STALL_TIME && time < MAX_TIME)
        {
            _animator.SetTrigger(_animParm[(int)CarpAnim.Dive]);
            PlaySplash();
            SpawnBubbleBroth();
            time = MAX_TIME;
        }
        else if (transform.position.y > _initalPos)
        {
            //Move fish back down
            ApplyConstantMoveForce(Vector3.down, 1.0f * transform.localScale.y, 1.0f);
        }
        else
        {
            StopMotion();
            ReturnToInitalPosition();
            return false;
        }

        return true;
    }

    /// <summary>
    /// Used for KoiBoss, koi follows player underwater, tries to get
    /// beneath them.
    /// </summary>
    /// <param name="time">Current time</param>
    /// <returns></returns>
    protected bool KoiUnderwaterFollow(ref float time)
    {
        float MAX_TIME = EnemyConfig.Instance.KoiBoss.UnderwaterAttack.FollowTime + EnemyConfig.Instance.KoiBoss.UnderwaterAttack.StallTime;
        float STALL_TIME = EnemyConfig.Instance.KoiBoss.UnderwaterAttack.StallTime;

        if (time == 0)
        {
            _maxSpeed += EnemyConfig.Instance.KoiBoss.UnderwaterAttack.FollowSpeed;
            if (DoTelegraphs())
            {
                CreateTelegraph(new Vector3(0, _detectPosition.localPosition.y - 0.1f, 0), new Vector3(_widthMult, 1, _lengthMult), Quaternion.identity, TelegraphType.Circle, true);
            }
        }

        if (time < MAX_TIME - STALL_TIME)
        {
            Vector3 destination = Vector3.zero;
            if (CheckObstacle(new Vector3(PlayerPosition().x, transform.position.y, PlayerPosition().z)))
            {
                destination = transform.position + AvoidObstacle(new Vector3(PlayerPosition().x, transform.position.y, PlayerPosition().z));
            }
            else
            {
                destination = new Vector3(PlayerPosition().x, transform.position.y, PlayerPosition().z);
            }
            Vector3 netForce = Seek(destination) * (1 + _enemyUpgrades.masterUpgrade[StatusType.Speed]);
            netForce += new Vector3(transform.forward.x, 0, transform.forward.z).normalized * 2.0f * (1 + _enemyUpgrades.masterUpgrade[StatusType.Speed]);

            //Rotate in towards direction of velocity
            if (_velocity != Vector3.zero)
            {
                Quaternion desiredRotation = Quaternion.LookRotation(_velocity);
                SetSmoothRotation(desiredRotation, 1.0f, 0.5f, 2.0f);
                //rotation = Quaternion.RotateTowards(rotation, Quaternion.LookRotation(velocity), 4.0f);
            }
            //Debug.DrawLine(transform.position, transform.position + netForce, Color.red);
            ApplyForce(netForce * 2.0f);

            if (_playerDistance <= 3.1f)
            {
                StopMotion();
                time = MAX_TIME - STALL_TIME;
            }
        }

        _animator.SetFloat(_animParm[(int)Anim.Velocity], _velocity.sqrMagnitude);

        if (time >= MAX_TIME)
        {
            _maxSpeed -= 5.0f;
            StopMotion();
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Used for KoiBoss, Koi jumps out of water to attack player
    /// If they miss, player gets a chance to attack for 3 seconds
    /// If they hit, Koi goes straight back underwater
    /// </summary>
    /// <param name="time">Current Time</param>
    /// <returns></returns>
    protected bool KoiUnderwaterAttack(ref float time)
    {
        //This one's too complicated to add constant times to work with

        if (time == 0.0f)
        {
            _hitboxes.Add(CreateHitbox(Vector3.zero, new Vector3(1.5f, 1.5f, 5.25f), HitboxType.EnemyHitbox, EnemyConfig.Instance.KoiBoss.UnderwaterAttack.Damage, new Vector2(90, 0), 3000));
            _gravity = ApplyArcForce(Vector3.up, 0.0f, 15.0f, 1.0f);
            _animator.SetTrigger(_animParm[(int)CarpAnim.UAttack]);
            if (DoTelegraphs())
            {
                _telegraphs[0].transform.parent = null;
                ClearTelegraphs();
            }
        }

        if (time <= 0.95f)
        {
            if (!_inKnockback)
            {
                //If monster hits player, stop special
                if (_playerCollision || _obsticalCollision)
                {
                    StopMotion();
                    _inKnockback = true;
                }
                ApplyForce(_gravity);
            }
            //Do knockback if there was a hit
            else
            {
                //Stop moving fish if it goes below its original height
                if (transform.position.y <= _initalPos)
                {
                    ReturnToInitalPosition();
                    time = EnemyConfig.Instance.KoiBoss.UnderwaterAttack.VulnerabilityTime + 0.95f;
                }
                else
                {
                    ApplyConstantMoveForce(Vector3.down, 1.0f * transform.localScale.y, 1.0f);
                }
            }
        }
        //At the end of the attack, stop motion and remove hitbox
        if (time > 0.95f && _hitboxes.Count > 0)
        {
            if (!_inKnockback)
            {
                _position = new Vector3(_position.x, _startPos.y, _position.z);
            }
            PlaySplash();
            StopMotion();
            Destroy(_hitboxes[_hitboxes.Count - 1]);
            _hitboxes.RemoveAt(_hitboxes.Count - 1);
        }

        //If player was not hit, make the koi go back under water
        if (time >= EnemyConfig.Instance.KoiBoss.UnderwaterAttack.VulnerabilityTime + 0.95f && !_inKnockback)
        {
            //Move fish down until its back in original position
            if (transform.position.y > _initalPos)
            {
                ApplyConstantMoveForce(Vector3.down, 1.0f * transform.localScale.y, 1.0f);
            }
            else
            {
                StopMotion();
                ReturnToInitalPosition();
                _inKnockback = true;
            }
        }
        //Finish attack
        else if (time >= EnemyConfig.Instance.KoiBoss.UnderwaterAttack.VulnerabilityTime + 0.95f)
        {
            StopMotion();
            _inKnockback = false;
            return false;
        }

        return true;
    }
}

//Actions available to rock crabs
public partial class RockCrab : Enemy
{
    protected bool RockCrabFlingCharge(ref float time)
    {
        float MAX_TIME = EnemyConfig.Instance.RockCrab.FlingAttack.ChargeTime;

        if(time == 0)
        {
            //Set up telegraph
            if (DoTelegraphs())
            {
                CreateTelegraph(new Vector3(0, 0, EnemyConfig.Instance.RockCrab.FlingAttack.Distance / 2.0f), new Vector3(2, 1, EnemyConfig.Instance.RockCrab.FlingAttack.Distance), Quaternion.identity, TelegraphType.Square, true);
            }
        }

        //Look towards player
        _destination = new Vector3(PlayerPosition().x, transform.position.y, PlayerPosition().z);
        Quaternion desiredRotation = Quaternion.LookRotation(_destination - transform.position);
        SetSmoothRotation(desiredRotation, 1.0f, 0.5f, 3.0f);

        if (time >= MAX_TIME)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Used for Rock Crab
    /// Crab flings itself forward towards the player
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected bool RockCrabFling(ref float time)
    {
        const float MAX_TIME = 1.0f;

        //Start dashing
        if (time == 0.0f)
        {
            _hitboxes.Add(CreateHitbox(Vector3.zero, new Vector3(1.66f, 3f, 1.66f) * transform.localScale.x / 2.0f, HitboxType.EnemyHitbox, EnemyConfig.Instance.RockCrab.FlingAttack.Damage, new Vector2(0, 0), EnemyConfig.Instance.RockCrab.FlingAttack.Knockback));
            _gravity = ApplyArcForce(transform.forward, EnemyConfig.Instance.RockCrab.FlingAttack.Distance, 2f * transform.localScale.y, 1.0f);
            _animator.SetTrigger(_animParm[(int)CrabAnim.Jump]);
            if (DoTelegraphs())
            {
                _telegraphs[0].transform.parent = null;
                ClearTelegraphs();
            }
        }

        if (!_inKnockback)
        {
            //If monster hits player, stop special
            if (_playerCollision || _obsticalCollision)
            {
                _inKnockback = true;
                _velocity.x = 0;
                _velocity.z = 0;
                ApplyMoveForce(-transform.forward, 2.0f * _speed, 0.3f);
            }
            ApplyForce(_gravity);
        }
        //Do knockback if there was a hit
        else
        {
            ApplyForce(_gravity);

            //Stop moving fish if it goes below its original height
            if (transform.position.y <= _initalPos)
            {
                time = MAX_TIME;
            }
        }

        if (time >= MAX_TIME)
        {
            ReturnToInitalPosition();
            PlaySplash();
            StopMotion();
            GameObject.Destroy(_hitboxes[_hitboxes.Count - 1]);
            _hitboxes.RemoveAt(_hitboxes.Count - 1);
            _inKnockback = false;
            return false;
        }
        else
        {
            return true;
        }
    }
}

public partial class FlowerFrog : Enemy
{
    /// <summary>
    /// Frog rotates towards player to prepare to shoot tounge
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected bool ToungeCharge(ref float time)
    {
        float MAX_TIME = EnemyConfig.Instance.FlowerFrog.ToungeLatch.ChargeTime;

        if (time < MAX_TIME)
        {
            //Stop motion at start
            if (time == 0)
            {
                StopMotion();
                if(DoTelegraphs())
                {
                    CreateTelegraph(new Vector3(0, 0, 10 / transform.localScale.z), new Vector3(_widthMult, 1, 20), Quaternion.identity, TelegraphType.Square, true);
                }
            }

            //Look towards player
            _destination = new Vector3(PlayerPosition().x, transform.position.y, PlayerPosition().z) + PlayerVelocity();
            Quaternion desiredRotation = Quaternion.LookRotation(_destination - transform.position);
            SetSmoothRotation(desiredRotation, 1.2f, 0.75f, 3.0f);
            //rotation = Quaternion.RotateTowards(rotation, Quaternion.LookRotation(destination - transform.position), 1.0f);
        }

        if (time >= MAX_TIME)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Frog shoots tounge towards the player
    /// If the tounge hits the player, the frog latches on
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected bool ShootTounge(ref float time)
    {
        const float SHOOT_TIME = 0.8f;
        const float WINDUP_TIME = 0.2f;
        const float MAX_TIME = SHOOT_TIME + WINDUP_TIME;

        if (time == 0)
        {
            _animator.SetTrigger(_animParm[(int)FrogAnim.Attack]);
            _hitboxes.Add(CreateHitbox(_tounge.transform.localPosition, new Vector3(1, 1, 1), HitboxType.EnemyHitbox, 0));
            _hitboxes[_hitboxes.Count - 1].transform.parent = _tounge.transform;
            _hitboxes[_hitboxes.Count - 1].GetComponent<Hitbox>().OnTrigger += OnToungeLatch;
            if (DoTelegraphs())
            {
                _telegraphs[0].transform.parent = null;
                ClearTelegraphs();
            }
        }

        if (time > WINDUP_TIME)
        {
            if (_playerCollision)
            {
                //LATCH ONTO PLAYER
                _tounge.SetPosition(1, PlayerPosition() - transform.position);
                _activeStates[(int)FlowerFrogAttackState.Latched] = true;
                _latchStartHealth = _health;
                time = MAX_TIME;
            }

            if (_obsticalCollision)
            {
                //If obstical is hit, move to return phase
                time = MAX_TIME;
            }

            if (time < MAX_TIME)
            {
                //Move tounge forward
                _tounge.SetPosition(1, _tounge.GetPosition(1) + transform.forward * 25.0f * Time.deltaTime);
                _hitboxes[_hitboxes.Count - 1].transform.localPosition = _tounge.GetPosition(1);
            }
        }

        if(time >= MAX_TIME)
        {
            ClearHitboxes();
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Returns frogs tounge to inside it's mouth
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected bool ToungeReturn(ref float time)
    {
        //If frog is latched, don't return the tounge
        if(_activeStates[(int)FlowerFrogAttackState.Latched])
        {
            return false;
        }

        //Move tounge backwards
        _tounge.SetPosition(1, _tounge.GetPosition(1) - (_tounge.GetPosition(1) - _tounge.GetPosition(0)).normalized * 30.0f * Time.deltaTime);

        //If tonge moves back too far, set time to max
        if (_tounge.GetPosition(1).z < 0.5f && _tounge.GetPosition(1).z > -0.5f)
        {
            _animator.SetTrigger(_animParm[(int)FrogAnim.Close]);
            _tounge.SetPosition(1, Vector3.zero);
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Frog drags behind player by it's tounge
    /// </summary>
    protected void ToungeDrag()
    { 
        //Seek destination
        Vector3 netForce = Vector3.zero;
        if(Vector3.SqrMagnitude(transform.position - PlayerPosition()) > MAX_DRAG_DIST * MAX_DRAG_DIST)
        {
            netForce += PlayerPosition() - transform.position;
            netForce = new Vector3(netForce.x, 0, netForce.z);
            netForce = netForce.normalized * 20.0f;
        }

        _destination = new Vector3(PlayerPosition().x, transform.position.y, PlayerPosition().z);
        Quaternion desiredRotation = Quaternion.LookRotation(_destination - transform.position);
        SetSmoothRotation(desiredRotation, 2.0f, 2.0f, 3.0f);

        ApplyForce(netForce);
        ApplyFriction(0.5f);
    }
}

public partial class ClamBoss : Enemy
{
    /// <summary>
    /// Spawns a circle of tentacles around the clam that slap down away
    /// from the clam
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected bool ClamCircleAttack(ref float time)
    {
        float MAX_TIME = EnemyConfig.Instance.ClamBoss.CircleAttack.ChargeTime * _speedScale;

        if(time == 0)
        {
            //Spawn Tentcales around clam in a radius
            float tentaclePerDegree = 360 / EnemyConfig.Instance.ClamBoss.CircleAttack.TentacleAmount;
            for (int i = 0; i < EnemyConfig.Instance.ClamBoss.CircleAttack.TentacleAmount; i++)
            {
                Vector3 tentaclePosition = transform.position + Quaternion.Euler(0, tentaclePerDegree * i, 0) * transform.forward * EnemyConfig.Instance.ClamBoss.CircleAttack.CircleRadius;
                Quaternion tentacleRotation = Quaternion.LookRotation(tentaclePosition - transform.position);
                Instantiate(_tentaclePrefab, tentaclePosition + Vector3.up * -2, tentacleRotation)
                    .GetComponent<ClamTentacle>()
                    .SetTentacle(TentacleMode.StationarySlap, PlayerPosition, EnemyConfig.Instance.ClamBoss.CircleAttack.Damage, EnemyConfig.Instance.ClamBoss.CircleAttack.RiseTime, EnemyConfig.Instance.ClamBoss.CircleAttack.TrackTime, EnemyConfig.Instance.ClamBoss.CircleAttack.AttackTime, _speedScale, 1);
            }

            /*if(DoTelegraphs())
            {
                CreateTelegraph(Vector3.zero, new Vector3(CIRCLE_RADIUS + 2.0f, 1, CIRCLE_RADIUS + 2.0f), Quaternion.identity, true);
            }*/
        }

        if(time >= MAX_TIME)
        {
            ClearTelegraphs();
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Spawns a tentacle near the player that
    /// will track their movement to try and hit them
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected bool ClamTrackAttack(ref float time)
    {
        float MAX_TIME = EnemyConfig.Instance.ClamBoss.TrackingAttack.ChargeTime * _speedScale;

        if (time == 0)
        {
            //Spawn Tentcale near the player
            Vector2 randomOnCircle = Random.insideUnitCircle.normalized * EnemyConfig.Instance.ClamBoss.TrackingAttack.CircleRadius;
            Vector3 tentaclePosition = PlayerPosition() + new Vector3(randomOnCircle.x, 0, randomOnCircle.y);
            Quaternion tentacleRotation = Quaternion.LookRotation(tentaclePosition - PlayerPosition());
            Instantiate(_tentaclePrefab, tentaclePosition, tentacleRotation)
                .GetComponent<ClamTentacle>()
                .SetTentacle(TentacleMode.TrackingSlap, PlayerPosition, EnemyConfig.Instance.ClamBoss.TrackingAttack.Damage, EnemyConfig.Instance.ClamBoss.TrackingAttack.RiseTime, EnemyConfig.Instance.ClamBoss.TrackingAttack.TrackTime, EnemyConfig.Instance.ClamBoss.TrackingAttack.AttackTime, _speedScale, 1);
        }

        if (time >= MAX_TIME)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Spawns tentacles in a line in front of the clam
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected bool ClamLineAttack(ref float time)
    {
        float MAX_TIME = EnemyConfig.Instance.ClamBoss.LineAttack.ChargeTime * _speedScale;

        if (time == 0)
        {
            //Spawn Tentcale in a line towards the player
            Vector3 tentaclePosition = transform.position + _lineForward * _lineOffset;
            Quaternion tentacleRotation = _rotation;
            Instantiate(_tentaclePrefab, tentaclePosition + Vector3.up * -2f, tentacleRotation)
                .GetComponent<ClamTentacle>()
                .SetTentacle(TentacleMode.RisingAttack, PlayerPosition, EnemyConfig.Instance.ClamBoss.LineAttack.Damage, EnemyConfig.Instance.ClamBoss.LineAttack.RiseTime, EnemyConfig.Instance.ClamBoss.LineAttack.TrackTime, EnemyConfig.Instance.ClamBoss.LineAttack.AttackTime, _speedScale, 1.5f);
            _lineOffset += 5.0f;
            if(DoTelegraphs() && _lineOffset == 10.0f)
            {
                CreateTelegraph(transform.InverseTransformVector(_lineForward.normalized) * 20, new Vector3(1.2f, 1, 40 / transform.localScale.z), Quaternion.LookRotation(_lineForward), TelegraphType.Square, true);
                ClearTelegraphs();
            }
        }

        if (time >= MAX_TIME)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Summons various tentacles in a radius around the clam
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected bool ClamBurstAttack(ref float time)
    {
        float MAX_TIME = EnemyConfig.Instance.ClamBoss.BurstAttack.Duration;

        if (time == 0)
        {
            //Spawn Tentcales around clam in a radius
            for (int i = 0; i < EnemyConfig.Instance.ClamBoss.BurstAttack.TentacleAmount; i++)
            {
                Vector2 randomOnCircle = Random.insideUnitCircle * EnemyConfig.Instance.ClamBoss.BurstAttack.AttackRadius;
                Vector3 tentaclePosition = transform.position + new Vector3(randomOnCircle.x, 0, randomOnCircle.y);
                Instantiate(_tentaclePrefab, tentaclePosition + Vector3.up * -2, _rotation)
                    .GetComponent<ClamTentacle>()
                    .SetTentacle(TentacleMode.RisingAttack, PlayerPosition, EnemyConfig.Instance.ClamBoss.BurstAttack.Damage, EnemyConfig.Instance.ClamBoss.BurstAttack.RiseTime, EnemyConfig.Instance.ClamBoss.BurstAttack.TrackTime, EnemyConfig.Instance.ClamBoss.BurstAttack.AttackTime, _speedScale, 1);
            }
        }

        if (time >= MAX_TIME)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Opens up clam to prepare for open attack
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected bool ClamOpen(ref float time)
    {
        float MAX_TIME = EnemyConfig.Instance.ClamBoss.OpenAttack.ChargeTime * _speedScale;

        if(time == 0)
        {
            _animator.Play(_animParm[(int)ClamAnim.Open]);
            //Choose a random open state
            _openState = (ClamOpenState)Random.Range(0, 3);
        }

        if(time >= MAX_TIME)
        {
            //Activate canvas, show creature inside
            SetPuppetCanvas(true);
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Clam waits 2 seconds to show off puppet
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected bool ClamWait(ref float time)
    {
        float MAX_TIME = EnemyConfig.Instance.ClamBoss.OpenAttack.WaitTime * _speedScale;

        if(time >= MAX_TIME)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Clam closes after open attack
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected bool ClamClose(ref float time)
    {
        float MAX_TIME = EnemyConfig.Instance.ClamBoss.OpenAttack.ChargeTime * _speedScale;

        if (time == 0)
        {
            _animator.Play(_animParm[(int)ClamAnim.Close]);
            //Deactivate canvas
            SetPuppetCanvas(false);
        }

        if (time >= MAX_TIME)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Clam starts water spout
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected bool ClamWaterSpoutStart(ref float time)
    {
        float MAX_TIME = EnemyConfig.Instance.ClamBoss.OpenAttack.WaterSpoutAttack.ChargeTime * _speedScale;

        if (time == 0)
        {
            _waterSpoutUp = Instantiate(_waterSpoutUpPrefab, transform.position, _waterSpoutUpPrefab.transform.rotation, transform).GetComponent<ParticleSystem>();
            ParticleSystem.MainModule main = _waterSpoutUp.main;
            main.startSpeedMultiplier *= 1 / _speedScale;
            main.startLifetimeMultiplier *= _speedScale;
        }

        if (time >= MAX_TIME)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Clam starts water spout
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected bool ClamWaterSpoutAttack(ref float time)
    {
        float MAX_TIME = EnemyConfig.Instance.ClamBoss.OpenAttack.WaterSpoutAttack.Duration * _speedScale;
        float STALL_TIME = EnemyConfig.Instance.ClamBoss.OpenAttack.WaterSpoutAttack.StallTime * _speedScale;

        ParticleSystem.ShapeModule shape;

        if (time == 0)
        {
            //Create hitbox and water spout
            _waterSpoutDown = Instantiate(_waterSpoutDownPrefab, new Vector3(_position.x, _position.y + 26f, _position.z), _waterSpoutDownPrefab.transform.rotation, transform).GetComponent<ParticleSystem>();
            shape = _waterSpoutDown.shape;
            shape.scale = new Vector3(0, 0, -1);
            ParticleSystem.MainModule main = _waterSpoutDown.main;
            main.startSpeedMultiplier *= 1 / _speedScale;
            main.startLifetimeMultiplier *= _speedScale;
            _hitboxes.Add(CreateHitbox(Vector3.zero, new Vector3(0,2,0), HitboxType.EnemyHitbox, 0));
            _hitboxes[_hitboxes.Count - 1].GetComponent<Hitbox>().OnStay += DealWaterSpoutDamage;
            if(DoTelegraphs())
            {
                CreateTelegraph(Vector3.zero, new Vector3(13, 1, 13), Quaternion.identity, TelegraphType.Circle, true);
            }
        }

        if (time < MAX_TIME - STALL_TIME)
        {
            //Make water spout and hitbox grow over time
            shape = _waterSpoutDown.shape;
            shape.scale = new Vector3(shape.scale.x + 33 * Time.deltaTime / (MAX_TIME - STALL_TIME), shape.scale.y + 30 * Time.deltaTime / (MAX_TIME - STALL_TIME), -1);
            _hitboxes[_hitboxes.Count - 1].transform.localScale = new Vector3(_hitboxes[_hitboxes.Count - 1].transform.localScale.x + 11 * Time.deltaTime / (MAX_TIME - STALL_TIME), _hitboxes[_hitboxes.Count - 1].transform.localScale.y, _hitboxes[_hitboxes.Count - 1].transform.localScale.z + 11 * Time.deltaTime / (MAX_TIME - STALL_TIME));
        }
        else
        {
            //Stop emmitting and clear hitboxes
            if (_waterSpoutUp.isEmitting)
            {
                ClearHitboxes();
                ClearTelegraphs();
                //Don't stop particles instantly, let them trickel off
                _waterSpoutUp.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                _waterSpoutDown.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }

        if(time > MAX_TIME)
        {
            Destroy(_waterSpoutDown);
            Destroy(_waterSpoutUp);
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Clam throws sticks and rocks towards the player
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected bool ClamBirdAttack(ref float time)
    {
        float MAX_TIME = EnemyConfig.Instance.ClamBoss.OpenAttack.BirdAttack.Duration * _speedScale;

        //Fire a random projectile randomly every couple frames
        if(Random.Range(0, (int)Mathf.Ceil(8 * _speedScale)) == 0)
        {
            if(DoTelegraphs())
            {
                ClearTelegraphs();
            }
            //Get Spawn in position
            Vector3 spawnPosition = _position + (PlayerPosition() - _position).normalized * 3.0f;
            spawnPosition.y += 1.0f;

            //Get Direction of fire
            Vector2 randomOnCircle = Random.insideUnitCircle * 5.0f;
            Vector3 fireTarget = PlayerPosition() + new Vector3(randomOnCircle.x, 0, randomOnCircle.y);
            Vector3 fireDirection = fireTarget - spawnPosition;
            fireDirection = new Vector3(fireDirection.x, 0, fireDirection.z).normalized;

            //Set up projectile
            GameObject projectile;
            if(Random.Range(0,2) == 0)
            {
                projectile = Instantiate(_rockPrefab, spawnPosition, Quaternion.LookRotation(fireDirection));
            }
            else
            {
                projectile = Instantiate(_stickPrefab, spawnPosition, Quaternion.LookRotation(fireDirection));
            }
            projectile.GetComponent<EnemyProjectile>().LoadProjectile(fireDirection, EnemyConfig.Instance.ClamBoss.OpenAttack.BirdAttack.RockSpeed * (1 / _speedScale), EnemyConfig.Instance.ClamBoss.OpenAttack.BirdAttack.Damage, 1, MovementPattern.Forward, Vector2.zero, EnemyConfig.Instance.ClamBoss.OpenAttack.BirdAttack.Knockback, new Vector3(1.8f, 1.8f, 1.6f));
            if (DoTelegraphs())
            {
                CreateTelegraph(transform.InverseTransformVector(fireDirection.normalized) * 20, new Vector3(0.5f, 1, 40 / transform.localScale.z), Quaternion.LookRotation(fireDirection), TelegraphType.Square, true);
            }

            //Randomly rotate projectile
            Quaternion randomRotation = Random.rotation;
            projectile.transform.GetChild(0).transform.rotation = randomRotation;
        }

        if(time > MAX_TIME)
        {
            ClearTelegraphs();
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Charges up dragon smoke attack by making smoke clouds around clam
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected bool ClamDragonCharge(ref float time)
    {
        float MAX_TIME = EnemyConfig.Instance.ClamBoss.OpenAttack.DragonAttack.ChargeTime * _speedScale;

        if(time == 0)
        {
            //Spawn clouds of poison smoke around clam in a radius
            const float CIRCLE_RADIUS = 6.0f;
            float tentaclePerDegree = 360 / EnemyConfig.Instance.ClamBoss.OpenAttack.DragonAttack.SmokeClouds;
            for (int i = 0; i < EnemyConfig.Instance.ClamBoss.OpenAttack.DragonAttack.SmokeClouds; i++)
            {
                Vector3 smokePosition = transform.position + Quaternion.Euler(0, tentaclePerDegree * i, 0) * transform.forward * CIRCLE_RADIUS + Vector3.up * 2.0f;
                Vector3 smokeDistanceVec = smokePosition - transform.position;
                Vector3 smokeDirection = new Vector3(smokeDistanceVec.x, 0, smokeDistanceVec.z);
                Quaternion smokeRotation = Quaternion.LookRotation(smokeDirection);
                _dragonSmokeParticles.Add(Instantiate(_dragonSmokePrefab, smokePosition + Vector3.up * -2, smokeRotation).GetComponent<ParticleSystem>());
                _dragonSmokeParticles[i].transform.localScale = Vector3.zero;
                GameObject hitbox = Instantiate(_hitbox, _dragonSmokeParticles[i].transform);
                hitbox.GetComponent<Hitbox>().SetHitbox(gameObject, Vector3.zero, new Vector3(4.5f, 4.5f, 4.5f), HitboxType.EnemyHitbox, 0);

                //Add poison damage when player enters hitbox
                hitbox.GetComponent<Hitbox>().OnTrigger += DealDragonPoison;

                if(DoTelegraphs())
                {
                    CreateTelegraph(transform.InverseTransformVector(smokeDirection.normalized) * 23, new Vector3(1.5f, 1, 46 / transform.localScale.z), Quaternion.LookRotation(smokeDirection), TelegraphType.Square, true);
                }
            }
        }

        for (int i = 0; i < EnemyConfig.Instance.ClamBoss.OpenAttack.DragonAttack.SmokeClouds; i++)
        {
            _dragonSmokeParticles[i].transform.localScale = new Vector3(_dragonSmokeParticles[i].transform.localScale.x + Time.deltaTime / MAX_TIME, _dragonSmokeParticles[i].transform.localScale.y + Time.deltaTime / MAX_TIME, _dragonSmokeParticles[i].transform.localScale.z + Time.deltaTime / MAX_TIME);
        }

        if (time > MAX_TIME)
        {
            ClearTelegraphs();
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Uses dragon smoke attack by sending out smoke clouds towards player
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected bool ClamDragonAttack(ref float time)
    {
        float MAX_TIME = EnemyConfig.Instance.ClamBoss.OpenAttack.DragonAttack.Duration * _speedScale;

        //Move smoke clouds forward
        for (int i = 0; i < EnemyConfig.Instance.ClamBoss.OpenAttack.DragonAttack.SmokeClouds; i++)
        {
            _dragonSmokeParticles[i].transform.Translate(_dragonSmokeParticles[i].transform.forward * EnemyConfig.Instance.ClamBoss.OpenAttack.DragonAttack.Distance * Time.deltaTime / MAX_TIME, Space.World);
        }

        if (time > MAX_TIME)
        {
            for (int i = 0; i < EnemyConfig.Instance.ClamBoss.OpenAttack.DragonAttack.SmokeClouds; i++)
            {
                _dragonSmokeParticles[i].Stop();
            }
            _dragonSmokeParticles.Clear();
            return false;
        }
        else
        {
            return true;
        }
    }
}

public partial class Pandatee : Enemy
{
    /// <summary>
    /// Pandatee eats for 5 seconds then returns to swimming
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected bool PandateeEat(ref float time)
    {
        const float MAX_TIME = 5.0f;

        if(time == 0)
        {
            StopMotion();
            _animator.Play(_animParm[(int)PandateeAnim.Situp]);
        }

        if(time > MAX_TIME)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}

public partial class ChickenFishFlock : Enemy
{
    /// <summary>
    /// One chicken fish in the pack jumps out towards the player
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected bool ChickenFishAttack(ref float time)
    {
        float MAX_TIME = EnemyConfig.Instance.ChickenFish.Jump.Duration;

        if(time == 0)
        {
            StopMotion();
            //Choose a random chicken to attack player
            _attackingChickenID = Random.Range(0, _chickenFlock.Count);

            //Get direction to attack player
            Vector3 attackDirection = PlayerPosition() - _chickenFlock[_attackingChickenID].Position;

            //Set position and physics values to prepare for attack
            _initalPos = _chickenFlock[_attackingChickenID].Position.y;
            _chickenFlock[_attackingChickenID].StopMotion();
            _chickenFlock[_attackingChickenID].Rotation = Quaternion.LookRotation(new Vector3(attackDirection.x, 0, attackDirection.z).normalized);

            //Apply force to move chicken towards player
            _gravity = _chickenFlock[_attackingChickenID].ApplyArcForce(new Vector3(attackDirection.x, 0, attackDirection.z).normalized, EnemyConfig.Instance.ChickenFish.Jump.Distance, EnemyConfig.Instance.ChickenFish.Jump.Height, MAX_TIME);
            //Play animation
            _chickenFlock[_attackingChickenID].FlockerAnimator.Play(_animParm[(int)ChickenFishAnim.Fly]);

            //Set up hitbox
            GameObject hitbox = Instantiate(_hitbox, _chickenFlock[_attackingChickenID].transform);
            hitbox.GetComponent<Hitbox>().SetHitbox(gameObject, new Vector3(0, 0, 0.11f), new Vector3(2.2f, 2.2f, 2.2f), HitboxType.EnemyHitbox, EnemyConfig.Instance.ChickenFish.Jump.Damage, Vector2.zero, EnemyConfig.Instance.ChickenFish.Jump.Knockback);
            hitbox.GetComponent<Hitbox>().OnTrigger += HitboxTriggered;

            //Set up telegraph
            if (DoTelegraphs())
            {
                GameObject temp = Instantiate(_telegraphPrefab[(int)TelegraphType.Square], _chickenFlock[_attackingChickenID].transform.position, _chickenFlock[_attackingChickenID].transform.rotation, _chickenFlock[_attackingChickenID].transform);
                temp.transform.localPosition = new Vector3(0,0, EnemyConfig.Instance.ChickenFish.Jump.Distance / 2 / _chickenFlock[_attackingChickenID].transform.localScale.z);
                temp.transform.localScale = new Vector3(1, 1, EnemyConfig.Instance.ChickenFish.Jump.Distance);
                _telegraphs.Add(temp);
                _telegraphs[0].transform.parent = null;
                ClearTelegraphs();
            }
        }

        //While not in knockback
        if (!_inKnockback)
        {
            //If monster hits player or obstical do knockback
            if (_playerCollision || _obsticalCollision)
            {
                _inKnockback = true;
                _chickenFlock[_attackingChickenID].StopHorizontalMotion();
                _chickenFlock[_attackingChickenID].ApplyMoveForce(-_chickenFlock[_attackingChickenID].transform.forward, 2.0f, 0.3f);
            }
            _chickenFlock[_attackingChickenID].ApplyForce(_gravity);
        }
        //Do knockback if there was a hit
        else
        {
            _chickenFlock[_attackingChickenID].ApplyForce(_gravity);

            //Stop moving fish if it goes below its original height
            if (_chickenFlock[_attackingChickenID].transform.position.y <= _initalPos)
            {
                time = MAX_TIME;
            }
        }

        if (time >= MAX_TIME)
        {
            _inKnockback = false;
            //Return to swimming
            _chickenFlock[_attackingChickenID].FlockerAnimator.SetTrigger(_animParm[(int)ChickenFishAnim.Swim]);

            //Reset motion and position of chicken
            _chickenFlock[_attackingChickenID].StopMotion();
            _chickenFlock[_attackingChickenID].Position = new Vector3(_chickenFlock[_attackingChickenID].Position.x, _initalPos, _chickenFlock[_attackingChickenID].Position.z);

            //Play splash animation
            PlaySplash(_chickenFlock[_attackingChickenID].Position, _chickenFlock[_attackingChickenID].transform.localScale.x);

            //Destroy hitbox
            Destroy(_chickenFlock[_attackingChickenID].transform.GetChild(_chickenFlock[_attackingChickenID].transform.childCount - 1).gameObject);
            //Reset attacking chicken ID
            _attackingChickenID = -1;
            return false;
        }
        else
        {
            return true;
        }
    }
}

public partial class Stingray : Enemy
{
    /// <summary>
    /// Charges an electric bolt to shoot towards player
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected bool StingrayBoltCharge(ref float time)
    {
        float MAX_TIME = EnemyConfig.Instance.Stingray.BoltAttack.ChargeTime;
        float STALL_TIME = EnemyConfig.Instance.Stingray.BoltAttack.StallTime;

        if (time < MAX_TIME - STALL_TIME)
        {
            //Stop motion at start
            if (time == 0)
            {
                StopMotion();
                _electricChargeParticles.Play();
                if (DoTelegraphs())
                {
                    CreateTelegraph(new Vector3(0, 0, (_lengthMult + EnemyConfig.Instance.Stingray.BoltAttack.BoltLength / 2) / transform.localScale.z), new Vector3(5.0f, 1, EnemyConfig.Instance.Stingray.BoltAttack.BoltLength + 2.0f / transform.localScale.z), Quaternion.identity, TelegraphType.Square, true);
                }
            }

            //Look towards player
            _destination = new Vector3(PlayerPosition().x, transform.position.y, PlayerPosition().z);
            Quaternion desiredRotation = Quaternion.LookRotation(_destination - transform.position);
            SetSmoothRotation(desiredRotation, 1.0f, 0.5f, 3.0f);
            //rotation = Quaternion.RotateTowards(rotation, Quaternion.LookRotation(destination - transform.position), 1.0f);
            ApplyFriction(0.99f);
        }

        if (time >= MAX_TIME)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Shoots electric bolt towards player
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected bool StingrayBoltAttack(ref float time)
    {
        const float MAX_TIME = 0.5f;

        if (time == 0)
        {
            //Set up bolt particles
            _electricBoltParticles = Instantiate(_electricBoltParticlesPrefab, transform.position, transform.rotation, transform);
            _electricBoltParticles.transform.localPosition = new Vector3(0, 0, (_lengthMult + EnemyConfig.Instance.Stingray.BoltAttack.BoltLength / 2 - 2.0f) / transform.localScale.z);
            _electricBoltParticles.transform.localScale = new Vector3(5.0f, 1, EnemyConfig.Instance.Stingray.BoltAttack.BoltLength / 2 / transform.localScale.z);
            _electricBoltParticles.transform.parent = null;
            _electricChargeParticles.Stop();

            if (DoTelegraphs())
            {
                _telegraphs[0].transform.parent = null;
                ClearTelegraphs();
            }

            //Set up hitbox
            _hitboxes.Add(CreateHitbox(new Vector3(0, 0.6f, EnemyConfig.Instance.Stingray.BoltAttack.BoltLength / 2 + 2.0f), new Vector3(3.5f, 2.0f, EnemyConfig.Instance.Stingray.BoltAttack.BoltLength), HitboxType.EnemyHitbox, EnemyConfig.Instance.Stingray.BoltAttack.Damage, Vector2.zero, EnemyConfig.Instance.Stingray.BoltAttack.Knockback));
            _hitboxes[0].transform.parent = null;
            _hitboxes[0].GetComponent<Hitbox>().OnTrigger += AddElectricEffect;
            _hitboxes[0].GetComponent<Hitbox>().OnExit += RemoveElectricEffectOnExit;
            _hitboxes[0].GetComponent<Hitbox>().OnDestruction += RemoveElectricEffect;
        }

        if(time >= MAX_TIME)
        {
            foreach(ParticleSystem particle in _electricBoltParticles.GetComponentsInChildren<ParticleSystem>())
            {
                particle.Stop();
            }
            ClearHitboxes();
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Sets up stingrays for using cross zap attack
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected bool StingrayCrossZapSetup(ref float time)
    {
        float MAX_TIME = EnemyConfig.Instance.Stingray.CrossZap.ChargeTime;

        if(time == 0)
        {
            StopMotion();
            _electricChargeParticles.Play();
            if (_crossZapParent)
            {
                //Create telegraph
                if (DoTelegraphs())
                {
                    float dist = Vector3.Distance(transform.position, _zapBuddy.transform.position);
                    Vector3 diffVec = _zapBuddy.transform.position - transform.position;
                    diffVec = new Vector3(diffVec.x, 0, diffVec.z).normalized;
                    CreateTelegraph(transform.InverseTransformVector(diffVec) * (dist / 2f), new Vector3(4.0f, 1, dist / transform.localScale.z), Quaternion.LookRotation(diffVec), TelegraphType.Square, true);
                }
            }
        }

        ApplyFriction(0.99f);

        if(time >= MAX_TIME)
        {
            if (_crossZapParent)
            {
                if (DoTelegraphs())
                {
                    _telegraphs[0].transform.parent = null;
                    ClearTelegraphs();
                }
                //Set up bolt particles
                _electricBoltParticles = Instantiate(_electricBoltParticlesPrefab, transform.position, transform.rotation, transform);
                float dist = Vector3.Distance(transform.position, _zapBuddy.transform.position);
                Vector3 diffVec = _zapBuddy.transform.position - transform.position;
                diffVec = new Vector3(diffVec.x, 0, diffVec.z).normalized;
                _electricBoltParticles.transform.localPosition = transform.InverseTransformVector(diffVec) * (dist / 2f);
                _electricBoltParticles.transform.localScale = new Vector3(5.0f, 1, dist / 2f / transform.localScale.z);
                _electricBoltParticles.transform.rotation = Quaternion.LookRotation(diffVec);

                //Setup hitbox
                _hitboxes.Add(CreateHitbox(transform.InverseTransformVector(diffVec) * (dist / 2f), new Vector3(2.0f, 2f, dist / transform.localScale.z), HitboxType.EnemyHitbox, 0));
                _hitboxes[0].transform.rotation = Quaternion.LookRotation(diffVec);
                _hitboxes[0].GetComponent<Hitbox>().OnTrigger += AddElectricEffect;
                _hitboxes[0].GetComponent<Hitbox>().OnStay += DealElectricDamage;
                _hitboxes[0].GetComponent<Hitbox>().OnDestruction += RemoveElectricEffect;
                _hitboxes[0].GetComponent<Hitbox>().OnExit += RemoveElectricEffectOnExit;
            }
            return false;
        }
        else
        {
            return true;
        }
    }
}

public partial class Mox : Enemy
{
    /// <summary>
    /// Mox charges dash for 1 secondish
    /// </summary>
    /// <param name="time">Current time</param>
    /// <returns></returns>
    protected bool MoxDashCharge(ref float time)
    {
        float MAX_TIME = EnemyConfig.Instance.Mox.DashAttack.ChargeTime;
        float STALL_TIME = EnemyConfig.Instance.Mox.DashAttack.StallTime;

        //Stop motion at begining of charge
        if (time == 0)
        {
            StopMotion();
            if (DoTelegraphs())
            {
                CreateTelegraph(new Vector3(0, -0.5f, (_lengthMult + 10f) / transform.localScale.z), new Vector3(_widthMult, 1, 17.0f / transform.localScale.z), Quaternion.identity, TelegraphType.Square, true);
            }
        }

        if (time <= MAX_TIME - STALL_TIME)
        {
            //Look towards player
            _destination = new Vector3(PlayerPosition().x, transform.position.y, PlayerPosition().z);
            Quaternion desiredRotation = Quaternion.LookRotation(_destination - transform.position);
            SetSmoothRotation(desiredRotation, 1.5f, 1.0f, 4.0f);
            //rotation = Quaternion.RotateTowards(rotation, Quaternion.LookRotation(destination - transform.position), 1.0f);
        }

        if (time >= MAX_TIME)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Dashes at player for 1 second
    /// </summary>
    /// <param name="time">Current Time</param>
    /// <returns></returns>
    protected bool MoxDashAttack(ref float time)
    {
        const float MAX_TIME = 1.0f;

        //Add hitbox and start dash
        if (time == 0.0f)
        {
            _hitboxes.Add(CreateHitbox(Vector3.forward * 2.2f + Vector3.up * 0.2f, new Vector3(4, 4, 4), HitboxType.EnemyHitbox, EnemyConfig.Instance.Mox.DashAttack.Damage, Vector2.zero, EnemyConfig.Instance.Mox.DashAttack.Knockback));
            ApplyMoveForce(transform.forward, EnemyConfig.Instance.Mox.DashAttack.Distance * _speed, 1.0f);
            _animator.SetFloat(_animParm[(int)MoxAnim.SwimSpeed], 2.0f);
            if (DoTelegraphs())
            {
                _telegraphs[0].transform.parent = null;
                ClearTelegraphs();
            }
        }

        if (!_inKnockback)
        {
            if (_playerCollision || _obsticalCollision)
            {
                //Go into knockback
                StopMotion();
                _inKnockback = true;
                time = 0.7f;
                ApplyMoveForce(-transform.forward, 2.0f * _speed, 0.3f);
            }
            //ApplyFriction(0.25f);
        }
        //Do knockback if there was a hit
        else
        {
            //Do nothing
        }

        _animator.SetFloat(_animParm[(int)Anim.Velocity], _velocity.sqrMagnitude);

        if (time >= MAX_TIME)
        {
            GameObject.Destroy(_hitboxes[_hitboxes.Count - 1]);
            _hitboxes.RemoveAt(_hitboxes.Count - 1);
            _inKnockback = false;
            StopMotion();
            _animator.SetFloat(_animParm[(int)MoxAnim.SwimSpeed], 1.0f);
            return false;
        }
        else
        {
            return true;
        }
    }
}

public partial class MonkeyBoss : Enemy
{
    /// <summary>
    /// Charges monkey's right hand push attack
    /// </summary>
    /// <returns></returns>
    protected bool MonkeyRightHandPushCharge(ref float time)
    {
        float MAX_TIME = EnemyConfig.Instance.MonkeyBoss.HandPush.ChargeTime;

        if(time == 0)
        {
            _rotateRightWithBody = false;
            if (DoTelegraphs())
            {
                CreateRightHandTelegraph(new Vector3(0, -1f, 10.0f / transform.localScale.z), new Vector3(5.0f, 1, 22.0f / transform.localScale.z), Quaternion.identity, TelegraphType.Square, true);
            }
            _animator.SetFloat(_animParm[(int)Anim.Velocity], 0);
        }

        LookAtPlayer();

        //Look towards player
        _destination = new Vector3(PlayerPosition().x, _rightHand.transform.position.y, PlayerPosition().z);
        Quaternion desiredRotation = Quaternion.LookRotation(_destination - _rightHand.transform.position);
        _rightHand.Rotation = Quaternion.RotateTowards(_rightHand.Rotation, desiredRotation, 3.0f);

        if (time >= MAX_TIME)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Monkey pushes out with his right hand, left hand prepares to be pushed out
    /// </summary>
    /// <returns></returns>
    protected bool MonkeyRightHandPush(ref float time)
    {
        float MAX_TIME = EnemyConfig.Instance.MonkeyBoss.HandPush.Duration;

        if (time == 0)
        {
            _rightHand.ApplyMoveForce(_rightHand.transform.forward, EnemyConfig.Instance.MonkeyBoss.HandPush.Distance * _speed, MAX_TIME);
            _rotateLeftWithBody = false;
            _moveRightWithBody = false;
            if (DoTelegraphs())
            {
                _telegraphs[0].transform.parent = null;
                ClearTelegraphs();
                CreateLeftHandTelegraph(new Vector3(0, -1f, 10.0f / transform.localScale.z), new Vector3(5.0f, 1, 22.0f / transform.localScale.z), Quaternion.identity, TelegraphType.Square, true);
            }
            _hitboxes.Add(CreateRightHandHitbox(new Vector3(0, 0, 1.0f), new Vector3(4.5f, 7.0f, 1.5f), HitboxType.EnemyHitbox, EnemyConfig.Instance.MonkeyBoss.HandPush.Damage, Vector2.zero, EnemyConfig.Instance.MonkeyBoss.HandPush.Knockback));
            _hitboxes.Add(CreateRightHandHitbox(new Vector3(0, 0, 1.0f), new Vector3(4.5f, 7.0f, 1.5f), HitboxType.PlayerHitbox, 0, Vector2.zero, EnemyConfig.Instance.MonkeyBoss.HandPush.Knockback));

            _rightHandAnimator.SetTrigger(_animParm[(int)MonkeyAnim.Still]);
        }

        LookAtPlayer();

        //Left hand looks towards player
        _destination = new Vector3(PlayerPosition().x, _leftHand.transform.position.y, PlayerPosition().z);
        Quaternion desiredRotation = Quaternion.LookRotation(_destination - _leftHand.transform.position);
        _leftHand.Rotation = Quaternion.RotateTowards(_leftHand.Rotation, desiredRotation, 3.0f);

        //Right hand
        if (!_inKnockback)
        {
            if (_playerCollision || _obsticalCollision)
            {
                //Go into knockback
                _rightHand.StopMotion();
                _inKnockback = true;
                //time = MAX_TIME - 0.2f;
                _rightHand.ApplyMoveForce(-_rightHand.transform.forward, 2.0f * _speed, 0.2f);
            }
        }
        //Do knockback if there was a hit
        else
        {
            //Do nothing
        }

        if (time >= MAX_TIME)
        {
            _rightHandAnimator.SetTrigger(_animParm[(int)MonkeyAnim.Idle]);
            _inKnockback = false;
            _rightHand.StopMotion();
            ClearHitboxes();
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Monkey pushes out with his right hand, left hand prepares to be pushed out
    /// </summary>
    /// <returns></returns>
    protected bool MonkeyLeftHandPush(ref float time)
    {
        float MAX_TIME = EnemyConfig.Instance.MonkeyBoss.HandPush.Duration;

        if (time == 0)
        {
            _rightHandReturnDist = Vector3.Magnitude(transform.TransformPoint(_rightHandStartPos) - _rightHand.Position);
            _leftHand.ApplyMoveForce(_leftHand.transform.forward, EnemyConfig.Instance.MonkeyBoss.HandPush.Distance * _speed, MAX_TIME);
            _moveLeftWithBody = false;
            if (DoTelegraphs())
            {
                _telegraphs[0].transform.parent = null;
                ClearTelegraphs();
            }
            _hitboxes.Add(CreateLeftHandHitbox(new Vector3(0, 0, 1.0f), new Vector3(4.5f, 7.0f, 1.5f), HitboxType.EnemyHitbox, EnemyConfig.Instance.MonkeyBoss.HandPush.Damage, Vector2.zero, EnemyConfig.Instance.MonkeyBoss.HandPush.Knockback));
            _hitboxes.Add(CreateLeftHandHitbox(new Vector3(0, 0, 1.0f), new Vector3(4.5f, 7.0f, 1.5f), HitboxType.PlayerHitbox, 0f, Vector2.zero, EnemyConfig.Instance.MonkeyBoss.HandPush.Knockback));

            _leftHandAnimator.SetTrigger(_animParm[(int)MonkeyAnim.Still]);
        }

        //Right hand move back to original position
        Vector3 rightHandReturnDir = transform.TransformPoint(_rightHandStartPos) -_rightHand.Position;
        rightHandReturnDir.Normalize();
        _rightHand.ApplyConstantMoveForce(rightHandReturnDir, _rightHandReturnDist, MAX_TIME);
        _rightHand.Rotation = Quaternion.RotateTowards(_rightHand.Rotation, Rotation, 1.0f);

        //Left hand
        if (!_inKnockback)
        {
            if (_playerCollision || _obsticalCollision)
            {
                //Go into knockback
                _leftHand.StopMotion();
                _inKnockback = true;
                //time = MAX_TIME - 0.2f;
                _leftHand.ApplyMoveForce(-_leftHand.transform.forward, 2.0f * _speed, 0.2f);
            }
        }
        //Do knockback if there was a hit
        else
        {
            //Do nothing
        }

        if (time >= MAX_TIME)
        {
            _leftHandAnimator.SetTrigger(_animParm[(int)MonkeyAnim.Idle]);
            ClearHitboxes();
            //Set right hand back to normal
            ResetRightHand();
            _leftHand.StopMotion();
            _inKnockback = false;
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Monkey pushes out with his right hand, left hand prepares to be pushed out
    /// </summary>
    /// <returns></returns>
    protected bool MonkeyLeftHandReturn(ref float time)
    {
        const float MAX_TIME = 1.0f;

        if (time == 0)
        {
            _leftHandReturnDist = Vector3.Magnitude(transform.TransformPoint(_leftHandStartPos) - _leftHand.Position);
        }

        //Left hand move back to original position
        Vector3 leftHandReturnDir = transform.TransformPoint(_leftHandStartPos) - _leftHand.Position;
        leftHandReturnDir.Normalize();
        _leftHand.ApplyConstantMoveForce(leftHandReturnDir, _leftHandReturnDist, MAX_TIME);
        _leftHand.Rotation = Quaternion.RotateTowards(_leftHand.Rotation, Rotation, 1.0f);

        if (time >= MAX_TIME)
        {
            //Set right hand back to normal
            ResetLeftHand();
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Monkey charges swipe attack
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected bool MonkeyRightHandSwipeCharge(ref float time)
    {
        float MAX_TIME = EnemyConfig.Instance.MonkeyBoss.HandSwipe.ChargeTime;

        if(time == 0)
        {
            _moveRightWithBody = false;
            _rightHandAnimator.Play(_animParm[(int)MonkeyAnim.Swipe]);
            _animator.SetFloat(_animParm[(int)Anim.Velocity], 0);
        }

        LookAtPlayer();

        //Move hand to side of player
        Vector3 rightHandMoveDir = transform.TransformPoint(new Vector3(EnemyConfig.Instance.MonkeyBoss.HandSwipe.XDisplacement, _rightHandStartPos.y, _playerDistance)) - _rightHand.Position;
        rightHandMoveDir.Normalize();
        _rightHand.ApplyConstantMoveForce(rightHandMoveDir, _playerDistance, MAX_TIME);

        if(time >= MAX_TIME)
        {
            _rightHand.StopMotion();
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Monkey swipes with right hand
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected bool MonkeyRightHandSwipe(ref float time)
    {
        float MAX_TIME = EnemyConfig.Instance.MonkeyBoss.HandSwipe.Duration;

        if (time == 0)
        {
            //Apply force to move right hand
            Vector3 playerDirection = new Vector3(PlayerPosition().x, _rightHand.transform.position.y, PlayerPosition().z) - _rightHand.Position;
            playerDirection.Normalize();
            _rightHand.ApplyMoveForce(playerDirection, EnemyConfig.Instance.MonkeyBoss.HandSwipe.Distance, MAX_TIME);

            _moveLeftWithBody = false;

            if(DoTelegraphs())
            {
                CreateRightHandTelegraph(Vector3.forward * 4.0f, new Vector3(5.0f, 1, 15.0f / transform.localScale.z), Quaternion.identity, TelegraphType.Square, false);
                _telegraphs[0].transform.rotation = Quaternion.LookRotation(playerDirection);
                _telegraphs[0].transform.position += _telegraphs[0].transform.forward * 8.0f + Vector3.down;
            }
            _hitboxes.Add(CreateRightHandHitbox(Vector3.forward * 4.0f, new Vector3(1.0f, 1.0f, 5.0f), HitboxType.EnemyHitbox, EnemyConfig.Instance.MonkeyBoss.HandSwipe.Damage, Vector2.zero, EnemyConfig.Instance.MonkeyBoss.HandSwipe.Knockback));

            _leftHandAnimator.Play(_animParm[(int)MonkeyAnim.Swipe]);
        }

        if (DoTelegraphs() && _telegraphs.Count > 0 && time != 0)
        {
            ClearTelegraphs();
        }

        LookAtPlayer();

        //Move left hand to side of player
        Vector3 leftHandMoveDir = transform.TransformPoint(new Vector3(-EnemyConfig.Instance.MonkeyBoss.HandSwipe.XDisplacement, _leftHandStartPos.y, _playerDistance)) - _leftHand.Position;
        leftHandMoveDir.Normalize();
        _leftHand.ApplyConstantMoveForce(leftHandMoveDir, _playerDistance, MAX_TIME);

        if (time >= MAX_TIME)
        {
            ClearHitboxes();
            _rightHand.StopMotion();
            _leftHand.StopMotion();
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Monkey swipes with left hand, right hand returns to side
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected bool MonkeyLeftHandSwipe(ref float time)
    {
        float MAX_TIME = EnemyConfig.Instance.MonkeyBoss.HandSwipe.Duration;

        if(time ==  0)
        {
            _rightHandReturnDist = Vector3.Magnitude(transform.TransformPoint(_rightHandStartPos) - _rightHand.Position);

            //Apply force to move left hand
            Vector3 playerDirection = new Vector3(PlayerPosition().x, _leftHand.transform.position.y, PlayerPosition().z) - _leftHand.Position;
            playerDirection.Normalize();
            _leftHand.ApplyMoveForce(playerDirection, EnemyConfig.Instance.MonkeyBoss.HandSwipe.Distance, MAX_TIME);

            if (DoTelegraphs())
            {
                CreateLeftHandTelegraph(Vector3.zero, new Vector3(5.0f, 1, 15.0f / transform.localScale.z), Quaternion.identity, TelegraphType.Square, false);
                _telegraphs[0].transform.rotation = Quaternion.LookRotation(playerDirection);
                _telegraphs[0].transform.position += _telegraphs[0].transform.forward * 8.0f + Vector3.down;
            }

            _hitboxes.Add(CreateLeftHandHitbox(Vector3.forward * 4.0f, new Vector3(1.0f, 1.0f, 5.0f), HitboxType.EnemyHitbox, EnemyConfig.Instance.MonkeyBoss.HandSwipe.Damage, Vector2.zero, EnemyConfig.Instance.MonkeyBoss.HandSwipe.Knockback));
        }

        if (DoTelegraphs() && _telegraphs.Count > 0 && time != 0)
        {
            ClearTelegraphs();
        }

        //Right hand move back to original position
        Vector3 rightHandReturnDir = transform.TransformPoint(_rightHandStartPos) - _rightHand.Position;
        rightHandReturnDir.Normalize();
        _rightHand.ApplyConstantMoveForce(rightHandReturnDir, _rightHandReturnDist, MAX_TIME);
        _rightHand.Rotation = Quaternion.RotateTowards(_rightHand.Rotation, Rotation, 1.0f);

        if (time >= MAX_TIME)
        {
            ClearHitboxes();
            ResetRightHand();
            _leftHand.StopMotion();
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Charges monkey's clap attack
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected bool MonkeyClapCharge(ref float time)
    {
        float MAX_TIME = EnemyConfig.Instance.MonkeyBoss.HandClap.ChargeTime;

        if (time == 0)
        {
            _moveRightWithBody = false;
            _rotateRightWithBody = false;
            _moveLeftWithBody = false;
            _rotateLeftWithBody = false;
            _animator.SetFloat(_animParm[(int)Anim.Velocity], 0);
        }

        LookAtPlayer();

        //Move right hand to side of player
        Vector3 rightHandMoveDir = transform.TransformPoint(new Vector3(EnemyConfig.Instance.MonkeyBoss.HandClap.XDisplacement, _rightHandStartPos.y, _playerDistance)) - _rightHand.Position;
        rightHandMoveDir.Normalize();
        _rightHand.ApplyConstantMoveForce(rightHandMoveDir, _playerDistance, MAX_TIME);
        _rightHand.Rotation = Quaternion.RotateTowards(_rightHand.Rotation, Quaternion.LookRotation(_leftHand.Position - _rightHand.Position), 2.0f);

        //Move left hand to side of player
        Vector3 leftHandMoveDir = transform.TransformPoint(new Vector3(-EnemyConfig.Instance.MonkeyBoss.HandClap.XDisplacement, _leftHandStartPos.y, _playerDistance)) - _leftHand.Position;
        leftHandMoveDir.Normalize();
        _leftHand.ApplyConstantMoveForce(leftHandMoveDir, _playerDistance, MAX_TIME);
        _leftHand.Rotation = Quaternion.RotateTowards(_leftHand.Rotation, Quaternion.LookRotation(_rightHand.Position - _leftHand.Position), 2.0f);

        if (time > MAX_TIME)
        {
            //Set rotation
            _rightHand.Rotation = Quaternion.LookRotation(_leftHand.Position - _rightHand.Position);
            _leftHand.Rotation = Quaternion.LookRotation(_rightHand.Position - _leftHand.Position);

            //Stop motion
            _rightHand.StopMotion();
            _leftHand.StopMotion();
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Monkey's clap attack
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected bool MonkeyClap(ref float time)
    {
        float MAX_TIME = EnemyConfig.Instance.MonkeyBoss.HandClap.Duration;
        float WAIT_TIME = EnemyConfig.Instance.MonkeyBoss.HandClap.StallTime;

        if (time == 0)
        {
            //Apply force to move right hand
            Vector3 handDirection = _leftHand.Position - _rightHand.Position;
            handDirection.Normalize();

            if (DoTelegraphs())
            {
                CreateRightHandTelegraph(Vector3.zero, new Vector3(5.0f, 1, 24.0f), Quaternion.identity, TelegraphType.Square, false);
                _telegraphs[0].transform.rotation = Quaternion.LookRotation(handDirection);
                _telegraphs[0].transform.position += _telegraphs[0].transform.forward * 12.0f + Vector3.down;
            }

            _leftHandAnimator.SetTrigger(_animParm[(int)MonkeyAnim.Still]);
            _rightHandAnimator.SetTrigger(_animParm[(int)MonkeyAnim.Still]);
        }

        //Move hands after waiting a couple frames
        if(time >= WAIT_TIME)
        {
            //Remove telegraphs
            if(_telegraphs.Count > 0)
            {
                ClearTelegraphs();
            }

            //Create hitboxes
            if(_hitboxes.Count == 0)
            {
                _hitboxes.Add(CreateLeftHandHitbox(new Vector3(0, 0, 1.0f), new Vector3(4.5f, 7.0f, 1.5f), HitboxType.EnemyHitbox, EnemyConfig.Instance.MonkeyBoss.HandClap.Damage, Vector2.zero, EnemyConfig.Instance.MonkeyBoss.HandClap.Knockback));
                _hitboxes.Add(CreateRightHandHitbox(new Vector3(0, 0, 1.0f), new Vector3(4.5f, 7.0f, 1.5f), HitboxType.EnemyHitbox, EnemyConfig.Instance.MonkeyBoss.HandClap.Damage, Vector2.zero, EnemyConfig.Instance.MonkeyBoss.HandClap.Knockback));
                //Add hitboxes to knockback other enemies
                _hitboxes.Add(CreateLeftHandHitbox(new Vector3(0, 0, 1.0f), new Vector3(4.5f, 7.0f, 1.5f), HitboxType.PlayerHitbox, 0, Vector2.zero, EnemyConfig.Instance.MonkeyBoss.HandClap.Knockback));
                _hitboxes.Add(CreateRightHandHitbox(new Vector3(0, 0, 1.0f), new Vector3(4.5f, 7.0f, 1.5f), HitboxType.PlayerHitbox, 0, Vector2.zero, EnemyConfig.Instance.MonkeyBoss.HandClap.Knockback));
            }

            if(!_inKnockback)
            {
                //Move hands together
                _leftHand.ApplyConstantMoveForce(_leftHand.transform.forward, EnemyConfig.Instance.MonkeyBoss.HandClap.XDisplacement, MAX_TIME - WAIT_TIME);
                _rightHand.ApplyConstantMoveForce(_rightHand.transform.forward, EnemyConfig.Instance.MonkeyBoss.HandClap.XDisplacement, MAX_TIME - WAIT_TIME);

                if (_playerCollision || _obsticalCollision)
                {
                    //Go into knockback
                    _leftHand.StopMotion();
                    _rightHand.StopMotion();
                    _inKnockback = true;
                    time = MAX_TIME - 0.2f;
                    _leftHand.ApplyMoveForce(-_leftHand.transform.forward, 2.0f, 0.2f);
                    _rightHand.ApplyMoveForce(-_rightHand.transform.forward, 2.0f, 0.2f);
                }
            }
            else
            {
                //If in knock back, do nothing
            }
        }

        if (time > MAX_TIME)
        {
            _inKnockback = false;
            ClearHitboxes();
            _leftHand.StopMotion();
            _rightHand.StopMotion();
            _leftHandAnimator.SetTrigger(_animParm[(int)MonkeyAnim.Idle]);
            _rightHandAnimator.SetTrigger(_animParm[(int)MonkeyAnim.Idle]);
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Returns monkey's hands after clapping
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected bool MonkeyHandsReturn(ref float time)
    {
        const float MAX_TIME = 1.0f;

        if (time == 0)
        {
            _leftHandReturnDist = Vector3.Magnitude(transform.TransformPoint(_leftHandStartPos) - _leftHand.Position);
            _rightHandReturnDist = Vector3.Magnitude(transform.TransformPoint(_rightHandStartPos) - _rightHand.Position);
        }

        //Left hand move back to original position
        Vector3 leftHandReturnDir = transform.TransformPoint(_leftHandStartPos) - _leftHand.Position;
        leftHandReturnDir.Normalize();
        _leftHand.ApplyConstantMoveForce(leftHandReturnDir, _leftHandReturnDist, MAX_TIME);
        _leftHand.Rotation = Quaternion.RotateTowards(_leftHand.Rotation, Rotation, 1.0f);

        //Right hand move back to original position
        Vector3 rightHandReturnDir = transform.TransformPoint(_rightHandStartPos) - _rightHand.Position;
        rightHandReturnDir.Normalize();
        _rightHand.ApplyConstantMoveForce(rightHandReturnDir, _rightHandReturnDist, MAX_TIME);
        _rightHand.Rotation = Quaternion.RotateTowards(_rightHand.Rotation, Rotation, 1.0f);

        if (time >= MAX_TIME)
        {
            //Set right hand back to normal
            ResetLeftHand();
            ResetRightHand();
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Charges monkey's protection attack
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected bool MonkeyProtectCharge(ref float time)
    {
        float MAX_TIME = EnemyConfig.Instance.MonkeyBoss.HandProtect.ChargeTime;

        if(time == 0)
        {
            _moveLeftWithBody = false;
            _moveRightWithBody = false;
            _animator.SetFloat(_animParm[(int)Anim.Velocity], 0);
        }

        LookAtPlayer();

        //Move right hand to side of player
        Vector3 rightHandMoveDir = transform.TransformPoint(new Vector3(2.3f, _rightHandStartPos.y, 6)) - _rightHand.Position;
        rightHandMoveDir.Normalize();
        _rightHand.ApplyConstantMoveForce(rightHandMoveDir, 6f - 2.3f, MAX_TIME);

        //Move left hand to side of player
        Vector3 leftHandMoveDir = transform.TransformPoint(new Vector3(-2.3f, _leftHandStartPos.y, 6)) - _leftHand.Position;
        leftHandMoveDir.Normalize();
        _leftHand.ApplyConstantMoveForce(leftHandMoveDir, 6f - 2.3f, MAX_TIME);

        if (time >= MAX_TIME)
        {
            _rightHand.StopMotion();
            _leftHand.StopMotion();
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Monkey Protect
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected bool MonkeyProtect(ref float time)
    {
        float MAX_TIME = EnemyConfig.Instance.MonkeyBoss.HandProtect.ProtectDuration;

        LookAtPlayer();

        _rightHand.LocalPosition = new Vector3(2.3f, _rightHandStartPos.y, 6);
        _leftHand.LocalPosition = new Vector3(-2.3f, _leftHandStartPos.y, 6);

        if(time >= MAX_TIME)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Monkey counter attacks after protecting itself
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected bool MonkeyCounter(ref float time)
    {
        float MAX_TIME = EnemyConfig.Instance.MonkeyBoss.HandProtect.CounterDuration;
        float WAIT_TIME = EnemyConfig.Instance.MonkeyBoss.HandProtect.CounterStallTime;

        if (time == 0)
        {
            if (DoTelegraphs())
            {
                CreateRightHandTelegraph(new Vector3(-2.3f, -1, 10.0f), new Vector3(10.0f, 1, 24.0f), Quaternion.identity, TelegraphType.Square, false);
            }
        }

        //Move hands after waiting a couple frames
        if (time >= WAIT_TIME)
        {
            //Remove telegraphs
            if (_telegraphs.Count > 0)
            {
                ClearTelegraphs();
            }

            //Create hitboxes
            if (_hitboxes.Count == 0)
            {
                _hitboxes.Add(CreateLeftHandHitbox(new Vector3(0, 0, 1.0f), new Vector3(4.5f, 7.0f, 1.5f), HitboxType.EnemyHitbox, EnemyConfig.Instance.MonkeyBoss.HandProtect.Damage, Vector2.zero, EnemyConfig.Instance.MonkeyBoss.HandProtect.Knockback));
                _hitboxes.Add(CreateRightHandHitbox(new Vector3(0, 0, 1.0f), new Vector3(4.5f, 7.0f, 1.5f), HitboxType.EnemyHitbox, EnemyConfig.Instance.MonkeyBoss.HandProtect.Damage, Vector2.zero, EnemyConfig.Instance.MonkeyBoss.HandProtect.Knockback));
                _hitboxes.Add(CreateLeftHandHitbox(new Vector3(0, 0, 1.0f), new Vector3(4.5f, 7.0f, 1.5f), HitboxType.PlayerHitbox, 0, Vector2.zero, EnemyConfig.Instance.MonkeyBoss.HandProtect.Knockback));
                _hitboxes.Add(CreateRightHandHitbox(new Vector3(0, 0, 1.0f), new Vector3(4.5f, 7.0f, 1.5f), HitboxType.PlayerHitbox, 0, Vector2.zero, EnemyConfig.Instance.MonkeyBoss.HandProtect.Knockback));

                _leftHandAnimator.SetTrigger(_animParm[(int)MonkeyAnim.Still]);
                _rightHandAnimator.SetTrigger(_animParm[(int)MonkeyAnim.Still]);
            }

            if (!_inKnockback)
            {
                //Move hands together
                _leftHand.ApplyConstantMoveForce(_leftHand.transform.forward, EnemyConfig.Instance.MonkeyBoss.HandProtect.Distance, MAX_TIME - WAIT_TIME);
                _rightHand.ApplyConstantMoveForce(_rightHand.transform.forward, EnemyConfig.Instance.MonkeyBoss.HandProtect.Distance, MAX_TIME - WAIT_TIME);

                if (_playerCollision || _obsticalCollision)
                {
                    //Go into knockback
                    _leftHand.StopMotion();
                    _rightHand.StopMotion();
                    _inKnockback = true;
                    time = MAX_TIME - 0.2f;
                    _leftHand.ApplyMoveForce(-_leftHand.transform.forward, 2.0f, 0.2f);
                    _rightHand.ApplyMoveForce(-_rightHand.transform.forward, 2.0f, 0.2f);
                }
            }
            else
            {
                //If in knock back, do nothing
            }
        }

        if (time > MAX_TIME)
        {
            _inKnockback = false;
            ClearHitboxes();
            _leftHand.StopMotion();
            _rightHand.StopMotion();
            _leftHandAnimator.SetTrigger(_animParm[(int)MonkeyAnim.Idle]);
            _rightHandAnimator.SetTrigger(_animParm[(int)MonkeyAnim.Idle]);
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Charges monkey's screech attack
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected bool MonkeyScreechCharge(ref float time)
    {
        float MAX_TIME = EnemyConfig.Instance.MonkeyBoss.Screech.ChargeTime;

        if(time == 0)
        {
            if(DoTelegraphs())
            {
                CreateTelegraph(new Vector3(0, -2f, EnemyConfig.Instance.MonkeyBoss.Screech.Distance/2), new Vector3(EnemyConfig.Instance.MonkeyBoss.Screech.Distance, 0, EnemyConfig.Instance.MonkeyBoss.Screech.Distance), Quaternion.identity, TelegraphType.Cone, true);
            }
            _animator.SetFloat(_animParm[(int)Anim.Velocity], _velocity.sqrMagnitude);
        }

        LookAtPlayer();

        if(time >= 0.25f && !_inKnockback)
        {
            _animator.Play(_animParm[(int)MonkeyAnim.ScreechAttack]);
            _inKnockback = true;
        }

        if(time >= MAX_TIME)
        {
            _inKnockback = false;
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Monkey releases screech attack towards player
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected bool MonkeyScreech(ref float time)
    {
        float MAX_TIME = EnemyConfig.Instance.MonkeyBoss.Screech.Duration;

        if (time == 0)
        {
            if (DoTelegraphs())
            {
                ClearTelegraphs();
            }
            ToggleScreechParticles(true);

            _hitboxes.Add(CreateHitbox(new Vector3(0, -1.0f, 4.0f), new Vector3(8.0f, 2.0f, 1.5f), HitboxType.EnemyHitbox, EnemyConfig.Instance.MonkeyBoss.Screech.Damage, Vector2.zero, EnemyConfig.Instance.MonkeyBoss.Screech.Knockback));
        }

        _hitboxes[0].transform.position += _hitboxes[0].transform.forward * (EnemyConfig.Instance.MonkeyBoss.Screech.Distance / MAX_TIME) * Time.deltaTime;

        if (time >= MAX_TIME)
        {
            ClearHitboxes();
            ToggleScreechParticles(false);
            _animator.SetTrigger(_animParm[(int)MonkeyAnim.Return]);
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Charges monkey's right hand push wave attack
    /// </summary>
    /// <returns></returns>
    protected bool MonkeyRightHandWaveCharge(ref float time)
    {
        float MAX_TIME = EnemyConfig.Instance.MonkeyBoss.HandPushWave.ChargeTime;

        if (time == 0)
        {
            _rotateRightWithBody = false;
            _moveRightWithBody = false;
            if (DoTelegraphs())
            {
                CreateRightHandTelegraph(new Vector3(0, -1f, 15.0f / transform.localScale.z), new Vector3(5.0f, 1, 30.0f / transform.localScale.z), Quaternion.identity, TelegraphType.Square, true);
            }
            _rightHandAnimator.SetTrigger(_animParm[(int)MonkeyAnim.Still]);

            _initalPos = _telegraphs[0].transform.position.y;

            _animator.SetFloat(_animParm[(int)Anim.Velocity], 0);
        }

        LookAtPlayer();

        //Look towards player
        _destination = new Vector3(PlayerPosition().x, _rightHand.transform.position.y, PlayerPosition().z);
        Quaternion desiredRotation = Quaternion.LookRotation(_destination - _rightHand.transform.position);
        _rightHand.Rotation = Quaternion.RotateTowards(_rightHand.Rotation, desiredRotation, 3.0f);

        //Move right hand down a bit
        Vector3 rightHandMoveDir = transform.TransformPoint(new Vector3(6, _rightHandStartPos.y - 2.0f, 6)) - _rightHand.Position;
        rightHandMoveDir.Normalize();
        _rightHand.ApplyConstantMoveForce(rightHandMoveDir, 2.0f, MAX_TIME);
        
        if(DoTelegraphs())
        {
            _telegraphs[0].transform.position = new Vector3(_telegraphs[0].transform.position.x, _initalPos, _telegraphs[0].transform.position.z);
        }

        if (time >= MAX_TIME)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Monkey pushes out with his right hand and creates a wave
    /// </summary>
    /// <returns></returns>
    protected bool MonkeyRightHandWaveAttack(ref float time)
    {
        float MAX_TIME = EnemyConfig.Instance.MonkeyBoss.HandPushWave.Duration;

        if (time == 0)
        {
            _rightHand.ApplyMoveForce((_rightHand.transform.forward * 5.0f + Vector3.up * 2.0f).normalized, 5.4f, MAX_TIME);
            if (DoTelegraphs())
            {
                _telegraphs[0].transform.parent = null;
                ClearTelegraphs();
            }
        }

        if (time >= MAX_TIME)
        {
            _rightHand.StopMotion();
            ForwardWave wave = Instantiate(_forwardWavePrefab, _rightHand.transform.position + Vector3.down * 3, _rightHand.Rotation).GetComponent<ForwardWave>();
            //wave.transform.localScale = new Vector3(3.0f, 4.0f, 2.0f);
            wave.StartWave(EnemyConfig.Instance.MonkeyBoss.HandPushWave.WaveDistance, EnemyConfig.Instance.MonkeyBoss.HandPushWave.WaveDuration, EnemyConfig.Instance.MonkeyBoss.HandPushWave.Damage, EnemyConfig.Instance.MonkeyBoss.HandPushWave.Knockback);
            _rightHandAnimator.SetTrigger(_animParm[(int)MonkeyAnim.Idle]);
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Charges monkey's left hand push wave attack
    /// </summary>
    /// <returns></returns>
    protected bool MonkeyLeftHandWaveCharge(ref float time)
    {
        float MAX_TIME = EnemyConfig.Instance.MonkeyBoss.HandPushWave.ChargeTime;

        if (time == 0)
        {
            _rotateLeftWithBody = false;
            _moveLeftWithBody = false;
            if (DoTelegraphs())
            {
                CreateLeftHandTelegraph(new Vector3(0, -1f, 15.0f / transform.localScale.z), new Vector3(5.0f, 1, 30.0f / transform.localScale.z), Quaternion.identity, TelegraphType.Square, true);
            }

            _rightHandReturnDist = Vector3.Magnitude(transform.TransformPoint(_rightHandStartPos) - _rightHand.Position);
            _initalPos = _telegraphs[0].transform.position.y;

            _leftHandAnimator.SetTrigger(_animParm[(int)MonkeyAnim.Still]);
        }

        LookAtPlayer();

        //left hand Looks towards player
        _destination = new Vector3(PlayerPosition().x, _leftHand.transform.position.y, PlayerPosition().z);
        Quaternion desiredRotation = Quaternion.LookRotation(_destination - _leftHand.transform.position);
        _leftHand.Rotation = Quaternion.RotateTowards(_leftHand.Rotation, desiredRotation, 3.0f);

        //Move left hand down a bit
        Vector3 leftHandMoveDir = transform.TransformPoint(new Vector3(-6, _leftHandStartPos.y - 2.0f, 6)) - _leftHand.Position;
        leftHandMoveDir.Normalize();
        _leftHand.ApplyConstantMoveForce(leftHandMoveDir, 2.0f, MAX_TIME);

        if (DoTelegraphs())
        {
            _telegraphs[0].transform.position = new Vector3(_telegraphs[0].transform.position.x, _initalPos, _telegraphs[0].transform.position.z);
        }

        //Right hand move back to original position
        Vector3 rightHandReturnDir = transform.TransformPoint(_rightHandStartPos) - _rightHand.Position;
        rightHandReturnDir.Normalize();
        _rightHand.ApplyConstantMoveForce(rightHandReturnDir, _rightHandReturnDist, MAX_TIME);
        _rightHand.Rotation = Quaternion.RotateTowards(_rightHand.Rotation, Rotation, 1.0f);

        if (time >= MAX_TIME)
        {
            ResetRightHand();
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Monkey pushes out with his left hand and creates a wave
    /// </summary>
    /// <returns></returns>
    protected bool MonkeyLeftHandWaveAttack(ref float time)
    {
        float MAX_TIME = EnemyConfig.Instance.MonkeyBoss.HandPushWave.Duration;

        if (time == 0)
        {
            _leftHand.ApplyMoveForce((_leftHand.transform.forward * 5.0f + Vector3.up * 2.0f).normalized, 5.4f, MAX_TIME);
            if (DoTelegraphs())
            {
                _telegraphs[0].transform.parent = null;
                ClearTelegraphs();
            }
        }

        if (time >= MAX_TIME)
        {
            _leftHand.StopMotion();
            ForwardWave wave = Instantiate(_forwardWavePrefab, _leftHand.transform.position + Vector3.down * 3, _leftHand.Rotation).GetComponent<ForwardWave>();
            //wave.transform.localScale = new Vector3(3.0f, 4.0f, 2.0f);
            wave.StartWave(EnemyConfig.Instance.MonkeyBoss.HandPushWave.WaveDistance, EnemyConfig.Instance.MonkeyBoss.HandPushWave.WaveDuration, EnemyConfig.Instance.MonkeyBoss.HandPushWave.Damage, EnemyConfig.Instance.MonkeyBoss.HandPushWave.Knockback);
            _leftHandAnimator.SetTrigger(_animParm[(int)MonkeyAnim.Idle]);
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Charges monkey's right hand push wave attack
    /// </summary>
    /// <returns></returns>
    protected bool MonkeyRightHandWaveRecharge(ref float time)
    {
        float MAX_TIME = EnemyConfig.Instance.MonkeyBoss.HandPushWave.ChargeTime;

        if (time == 0)
        {
            _rotateRightWithBody = false;
            _moveRightWithBody = false;
            if (DoTelegraphs())
            {
                CreateRightHandTelegraph(new Vector3(0, -1f, 15.0f / transform.localScale.z), new Vector3(5.0f, 1, 30.0f / transform.localScale.z), Quaternion.identity, TelegraphType.Square, true);
            }

            _leftHandReturnDist = Vector3.Magnitude(transform.TransformPoint(_leftHandStartPos) - _leftHand.Position);
            _initalPos = _telegraphs[0].transform.position.y;

            _rightHandAnimator.SetTrigger(_animParm[(int)MonkeyAnim.Still]);
        }

        LookAtPlayer();

        //Look towards player
        _destination = new Vector3(PlayerPosition().x, _rightHand.transform.position.y, PlayerPosition().z);
        Quaternion desiredRotation = Quaternion.LookRotation(_destination - _rightHand.transform.position);
        _rightHand.Rotation = Quaternion.RotateTowards(_rightHand.Rotation, desiredRotation, 3.0f);

        //Move right hand down a bit
        Vector3 rightHandMoveDir = transform.TransformPoint(new Vector3(6, _rightHandStartPos.y - 2.0f, 6)) - _rightHand.Position;
        rightHandMoveDir.Normalize();
        _rightHand.ApplyConstantMoveForce(rightHandMoveDir, 2.0f, MAX_TIME);

        if (DoTelegraphs())
        {
            _telegraphs[0].transform.position = new Vector3(_telegraphs[0].transform.position.x, _initalPos, _telegraphs[0].transform.position.z);
        }

        //Left hand move back to original position
        Vector3 leftHandReturnDir = transform.TransformPoint(_leftHandStartPos) - _leftHand.Position;
        leftHandReturnDir.Normalize();
        _leftHand.ApplyConstantMoveForce(leftHandReturnDir, _leftHandReturnDist, MAX_TIME);
        _leftHand.Rotation = Quaternion.RotateTowards(_leftHand.Rotation, Rotation, 1.0f);

        if (time >= MAX_TIME)
        {
            ResetLeftHand();
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Charges monkey's left hand returns after wave attack
    /// </summary>
    /// <returns></returns>
    protected bool MonkeyLeftHandWaveReturn(ref float time)
    {
        float MAX_TIME = EnemyConfig.Instance.MonkeyBoss.HandPushWave.ChargeTime;

        if (time == 0)
        {
            _leftHandReturnDist = Vector3.Magnitude(transform.TransformPoint(_leftHandStartPos) - _leftHand.Position);
        }

        //Left hand move back to original position
        Vector3 leftHandReturnDir = transform.TransformPoint(_leftHandStartPos) - _leftHand.Position;
        leftHandReturnDir.Normalize();
        _leftHand.ApplyConstantMoveForce(leftHandReturnDir, _leftHandReturnDist, MAX_TIME);
        _leftHand.Rotation = Quaternion.RotateTowards(_leftHand.Rotation, Rotation, 1.0f);

        if (time >= MAX_TIME)
        {
            ResetLeftHand();
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Charges monkey's clap attack
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected bool MonkeyCircleWaveCharge(ref float time)
    {
        float MAX_TIME = EnemyConfig.Instance.MonkeyBoss.SlamWave.ChargeTime;

        if (time == 0)
        {
            _moveRightWithBody = false;
            _moveLeftWithBody = false;
            _animator.SetFloat(_animParm[(int)Anim.Velocity], 0);
        }

        LookAtPlayer();

        //Move right hand to side of player
        Vector3 rightHandMoveDir = transform.TransformPoint(new Vector3(EnemyConfig.Instance.MonkeyBoss.SlamWave.Displacement, _rightHandStartPos.y, EnemyConfig.Instance.MonkeyBoss.SlamWave.Displacement)) - _rightHand.Position;
        rightHandMoveDir.Normalize();
        _rightHand.ApplyConstantMoveForce(rightHandMoveDir, _playerDistance, MAX_TIME);

        //Move left hand to side of player
        Vector3 leftHandMoveDir = transform.TransformPoint(new Vector3(-EnemyConfig.Instance.MonkeyBoss.SlamWave.Displacement, _leftHandStartPos.y, EnemyConfig.Instance.MonkeyBoss.SlamWave.Displacement)) - _leftHand.Position;
        leftHandMoveDir.Normalize();
        _leftHand.ApplyConstantMoveForce(leftHandMoveDir, _playerDistance, MAX_TIME);

        if (time > MAX_TIME)
        {
            //Stop motion
            _rightHand.StopMotion();
            _leftHand.StopMotion();
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Monkey pushes out with his right hand and creates a wave
    /// </summary>
    /// <returns></returns>
    protected bool MonkeyCircleWaveAttack(ref float time)
    {
        float STALL_TIME = EnemyConfig.Instance.MonkeyBoss.SlamWave.WaitTime;
        float MAX_TIME = STALL_TIME + 0.1f;

        if (time == 0)
        {
            if (DoTelegraphs())
            {
                CreateLeftHandTelegraph(Vector3.zero, new Vector3(EnemyConfig.Instance.MonkeyBoss.SlamWave.WaveDistance * 3, 1, EnemyConfig.Instance.MonkeyBoss.SlamWave.WaveDistance * 3), Quaternion.identity, TelegraphType.Circle, false);
                CreateRightHandTelegraph(Vector3.zero, new Vector3(EnemyConfig.Instance.MonkeyBoss.SlamWave.WaveDistance * 3, 1, EnemyConfig.Instance.MonkeyBoss.SlamWave.WaveDistance * 3), Quaternion.identity, TelegraphType.Circle, false);
            }
            _leftHandAnimator.Play(_animParm[(int)MonkeyAnim.Slam]);
            _rightHandAnimator.Play(_animParm[(int)MonkeyAnim.Slam]);
        }

        if(time >= STALL_TIME)
        {
            if(DoTelegraphs())
            {
                if(_telegraphs.Count > 0)
                {
                    ClearTelegraphs();
                }
            }
        }

        if (time >= MAX_TIME)
        {
            CircleWave rightWave = Instantiate(_circleWavePrefab, _rightHand.transform.position + Vector3.down * 3, _rightHand.Rotation).GetComponent<CircleWave>();
            rightWave.StartWave(EnemyConfig.Instance.MonkeyBoss.SlamWave.WaveDistance, EnemyConfig.Instance.MonkeyBoss.SlamWave.WaveDuration, EnemyConfig.Instance.MonkeyBoss.SlamWave.Damage, EnemyConfig.Instance.MonkeyBoss.SlamWave.Knockback);
            CircleWave leftWave = Instantiate(_circleWavePrefab, _leftHand.transform.position + Vector3.down * 3, _leftHand.Rotation).GetComponent<CircleWave>();
            leftWave.StartWave(EnemyConfig.Instance.MonkeyBoss.SlamWave.WaveDistance, EnemyConfig.Instance.MonkeyBoss.SlamWave.WaveDuration, EnemyConfig.Instance.MonkeyBoss.SlamWave.Damage, EnemyConfig.Instance.MonkeyBoss.SlamWave.Knockback);
            return false;
        }
        else
        {
            return true;
        }
    }
}

public partial class MonkeyStormCloud : Enemy
{
    /// <summary>
    /// Spawns a storm cloud
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected bool MonkeyStormTrackingStorm(ref float time)
    {
        float MAX_TIME = EnemyConfig.Instance.MonkeyBoss.StormCloud.TrackingStorm.StallTime;

        if (time == 0)
        {
            //Spawn storm cloud near the player
            float CIRCLE_RADIUS = EnemyConfig.Instance.MonkeyBoss.StormCloud.TrackingStorm.RadiusFromPlayer;
            Vector2 randomOnCircle = Random.insideUnitCircle.normalized * CIRCLE_RADIUS;
            Vector3 stormPosition = PlayerPosition() + new Vector3(randomOnCircle.x, 0, randomOnCircle.y);
            stormPosition += Vector3.up * 3.0f;
            Quaternion stormRotation = Quaternion.identity;

            GameObject _telegraph = null;
            if(DoTelegraphs())
            {
                _telegraph = Instantiate(_telegraphPrefab[(int)TelegraphType.Circle], stormPosition + Vector3.down * 6, stormRotation);
                _telegraph.transform.localScale = new Vector3(6, 6, 6);
            }

            //Spawn in storm cloud
            Instantiate(_stormCloud, stormPosition, stormRotation)
                .GetComponent<StormCloud>()
                .SetStorm(EnemyConfig.Instance.MonkeyBoss.StormCloud.TrackingStorm.Duration, EnemyConfig.Instance.MonkeyBoss.StormCloud.TrackingStorm.Damage, EnemyConfig.Instance.MonkeyBoss.StormCloud.TrackingStorm.Knockback, Vector3.zero, 0, _telegraph);
        }

        if (time >= MAX_TIME)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Creates monkey storm clouds in a circle and fires them towards the players
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected bool MonkeyStormCircleStorm(ref float time)
    {
        float MAX_TIME = EnemyConfig.Instance.MonkeyBoss.StormCloud.CircleStorm.StallTime;

        if (time == 0)
        {
            //Spawn storms in cicle around player
            float CIRCLE_RADIUS = EnemyConfig.Instance.MonkeyBoss.StormCloud.CircleStorm.Radius;
            float stormClouds = Random.Range(EnemyConfig.Instance.MonkeyBoss.StormCloud.CircleStorm.MinStormClouds, EnemyConfig.Instance.MonkeyBoss.StormCloud.CircleStorm.MaxStormClouds);
            float stormPerDegree = 360 / stormClouds;
            for (int i = 0; i < stormClouds; i++)
            {
                Vector3 stormPosition = transform.position + Quaternion.Euler(0, stormPerDegree * i, 0) * transform.forward * CIRCLE_RADIUS + Vector3.down * 10f;
                Vector3 stormDistanceVec = stormPosition - transform.position;
                Vector3 stormDirection = new Vector3(stormDistanceVec.x, 0, stormDistanceVec.z);
                Quaternion stormRotation = Quaternion.LookRotation(stormDirection);
                //Spawn in storm cloud
                Instantiate(_stormCloud, stormPosition, stormRotation, transform)
                    .GetComponent<StormCloud>()
                    .SetStorm(EnemyConfig.Instance.MonkeyBoss.StormCloud.CircleStorm.Duration, EnemyConfig.Instance.MonkeyBoss.StormCloud.CircleStorm.Damage, EnemyConfig.Instance.MonkeyBoss.StormCloud.CircleStorm.Knockback, stormDirection.normalized, EnemyConfig.Instance.MonkeyBoss.StormCloud.CircleStorm.Distance, null);

                if (DoTelegraphs())
                {
                    CreateTelegraph(transform.InverseTransformVector(stormDirection.normalized) * (EnemyConfig.Instance.MonkeyBoss.StormCloud.CircleStorm.Distance / 2f) + Vector3.down * 12f, new Vector3(3f, 1, EnemyConfig.Instance.MonkeyBoss.StormCloud.CircleStorm.Distance), stormRotation, TelegraphType.Square, true);
                    if(stormRotation == Quaternion.identity)
                    {
                        _telegraphs[_telegraphs.Count - 1].transform.rotation = Quaternion.identity;
                    }
                }
            }
        }

        if (time > MAX_TIME)
        {
            ClearTelegraphs();
            return false;
        }
        else
        {
            return true;
        }
    }
}

public partial class Waterdeer : Enemy
{
    /// <summary>
    /// Mox charges dash for 1 secondish
    /// </summary>
    /// <param name="time">Current time</param>
    /// <returns></returns>
    protected bool WaterdeerDashCharge(ref float time)
    {
        float MAX_TIME = EnemyConfig.Instance.Waterdeer.DashAttack.ChargeTime;
        float STALL_TIME = EnemyConfig.Instance.Waterdeer.DashAttack.StallTime;

        //Stop motion at begining of charge
        if (time == 0)
        {
            StopMotion();
            if (DoTelegraphs())
            {
                CreateTelegraph(new Vector3(0, -0.5f, (_lengthMult + 20f * (1 + _enemyUpgrades.masterUpgrade[StatusType.Speed]) / 2f) / transform.localScale.z), new Vector3(_widthMult, 1, 17 * (1 +_enemyUpgrades.masterUpgrade[StatusType.Speed]) / transform.localScale.z), Quaternion.identity, TelegraphType.Square, true);
            }
        }

        if (time <= MAX_TIME - STALL_TIME)
        {
            //Look towards player
            _destination = new Vector3(PlayerPosition().x, transform.position.y, PlayerPosition().z);
            Quaternion desiredRotation = Quaternion.LookRotation(_destination - transform.position);
            SetSmoothRotation(desiredRotation, 1.5f, 1.0f, 4.0f);
            //rotation = Quaternion.RotateTowards(rotation, Quaternion.LookRotation(destination - transform.position), 1.0f);
        }

        if (time >= MAX_TIME)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Dashes at player for 1 second
    /// </summary>
    /// <param name="time">Current Time</param>
    /// <returns></returns>
    protected bool WaterdeerDashAttack(ref float time)
    {
        float MAX_TIME = EnemyConfig.Instance.Waterdeer.DashAttack.Duration;

        //Add hitbox and start dash
        if (time == 0.0f)
        {
            _hitboxes.Add(CreateHitbox(Vector3.forward * 2.2f + Vector3.up * 0.2f, new Vector3(3, 3, 3), HitboxType.EnemyHitbox, EnemyConfig.Instance.Waterdeer.DashAttack.Damage, Vector2.zero, EnemyConfig.Instance.Waterdeer.DashAttack.Knockback));
            ApplyMoveForce(transform.forward, EnemyConfig.Instance.Waterdeer.DashAttack.Distance * (1 + _enemyUpgrades.masterUpgrade[StatusType.Speed]), MAX_TIME);
            _animator.SetFloat(_animParm[(int)WaterdeerAnim.SwimSpeed], 2.0f);
            if (DoTelegraphs())
            {
                _telegraphs[0].transform.parent = null;
                ClearTelegraphs();
            }
        }

        if (!_inKnockback)
        {
            if (_playerCollision || _obsticalCollision)
            {
                //Go into knockback
                StopMotion();
                _inKnockback = true;
                time = MAX_TIME - 0.2f;
                ApplyMoveForce(-transform.forward, 2.0f * _speed, 0.2f);
            }
            //ApplyFriction(0.25f);
        }
        //Do knockback if there was a hit
        else
        {
            //Do nothing
        }

        _animator.SetFloat(_animParm[(int)Anim.Velocity], _velocity.sqrMagnitude);

        if (time >= MAX_TIME)
        {
            GameObject.Destroy(_hitboxes[_hitboxes.Count - 1]);
            _hitboxes.RemoveAt(_hitboxes.Count - 1);
            _inKnockback = false;
            StopMotion();
            _animator.SetFloat(_animParm[(int)WaterdeerAnim.SwimSpeed], 1.0f);
            return false;
        }
        else
        {
            return true;
        }
    }
}

public partial class BombCrab : Enemy
{
    /// <summary>
    /// Bomb crab charges explosion
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected bool BombCrabChargeExplosion(ref float time)
    {
        float MAX_TIME = EnemyConfig.Instance.BombCrab.Explosion.ChargeTime;

        if(time == 0)
        {
            //Play explosion animation
            _animator.Play(_animParm[(int)BombCrabAnim.Explode]);
            StopMotion();
        }

        if(time >= MAX_TIME)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Bomb crab charges explosion
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected bool BombCrabExplode(ref float time)
    {
        if (time == 0)
        {
            _explosionParticles.Play();
            float radius = EnemyConfig.Instance.BombCrab.Explosion.DamageRadius;
            _hitboxes.Add(CreateHitbox(new Vector3(0, 0, 0), new Vector3(radius, radius, radius), HitboxType.EnemyHitbox, EnemyConfig.Instance.BombCrab.Explosion.Damage, Vector2.zero, EnemyConfig.Instance.BombCrab.Explosion.Knockback));
            _hitboxes.Add(CreateHitbox(new Vector3(0, 0, 0), new Vector3(radius, radius, radius), HitboxType.PlayerHitbox, EnemyConfig.Instance.BombCrab.Explosion.Damage / 2, Vector2.zero, EnemyConfig.Instance.BombCrab.Explosion.Knockback));
        }

        if (time > 0)
        {
            ClearHitboxes();
            TakeDamage(9999);
            return false;
        }
        else
        {
            return true;
        }
    }
}