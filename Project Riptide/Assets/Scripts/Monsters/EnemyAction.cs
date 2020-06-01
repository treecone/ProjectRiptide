using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Enemy : Physics
{
    //General method for making the moster follow the player
    //Usually should be used for enemy AI when not in an action
    private void FollowPlayer()
    {
        Vector3 destination = Vector3.zero;
        //Check for obstacle
        if (CheckObstacle(new Vector3(_PlayerPosition().x, transform.position.y, _PlayerPosition().z)))
        {
            //Set destination to closest way to player that avoids obstacles
            destination = transform.position + AvoidObstacle(new Vector3(_PlayerPosition().x, transform.position.y, _PlayerPosition().z));
        }
        else
        {
            //Set destination to player
            destination = new Vector3(_PlayerPosition().x, transform.position.y, _PlayerPosition().z);
        }
        //Seek destination
        Vector3 netForce = Seek(destination);
        netForce += new Vector3(transform.forward.x, 0, transform.forward.z).normalized * 2.0f;

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

    //General method for making the moster follow the player
    //Usually should be used for enemy AI when not in an action
    private void FleePlayer(float speed)
    {
        Vector3 destination = Vector3.zero;
        Vector3 avoidDirection = (transform.position - _PlayerPosition()) + transform.position;
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
        Vector3 netForce = Seek(destination);
        netForce += new Vector3(transform.forward.x, 0, transform.forward.z).normalized * 2.0f;

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

    //General method for making the moster circle around the player
    //Used when monster is too close to player
    private void CirclePlayer()
    {
        //Calculate net force
        Vector3 netForce = new Vector3(transform.forward.x, 0, transform.forward.z).normalized * 2.0f;

        if (CheckObstacle(new Vector3(_PlayerPosition().x, transform.position.y, _PlayerPosition().z)))
        {
            netForce += transform.position + AvoidObstacle(new Vector3(_PlayerPosition().x, transform.position.y, _PlayerPosition().z));
        }
        else
        {
            Vector3 crossForce = _PlayerPosition() - transform.position;
            crossForce = new Vector3(crossForce.x, 0, crossForce.z);
            crossForce.Normalize();
            crossForce *= 1f;
            crossForce = Vector3.Cross(Vector3.up, crossForce);
            crossForce = new Vector3(crossForce.x, 0, crossForce.z);
            netForce += crossForce;
        }

        //Rotate in towards direction of velocity
        if (_velocity != Vector3.zero)
        {
            Quaternion desiredRotation = Quaternion.LookRotation(_velocity);
            SetSmoothRotation(desiredRotation, 1.0f, 0.5f, 2.0f);
        }

        ApplyForce(netForce);
    }

    /// <summary>
    /// Go through enemy's action queue and play each action
    /// Returns false when finished
    /// </summary>
    /// <returns></returns>
    private bool DoActionQueue()
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
    private bool DashCharge(ref float time)
    {
        const float MAX_TIME = 2.0f;

        //Stop motion before charge
        if (time == 0)
        {
            StopMotion();
        }

        //Look towards player
        _destination = new Vector3(_PlayerPosition().x, transform.position.y, _PlayerPosition().z);
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
    private bool DashAttack(ref float time)
    {
        const float MAX_TIME = 1.0f;

        //Add hitbox at begining
        if (time == 0.0f)
        {
            _hitboxes.Add(CreateHitbox(transform.position + transform.forward * 3.0f, new Vector3(1, 1, 1), HitboxType.EnemyHitbox, _ramingDamage));
        }

        if (!_inKnockback)
        {
            //If monster hits player, stop special
            if (_playerCollision || _obsticalCollision)
            {
                _inKnockback = true;
                if(_playerCollision)
                {
                    Vector3 knockback = _PlayerPosition() - transform.position;
                    knockback.Normalize();
                    knockback *= 40.0f;
                    _SendKnockback(knockback);
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
    /// Transition to charge attack by slowing down
    /// </summary>
    /// <param name="time">Current time</param>
    /// <returns></returns>
    private bool KoiStopTransition(ref float time)
    {
        const float MAX_TIME = 0.25f;

        ApplyFriction(0.50f);

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
    private bool KoiDashCharge(ref float time)
    {
        const float MAX_TIME = 1.5f;
        const float STALL_TIME = 0.2f;

        //Stop motion at begining of charge
        if (time == 0)
        {
            StopMotion();
        }

        if (time <= MAX_TIME - STALL_TIME)
        {
            //Look towards player
            _destination = new Vector3(_PlayerPosition().x, transform.position.y, _PlayerPosition().z);
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
    private bool KoiDashAttack(ref float time)
    {
        const float MAX_TIME = 1.0f;

        //Add hitbox and start dash
        if (time == 0.0f)
        {
            _hitboxes.Add(CreateHitbox(transform.position + transform.forward * 1.5f * transform.localScale.x, new Vector3(1, 1, 1) * (transform.localScale.x / 2.0f), HitboxType.EnemyHitbox, _ramingDamage, Vector2.zero, 500));
            ApplyMoveForce(transform.forward, 30.0f * _speed, 1.0f);
            _animator.SetFloat(_animParm[(int)CarpAnim.SwimSpeed], 2.0f);
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

        if (time >= MAX_TIME)
        {
            GameObject.Destroy(_hitboxes[_hitboxes.Count - 1]);
            _hitboxes.RemoveAt(_hitboxes.Count - 1);
            _inKnockback = false;
            StopMotion();
            _animator.SetFloat(_animParm[(int)CarpAnim.SwimSpeed], 1.0f);
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
    private bool KoiBubbleBlastTransitionDown(ref float time)
    {
        const float MAX_TIME = 1.0f;

        if(time == 0)
        {
            StopMotion();
            _animator.SetTrigger(_animParm[(int)CarpAnim.Dive]);
        }

        ApplyConstantMoveForce(Vector3.down, 3.0f * transform.localScale.y, 1.0f);

        if(time >= MAX_TIME)
        {
            _animator.ResetTrigger(_animParm[(int)CarpAnim.Dive]);
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
    private bool KoiBubbleBlastTransitionUp(ref float time)
    {
        const float MAX_TIME = 1.0f;

        if (time == 0)
        {
            StopMotion();
            _animator.SetTrigger(_animParm[(int)CarpAnim.Shoot]);
        }

        ApplyConstantMoveForce(Vector3.up, 3.0f * transform.localScale.y, 1.0f);

        if (time >= MAX_TIME)
        {
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
    private bool KoiBubbleBlastCharge(ref float time)
    {
        const float MAX_TIME = 2.0f;
        const float STALL_TIME = 0.1f;

        if (time < MAX_TIME - STALL_TIME)
        {
            //Stop motion at start
            if (time == 0)
            {
                StopMotion();
            }

            //Look towards player
            _destination = new Vector3(_PlayerPosition().x, transform.position.y, _PlayerPosition().z);
            Quaternion desiredRotation = Quaternion.LookRotation(_destination - transform.position);
            SetSmoothRotation(desiredRotation, 1.0f, 0.5f, 3.0f);
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
    /// Used for KoiBoss, sends out 7 projectiles in an arc
    /// </summary>
    /// <param name="time">Current time</param>
    /// <returns></returns>
    private bool KoiBubbleBlastAttack(ref float time)
    {
        //Spawn projectiles
        SpawnProjectile(new Vector3(0, 0, (5 * _lengthMult / 6)), 0.5f, 10, 3.0f, MovementPattern.Forward, Vector2.zero, 200);
        SpawnProjectile(new Vector3(-0.10f, 0, (5 * _lengthMult / 6) - 0.25f), 0.5f, 10, 3.0f, MovementPattern.Forward, Vector2.zero, 200);
        SpawnProjectile(new Vector3(-0.25f, 0, (5 * _lengthMult / 6) - 0.75f), 0.5f, 10, 3.0f, MovementPattern.Forward, Vector2.zero, 200);
        SpawnProjectile(new Vector3(-0.50f, 0, (5 * _lengthMult / 6) - 1.50f), 0.5f, 10, 3.0f, MovementPattern.Forward, Vector2.zero, 200);
        SpawnProjectile(new Vector3(0.10f, 0, (5 * _lengthMult / 6) - 0.25f), 0.5f, 10, 3.0f, MovementPattern.Forward, Vector2.zero, 200);
        SpawnProjectile(new Vector3(0.25f, 0, (5 * _lengthMult / 6) - 0.75f), 0.5f, 10, 3.0f, MovementPattern.Forward, Vector2.zero, 200);
        SpawnProjectile(new Vector3(0.50f, 0, (5 * _lengthMult / 6) - 1.50f), 0.5f, 10, 3.0f, MovementPattern.Forward, Vector2.zero, 200);

        return false;
    }

    /// <summary>
    /// Used for KoiBoss, sends out one projectile towards player
    /// </summary>
    /// <param name="time">Current time</param>
    /// <returns></returns>
    private bool KoiBubbleAttack(ref float time)
    {
        SpawnProjectile(new Vector3(0, 0, 5 * _lengthMult / 6), 1.0f, 10, 3.0f, MovementPattern.Forward, Vector2.zero, 200);

        return false;
    }

    /// <summary>
    /// Used for KoiBoss, Koi dashes three times at player going above and bellow the water
    /// </summary>
    /// <param name="time">Current time</param>
    /// <returns></returns>
    private bool KoiUnderwaterDash(ref float time)
    {
        const float MAX_TIME = 1.0f;

        //Start dashing
        if (time == 0.0f)
        {
            _hitboxes.Add(CreateHitbox(transform.position + transform.forward * 1.5f * transform.localScale.x, new Vector3(1, 1, 1) * (transform.localScale.x / 2.0f), HitboxType.EnemyHitbox, _ramingDamage));
            _gravity = ApplyArcForce(transform.forward, 30.0f * _speed, 2f * transform.localScale.y, 1.0f);
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
                ReturnToInitalPosition();
                time = MAX_TIME;
            }
        }

        if (time >= MAX_TIME)
        {
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

    /// <summary>
    /// Used for KoiBoss, Koi comes above water for player to get a chance
    /// to attack, then goes back under.
    /// </summary>
    /// <param name="time">Current Time</param>
    /// <returns></returns>
    private bool KoiUnderwaterDashReturn(ref float time)
    {
        const float MAX_TIME = 4.0f;
        const float STALL_TIME = 3.0f;

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
        else if(time < MAX_TIME + 0.5f)
        {
            _animator.SetTrigger(_animParm[(int)CarpAnim.Dive]);
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
    private bool KoiBubbleBlastUnderwaterCharge(ref float time)
    {
        const float MAX_TIME = 1.0f;

        if (time == 0)
        {
            StopMotion();
            _animator.SetTrigger(_animParm[(int)CarpAnim.Shoot]);
        }

        //Move fish out of water
        ApplyConstantMoveForce(Vector3.up, 1.0f * transform.localScale.y, 1.0f);

        if (time >= MAX_TIME)
        {
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
    private bool KoiBubbleBlastReturn(ref float time)
    {
        const float MAX_TIME = 2.0f;
        const float STALL_TIME = 1.5f;

        if (time < STALL_TIME)
        {
            //Do nothing, give player chance to attack
        }
        else if (time >= STALL_TIME && time < MAX_TIME)
        {
            _animator.SetTrigger(_animParm[(int)CarpAnim.Dive]);
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
    private bool KoiUnderwaterFollow(ref float time)
    {
        const float MAX_TIME = 4.5f;
        const float STALL_TIME = 0.5f;

        if (time < MAX_TIME - STALL_TIME)
        {
            Vector3 destination = Vector3.zero;
            if (CheckObstacle(new Vector3(_PlayerPosition().x, transform.position.y, _PlayerPosition().z)))
            {
                destination = transform.position + AvoidObstacle(new Vector3(_PlayerPosition().x, transform.position.y, _PlayerPosition().z));
            }
            else
            {
                destination = new Vector3(_PlayerPosition().x, transform.position.y, _PlayerPosition().z);
            }
            Vector3 netForce = Seek(destination);
            netForce += new Vector3(transform.forward.x, 0, transform.forward.z).normalized * 2.0f;

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

        if (time >= MAX_TIME)
        {
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
    private bool KoiUnderwaterAttack(ref float time)
    {
        //This one's too complicated to add constant times to work with

        if (time == 0.0f)
        {
            _hitboxes.Add(CreateHitbox(transform.position, new Vector3(0.66f, 1.66f, 4) * transform.localScale.x / 2.0f, HitboxType.EnemyHitbox, _ramingDamage, new Vector2(90, 0), 1000));
            _gravity = ApplyArcForce(Vector3.up, 0.0f, 15.0f, 1.0f);
            _animator.SetTrigger(_animParm[(int)CarpAnim.UAttack]);
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
                    time = 3.95f;
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
            StopMotion();
            Destroy(_hitboxes[_hitboxes.Count - 1]);
            _hitboxes.RemoveAt(_hitboxes.Count - 1);
        }

        //If player was not hit, make the koi go back under water
        if (time >= 3.95f && !_inKnockback)
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
        else if (time >= 3.95f)
        {
            StopMotion();
            _inKnockback = false;
            return false;
        }

        return true;
    }

    /// <summary>
    /// Used for Rock Crab
    /// Crab flings itself forward towards the player
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    private bool CrabRockFling(ref float time)
    {
        const float MAX_TIME = 1.0f;

        //Start dashing
        if (time == 0.0f)
        {
            _hitboxes.Add(CreateHitbox(transform.position, new Vector3(1.66f, 3f, 1.66f) * transform.localScale.x / 2.0f, HitboxType.EnemyHitbox, _ramingDamage, new Vector2(90, 0), 1000));
            _gravity = ApplyArcForce(transform.forward, _playerDistance * _speed * 1.5f, 2f * transform.localScale.y, 1.0f);
            _animator.SetTrigger(_animParm[(int)CrabAnim.Jump]);
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