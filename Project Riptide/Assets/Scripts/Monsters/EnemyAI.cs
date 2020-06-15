/*This script contains AI used by various enemy classes
 * uses partial classes to declare AI
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum KoiAttackState { TripleDash = 2, BubbleBlast = 4, UnderwaterAttack = 3, BubbleAttack = 3 }
public enum AttackState { Active = 0, FormChanged = 1, FormChangeInProgress = 2 }
public enum FlowerFrogAttackState { Latched = 1 }
public enum ClamAttackState { Opened = 1, TentacleBurst = 2, TentacleTripleBurst = 3, OpenAttack = 4, TentacleCircle = 5, TentacleTrack = 6, TentacleLine = 7 }

//AI available to all enemys
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
            _destination = new Vector3(_startPos.x, transform.position.y, _startPos.z);
        }
        else
        {
            //If time between movements is over select a new destination
            if (_timeCurrent >= _timeBetween)
            {
                //Select new destination that is inside wander radius
                do
                {
                    _destination = new Vector3(transform.position.x + Random.Range(-30, 30), transform.position.y, transform.position.z + Random.Range(-30, 30));
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
        netForce += new Vector3(transform.forward.x, 0, transform.forward.z).normalized * 1.0f;

        //Rotate in towards direction of velocity
        if (_velocity != Vector3.zero)
        {
            Quaternion desiredRotation = Quaternion.LookRotation(_velocity);
            SetSmoothRotation(desiredRotation, 1.0f, 0.5f, 2.0f);
        }
        _timeCurrent += Time.deltaTime;

        ApplyForce(netForce * 0.7f);

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
    /// Passive AI that returns a monster to it's wander radius
    /// Used for monsters that do nothing until triggered
    /// Then need to return back to their starting area when
    /// they return to passive
    /// </summary>
    public void PassiveReturnToRadius()
    {
        if (_enemyDistance >= _wanderRadius)
        {
            Vector3 destination = new Vector3(_startPos.x, transform.position.y, _startPos.z);
            //Check for obstacle
            if (CheckObstacle(destination))
            {
                //Set destination to closest way to player that avoids obstacles
                destination = transform.position + AvoidObstacle(destination);
            }
            //Seek destination
            Vector3 netForce = Seek(destination);
            netForce += new Vector3(transform.forward.x, 0, transform.forward.z).normalized * 1.0f;

            //Rotate in towards direction of velocity
            if (_velocity != Vector3.zero)
            {
                Quaternion desiredRotation = Quaternion.LookRotation(_velocity);
                SetSmoothRotation(desiredRotation, 1.0f, 0.5f, 2.0f);
            }
            _timeCurrent += Time.deltaTime;

            ApplyForce(netForce);

            //ApplyFriction(0.25f);
            if (_animator != null)
                _animator.SetFloat(_animParm[(int)Anim.Velocity], _velocity.sqrMagnitude);
        }
        else if(_velocity != Vector3.zero)
        {
            StopMotion();
        }
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

        FleePlayer(1.5f);
    }
}

//AI for koi boss
public partial class KoiBoss : Enemy
{
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
    public void HostileKoiBoss()
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
                        FleePlayer(1.0f);
                    }

                    //Decrement overall special cooldown, no special can be used while this is in cooldown.
                    if (_specialCooldown[(int)AttackState.Active] > 0)
                        _specialCooldown[(int)AttackState.Active] -= Time.deltaTime;

                    //Check to see if monster can use triple dash special attack
                    if (_playerDistance < 13.0f)
                    {
                        _specialCooldown[(int)KoiAttackState.TripleDash] -= Time.deltaTime;
                        if (_specialCooldown[(int)AttackState.Active] < 0.0f && _specialCooldown[(int)KoiAttackState.TripleDash] < 0.0f && Random.Range(1, 4) == 1)
                        {
                            _activeStates[(int)AttackState.Active] = true;
                            _specialCooldown[(int)AttackState.Active] = 5.0f;
                            _specialCooldown[(int)KoiAttackState.TripleDash] = 6.0f;
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
                        _specialCooldown[(int)KoiAttackState.BubbleAttack] -= Time.deltaTime;
                        if (_specialCooldown[(int)AttackState.Active] < 0.0f && _specialCooldown[(int)KoiAttackState.BubbleAttack] < 0.0f && Random.Range(1, 4) == 1)
                        {
                            //Load projectile
                            _activeStates[(int)AttackState.Active] = true;
                            _specialCooldown[(int)AttackState.Active] = 2.0f;
                            _specialCooldown[(int)KoiAttackState.BubbleAttack] = 3.0f;
                            _currTime = 0;
                            _actionQueue.Enqueue(KoiBubbleAttack);
                        }
                    }

                    //Check to see if player can use charge projectile special attack
                    if (_playerDistance < 20.0f)
                    {
                        _specialCooldown[(int)KoiAttackState.BubbleBlast] -= Time.deltaTime;
                        if (_specialCooldown[(int)AttackState.Active] < 0.0f && _specialCooldown[(int)KoiAttackState.BubbleBlast] < 0.0f && Random.Range(1, 4) == 1)
                        {
                            _activeStates[(int)AttackState.Active] = true;
                            _specialCooldown[(int)AttackState.Active] = 6.0f;
                            _specialCooldown[(int)KoiAttackState.BubbleBlast] = 8.0f;
                            _initalPos = transform.position.y;
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
                    _initalPos = _initalPos - 1.0f * transform.localScale.y;
                    ReturnToInitalPosition();

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
                        FleePlayer(1.0f);
                    }

                    //Decrement overall special cooldown, no special can be used while this is in cooldown.
                    if (_specialCooldown[(int)AttackState.Active] > 0)
                        _specialCooldown[(int)AttackState.Active] -= Time.deltaTime;

                    //Check to see if monster can use triple dash special attack
                    if (_playerDistance < 16.0f)
                    {
                        _specialCooldown[(int)KoiAttackState.TripleDash] -= Time.deltaTime;
                        if (_specialCooldown[(int)AttackState.Active] < 0.0f && _specialCooldown[(int)KoiAttackState.TripleDash] < 0.0f && Random.Range(1, 4) == 1)
                        {
                            //Use cooldown to store original height
                            _initalPos = transform.position.y;
                            _activeStates[(int)AttackState.Active] = true;
                            _specialCooldown[(int)AttackState.Active] = 5.0f;
                            _specialCooldown[(int)KoiAttackState.TripleDash] = 6.0f;
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
                        _specialCooldown[(int)KoiAttackState.UnderwaterAttack] -= Time.deltaTime;
                        if (_specialCooldown[(int)AttackState.Active] < 0.0f && _specialCooldown[(int)KoiAttackState.UnderwaterAttack] < 0.0f && Random.Range(1, 4) == 1)
                        {
                            //Use cooldown to store original height
                            _initalPos = transform.position.y;
                            _activeStates[(int)AttackState.Active] = true;
                            _specialCooldown[(int)AttackState.Active] = 5.0f;
                            _specialCooldown[(int)KoiAttackState.UnderwaterAttack] = 8.0f;
                            //Set up Underwater attack
                            _actionQueue.Enqueue(KoiUnderwaterFollow);
                            _actionQueue.Enqueue(KoiUnderwaterAttack);
                        }
                    }

                    //Check to see if player can use charge projectile special attack
                    if (_playerDistance < 23.0f)
                    {
                        _specialCooldown[(int)KoiAttackState.BubbleBlast] -= Time.deltaTime;
                        if (_specialCooldown[(int)AttackState.Active] < 0.0f && _specialCooldown[(int)KoiAttackState.BubbleBlast] < 0.0f && Random.Range(1, 4) == 1)
                        {
                            //Use cooldown to store original height
                            _initalPos = transform.position.y;
                            _activeStates[(int)AttackState.Active] = true;
                            _specialCooldown[(int)AttackState.Active] = 6.0f;
                            _specialCooldown[(int)KoiAttackState.BubbleBlast] = 9.0f;
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
}

//AI for Rock crab
public partial class RockCrab : Enemy
{
    /// <summary>
    /// Passive AI for the rock crab
    /// Does nothing but hide underwater and wait for player
    /// When the rock crab is far away from starting position,
    /// Run back to starting area
    /// </summary>
    public void PassiveRockCrab()
    {
        if (_position.y == _startPos.y)
        {
            //Do nothing
            return;
        }

        //While crab is too far away from starting pos, move towards starting pos
        if (_enemyDistance >= 10.0f)
        {

            Vector3 destination = new Vector3(_startPos.x, transform.position.y, _startPos.z);
            //Check for obstacle
            if (CheckObstacle(destination))
            {
                //Set destination to closest way to player that avoids obstacles
                destination = transform.position + AvoidObstacle(destination);
            }
            //Seek destination
            Vector3 netForce = Seek(destination);
            netForce += new Vector3(transform.forward.x, 0, transform.forward.z).normalized * 1.0f;

            //Rotate in towards direction of velocity
            if (_velocity != Vector3.zero)
            {
                Quaternion desiredRotation = Quaternion.LookRotation(_velocity);
                SetSmoothRotation(desiredRotation, 1.0f, 0.5f, 2.0f);
            }
            _timeCurrent += Time.deltaTime;

            ApplyForce(netForce);

            //ApplyFriction(0.25f);
            if (_animator != null)
                _animator.SetFloat(_animParm[(int)Anim.Velocity], _velocity.sqrMagnitude);
        }
        //When crab is close enough, move down to hide again
        else if (transform.position.y > _startPos.y)
        {
            //Set passive cooldown so rock crab cannot be triggered during transition
            _passiveCooldown = 1.0f;
            //Move down
            ApplyConstantMoveForce(Vector3.down, 3.0f, 1.0f);
        }
        //If crab moves down to far, return to initial y pos
        else if (transform.position.y < _startPos.y)
        {
            StopMotion();
            _animator.SetTrigger(_animParm[(int)CrabAnim.Close]);
            _position = new Vector3(_position.x, _startPos.y, _position.z);
        }
    }

    /// <summary>
    /// Hostile AI for Rock Crab
    /// Rock crab hides underwater and jumps up when Hostile AI is triggered
    /// Attacks by jumping towards the player every so often
    /// </summary>
    protected void HostileRockCrab()
    {
        //If enemy is outside max radius, set to passive
        if (_enemyDistance > _maxRadius && !_activeStates[(int)AttackState.Active])
        {
            _state = EnemyState.Passive;
            ResetHostile();
            //Keep monster passive for 5 seconds at least
            _passiveCooldown = 5.0f;
        }
        //When crab first activates, jump out of the water
        else if (!_activeStates[(int)AttackState.FormChanged])
        {
            if (!_activeStates[(int)AttackState.FormChangeInProgress])
            {
                //Only jump out of the water if the crab is underwater
                //When reactivating hostile AI, crab may be still above water
                if (_position.y == _startPos.y)
                {
                    _gravity = ApplyArcForce(transform.forward, 0, 3, 1.0f);
                    _currTime = 0;
                    _animator.SetTrigger(_animParm[(int)CrabAnim.Open]);
                    _activeStates[(int)AttackState.FormChangeInProgress] = true;
                }
                else
                {
                    _activeStates[(int)AttackState.FormChanged] = true;
                    _activeStates[(int)AttackState.FormChangeInProgress] = true;
                }
            }

            ApplyForce(_gravity);

            if (_currTime >= 0.80f)
            {
                _activeStates[(int)AttackState.FormChanged] = true;
                _position = new Vector3(_position.x, _startPos.y + 2.0f, _position.z);
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

                //Cooldown special while in 20 units of player
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
                    _actionQueue.Enqueue(RockCrabFling);
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

public partial class FlowerFrog : Enemy
{
    protected void HostileFlowerFrog()
    {
        //If enemy is outside max radius, set to passive
        if (_enemyDistance > _maxRadius && !_activeStates[(int)AttackState.Active])
        {
            _state = EnemyState.Passive;
            //Reset tounge position
            OnPassive();
        }
        else
        {
            //If enemy is not in special
            if (!_activeStates[(int)AttackState.Active])
            {
                if (!_activeStates[(int)FlowerFrogAttackState.Latched])
                {
                    //Follow the player
                    //FollowPlayer();
                    LookAtPlayer();

                    ApplyFriction(0.5f);

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
                        //Load an attack that shoots the frogs tounge out to latch on to player
                        _actionQueue.Enqueue(ToungeCharge);
                        _actionQueue.Enqueue(ShootTounge);
                        _actionQueue.Enqueue(ToungeReturn);
                    }
                }
                else
                {
                    //While latched
                    ToungeDrag();
                    _tounge.transform.rotation = Quaternion.identity;
                    _tounge.SetPosition(1, (PlayerPosition() - transform.position) * 1.1f);
                    if(_latchStartHealth - _health > LATCH_DAMAGE_CAP || _playerDistance > MAX_LATCH_DIST)
                    {
                        _activeStates[(int)FlowerFrogAttackState.Latched] = false;
                        _activeStates[(int)AttackState.Active] = true;
                        _currTime = 0;
                        _actionQueue.Enqueue(ToungeReturn);
                    }
                    SendFriction(0.4f);
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
            //_animator.SetFloat(_animParm[(int)Anim.Velocity], _velocity.sqrMagnitude);
        }
    }
}

public partial class ClamBoss : Enemy
{
    /// <summary>
    /// Defines the AI for the Clam Boss Fight
    /// Uses tentacles to attack
    /// 4 different tentacle attacks
    /// Will sometimes open up and use a different
    /// attack based on what shows up in its mouth
    /// 3 different open attacks
    /// attacks get faster as hp gets lower
    /// </summary>
    public void HostileClamBoss()
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
            //If the Clam is not in any special
            if (!_activeStates[(int)AttackState.Active])
            {
                //When clam is not opened do normal attacks
                if (!_activeStates[(int)ClamAttackState.Opened])
                {
                    //Decrement overall special cooldown, no special can be used while this is in cooldown.
                    if (_specialCooldown[(int)AttackState.Active] > 0)
                    {
                        _specialCooldown[(int)AttackState.Active] -= Time.deltaTime;
                    }

                    //If clam is under 50% health, do trigger attack
                    if(!_activeStates[(int)ClamAttackState.TentacleBurst] && _health < _maxHealth / 2)
                    {
                        _activeStates[(int)AttackState.Active] = true;
                        _activeStates[(int)ClamAttackState.TentacleBurst] = true;
                        _specialCooldown[(int)AttackState.Active] = 5.0f * _speedScale;
                        _currTime = 0;
                        //Set up circle tentacle attack attack
                        //_actionQueue.Enqueue(ClamBurstCharge);
                        _actionQueue.Enqueue(ClamBurstAttack);
                    }

                    //If clam is under 10% health, do trigger attack
                    if (!_activeStates[(int)ClamAttackState.TentacleTripleBurst] && _health < _maxHealth / 10)
                    {
                        _activeStates[(int)AttackState.Active] = true;
                        _activeStates[(int)ClamAttackState.TentacleTripleBurst] = true;
                        _specialCooldown[(int)AttackState.Active] = 5.0f * _speedScale;
                        _currTime = 0;
                        //Set up circle tentacle attack attack
                        //_actionQueue.Enqueue(ClamBurstCharge);
                        _actionQueue.Enqueue(ClamBurstAttack);
                        _actionQueue.Enqueue(ClamBurstAttack);
                        _actionQueue.Enqueue(ClamBurstAttack);
                    }

                    //Proritize open attack if it is out of cooldown
                    if (_specialCooldown[(int)ClamAttackState.OpenAttack] > 0)
                    {
                        //Check to see if monster can use circle tentacle special attack
                        if (_playerDistance < 15.0f)
                        {
                            _specialCooldown[(int)ClamAttackState.TentacleCircle] -= Time.deltaTime;
                            if (!_activeStates[(int)AttackState.Active] && _specialCooldown[(int)AttackState.Active] < 0.0f && _specialCooldown[(int)ClamAttackState.TentacleCircle] < 0.0f)
                            {
                                _activeStates[(int)AttackState.Active] = true;
                                _specialCooldown[(int)AttackState.Active] = 5.0f * _speedScale;
                                _specialCooldown[(int)ClamAttackState.TentacleCircle] = 3.0f * _speedScale;
                                _currTime = 0;
                                //Set up circle tentacle attack attack
                                //_actionQueue.Enqueue(ClamCircleCharge);
                                _actionQueue.Enqueue(ClamCircleAttack);
                            }
                        }

                        //Check to see if monster can use tracking tentacle attack
                        if (_playerDistance < 25.0f)
                        {
                            _specialCooldown[(int)ClamAttackState.TentacleTrack] -= Time.deltaTime;
                            if (!_activeStates[(int)AttackState.Active] && _specialCooldown[(int)AttackState.Active] < 0.0f && _specialCooldown[(int)ClamAttackState.TentacleTrack] < 0.0f && Random.Range(1, 4) == 1)
                            {
                                //Load projectile
                                _activeStates[(int)AttackState.Active] = true;
                                _specialCooldown[(int)AttackState.Active] = 4.0f * _speedScale;
                                _specialCooldown[(int)ClamAttackState.TentacleTrack] = 5.0f * _speedScale;
                                _currTime = 0;
                                //Set up tracking tentacles attack
                                _actionQueue.Enqueue(ClamTrackAttack);
                                _actionQueue.Enqueue(ClamTrackAttack);
                                _actionQueue.Enqueue(ClamTrackAttack);
                            }
                        }

                        //Check to see if player can use tentacle line attack
                        if (_playerDistance < 20.0f)
                        {
                            _specialCooldown[(int)ClamAttackState.TentacleLine] -= Time.deltaTime;
                            if (!_activeStates[(int)AttackState.Active] && _specialCooldown[(int)AttackState.Active] < 0.0f && _specialCooldown[(int)ClamAttackState.TentacleLine] < 0.0f && Random.Range(1, 4) == 1)
                            {
                                _activeStates[(int)AttackState.Active] = true;
                                _specialCooldown[(int)AttackState.Active] = 5.0f * _speedScale;
                                _specialCooldown[(int)ClamAttackState.TentacleLine] = 8.0f * _speedScale;
                                _currTime = 0;
                                _lineOffset = 5.0f;
                                _lineForward = new Vector3(PlayerPosition().x, transform.position.y, PlayerPosition().z) + PlayerVelocity() * 4.0f;
                                _lineForward -= transform.position;
                                _lineForward.Normalize();
                                //Set up line attack
                                //_actionQueue.Enqueue(ClamLineCharge);
                                _actionQueue.Enqueue(ClamLineAttack);
                                _actionQueue.Enqueue(ClamLineAttack);
                                _actionQueue.Enqueue(ClamLineAttack);
                                _actionQueue.Enqueue(ClamLineAttack);
                                _actionQueue.Enqueue(ClamLineAttack);
                                _actionQueue.Enqueue(ClamLineAttack);
                            }
                        }
                    }

                    //Check to see if player can use open attack
                    if (_playerDistance < 25.0f)
                    {
                        _specialCooldown[(int)ClamAttackState.OpenAttack] -= Time.deltaTime;
                        if (!_activeStates[(int)AttackState.Active] && _specialCooldown[(int)AttackState.Active] < 0.0f && _specialCooldown[(int)ClamAttackState.OpenAttack] < 0.0f)
                        {
                            _activeStates[(int)AttackState.Active] = true;
                            _activeStates[(int)ClamAttackState.Opened] = true;
                            _specialCooldown[(int)AttackState.Active] = 0.0f;
                            _specialCooldown[(int)ClamAttackState.OpenAttack] = 15.0f * _speedScale;
                            _currTime = 0;
                            //Set up open attack
                            _actionQueue.Enqueue(ClamOpen);
                            _actionQueue.Enqueue(ClamWait);
                        }
                    }
                }
                //If in opened state
                else
                {
                    //Prepare open attack
                    _activeStates[(int)AttackState.Active] = true;
                    _activeStates[(int)ClamAttackState.Opened] = false;
                    _specialCooldown[(int)AttackState.Active] = 5.0f * _speedScale;
                    _currTime = 0;

                    //Do a different attack based on open state
                    switch (_openState)
                    {
                        case ClamOpenState.Bird:
                            _actionQueue.Enqueue(ClamBirdAttack);
                            break;
                        case ClamOpenState.WaterSpout:
                            _actionQueue.Enqueue(ClamWaterSpoutStart);
                            _actionQueue.Enqueue(ClamWaterSpoutAttack);
                            break;
                        case ClamOpenState.Dragon:
                            _actionQueue.Enqueue(ClamDragonCharge);
                            _actionQueue.Enqueue(ClamDragonAttack);
                            break;
                    }
                    _actionQueue.Enqueue(ClamClose);
                }
            }
            else
            {
                //Go through enmeies action queue
                if (!DoActionQueue())
                {
                    _activeStates[(int)AttackState.Active] = false;
                }
            }
        }
        ApplyFriction(0.99f);
        if (_animator != null)
        {
            _animator.SetFloat(_animParm[(int)Anim.Velocity], _velocity.sqrMagnitude);
        }
    }
}