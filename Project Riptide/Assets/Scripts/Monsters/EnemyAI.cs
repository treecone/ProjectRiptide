using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum KoiAttack { TripleDash = 2, BubbleBlast = 4, UnderwaterAttack = 3, BubbleAttack = 3 }
public enum AttackState { Active = 0, FormChanged = 1, FormChangeInProgress = 2 }

public partial class Enemy : Physics
{
    /// <summary>
    /// Moves the monster randomly within a certain radius
    /// </summary>
    public void PassiveWanderRadius()
    {
        //If the monster is currently outside the wander radius, go back to the radius.
        //This is important if the monster 
        if (_enemyDistance > _wanderRadius)
        {
            _destination = _startPos;
        }
        else
        {
            //If time between movements is over select a new destination
            if (_timeCurrent >= _timeBetween)
            {
                //Select new destination that is inside wander radius
                do
                {
                    _destination = new Vector3(transform.position.x + Random.Range(-10, 10), transform.position.y, transform.position.z + Random.Range(-10, 10));
                } while (Vector3.Distance(_destination, _startPos) > _wanderRadius);
                _timeCurrent = 0;
            }
        }

        Vector3 destination = Vector3.zero;
        //Check for obstacle
        if (CheckObstacle(_destination))
        {
            //Set destination to closest way to player that avoids obstacles
            destination = transform.position + AvoidObstacle(_destination);
        }
        else
        {
            //Set destination to player
            destination = _destination;
        }
        //Seek destination
        Vector3 netForce = Seek(destination);
        netForce += new Vector3(transform.forward.x, 0, transform.forward.z).normalized * 2.0f;

        //Rotate in towards direction of velocity
        if (_velocity != Vector3.zero)
        {
            Quaternion desiredRotation = Quaternion.LookRotation(_velocity);
            SetSmoothRotation(desiredRotation, 1.0f, 0.5f, 2.0f);
        }
        _timeCurrent += Time.deltaTime;

        ApplyForce(netForce);

        //ApplyFriction(0.25f);
        if(_animator != null)
            _animator.SetFloat(_animParm[(int)Anim.Velocity], _velocity.sqrMagnitude);
    }

    /// <summary>
    /// Passive enemy AI where the enemy does not move while passive
    /// </summary>
    public void PassiveDoNothing()
    {
        //Do nothing
    }

    /// <summary>
    /// Most moves towards the player and circles him
    /// On certain intervals the monster will charge up
    /// and dash at the player then continue to circle them.
    /// If they hit a player or object, the dash stops and 
    /// they bounce back
    /// </summary>
    public void HostileFollowAndDash()
    {
        //If enemy is outside max radius, set to passive
        if (_enemyDistance > _maxRadius)
        {
            _state = EnemyState.Passive;
            ResetHostile();
            //Keep monster passive for 5 seconds at least
            _passiveCooldown = 5.0f;
        }
        else
        {
            //If enemy is not in special
            if (!_activeStates[0])
            {
                //Follow the player
                FollowPlayer();

                //Cooldown special while in a 10 units of player
                if (_playerDistance < 10.0f)
                {
                    _specialCooldown[0] -= Time.deltaTime;
                }
                //If cooldown is finished, switch to special
                if (_specialCooldown[0] <= 0)
                {
                    _activeStates[0] = true;
                    _currTime = 0;
                    //Load an attack that charges a dash then attacks
                    _actionQueue.Enqueue(DashCharge);
                    _actionQueue.Enqueue(DashAttack);
                }
            }
            else
            {
                //Go through enmeies action queue
                if (!DoActionQueue())
                {
                    _activeStates[0] = false;
                    _specialCooldown[0] = 5.0f;
                }
            }
        }
    }

