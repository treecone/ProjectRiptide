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
    protected void FleePlayer(float speed)
    {
        Vector3 destination = Vector3.zero;
        Vector3 avoidDirection = (transform.position - PlayerPosition()) + transform.position;
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
            _hitboxes.Add(CreateHitbox(transform.forward * 3.0f, new Vector3(1, 1, 1), HitboxType.EnemyHitbox, _ramingDamage));
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
            _hitboxes.Add(CreateHitbox(transform.forward * 2.2f * transform.localScale.x, new Vector3(1, 1, 1) * (transform.localScale.x / 2.0f), HitboxType.EnemyHitbox, _ramingDamage, Vector2.zero, 500));
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
    protected bool KoiBubbleBlastTransitionDown(ref float time)
    {
        const float MAX_TIME = 1.0f;

        if (time == 0)
        {
            StopMotion();
            _animator.SetTrigger(_animParm[(int)CarpAnim.Dive]);
        }

        ApplyConstantMoveForce(Vector3.down, 3.0f * transform.localScale.y, 1.0f);

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
        const float MAX_TIME = 1.0f;

        if (time == 0)
        {
            StopMotion();
            _animator.SetTrigger(_animParm[(int)CarpAnim.Shoot]);
        }

        ApplyConstantMoveForce(Vector3.up, 3.0f * transform.localScale.y, 1.0f);

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
            _destination = new Vector3(PlayerPosition().x, transform.position.y, PlayerPosition().z);
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
    protected bool KoiBubbleBlastAttack(ref float time)
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
    protected bool KoiBubbleAttack(ref float time)
    {
        SpawnProjectile(new Vector3(0, 0, 5 * _lengthMult / 6), 1.0f, 10, 3.0f, MovementPattern.Forward, Vector2.zero, 200);

        return false;
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
            _hitboxes.Add(CreateHitbox(transform.forward * 1.5f * transform.localScale.x, new Vector3(1, 1, 1) * (transform.localScale.x / 2.0f), HitboxType.EnemyHitbox, _ramingDamage));
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
                time = MAX_TIME;
            }
        }

        if (time >= MAX_TIME)
        {
            StopMotion();
            GameObject.Destroy(_hitboxes[_hitboxes.Count - 1]);
            _hitboxes.RemoveAt(_hitboxes.Count - 1);
            ReturnToInitalPosition();
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
    protected bool KoiUnderwaterDashReturn(ref float time)
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
        else if (time < MAX_TIME + 0.5f)
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
    protected bool KoiBubbleBlastUnderwaterCharge(ref float time)
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
    protected bool KoiUnderwaterFollow(ref float time)
    {
        const float MAX_TIME = 4.5f;
        const float STALL_TIME = 0.5f;

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
    protected bool KoiUnderwaterAttack(ref float time)
    {
        //This one's too complicated to add constant times to work with

        if (time == 0.0f)
        {
            _hitboxes.Add(CreateHitbox(Vector3.zero, new Vector3(0.66f, 1.66f, 4) * transform.localScale.x / 2.0f, HitboxType.EnemyHitbox, _ramingDamage, new Vector2(90, 0), 1000));
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
            _position = new Vector3(_position.x, _startPos.y, _position.z);
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
}

//Actions available to rock crabs
public partial class RockCrab : Enemy
{
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
            _hitboxes.Add(CreateHitbox(Vector3.zero, new Vector3(1.66f, 3f, 1.66f) * transform.localScale.x / 2.0f, HitboxType.EnemyHitbox, _ramingDamage, new Vector2(90, 0), 1000));
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

public partial class FlowerFrog : Enemy
{
    /// <summary>
    /// Frog rotates towards player to prepare to shoot tounge
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected bool ToungeCharge(ref float time)
    {
        const float MAX_TIME = 1.5f;
        const float STALL_TIME = 0.0f;

        if (time < MAX_TIME - STALL_TIME)
        {
            //Stop motion at start
            if (time == 0)
            {
                StopMotion();
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

        if(time == 0)
        {
            _animator.SetTrigger(_animParm[(int)FrogAnim.Attack]);
            _hitboxes.Add(CreateHitbox(_tounge.transform.localPosition, new Vector3(1, 1, 1), HitboxType.EnemyHitbox, 0));
            _hitboxes[_hitboxes.Count - 1].transform.parent = _tounge.transform;
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
        float MAX_TIME = 2.0f * _speedScale;

        if(time == 0)
        {
            //Spawn Tentcales around clam in a radius
            const float CIRCLE_TENTACLES = 10;
            const float CIRCLE_RADIUS = 7.0f;
            float tentaclePerDegree = 360 / 10;
            for (int i = 0; i < CIRCLE_TENTACLES; i++)
            {
                Vector3 tentaclePosition = transform.position + Quaternion.Euler(0, tentaclePerDegree * i, 0) * transform.forward * CIRCLE_RADIUS;
                Quaternion tentacleRotation = Quaternion.LookRotation(tentaclePosition - transform.position);
                Instantiate(_tentaclePrefab, tentaclePosition + Vector3.up * -2, tentacleRotation)
                    .GetComponent<ClamTentacle>()
                    .SetTentacle(TentacleMode.StationarySlap, PlayerPosition, 5.0f, 2.0f, 1.0f, 2.0f, _speedScale, 1);
            }
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
    /// Spawns a tentacle near the player that
    /// will track their movement to try and hit them
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    protected bool ClamTrackAttack(ref float time)
    {
        float MAX_TIME = 1.0f * _speedScale;

        if (time == 0)
        {
            //Spawn Tentcale near the player
            const float CIRCLE_RADIUS = 5.0f;
            Vector2 randomOnCircle = Random.insideUnitCircle.normalized * CIRCLE_RADIUS;
            Vector3 tentaclePosition = PlayerPosition() + new Vector3(randomOnCircle.x, 0, randomOnCircle.y);
            Quaternion tentacleRotation = Quaternion.LookRotation(tentaclePosition - PlayerPosition());
            Instantiate(_tentaclePrefab, tentaclePosition, tentacleRotation)
                .GetComponent<ClamTentacle>()
                .SetTentacle(TentacleMode.TrackingSlap, PlayerPosition, 5.0f, 2.0f, 1.0f, 1.0f, _speedScale, 1);
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
        float MAX_TIME = 0.25f * _speedScale;

        if (time == 0)
        {
            //Spawn Tentcale in a line towards the player
            Vector3 tentaclePosition = transform.position + _lineForward * _lineOffset;
            Quaternion tentacleRotation = _rotation;
            Instantiate(_tentaclePrefab, tentaclePosition + Vector3.up * -2f, tentacleRotation)
                .GetComponent<ClamTentacle>()
                .SetTentacle(TentacleMode.RisingAttack, PlayerPosition, 10.0f, 1.0f, 1.0f, 1.0f, _speedScale, 1.5f);
            _lineOffset += 5.0f;
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
        const float MAX_TIME = 2.0f;

        if (time == 0)
        {
            //Spawn Tentcales around clam in a radius
            const float CIRCLE_TENTACLES = 50;
            const float CIRCLE_RADIUS = 30.0f;
            for (int i = 0; i < CIRCLE_TENTACLES; i++)
            {
                Vector2 randomOnCircle = Random.insideUnitCircle * CIRCLE_RADIUS;
                Vector3 tentaclePosition = transform.position + new Vector3(randomOnCircle.x, 0, randomOnCircle.y);
                Instantiate(_tentaclePrefab, tentaclePosition + Vector3.up * -2, _rotation)
                    .GetComponent<ClamTentacle>()
                    .SetTentacle(TentacleMode.RisingAttack, PlayerPosition, 10.0f, 1.0f, 1.0f, 1.0f, _speedScale, 1);
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
        const float MAX_TIME = 2.0f;

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
        const float MAX_TIME = 2.0f;

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
        const float MAX_TIME = 2.0f;

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
}