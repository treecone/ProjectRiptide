﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Attack { TripleDash = 2, BubbleBlast = 4, UnderwaterAttack = 3, BubbleAttack = 3 }
public enum State { Active = 0, FormChanged = 1, FormChangeInProgress = 2 }

public partial class Enemy : PhysicsScript
{
    /// <summary>
    /// Moves the monster randomly within a certain radius
    /// </summary>
    public void PassiveWanderRadius()
    {
        //If the monster is currently outside the wander radius, go back to the radius.
        //This is important if the monster 
        if (enemyDistance > wanderRadius)
        {
            destination = startPos;
        }
        else
        {
            //If time between movements is over select a new destination
            if (timeCurrent >= timeBetween)
            {
                //Select new destination that is inside wander radius
                do
                {
                    destination = new Vector3(transform.position.x + Random.Range(-10, 10), transform.position.y, transform.position.z + Random.Range(-10, 10));
                } while (Vector3.Distance(destination, startPos) > wanderRadius);
                timeCurrent = 0;
            }
        }

        //Calculate net force
        Vector3 netForce = Seek(destination);
        netForce += new Vector3(transform.forward.x, 0, transform.forward.z).normalized * maxSpeed;

        //Check for collision
        if(CheckObstacle())
        {
            ApplyForce(Steer(AvoidObstacle()) * 2.0f);
        }

        //Rotate in towards direction of velocity
        rotation = Quaternion.RotateTowards(rotation, Quaternion.LookRotation(velocity), 4.0f);

        timeCurrent += Time.deltaTime;

        ApplyForce(netForce);
        //ApplyFriction(0.25f);
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
        if (enemyDistance > maxRadius)
        {
            state = EnemyState.Passive;
            ResetHostile();
            //Keep monster passive for 5 seconds at least
            passiveCooldown = 5.0f;
        }
        else
        {
            //If enemy is not in special
            if (!activeStates[0])
            {
                //Follow the player
                FollowPlayer();

                //Cooldown special while in a 10 units of player
                if (playerDistance < 10.0f)
                {
                    specialCooldown[0] -= Time.deltaTime;
                }
                //If cooldown is finished, switch to special
                if (specialCooldown[0] <= 0)
                {
                    activeStates[0] = true;
                    currTime = 0;
                    //Load an attack that charges a dash then attacks
                    actionQueue.Enqueue(DashCharge);
                    actionQueue.Enqueue(DashAttack);
                }
            }
            else
            {
                //Go through enmeies action queue
                if (!DoActionQueue())
                {
                    activeStates[0] = false;
                    specialCooldown[0] = 5.0f;
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
        if (enemyDistance > maxRadius)
        {
            state = EnemyState.Passive;
            ResetHostile();
            //Keep monster passive for 5 seconds at least
            passiveCooldown = 5.0f;
        }
        else
        {
            //Phase 1: greater than 50% health
            if (health > maxHealth / 2)
            {
                //If the Koi is not in any special
                if (!activeStates[(int)State.Active])
                {
                    FollowPlayer();

                    //Decrement overall special cooldown, no special can be used while this is in cooldown.
                    if (specialCooldown[(int)State.Active] > 0)
                        specialCooldown[(int)State.Active] -= Time.deltaTime;

                    //Check to see if monster can use triple dash special attack
                    if (playerDistance < 13.0f)
                    {
                        specialCooldown[(int)Attack.TripleDash] -= Time.deltaTime;
                        if (specialCooldown[(int)State.Active] < 0.0f && specialCooldown[(int)Attack.TripleDash] < 0.0f && Random.Range(1, 4) == 1)
                        {
                            activeStates[(int)State.Active] = true;
                            specialCooldown[(int)State.Active] = 5.0f;
                            specialCooldown[(int)Attack.TripleDash] = 6.0f;
                            currTime = 0;
                            //Set up triple dash attack
                            actionQueue.Enqueue(KoiDashCharge);
                            actionQueue.Enqueue(KoiDashAttack);
                            actionQueue.Enqueue(KoiDashCharge);
                            actionQueue.Enqueue(KoiDashAttack);
                            actionQueue.Enqueue(KoiDashCharge);
                            actionQueue.Enqueue(KoiDashAttack);
                        }
                    }

                    //Check to see if monster can use bubble attack
                    if (playerDistance > 10.0f)
                    {
                        specialCooldown[(int)Attack.BubbleAttack] -= Time.deltaTime;
                        if (specialCooldown[(int)State.Active] < 0.0f && specialCooldown[(int)Attack.BubbleAttack] < 0.0f && Random.Range(1, 4) == 1)
                        {
                            //Load projectile
                            activeStates[(int)State.Active] = true;
                            specialCooldown[(int)State.Active] = 2.0f;
                            specialCooldown[(int)Attack.BubbleAttack] = 3.0f;
                            currTime = 0;
                            actionQueue.Enqueue(KoiBubbleAttack);
                        }
                    }

                    //Check to see if player can use charge projectile special attack
                    if (playerDistance < 20.0f)
                    {
                        specialCooldown[(int)Attack.BubbleBlast] -= Time.deltaTime;
                        if (specialCooldown[(int)State.Active] < 0.0f && specialCooldown[3] < 0.0f && Random.Range(1, 4) == 1)
                        {
                            activeStates[(int)State.Active] = true;
                            specialCooldown[(int)State.Active] = 6.0f;
                            specialCooldown[(int)Attack.BubbleBlast] = 8.0f;
                            currTime = 0;
                            //Set up bubble blast attack
                            actionQueue.Enqueue(KoiBubbleBlastCharge);
                            actionQueue.Enqueue(KoiBubbleBlastAttack);
                        }
                    }
                }
                else
                {
                    //Go through enmeies action queue
                    if (!DoActionQueue())
                    {
                        activeStates[0] = false;
                    }
                }
            }
            //Switch to phase 2
            else if (!activeStates[(int)State.FormChanged])
            {
                //Check to see if form changing is just beginning
                if (!activeStates[(int)State.FormChangeInProgress])
                {
                    //Reset any specials the Koi may be in
                    activeStates[(int)State.Active] = false;
                    specialCooldown[(int)State.Active] = 5.0f;
                    isRaming = false;
                    inKnockback = false;
                    actionQueue.Clear();
                    ClearHitboxes();
                    currTime = 0;
                    StopMotion();
                    activeStates[(int)State.FormChangeInProgress] = true;
                }

                if (currTime < 1.0f)
                {
                    ApplyConstantMoveForce(Vector3.down, 1.5f * transform.localScale.y, 1.0f);
                    currTime += Time.deltaTime;
                }
                else
                {
                    StopMotion();
                    currTime = 0;
                    activeStates[(int)State.FormChanged] = true;
                }
            }
            //Phase 2 AI
            else
            {
                //If the Koi is not in any special
                if (!activeStates[(int)State.Active])
                {
                    FollowPlayer();

                    //Decrement overall special cooldown, no special can be used while this is in cooldown.
                    if (specialCooldown[(int)State.Active] > 0)
                        specialCooldown[(int)State.Active] -= Time.deltaTime;

                    //Check to see if monster can use triple dash special attack
                    if (playerDistance < 16.0f)
                    {
                        specialCooldown[(int)Attack.TripleDash] -= Time.deltaTime;
                        if (specialCooldown[(int)State.Active] < 0.0f && specialCooldown[(int)Attack.TripleDash] < 0.0f && Random.Range(1, 4) == 1)
                        {
                            //Use cooldown to store original height
                            initalPos = transform.position.y;
                            activeStates[(int)State.Active] = true;
                            specialCooldown[(int)State.Active] = 5.0f;
                            specialCooldown[(int)Attack.TripleDash] = 6.0f;
                            //Set up triple dash attack
                            actionQueue.Enqueue(KoiDashCharge);
                            actionQueue.Enqueue(KoiUnderwaterDash);
                            actionQueue.Enqueue(KoiDashCharge);
                            actionQueue.Enqueue(KoiUnderwaterDash);
                            actionQueue.Enqueue(KoiDashCharge);
                            actionQueue.Enqueue(KoiUnderwaterDash);
                            actionQueue.Enqueue(KoiUnderwaterDashReturn);
                        }
                    }

                    //Check to see if monster can use underwater attack
                    if (playerDistance < 15.0f)
                    {
                        specialCooldown[(int)Attack.UnderwaterAttack] -= Time.deltaTime;
                        if (specialCooldown[(int)State.Active] < 0.0f && specialCooldown[(int)Attack.UnderwaterAttack] < 0.0f && Random.Range(1, 4) == 1)
                        {
                            //Use cooldown to store original height
                            initalPos = transform.position.y;
                            activeStates[(int)State.Active] = true;
                            specialCooldown[(int)State.Active] = 5.0f;
                            specialCooldown[(int)Attack.UnderwaterAttack] = 8.0f;
                            //Set up Underwater attack
                            actionQueue.Enqueue(KoiUnderwaterFollow);
                            actionQueue.Enqueue(KoiUnderwaterAttack);
                        }
                    }

                    //Check to see if player can use charge projectile special attack
                    if (playerDistance < 23.0f)
                    {
                        specialCooldown[(int)Attack.BubbleBlast] -= Time.deltaTime;
                        if (specialCooldown[(int)State.Active] < 0.0f && specialCooldown[(int)Attack.BubbleBlast] < 0.0f && Random.Range(1, 4) == 1)
                        {
                            //Use cooldown to store original height
                            initalPos = transform.position.y;
                            activeStates[(int)State.Active] = true;
                            specialCooldown[(int)State.Active] = 6.0f;
                            specialCooldown[(int)Attack.BubbleBlast] = 9.0f;
                            //Set up Underwater bubble blast
                            actionQueue.Enqueue(KoiBubbleBlastUnderwaterCharge);
                            actionQueue.Enqueue(KoiBubbleBlastCharge);
                            actionQueue.Enqueue(KoiBubbleBlastAttack);
                            actionQueue.Enqueue(KoiBubbleBlastReturn);
                        }
                    }
                }
                else
                {
                    //Go through enmeies action queue
                    if (!DoActionQueue())
                    {
                        activeStates[(int)State.Active] = false;
                        currTime = 0;
                    }
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
        if (enemyDistance > maxRadius)
        {
            state = EnemyState.Passive;
            ResetHostile();
            //Keep monster passive for 5 seconds at least
            passiveCooldown = 5.0f;
        }
        else
        {
            //Track player
            destination = new Vector3(GameObject.FindGameObjectWithTag("Player").transform.position.x, transform.position.y, GameObject.FindGameObjectWithTag("Player").transform.position.z);
            //Find the direction the monster should be looking, away from the player
            lookRotation = Quaternion.LookRotation(transform.position - destination);
            //Find local forward vector
            Vector3 forward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
            //When monster gets close to an obstical avoid it
            if (CheckCollision())
            {
                lookRotation = Quaternion.LookRotation(Vector3.Cross(Vector3.up, transform.forward));
            }

            //Rotate and move monster
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 0.4f);
            transform.Translate(new Vector3(forward.x, 0, forward.z) * speed / 40);
        }
    }
}