    /// <summary>
    /// Defines the AI for the Giant Koi Boss fight
    /// Phase 1 >50% health
    /// triple dash attack
    /// projectile attack
    /// charged projectile attack
    /// Phase 2 less than 50% health
    /// goes underwater
    /// triple dash attack
    /// charged projectile attack
    /// underwater attack (maybe)
    /// </summary>
    public void KoiBossHostile()
    {
        //If enemy is outside max radius, set to passive
        if (_enemyDistance > _maxRadius)
        {
            _state = EnemyState.Passive;
            ResetHostile();
            //Keep monster passive for 5 seconds at least
            _passiveCooldown = 5.0f;
        }
        else
        {
            //Phase 1: greater than 50% health
            if (_health > _maxHealth / 2)
            {
                //If the Koi is not in any special
                if (!_activeStates[(int)AttackState.Active])
                {
                    if (_playerDistance > 6.0f)
                    {
                        FollowPlayer();
                    }
                    else
                    {
                        //CirclePlayer();
                    }

                    //Decrement overall special cooldown, no special can be used while this is in cooldown.
                    if (_specialCooldown[(int)AttackState.Active] > 0)
                        _specialCooldown[(int)AttackState.Active] -= Time.deltaTime;

                    //Check to see if monster can use triple dash special attack
                    if (_playerDistance < 13.0f)
                    {
                        _specialCooldown[(int)KoiAttack.TripleDash] -= Time.deltaTime;
                        if (_specialCooldown[(int)AttackState.Active] < 0.0f && _specialCooldown[(int)KoiAttack.TripleDash] < 0.0f && Random.Range(1, 4) == 1)
                        {
                            _activeStates[(int)AttackState.Active] = true;
                            _specialCooldown[(int)AttackState.Active] = 5.0f;
                            _specialCooldown[(int)KoiAttack.TripleDash] = 6.0f;
                            _currTime = 0;
                            //Set up triple dash attack
                            _actionQueue.Enqueue(KoiStopTransition);
                            _actionQueue.Enqueue(KoiDashCharge);
                            _actionQueue.Enqueue(KoiDashAttack);
                            _actionQueue.Enqueue(KoiDashCharge);
                            _actionQueue.Enqueue(KoiDashAttack);
                            _actionQueue.Enqueue(KoiDashCharge);
                            _actionQueue.Enqueue(KoiDashAttack);
                        }
                    }

                    //Check to see if monster can use bubble attack
                    if (_playerDistance > 10.0f)
                    {
                        _specialCooldown[(int)KoiAttack.BubbleAttack] -= Time.deltaTime;
                        if (_specialCooldown[(int)AttackState.Active] < 0.0f && _specialCooldown[(int)KoiAttack.BubbleAttack] < 0.0f && Random.Range(1, 4) == 1)
                        {
                            //Load projectile
                            _activeStates[(int)AttackState.Active] = true;
                            _specialCooldown[(int)AttackState.Active] = 2.0f;
                            _specialCooldown[(int)KoiAttack.BubbleAttack] = 3.0f;
                            _currTime = 0;
                            _actionQueue.Enqueue(KoiBubbleAttack);
                        }
                    }

                    //Check to see if player can use charge projectile special attack
                    if (_playerDistance < 20.0f)
                    {
                        _specialCooldown[(int)KoiAttack.BubbleBlast] -= Time.deltaTime;
                        if (_specialCooldown[(int)AttackState.Active] < 0.0f && _specialCooldown[3] < 0.0f && Random.Range(1, 4) == 1)
                        {
                            _activeStates[(int)AttackState.Active] = true;
                            _specialCooldown[(int)AttackState.Active] = 6.0f;
                            _specialCooldown[(int)KoiAttack.BubbleBlast] = 8.0f;
                            _currTime = 0;
                            //Set up bubble blast attack
                            _actionQueue.Enqueue(KoiStopTransition);
                            _actionQueue.Enqueue(KoiBubbleBlastTransitionDown);
                            _actionQueue.Enqueue(KoiBubbleBlastTransitionUp);
                            _actionQueue.Enqueue(KoiBubbleBlastCharge);
                            _actionQueue.Enqueue(KoiBubbleBlastAttack);
                        }
                    }
                }
                else
                {
                    //Go through enmeies action queue
                    if (!DoActionQueue())
                    {
                        _activeStates[0] = false;
                    }
                }
            }
            //Switch to phase 2
            else if (!_activeStates[(int)AttackState.FormChanged])
            {
                //Check to see if form changing is just beginning
                if (!_activeStates[(int)AttackState.FormChangeInProgress])
                {
                    //Reset any specials the Koi may be in
                    _activeStates[(int)AttackState.Active] = false;
                    _specialCooldown[(int)AttackState.Active] = 5.0f;
                    _isRaming = false;
                    _inKnockback = false;
                    _actionQueue.Clear();
                    ClearHitboxes();
                    _currTime = 0;
                    StopMotion();
                    _activeStates[(int)AttackState.FormChangeInProgress] = true;
                    _animator.SetTrigger(_animParm[(int)CarpAnim.Dive]);
                    _initalPos = transform.position.y;
                }


                if (_currTime < 1.0f)
                {
                    ApplyConstantMoveForce(Vector3.down, 1.0f * transform.localScale.y, 1.0f);
                    _currTime += Time.deltaTime;
                }
                else
                {
                    //Change obstical detection position
                    Transform detect = transform.GetChild(transform.childCount - 1);
                    detect.position = new Vector3(detect.position.x, detect.position.y + 4.0f, detect.position.z);
                    //initalPos = initalPos - 1.0f * transform.localScale.y;
                    //ReturnToInitalPosition();

                    StopMotion();
                    _currTime = 0;
                    _activeStates[(int)AttackState.FormChanged] = true;
                }
            }
            //Phase 2 AI
            else
            {
                //If the Koi is not in any special
                if (!_activeStates[(int)AttackState.Active])
                {
                    if (_playerDistance > 10.0f)
                    {
                        FollowPlayer();
                    }
                    else
                    {
                        //CirclePlayer();
                    }

                    //Decrement overall special cooldown, no special can be used while this is in cooldown.
                    if (_specialCooldown[(int)AttackState.Active] > 0)
                        _specialCooldown[(int)AttackState.Active] -= Time.deltaTime;

                    //Check to see if monster can use triple dash special attack
                    if (_playerDistance < 16.0f)
                    {
                        _specialCooldown[(int)KoiAttack.TripleDash] -= Time.deltaTime;
                        if (_specialCooldown[(int)AttackState.Active] < 0.0f && _specialCooldown[(int)KoiAttack.TripleDash] < 0.0f && Random.Range(1, 4) == 1)
                        {
                            //Use cooldown to store original height
                            _initalPos = transform.position.y;
                            _activeStates[(int)AttackState.Active] = true;
                            _specialCooldown[(int)AttackState.Active] = 5.0f;
                            _specialCooldown[(int)KoiAttack.TripleDash] = 6.0f;
                            //Set up triple dash attack
                            _actionQueue.Enqueue(KoiDashCharge);
                            _actionQueue.Enqueue(KoiUnderwaterDash);
                            _actionQueue.Enqueue(KoiDashCharge);
                            _actionQueue.Enqueue(KoiUnderwaterDash);
                            _actionQueue.Enqueue(KoiDashCharge);
                            _actionQueue.Enqueue(KoiUnderwaterDash);
                            _actionQueue.Enqueue(KoiUnderwaterDashReturn);
                        }
                    }

                    //Check to see if monster can use underwater attack
                    if (_playerDistance < 15.0f)
                    {
                        _specialCooldown[(int)KoiAttack.UnderwaterAttack] -= Time.deltaTime;
                        if (_specialCooldown[(int)AttackState.Active] < 0.0f && _specialCooldown[(int)KoiAttack.UnderwaterAttack] < 0.0f && Random.Range(1, 4) == 1)
                        {
                            //Use cooldown to store original height
                            _initalPos = transform.position.y;
                            _activeStates[(int)AttackState.Active] = true;
                            _specialCooldown[(int)AttackState.Active] = 5.0f;
                            _specialCooldown[(int)KoiAttack.UnderwaterAttack] = 8.0f;
                            //Set up Underwater attack
                            _actionQueue.Enqueue(KoiUnderwaterFollow);
                            _actionQueue.Enqueue(KoiUnderwaterAttack);
                        }
                    }

                    //Check to see if player can use charge projectile special attack
                    if (_playerDistance < 23.0f)
                    {
                        _specialCooldown[(int)KoiAttack.BubbleBlast] -= Time.deltaTime;
                        if (_specialCooldown[(int)AttackState.Active] < 0.0f && _specialCooldown[(int)KoiAttack.BubbleBlast] < 0.0f && Random.Range(1, 4) == 1)
                        {
                            //Use cooldown to store original height
                            _initalPos = transform.position.y;
                            _activeStates[(int)AttackState.Active] = true;
                            _specialCooldown[(int)AttackState.Active] = 6.0f;
                            _specialCooldown[(int)KoiAttack.BubbleBlast] = 9.0f;
                            //Set up Underwater bubble blast
                            _actionQueue.Enqueue(KoiBubbleBlastUnderwaterCharge);
                            _actionQueue.Enqueue(KoiBubbleBlastCharge);
                            _actionQueue.Enqueue(KoiBubbleBlastAttack);
                            _actionQueue.Enqueue(KoiBubbleBlastReturn);
                        }
                    }
                }
                else
                {
                    //Go through enmeies action queue
                    if (!DoActionQueue())
                    {
                        _activeStates[(int)AttackState.Active] = false;
                        _currTime = 0;
                    }
                }
            }
        }
        _animator.SetFloat(_animParm[(int)Anim.Velocity], _velocity.sqrMagnitude);
    }

