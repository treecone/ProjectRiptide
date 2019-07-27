using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

                    //Decrement overall special cooldown, no special can be used while this is in cooldown.
                    if (specialCooldown[0] > 0)
                        specialCooldown[0] -= Time.deltaTime;

                    //Check to see if monster can use triple dash special attack
                    if (playerDistance < 13.0f)
                    {
                        specialCooldown[1] -= Time.deltaTime;
                        if (specialCooldown[0] < 0.0f && specialCooldown[1] < 0.0f && Random.Range(1, 4) == 1)
                        {
                            inSpecial[0] = true;
                            inSpecial[1] = true;
                        }
                    }

                    //Check to see if monster can use bubble attack
                    if (playerDistance > 10.0f)
                    {
                        specialCooldown[2] -= Time.deltaTime;
                        if (specialCooldown[0] < 0.0f && specialCooldown[2] < 0.0f && Random.Range(1, 4) == 1)
                        {
                            //Load projectile
                            SpawnProjectile(new Vector3(0, 0, 5 * lengthMult / 6), 1.0f, 10, 3.0f, MovementPattern.Forward);
                            specialCooldown[0] = 2.0f;
                            specialCooldown[2] = 3.0f;
                        }
                    }

                    //Check to see if player can use charge projectile special attack
                    if(playerDistance < 20.0f)
                    {
                        specialCooldown[3] -= Time.deltaTime;
                        if (specialCooldown[0] < 0.0f && specialCooldown[3] < 0.0f && Random.Range(1, 4) == 1)
                        {
                            inSpecial[0] = true;
                            inSpecial[3] = true;
                        }
                    }

                    //Rotate and move monster
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 0.4f);
                    transform.Translate(new Vector3(forward.x, 0, forward.z) * speed / 40);
                }
                //Knock back after missing
                else if(inSpecial[4])
                {
                    if (specialTimer[4] < 0.3f)
                    {
                        //Find local forward vector
                        Vector3 forward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
                        specialTimer[4] += Time.deltaTime;
                        transform.Translate(new Vector3(-forward.x, 0, -forward.z) * speed / 4);
                    }
                    else
                    {
                        //Go back to triple dash attack
                        inSpecial[4] = false;
                        specialTimer[4] = 0.0f;
                        inSpecial[1] = true;
                    }
                }
                //Triple dash attack
                else if(inSpecial[1])
                {
                    //Monster stays still and charges for 2 seconds
                    if (specialTimer[1] < 1.0f || (specialTimer[1] < 3.0f && specialTimer[1] >= 2.0f) || (specialTimer[1] < 5.0f && specialTimer[1] >= 4.0f))
                    {
                        isRaming = false;
                        //Track player
                        destination = new Vector3(GameObject.FindGameObjectWithTag("Player").transform.position.x, transform.position.y, GameObject.FindGameObjectWithTag("Player").transform.position.z);
                        //Find the direction the monster should be looking
                        lookRotation = Quaternion.LookRotation(destination - transform.position);
                        //Find local forward vector
                        Vector3 forward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
                        specialTimer[1] += Time.deltaTime;
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 3.0f);
                    }
                    else if (specialTimer[1] < 2.0f || (specialTimer[1] < 4.0f && specialTimer[1] >= 3.0f) || (specialTimer[1] < 6.0f && specialTimer[1] >= 5.0f))
                    {
                        isRaming = true;
                        //Find local forward vector
                        Vector3 forward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
                        CheckCollision();
                        //If monster hits player, stop special
                        if (playerCollision || obsticalCollision)
                        {
                            specialTimer[1] = Mathf.Ceil(Mathf.Log(specialTimer[1], 2)) * 2;
                            inSpecial[4] = true;
                        }
                        else
                            specialTimer[1] += Time.deltaTime;
                        transform.Translate(new Vector3(forward.x, 0, forward.z) * speed / 3);
                    }
                    else if(specialTimer[1] < 9.0f && specialTimer[1] >= 6.0f)
                    {
                        isRaming = false;
                        //Koi does not move for 3 seconds to give player a chance to attack
                        specialTimer[1] += Time.deltaTime;
                    }
                    //Set koi back to normal mode
                    else
                    {
                        inSpecial[0] = false;
                        specialTimer[0] = 0.0f;
                        specialCooldown[0] = 5.0f;
                        inSpecial[1] = false;
                        specialTimer[1] = 0.0f;
                        specialCooldown[1] = 10.0f;
                    }
                }
                else if(inSpecial[3])
                {
                    if(specialTimer[3] < 1.5f)
                    {
                        //Track player
                        destination = new Vector3(GameObject.FindGameObjectWithTag("Player").transform.position.x, transform.position.y, GameObject.FindGameObjectWithTag("Player").transform.position.z);
                        //Find the direction the monster should be looking
                        lookRotation = Quaternion.LookRotation(destination - transform.position);
                        //Find local forward vector
                        Vector3 forward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
                        specialTimer[3] += Time.deltaTime;
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 3.0f);
                    }
                    else if(specialTimer[3] < 2.0f && specialTimer[3] >= 1.5f)
                    {
                        specialTimer[3] += Time.deltaTime;
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
                        inSpecial[0] = false;
                        specialTimer[0] = 0.0f;
                        specialCooldown[0] = 5.0f;
                        inSpecial[3] = false;
                        specialTimer[3] = 0.0f;
                        specialCooldown[3] = 8.0f;
                    }
                }
            }
            //Switch to phase 2
            else if(!inSpecial[5])
            {
                if(specialTimer[5] < 1.0f)
                {
                    transform.Translate(Vector3.down * Time.deltaTime * 3);
                    shadow.transform.Translate(Vector3.up * Time.deltaTime * 3,Space.World);
                    heightMult += Vector3.up.y * Time.deltaTime * 3;
                    specialTimer[5] += Time.deltaTime;
                }
                else
                {
                    inSpecial[5] = true;
                    //Reset any specials the Koi may be in
                    inSpecial[0] = false;
                    specialTimer[0] = 0.0f;
                    inSpecial[1] = false;
                    specialTimer[1] = 0.0f;
                    inSpecial[2] = false;
                    specialTimer[2] = 0.0f;
                    inSpecial[3] = false;
                    specialTimer[3] = 0.0f;
                    inSpecial[4] = false;
                    specialTimer[4] = 0.0f;
                    isRaming = false;
                }
            }
            //Phase 2 AI
            else
            {
                //If the Koi is not in any special
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

                    //Decrement overall special cooldown, no special can be used while this is in cooldown.
                    if (specialCooldown[0] > 0)
                        specialCooldown[0] -= Time.deltaTime;

                    //Check to see if monster can use triple dash special attack
                    if (playerDistance < 16.0f)
                    {
                        specialCooldown[1] -= Time.deltaTime;
                        if (specialCooldown[0] < 0.0f && specialCooldown[1] < 0.0f && Random.Range(1, 4) == 1)
                        {
                            //Use cooldown to store original height
                            specialCooldown[5] = transform.position.y;
                            inSpecial[0] = true;
                            inSpecial[1] = true;
                        }
                    }

                    //Check to see if monster can use underwater attack
                    if (playerDistance < 15.0f)
                    {
                        specialCooldown[2] -= Time.deltaTime;
                        if (specialCooldown[0] < 0.0f && specialCooldown[2] < 0.0f && Random.Range(1, 4) == 1)
                        {
                            //Use cooldown to store original height
                            specialCooldown[5] = transform.position.y;
                            inSpecial[2] = true;
                            inSpecial[0] = true;
                        }
                    }

                    //Check to see if player can use charge projectile special attack
                    if (playerDistance < 23.0f)
                    {
                        specialCooldown[3] -= Time.deltaTime;
                        if (specialCooldown[0] < 0.0f && specialCooldown[3] < 0.0f && Random.Range(1, 4) == 1)
                        {
                            //Use cooldown to store original height
                            specialCooldown[5] = transform.position.y;
                            inSpecial[0] = true;
                            inSpecial[3] = true;
                        }
                    }

                    //Rotate and move monster
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 0.4f);
                    transform.Translate(new Vector3(forward.x, 0, forward.z) * speed / 40);
                }
                //Knock back after missing
                else if (inSpecial[4])
                {
                    if (inSpecial[1])
                    {
                        if (specialTimer[4] < 0.3f || specialTimer[5] < 1.0f)
                        {
                            //Find local forward vector
                            Vector3 forward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
                            specialTimer[4] += Time.deltaTime;
                            transform.Translate(new Vector3(-forward.x, 0, -forward.z) * speed / 4);

                            //Stop moving fish if it goes below its original height
                            if (transform.position.y <= specialCooldown[5])
                                specialTimer[5] = 1.0f;

                            if (specialTimer[5] < 1.0f)
                            {
                                specialTimer[5] += Time.deltaTime;
                                transform.Translate(new Vector3(0, (-32 * specialTimer[5] + 16) * Time.deltaTime, 0));
                                shadow.transform.Translate(new Vector3(0, (32 * specialTimer[5] - 16) * Time.deltaTime, 0), Space.World);
                                heightMult += (32 * specialTimer[5] - 16) * Time.deltaTime;
                            }
                        }
                        else
                        {
                            //Go back to triple dash attack
                            inSpecial[4] = false;
                            specialTimer[4] = 0.0f;
                        }
                    }
                    else if(inSpecial[2])
                    {
                        if(specialTimer[5] < 1.0f)
                        {
                            //Stop moving fish if it goes below its original height
                            if (transform.position.y <= specialCooldown[5])
                                specialTimer[5] = 1.0f;

                            if (specialTimer[5] < 1.0f)
                            {
                                specialTimer[5] += Time.deltaTime;
                                transform.Translate(new Vector3(0, (-64 * specialTimer[5] + 32) * Time.deltaTime, 0));
                                shadow.transform.Translate(new Vector3(0, (64 * specialTimer[5] - 32) * Time.deltaTime, 0), Space.World);
                                heightMult += (64 * specialTimer[5] - 32) * Time.deltaTime;
                            }
                        }
                        else
                        {
                            inSpecial[4] = false;
                            specialTimer[4] = 0.0f;
                        }
                    }
                }
                //Triple dash attack
                else if (inSpecial[1])
                {
                    //Monster stays still and charges for 2 seconds
                    if (specialTimer[1] < 1.0f || (specialTimer[1] < 3.0f && specialTimer[1] >= 2.0f) || (specialTimer[1] < 5.0f && specialTimer[1] >= 4.0f))
                    {
                        isRaming = false;
                        //Track player
                        destination = new Vector3(GameObject.FindGameObjectWithTag("Player").transform.position.x, transform.position.y, GameObject.FindGameObjectWithTag("Player").transform.position.z);
                        //Find the direction the monster should be looking
                        lookRotation = Quaternion.LookRotation(destination - transform.position);
                        //Find local forward vector
                        Vector3 forward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
                        specialTimer[1] += Time.deltaTime;
                        specialTimer[5] = 0;
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 3.0f);
                    }
                    else if (specialTimer[1] < 2.0f || (specialTimer[1] < 4.0f && specialTimer[1] >= 3.0f) || (specialTimer[1] < 6.0f && specialTimer[1] >= 5.0f))
                    {
                        isRaming = true;
                        //Find local forward vector
                        Vector3 forward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
                        CheckCollision();
                        //If monster hits player, stop special
                        if (playerCollision || obsticalCollision)
                        {
                            specialTimer[1] = Mathf.Ceil(Mathf.Log(specialTimer[1], 2)) * 2;
                            if(specialTimer[5] < 0.5f)
                                specialTimer[5] = 1.0f - specialTimer[5];
                            inSpecial[4] = true;
                        }
                        else
                        {
                            specialTimer[1] += Time.deltaTime;
                            specialTimer[5] += Time.deltaTime;
                        }
                        transform.Translate(new Vector3(forward.x, 0, forward.z) * speed / 2);
                        //Move Koi up and down parabolically
                        transform.Translate(new Vector3(0, (-32 * specialTimer[5] + 16) * Time.deltaTime, 0));
                        shadow.transform.Translate(new Vector3(0, (32 * specialTimer[5] - 16) * Time.deltaTime, 0), Space.World);
                        heightMult += (32 * specialTimer[5] - 16) * Time.deltaTime;

                    }
                    else if(specialTimer[1] < 7.0f && specialTimer[1] >= 6)
                    {
                        isRaming = false;
                        specialTimer[1] += Time.deltaTime;
                        transform.Translate(Vector3.up * Time.deltaTime * 3);
                        shadow.transform.Translate(Vector3.down * Time.deltaTime * 3, Space.World);
                        heightMult += Vector3.down.y * Time.deltaTime * 3;
                    }
                    else if (specialTimer[1] < 10.0f && specialTimer[1] >= 7.0f)
                    {
                        //Koi does not move for 3 seconds to give player a chance to attack
                        specialTimer[1] += Time.deltaTime;
                    }
                    else if(specialTimer[1] < 11.0f && specialTimer[1] >= 10.0f)
                    {
                        specialTimer[1] += Time.deltaTime;
                        if (transform.position.y > specialCooldown[5])
                        {
                            transform.Translate(Vector3.down * Time.deltaTime * 3);
                            shadow.transform.Translate(Vector3.up * Time.deltaTime * 3, Space.World);
                            heightMult += Vector3.up.y * Time.deltaTime * 3;
                        }
                    }
                    //Set koi back to normal mode
                    else
                    {
                        inSpecial[0] = false;
                        specialTimer[0] = 0.0f;
                        specialCooldown[0] = 5.0f;
                        inSpecial[1] = false;
                        specialTimer[1] = 0.0f;
                        specialCooldown[1] = 10.0f;
                        specialTimer[5] = 0.0f;
                    }
                }
                else if (inSpecial[3])
                {
                    //Rise out of water
                    if(specialTimer[3] < 1.0f)
                    {
                        specialTimer[3] += Time.deltaTime;
                        transform.Translate(Vector3.up * Time.deltaTime * 3);
                        shadow.transform.Translate(Vector3.down * Time.deltaTime * 3, Space.World);
                        heightMult += Vector3.down.y * Time.deltaTime * 3;
                    }
                    //Track player
                    else if (specialTimer[3] < 2.5f && specialTimer[3] >= 1.0f)
                    {
                        isRaming = true;
                        //Track player
                        destination = new Vector3(GameObject.FindGameObjectWithTag("Player").transform.position.x, transform.position.y, GameObject.FindGameObjectWithTag("Player").transform.position.z);
                        //Find the direction the monster should be looking
                        lookRotation = Quaternion.LookRotation(destination - transform.position);
                        //Find local forward vector
                        Vector3 forward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
                        specialTimer[3] += Time.deltaTime;
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 3.0f);
                    }
                    //Stop tracking for half a second
                    else if (specialTimer[3] < 3.0f && specialTimer[3] >= 2.5f)
                    {
                        isRaming = false;
                        specialTimer[3] += Time.deltaTime;
                    }
                    //Shoot projectile
                    else if(specialTimer[3] < 3.5 && specialTimer[3] >= 3.0f)
                    {
                        SpawnProjectile(new Vector3(0, 0, (5 * lengthMult / 6)), 0.5f, 10, 3.0f, MovementPattern.Forward);
                        SpawnProjectile(new Vector3(-0.10f, 0, (5 * lengthMult / 6) - 0.25f), 0.5f, 10, 3.0f, MovementPattern.Forward);
                        SpawnProjectile(new Vector3(-0.25f, 0, (5 * lengthMult / 6) - 0.75f), 0.5f, 10, 3.0f, MovementPattern.Forward);
                        SpawnProjectile(new Vector3(-0.50f, 0, (5 * lengthMult / 6) - 1.50f), 0.5f, 10, 3.0f, MovementPattern.Forward);
                        SpawnProjectile(new Vector3(0.10f, 0, (5 * lengthMult / 6) - 0.25f), 0.5f, 10, 3.0f, MovementPattern.Forward);
                        SpawnProjectile(new Vector3(0.25f, 0, (5 * lengthMult / 6) - 0.75f), 0.5f, 10, 3.0f, MovementPattern.Forward);
                        SpawnProjectile(new Vector3(0.50f, 0, (5 * lengthMult / 6) - 1.50f), 0.5f, 10, 3.0f, MovementPattern.Forward);
                        specialTimer[3] += 0.5f;
                    }
                    //Give player chance to attack
                    else if(specialTimer[3] < 5.0f && specialTimer[3] >= 3.5f)
                    {
                        specialTimer[3] += Time.deltaTime;
                    }
                    //Go back under water
                    else if(specialTimer[3] < 6.0f && specialTimer[3] >= 5.0f)
                    {
                        specialTimer[3] += Time.deltaTime;
                        if (transform.position.y > specialCooldown[5])
                        {
                            transform.Translate(Vector3.down * Time.deltaTime * 3);
                            shadow.transform.Translate(Vector3.up * Time.deltaTime * 3, Space.World);
                            heightMult += Vector3.up.y * Time.deltaTime * 3;
                        }
                    }
                    else
                    {
                        inSpecial[0] = false;
                        specialTimer[0] = 0.0f;
                        specialCooldown[0] = 5.0f;
                        inSpecial[3] = false;
                        specialTimer[3] = 0.0f;
                        specialCooldown[3] = 8.0f;
                    }
                }
                else if(inSpecial[2])
                {
                    //Track player underwater
                    if (specialTimer[2] < 4.0f)
                    {
                        specialTimer[2] += Time.deltaTime;
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
                            specialTimer[2] = 4.0f;
                    }
                    //Pause a second before attacking
                    else if(specialTimer[2] < 4.5f && specialTimer[2] >= 4.0f)
                    {
                        specialTimer[2] += Time.deltaTime;
                    }
                    else if(specialTimer[2] < 5.4f && specialTimer[2] >= 4.5f)
                    {
                        specialTimer[2] += Time.deltaTime;
                        specialTimer[5] += Time.deltaTime;
                        //Move Koi up and down parabolically
                        transform.Translate(new Vector3(0, (-64 * specialTimer[5] + 32) * Time.deltaTime, 0));
                        shadow.transform.Translate(new Vector3(0, (64 * specialTimer[5] - 32) * Time.deltaTime, 0), Space.World);
                        heightMult += (64 * specialTimer[5] - 32) * Time.deltaTime;

                        if(playerCollision)
                        {
                            specialTimer[2] = 9.5f;
                            if(specialTimer[5] < 0.5)
                                specialTimer[5] = 1 - specialTimer[5];
                            inSpecial[4] = true;
                        }
                    }
                    //Give player 3 seconds to attack if the fish missed
                    else if(specialTimer[2] < 8.4f && specialTimer[2] >= 5.4f)
                    {
                        specialTimer[2] += Time.deltaTime;
                    }
                    //Koi goes all the way back under
                    else if(specialTimer[2] < 9.4f && specialTimer[2] >= 8.4f)
                    {
                        specialTimer[2] += Time.deltaTime;
                        specialTimer[5] += Time.deltaTime;
                        //Move Koi up and down parabolically
                        if (transform.position.y > specialCooldown[5])
                        {
                            transform.Translate(Vector3.down * Time.deltaTime * 3);
                            shadow.transform.Translate(Vector3.up * Time.deltaTime * 3, Space.World);
                            heightMult += Vector3.up.y * Time.deltaTime * 3;
                        }
                    }
                    //Go back to normal
                    else
                    {
                        inSpecial[0] = false;
                        specialTimer[0] = 0.0f;
                        specialCooldown[0] = 4.0f;
                        inSpecial[2] = false;
                        specialTimer[2] = 0.0f;
                        specialCooldown[2] = 8.0f;
                        specialTimer[5] = 0.0f;
                    }
                }
            }
        }
    }
}
