using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Attack { TripleDash = 1, BubbleBlast = 3, UnderwaterAttack = 2, BubbleAttack = 2 }
public enum Action { Active = 0, Knockback = 4, Extra = 5}

public partial class Enemy : MonoBehaviour
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

        //Find the direction the monster should be looking
        lookRotation = Quaternion.LookRotation(destination - transform.position);
        //Increment time
        timeCurrent += Time.deltaTime;
        //Find local forward vector
        Vector3 forward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
        CheckCollision();
        //Rotate and move monster
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 0.4f);
        transform.Translate(new Vector3(forward.x, 0, forward.z) * speed / 40);
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
            //Special[0] is dash attack
            if (!inSpecial[0])
            {
                //Track player
                destination = new Vector3(GameObject.FindGameObjectWithTag("Player").transform.position.x, transform.position.y, GameObject.FindGameObjectWithTag("Player").transform.position.z);
                //Find the direction the monster should be looking
                lookRotation = Quaternion.LookRotation(destination - transform.position);
                //Find local forward vector
                Vector3 forward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
                //When monster gets close circle player
                if (!CheckCollision() && playerDistance < 5.0f)
                {
                    lookRotation = Quaternion.LookRotation(Vector3.Cross(Vector3.up, transform.forward));
                }

                //Cooldown special while in a 10 units of player
                if (playerDistance < 10.0f)
                {
                    specialCooldown[0] -= Time.deltaTime;
                }
                //If cooldown is finished, switch to special
                if (specialCooldown[0] <= 0)
                {
                    inSpecial[0] = true;
                }

                //Rotate and move monster
                transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 0.4f);
                transform.Translate(new Vector3(forward.x, 0, forward.z) * speed / 40);
            }
            else
            {
                //Monster stays still and charges for 2 seconds
                if (specialTimer[0] < 2.0f)
                {
                    //Track player
                    destination = new Vector3(GameObject.FindGameObjectWithTag("Player").transform.position.x, transform.position.y, GameObject.FindGameObjectWithTag("Player").transform.position.z);
                    //Find the direction the monster should be looking
                    lookRotation = Quaternion.LookRotation(destination - transform.position);
                    //Find local forward vector
                    Vector3 forward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
                    specialTimer[0] += Time.deltaTime;
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 1.0f);
                }
                //For one second charge at the player
                else if (specialTimer[0] < 3.0f)
                {
                    //Find local forward vector
                    Vector3 forward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
                    CheckCollision();
                    //If monster hits player, stop special
                    if (playerCollision || obsticalCollision)
                        specialTimer[0] = 5.0f;
                    specialTimer[0] += Time.deltaTime;
                    transform.Translate(new Vector3(forward.x, 0, forward.z) * speed / 2);
                }
                //If the monster hit something, move backwards
                else if (specialTimer[0] >= 5.0f && specialTimer[0] < 5.3f)
                {
                    //Find local forward vector
                    Vector3 forward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
                    specialTimer[0] += Time.deltaTime;
                    transform.Translate(new Vector3(-forward.x, 0, -forward.z) * speed / 6);
                }
                else
                {
                    inSpecial[0] = false;
                    specialTimer[0] = 0.0f;
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
        //Special[0]: Counts for any special
        //Special[1]: triple dash
        //Special[2]: projectile attack / underwater attack
        //Special[3]: charged projectile attack
        //Special[4]: used for koi bouncing backwards after hitting something, not an attack

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
                if (!inSpecial[(int)Action.Active])
                {
                    //Track player
                    destination = new Vector3(GameObject.FindGameObjectWithTag("Player").transform.position.x, transform.position.y, GameObject.FindGameObjectWithTag("Player").transform.position.z);
                    //Find the direction the monster should be looking
                    lookRotation = Quaternion.LookRotation(destination - transform.position);
                    //Find local forward vector
                    Vector3 forward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
                    //When monster gets close circle player
                    if (!CheckCollision() && playerDistance < 5.0f)
                    {
                        lookRotation = Quaternion.LookRotation(Vector3.Cross(Vector3.up, transform.forward));
                    }

                    //Decrement overall special cooldown, no special can be used while this is in cooldown.
                    if (specialCooldown[(int)Action.Active] > 0)
                        specialCooldown[(int)Action.Active] -= Time.deltaTime;

                    //Check to see if monster can use triple dash special attack
                    if (playerDistance < 13.0f)
                    {
                        specialCooldown[(int)Attack.TripleDash] -= Time.deltaTime;
                        if (specialCooldown[(int)Action.Active] < 0.0f && specialCooldown[(int)Attack.TripleDash] < 0.0f && Random.Range(1, 4) == 1)
                        {
                            inSpecial[(int)Action.Active] = true;
                            inSpecial[(int)Attack.TripleDash] = true;
                        }
                    }

                    //Check to see if monster can use bubble attack
                    if (playerDistance > 10.0f)
                    {
                        specialCooldown[(int)Attack.BubbleAttack] -= Time.deltaTime;
                        if (specialCooldown[(int)Action.Active] < 0.0f && specialCooldown[(int)Attack.BubbleAttack] < 0.0f && Random.Range(1, 4) == 1)
                        {
                            //Load projectile
                            SpawnProjectile(new Vector3(0, 0, 5 * lengthMult / 6), 1.0f, 10, 3.0f, MovementPattern.Forward);
                            specialCooldown[(int)Action.Active] = 2.0f;
                            specialCooldown[(int)Attack.BubbleAttack] = 3.0f;
                        }
                    }

                    //Check to see if player can use charge projectile special attack
                    if(playerDistance < 20.0f)
                    {
                        specialCooldown[(int)Attack.BubbleBlast] -= Time.deltaTime;
                        if (specialCooldown[(int)Action.Active] < 0.0f && specialCooldown[3] < 0.0f && Random.Range(1, 4) == 1)
                        {
                            inSpecial[(int)Action.Active] = true;
                            inSpecial[(int)Attack.BubbleBlast] = true;
                        }
                    }

                    //Rotate and move monster
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 0.4f);
                    transform.Translate(new Vector3(forward.x, 0, forward.z) * speed / 40);
                }
                //Knock back after missing
                else if(inSpecial[(int)Action.Knockback])
                {
                    if (specialTimer[(int)Action.Knockback] < 0.3f)
                    {
                        //Find local forward vector
                        Vector3 forward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
                        specialTimer[(int)Action.Knockback] += Time.deltaTime;
                        transform.Translate(new Vector3(-forward.x, 0, -forward.z) * speed / 4);
                    }
                    else
                    {
                        //Go back to triple dash attack
                        inSpecial[(int)Action.Knockback] = false;
                        specialTimer[(int)Action.Knockback] = 0.0f;
                        inSpecial[(int)Attack.TripleDash] = true;
                    }
                }
                //Triple dash attack
                else if(inSpecial[(int)Attack.TripleDash])
                {
                    //Monster stays still and charges for 2 seconds
                    if (specialTimer[(int)Attack.TripleDash] < 1.0f || (specialTimer[(int)Attack.TripleDash] < 3.0f && specialTimer[(int)Attack.TripleDash] >= 2.0f) ||
                        (specialTimer[(int)Attack.TripleDash] < 5.0f && specialTimer[(int)Attack.TripleDash] >= 4.0f))
                    {
                        isRaming = false;
                        //Track player
                        destination = new Vector3(GameObject.FindGameObjectWithTag("Player").transform.position.x, transform.position.y, GameObject.FindGameObjectWithTag("Player").transform.position.z);
                        //Find the direction the monster should be looking
                        lookRotation = Quaternion.LookRotation(destination - transform.position);
                        //Find local forward vector
                        Vector3 forward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
                        specialTimer[(int)Attack.TripleDash] += Time.deltaTime;
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 3.0f);
                    }
                    else if (specialTimer[(int)Attack.TripleDash] < 2.0f || (specialTimer[(int)Attack.TripleDash] < 4.0f && specialTimer[(int)Attack.TripleDash] >= 3.0f) ||
                        (specialTimer[(int)Attack.TripleDash] < 6.0f && specialTimer[(int)Attack.TripleDash] >= 5.0f))
                    {
                        isRaming = true;
                        //Find local forward vector
                        Vector3 forward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
                        CheckCollision();
                        //If monster hits player, stop special
                        if (playerCollision || obsticalCollision)
                        {
                            specialTimer[(int)Attack.TripleDash] = Mathf.Ceil(Mathf.Log(specialTimer[1], 2)) * 2;
                            inSpecial[(int)Action.Knockback] = true;
                        }
                        else
                            specialTimer[(int)Attack.TripleDash] += Time.deltaTime;
                        transform.Translate(new Vector3(forward.x, 0, forward.z) * speed / 3);
                    }
                    else if(specialTimer[(int)Attack.TripleDash] < 9.0f && specialTimer[1] >= 6.0f)
                    {
                        isRaming = false;
                        //Koi does not move for 3 seconds to give player a chance to attack
                        specialTimer[(int)Attack.TripleDash] += Time.deltaTime;
                    }
                    //Set koi back to normal mode
                    else
                    {
                        inSpecial[(int)Action.Active] = false;
                        specialTimer[(int)Action.Active] = 0.0f;
                        specialCooldown[(int)Action.Active] = 5.0f;
                        inSpecial[(int)Attack.TripleDash] = false;
                        specialTimer[(int)Attack.TripleDash] = 0.0f;
                        specialCooldown[(int)Attack.TripleDash] = 10.0f;
                    }
                }
                else if(inSpecial[(int)Attack.BubbleBlast])
                {
                    if(specialTimer[(int)Attack.BubbleBlast] < 1.5f)
                    {
                        //Track player
                        destination = new Vector3(GameObject.FindGameObjectWithTag("Player").transform.position.x, transform.position.y, GameObject.FindGameObjectWithTag("Player").transform.position.z);
                        //Find the direction the monster should be looking
                        lookRotation = Quaternion.LookRotation(destination - transform.position);
                        //Find local forward vector
                        Vector3 forward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
                        specialTimer[(int)Attack.BubbleBlast] += Time.deltaTime;
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 3.0f);
                    }
                    else if(specialTimer[(int)Attack.BubbleBlast] < 2.0f && specialTimer[(int)Attack.BubbleBlast] >= 1.5f)
                    {
                        specialTimer[(int)Attack.BubbleBlast] += Time.deltaTime;
                    }
                    else
                    {
                        SpawnProjectile(new Vector3(0, 0, (5 * lengthMult / 6)), 0.5f, 10, 3.0f, MovementPattern.Forward);
                        SpawnProjectile(new Vector3(-0.10f, 0, (5 * lengthMult / 6) - 0.25f), 0.5f, 10, 3.0f, MovementPattern.Forward);
                        SpawnProjectile(new Vector3(-0.25f, 0, (5 * lengthMult / 6) - 0.75f), 0.5f, 10, 3.0f, MovementPattern.Forward);
                        SpawnProjectile(new Vector3(-0.50f, 0, (5 * lengthMult / 6) - 1.50f), 0.5f, 10, 3.0f, MovementPattern.Forward);
                        SpawnProjectile(new Vector3(0.10f, 0, (5 * lengthMult / 6) - 0.25f), 0.5f, 10, 3.0f, MovementPattern.Forward);
                        SpawnProjectile(new Vector3(0.25f, 0, (5 * lengthMult / 6) - 0.75f), 0.5f, 10, 3.0f, MovementPattern.Forward);
                        SpawnProjectile(new Vector3(0.50f, 0, (5 * lengthMult / 6) - 1.50f), 0.5f, 10, 3.0f, MovementPattern.Forward);
                        inSpecial[(int)Action.Active] = false;
                        specialTimer[(int)Action.Active] = 0.0f;
                        specialCooldown[(int)Action.Active] = 5.0f;
                        inSpecial[(int)Attack.BubbleBlast] = false;
                        specialTimer[(int)Attack.BubbleBlast] = 0.0f;
                        specialCooldown[(int)Attack.BubbleBlast] = 8.0f;
                    }
                }
            }
            //Switch to phase 2
            else if(!inSpecial[(int)Action.Extra])
            {
                if(specialTimer[(int)Action.Extra] < 1.0f)
                {
                    transform.Translate(Vector3.down * Time.deltaTime * 3);
                    shadow.transform.Translate(Vector3.up * Time.deltaTime * 3,Space.World);
                    heightMult += Vector3.up.y * Time.deltaTime * 3;
                    specialTimer[(int)Action.Extra] += Time.deltaTime;
                }
                else
                {
                    inSpecial[(int)Action.Extra] = true;
                    specialTimer[(int)Action.Extra] = 0.0f;
                    //Reset any specials the Koi may be in
                    inSpecial[(int)Action.Active] = false;
                    specialTimer[(int)Action.Active] = 0.0f;
                    specialCooldown[(int)Action.Active] = 5.0f;
                    inSpecial[(int)Attack.TripleDash] = false;
                    specialTimer[(int)Attack.TripleDash] = 0.0f;
                    inSpecial[(int)Attack.BubbleAttack] = false;
                    specialTimer[(int)Attack.BubbleAttack] = 0.0f;
                    inSpecial[(int)Attack.BubbleBlast] = false;
                    specialTimer[(int)Attack.BubbleBlast] = 0.0f;
                    inSpecial[(int)Action.Knockback] = false;
                    specialTimer[(int)Action.Knockback] = 0.0f;
                    isRaming = false;
                }
            }
            //Phase 2 AI
            else
            {
                //If the Koi is not in any special
                if (!inSpecial[(int)Action.Active])
                {
                    //Track player
                    destination = new Vector3(GameObject.FindGameObjectWithTag("Player").transform.position.x, transform.position.y, GameObject.FindGameObjectWithTag("Player").transform.position.z);
                    //Find the direction the monster should be looking
                    lookRotation = Quaternion.LookRotation(destination - transform.position);
                    //Find local forward vector
                    Vector3 forward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
                    //When monster gets close circle player
                    if (!CheckCollision() && playerDistance < 5.0f)
                    {
                        lookRotation = Quaternion.LookRotation(Vector3.Cross(Vector3.up, transform.forward));
                    }

                    //Decrement overall special cooldown, no special can be used while this is in cooldown.
                    if (specialCooldown[(int)Action.Active] > 0)
                        specialCooldown[(int)Action.Active] -= Time.deltaTime;

                    /*//Check to see if monster can use triple dash special attack
                    if (playerDistance < 16.0f)
                    {
                        specialCooldown[(int)Attack.TripleDash] -= Time.deltaTime;
                        if (specialCooldown[int)Action.Active] < 0.0f && specialCooldown[1] < 0.0f && Random.Range(1, 4) == 1)
                        {
                            //Use cooldown to store original height
                            specialCooldown[5] = transform.position.y;
                            inSpecial[(int)Action.Active] = true;
                            inSpecial[(int)Attack.TripleDash] = true;
                        }
                    }*/

                    //Check to see if monster can use underwater attack
                    if (playerDistance < 15.0f)
                    {
                        specialCooldown[(int)Attack.UnderwaterAttack] -= Time.deltaTime;
                        if (specialCooldown[(int)Action.Active] < 0.0f && specialCooldown[(int)Attack.UnderwaterAttack] < 0.0f && Random.Range(1, 4) == 1)
                        {
                            //Use cooldown to store original height
                            specialCooldown[(int)Action.Extra] = transform.position.y;
                            inSpecial[(int)Attack.UnderwaterAttack] = true;
                            inSpecial[(int)Action.Active] = true;
                        }
                    }

                    /*//Check to see if player can use charge projectile special attack
                    if (playerDistance < 23.0f)
                    {
                        specialCooldown[(int)Attack.BubbleBlast] -= Time.deltaTime;
                        if (specialCooldown[(int)Action.Active] < 0.0f && specialCooldown[(int)Attack.BubbleBlast] < 0.0f && Random.Range(1, 4) == 1)
                        {
                            //Use cooldown to store original height
                            specialCooldown[5] = transform.position.y;
                            inSpecial[(int)Action.Active] = true;
                            inSpecial[(int)Attack.BubbleBlast] = true;
                        }
                    }*/

                    //Rotate and move monster
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 0.4f);
                    transform.Translate(new Vector3(forward.x, 0, forward.z) * speed / 40);
                }
                //Knock back after missing
                else if (inSpecial[(int)Action.Knockback])
                {
                    if (inSpecial[(int)Attack.TripleDash])
                    {
                        if (specialTimer[(int)Action.Knockback] < 0.3f || specialTimer[(int)Action.Extra] < 1.0f)
                        {
                            //Find local forward vector
                            Vector3 forward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
                            specialTimer[(int)Action.Knockback] += Time.deltaTime;
                            transform.Translate(new Vector3(-forward.x, 0, -forward.z) * speed / 4);

                            //Stop moving fish if it goes below its original height
                            if (transform.position.y <= specialCooldown[5])
                                specialTimer[(int)Action.Extra] = 1.0f;

                            if (specialTimer[(int)Action.Extra] < 1.0f)
                            {
                                specialTimer[(int)Action.Extra] += Time.deltaTime;
                                transform.Translate(new Vector3(0, (-32 * specialTimer[(int)Action.Extra] + 16) * Time.deltaTime, 0));
                                shadow.transform.Translate(new Vector3(0, (32 * specialTimer[(int)Action.Extra] - 16) * Time.deltaTime, 0), Space.World);
                                heightMult += (32 * specialTimer[(int)Action.Extra] - 16) * Time.deltaTime;
                            }
                        }
                        else
                        {
                            //Go back to triple dash attack
                            inSpecial[(int)Action.Knockback] = false;
                            specialTimer[(int)Action.Knockback] = 0.0f;
                        }
                    }
                    else if(inSpecial[(int)Attack.UnderwaterAttack])
                    {
                        Debug.Log("In special 4");
                        if(specialTimer[(int)Action.Extra] < 1.0f)
                        {
                            //Stop moving fish if it goes below its original height
                            if (transform.position.y <= specialCooldown[5])
                                specialTimer[(int)Action.Extra] = 1.0f;

                            if (specialTimer[(int)Action.Extra] < 1.0f)
                            {
                                specialTimer[(int)Action.Extra] += Time.deltaTime;
                                transform.Translate(new Vector3(0, (-64 * specialTimer[(int)Action.Extra] + 32) * Time.deltaTime, 0));
                                shadow.transform.Translate(new Vector3(0, (64 * specialTimer[(int)Action.Extra] - 32) * Time.deltaTime, 0), Space.World);
                                heightMult += (64 * specialTimer[(int)Action.Extra] - 32) * Time.deltaTime;
                            }
                        }
                        else
                        {
                            inSpecial[(int)Action.Knockback] = false;
                            specialTimer[(int)Action.Knockback] = 0.0f;
                        }
                    }
                }
                //Triple dash attack
                else if (inSpecial[(int)Attack.TripleDash])
                {
                    //Monster stays still and charges for 2 seconds
                    if (specialTimer[(int)Attack.TripleDash] < 1.0f || (specialTimer[(int)Attack.TripleDash] < 3.0f && specialTimer[1] >= 2.0f) || (specialTimer[(int)Attack.TripleDash] < 5.0f && specialTimer[(int)Attack.TripleDash] >= 4.0f))
                    {
                        isRaming = false;
                        //Track player
                        destination = new Vector3(GameObject.FindGameObjectWithTag("Player").transform.position.x, transform.position.y, GameObject.FindGameObjectWithTag("Player").transform.position.z);
                        //Find the direction the monster should be looking
                        lookRotation = Quaternion.LookRotation(destination - transform.position);
                        //Find local forward vector
                        Vector3 forward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
                        specialTimer[(int)Attack.TripleDash] += Time.deltaTime;
                        specialTimer[(int)Action.Extra] = 0;
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 3.0f);
                    }
                    else if (specialTimer[(int)Attack.TripleDash] < 2.0f || (specialTimer[(int)Attack.TripleDash] < 4.0f && specialTimer[(int)Attack.TripleDash] >= 3.0f) || (specialTimer[(int)Attack.TripleDash] < 6.0f && specialTimer[(int)Attack.TripleDash] >= 5.0f))
                    {
                        isRaming = true;
                        //Find local forward vector
                        Vector3 forward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
                        CheckCollision();
                        //If monster hits player, stop special
                        if (playerCollision || obsticalCollision)
                        {
                            specialTimer[(int)Attack.TripleDash] = Mathf.Ceil(Mathf.Log(specialTimer[1], 2)) * 2;
                            if(specialTimer[(int)Action.Extra] < 0.5f)
                                specialTimer[(int)Action.Extra] = 1.0f - specialTimer[5];
                            inSpecial[(int)Action.Knockback] = true;
                        }
                        else
                        {
                            specialTimer[(int)Attack.TripleDash] += Time.deltaTime;
                            specialTimer[(int)Action.Extra] += Time.deltaTime;
                        }
                        transform.Translate(new Vector3(forward.x, 0, forward.z) * speed / 2);
                        //Move Koi up and down parabolically
                        transform.Translate(new Vector3(0, (-32 * specialTimer[(int)Action.Extra] + 16) * Time.deltaTime, 0));
                        shadow.transform.Translate(new Vector3(0, (32 * specialTimer[(int)Action.Extra] - 16) * Time.deltaTime, 0), Space.World);
                        heightMult += (32 * specialTimer[(int)Action.Extra] - 16) * Time.deltaTime;

                    }
                    else if(specialTimer[(int)Attack.TripleDash] < 7.0f && specialTimer[(int)Attack.TripleDash] >= 6)
                    {
                        isRaming = false;
                        specialTimer[(int)Attack.TripleDash] += Time.deltaTime;
                        transform.Translate(Vector3.up * Time.deltaTime * 3);
                        shadow.transform.Translate(Vector3.down * Time.deltaTime * 3, Space.World);
                        heightMult += Vector3.down.y * Time.deltaTime * 3;
                    }
                    else if (specialTimer[(int)Attack.TripleDash] < 10.0f && specialTimer[(int)Attack.TripleDash] >= 7.0f)
                    {
                        //Koi does not move for 3 seconds to give player a chance to attack
                        specialTimer[(int)Attack.TripleDash] += Time.deltaTime;
                    }
                    else if(specialTimer[(int)Attack.TripleDash] < 11.0f && specialTimer[(int)Attack.TripleDash] >= 10.0f)
                    {
                        specialTimer[(int)Attack.TripleDash] += Time.deltaTime;
                        if (transform.position.y > specialCooldown[(int)Action.Extra])
                        {
                            transform.Translate(Vector3.down * Time.deltaTime * 3);
                            shadow.transform.Translate(Vector3.up * Time.deltaTime * 3, Space.World);
                            heightMult += Vector3.up.y * Time.deltaTime * 3;
                        }
                    }
                    //Set koi back to normal mode
                    else
                    {
                        inSpecial[(int)Action.Active] = false;
                        specialTimer[(int)Action.Active] = 0.0f;
                        specialCooldown[(int)Action.Active] = 5.0f;
                        inSpecial[(int)Attack.TripleDash] = false;
                        specialTimer[(int)Attack.TripleDash] = 0.0f;
                        specialCooldown[(int)Attack.TripleDash] = 10.0f;
                        specialTimer[(int)Action.Extra] = 0.0f;
                    }
                }
                else if (inSpecial[(int)Attack.BubbleBlast])
                {
                    //Rise out of water
                    if(specialTimer[(int)Attack.BubbleBlast] < 1.0f)
                    {
                        specialTimer[(int)Attack.BubbleBlast] += Time.deltaTime;
                        transform.Translate(Vector3.up * Time.deltaTime * 3);
                        shadow.transform.Translate(Vector3.down * Time.deltaTime * 3, Space.World);
                        heightMult += Vector3.down.y * Time.deltaTime * 3;
                    }
                    //Track player
                    else if (specialTimer[(int)Attack.BubbleBlast] < 2.5f && specialTimer[(int)Attack.BubbleBlast] >= 1.0f)
                    {
                        //Track player
                        destination = new Vector3(GameObject.FindGameObjectWithTag("Player").transform.position.x, transform.position.y, GameObject.FindGameObjectWithTag("Player").transform.position.z);
                        //Find the direction the monster should be looking
                        lookRotation = Quaternion.LookRotation(destination - transform.position);
                        //Find local forward vector
                        Vector3 forward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
                        specialTimer[(int)Attack.BubbleBlast] += Time.deltaTime;
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 3.0f);
                    }
                    //Stop tracking for half a second
                    else if (specialTimer[(int)Attack.BubbleBlast] < 3.0f && specialTimer[(int)Attack.BubbleBlast] >= 2.5f)
                    {
                        specialTimer[(int)Attack.BubbleBlast] += Time.deltaTime;
                    }
                    //Shoot projectile
                    else if(specialTimer[(int)Attack.BubbleBlast] < 3.5 && specialTimer[(int)Attack.BubbleBlast] >= 3.0f)
                    {
                        SpawnProjectile(new Vector3(0, 0, (5 * lengthMult / 6)), 0.5f, 10, 3.0f, MovementPattern.Forward);
                        SpawnProjectile(new Vector3(-0.10f, 0, (5 * lengthMult / 6) - 0.25f), 0.5f, 10, 3.0f, MovementPattern.Forward);
                        SpawnProjectile(new Vector3(-0.25f, 0, (5 * lengthMult / 6) - 0.75f), 0.5f, 10, 3.0f, MovementPattern.Forward);
                        SpawnProjectile(new Vector3(-0.50f, 0, (5 * lengthMult / 6) - 1.50f), 0.5f, 10, 3.0f, MovementPattern.Forward);
                        SpawnProjectile(new Vector3(0.10f, 0, (5 * lengthMult / 6) - 0.25f), 0.5f, 10, 3.0f, MovementPattern.Forward);
                        SpawnProjectile(new Vector3(0.25f, 0, (5 * lengthMult / 6) - 0.75f), 0.5f, 10, 3.0f, MovementPattern.Forward);
                        SpawnProjectile(new Vector3(0.50f, 0, (5 * lengthMult / 6) - 1.50f), 0.5f, 10, 3.0f, MovementPattern.Forward);
                        specialTimer[(int)Attack.BubbleBlast] += 0.5f;
                    }
                    //Give player chance to attack
                    else if(specialTimer[(int)Attack.BubbleBlast] < 5.0f && specialTimer[(int)Attack.BubbleBlast] >= 3.5f)
                    {
                        specialTimer[(int)Attack.BubbleBlast] += Time.deltaTime;
                    }
                    //Go back under water
                    else if(specialTimer[(int)Attack.BubbleBlast] < 6.0f && specialTimer[3] >= 5.0f)
                    {
                        specialTimer[(int)Attack.BubbleBlast] += Time.deltaTime;
                        if (transform.position.y > specialCooldown[5])
                        {
                            transform.Translate(Vector3.down * Time.deltaTime * 3);
                            shadow.transform.Translate(Vector3.up * Time.deltaTime * 3, Space.World);
                            heightMult += Vector3.up.y * Time.deltaTime * 3;
                        }
                    }
                    else
                    {
                        inSpecial[(int)Action.Active] = false;
                        specialTimer[(int)Action.Active] = 0.0f;
                        specialCooldown[(int)Action.Active] = 5.0f;
                        inSpecial[(int)Attack.BubbleBlast] = false;
                        specialTimer[(int)Attack.BubbleBlast] = 0.0f;
                        specialCooldown[(int)Attack.BubbleBlast] = 8.0f;
                    }
                }
                else if(inSpecial[(int)Attack.UnderwaterAttack])
                {
                    //Track player underwater
                    if (specialTimer[(int)Attack.UnderwaterAttack] < 4.0f)
                    {
                        specialTimer[(int)Attack.UnderwaterAttack] += Time.deltaTime;
                        //Track player
                        destination = new Vector3(GameObject.FindGameObjectWithTag("Player").transform.position.x, transform.position.y, GameObject.FindGameObjectWithTag("Player").transform.position.z);
                        //Find the direction the monster should be looking
                        lookRotation = Quaternion.LookRotation(destination - transform.position);
                        //Find local forward vector
                        Vector3 forward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
                        CheckCollision();

                        //Rotate and move monster
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 1.6f);
                        transform.Translate(new Vector3(forward.x, 0, forward.z) * speed / 10);

                        //If fish is right under player, change to attack
                        if(playerDistance <= 3.1f)
                            specialTimer[(int)Attack.UnderwaterAttack] = 4.0f;
                    }
                    //Pause a second before attacking
                    else if(specialTimer[(int)Attack.UnderwaterAttack] < 4.5f && specialTimer[(int)Attack.UnderwaterAttack] >= 4.0f)
                    {
                        specialTimer[(int)Attack.UnderwaterAttack] += Time.deltaTime;
                    }
                    else if(specialTimer[(int)Attack.UnderwaterAttack] < 5.4f && specialTimer[(int)Attack.UnderwaterAttack] >= 4.5f)
                    {
                        Debug.Log("Using special 2");
                        Debug.Log(specialTimer[(int)Action.Extra]);
                        specialTimer[(int)Attack.UnderwaterAttack] += Time.deltaTime;
                        specialTimer[(int)Action.Extra] += Time.deltaTime;
                        //Move Koi up and down parabolically
                        transform.Translate(new Vector3(0, (-64 * specialTimer[(int)Action.Extra] + 32) * Time.deltaTime, 0));
                        shadow.transform.Translate(new Vector3(0, (64 * specialTimer[(int)Action.Extra] - 32) * Time.deltaTime, 0), Space.World);
                        heightMult += (64 * specialTimer[(int)Action.Extra] - 32) * Time.deltaTime;

                        if(playerCollision)
                        {
                            specialTimer[(int)Attack.UnderwaterAttack] = 9.5f;
                            Debug.Log(specialTimer[(int)Action.Extra]);
                            if (specialTimer[(int)Action.Extra] < 0.5)
                            {
                                specialTimer[(int)Action.Extra] = 1 - specialTimer[5];
                                Debug.Log(specialTimer[(int)Action.Extra]);
                            }
                            inSpecial[(int)Action.Knockback] = true;
                        }
                    }
                    //Give player 3 seconds to attack if the fish missed
                    else if(specialTimer[(int)Attack.UnderwaterAttack] < 8.4f && specialTimer[(int)Attack.UnderwaterAttack] >= 5.4f)
                    {
                        specialTimer[(int)Attack.UnderwaterAttack] += Time.deltaTime;
                    }
                    //Koi goes all the way back under
                    else if(specialTimer[(int)Attack.UnderwaterAttack] < 9.4f && specialTimer[(int)Attack.UnderwaterAttack] >= 8.4f)
                    {
                        specialTimer[(int)Attack.UnderwaterAttack] += Time.deltaTime;
                        specialTimer[5] += Time.deltaTime;
                        //Move Koi up and down parabolically
                        if (transform.position.y > specialCooldown[(int)Action.Extra])
                        {
                            transform.Translate(Vector3.down * Time.deltaTime * 3);
                            shadow.transform.Translate(Vector3.up * Time.deltaTime * 3, Space.World);
                            heightMult += Vector3.up.y * Time.deltaTime * 3;
                        }
                    }
                    //Go back to normal
                    else
                    {
                        inSpecial[(int)Action.Active] = false;
                        specialTimer[(int)Action.Active] = 0.0f;
                        specialCooldown[(int)Action.Active] = 4.0f;
                        inSpecial[(int)Attack.UnderwaterAttack] = false;
                        specialTimer[(int)Attack.UnderwaterAttack] = 0.0f;
                        specialCooldown[(int)Attack.UnderwaterAttack] = 8.0f;
                        specialTimer[(int)Action.Extra] = 0.0f;
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