    /// <summary>
    /// Enemy runs away from player in their hostile state
    /// rather than trying to fight the player
    /// </summary>
    public void HostileRunAway()
    {
        //If enemy is outside max radius, set to passive
        if (_enemyDistance > _maxRadius)
        {
            _state = EnemyState.Passive;
            ResetHostile();
            //Keep monster passive for 5 seconds at least
            _passiveCooldown = 5.0f;
        }
        else
        {
            //Track player
            _destination = new Vector3(GameObject.FindGameObjectWithTag("Player").transform.position.x, transform.position.y, GameObject.FindGameObjectWithTag("Player").transform.position.z);
            //Find the direction the monster should be looking, away from the player
            _lookRotation = Quaternion.LookRotation(transform.position - _destination);
            //Find local forward vector
            Vector3 forward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
            //When monster gets close to an obstical avoid it
            /*if (CheckCollision())
            {
                lookRotation = Quaternion.LookRotation(Vector3.Cross(Vector3.up, transform.forward));
            }*/

            //Rotate and move monster
            transform.rotation = Quaternion.RotateTowards(transform.rotation, _lookRotation, 0.4f);
            transform.Translate(new Vector3(forward.x, 0, forward.z) * _speed / 40);
        }
    }

    public void HostileRockCrab()
    {
        //If enemy is outside max radius, set to passive
        if (_enemyDistance > _maxRadius)
        {
            _state = EnemyState.Passive;
            ResetHostile();
            //Keep monster passive for 5 seconds at least
            _passiveCooldown = 5.0f;
        }
        //When crab first activates, jump out of the water
        else if(!_activeStates[(int)AttackState.FormChanged])
        {
            if(!_activeStates[(int)AttackState.FormChangeInProgress])
            {
                _gravity = ApplyArcForce(transform.forward, 0, 3, 1.0f);
                _currTime = 0;
                _activeStates[(int)AttackState.FormChangeInProgress] = true;
            }

            ApplyForce(_gravity);

            if(_currTime >= 0.85f)
            {
                _activeStates[(int)AttackState.FormChanged] = true;
                StopMotion();
                _currTime = 0;
            }
            _currTime += Time.deltaTime;
        }
        else
        { 
            //If enemy is not in special
            if (!_activeStates[(int)AttackState.Active])
            {
                //Follow the player
                FollowPlayer();

                //Cooldown special while in a 10 units of player
                if (_playerDistance < 20.0f)
                {
                    _specialCooldown[(int)AttackState.Active] -= Time.deltaTime;
                }
                //If cooldown is finished, switch to special
                if (_specialCooldown[(int)AttackState.Active] <= 0)
                {
                    _activeStates[(int)AttackState.Active] = true;
                    _currTime = 0;
                    _initalPos = transform.position.y;
                    //Load an attack that charges a dash then attacks
                    _actionQueue.Enqueue(CrabRockFling);
                }
            }
            else
            {
                //Go through enmeies action queue
                if (!DoActionQueue())
                {
                    _activeStates[(int)AttackState.Active] = false;
                    _specialCooldown[(int)AttackState.Active] = 5.0f;
                }
            }
            _animator.SetFloat(_animParm[(int)Anim.Velocity], _velocity.sqrMagnitude);
        }
    }
}