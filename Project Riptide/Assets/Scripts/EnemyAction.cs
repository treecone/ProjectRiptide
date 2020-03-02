using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Enemy : PhysicsScript
{
    //General method for making the moster follow the player
    //Usually should be used for enemy AI when not in an action
    private void FollowPlayer()
    {
        //Calculate net force
        Vector3 netForce = Seek(new Vector3(PlayerPosition().x, transform.position.y, PlayerPosition().z));
        netForce += new Vector3(transform.forward.x, 0, transform.forward.z).normalized * maxSpeed;

        //Check for collision
        /*if (CheckCollision() || playerDistance < 5.0f)
        {
            netForce = Vector3.Cross(Vector3.up, netForce);
        }*/
        if (CheckObstacle())
        {
            ApplyForce(Steer(AvoidObstacle()) * 2.0f);
        }

        //Rotate in towards direction of velocity
        if (velocity != Vector3.zero) 
            rotation = Quaternion.RotateTowards(rotation, Quaternion.LookRotation(velocity), 4.0f);

        ApplyForce(netForce);
        //ApplyFriction(0.25f);
    }

    private bool DoActionQueue()
    {
        //Go through enmeies action queue
        if (actionQueue.Peek()(ref currTime))
        {
            currTime += Time.deltaTime;
        }
        //If action is finished, go to next action
        //or end attack sequence
        else
        {
            actionQueue.Dequeue();

            if (actionQueue.Count != 0)
            {
                currTime = 0;
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
        //Stop motion before charge
        if (time == 0)
            StopMotion();

        //Look towards player
        destination = new Vector3(PlayerPosition().x, transform.position.y, PlayerPosition().z);
        rotation = Quaternion.RotateTowards(rotation, Quaternion.LookRotation(destination - transform.position), 1.0f);

        if (time >= 2.0f)
            return false;
        else
            return true;
    }

    /// <summary>
    /// Dashes for 1 second
    /// </summary>
    /// <param name="time">Current Time</param>
    /// <returns></returns>
    private bool DashAttack(ref float time)
    {
        //Add hitbox at begining
        if (time == 0.0f)
            hitboxes.Add(CreateHitbox(transform.position + transform.forward * 3.0f, new Vector3(1, 1, 1), HitboxType.EnemyHitbox, ramingDamage));

        if (!inKnockback)
        {
            //If monster hits player, stop special
            if (playerCollision || obsticalCollision)
            {
                inKnockback = true;
                time = 0.7f;
            }

            //Move forwards
            ApplyConstantMoveForce(transform.forward, 20.0f * speed, 1.0f);
        }
        //Do knockback if there was a hit
        else
        {
            //Move backwards
            ApplyConstantMoveForce(-transform.forward, 10.0f * speed, 0.3f);
        }

        if (time >= 1.0f)
        {
            GameObject.Destroy(hitboxes[hitboxes.Count - 1]);
            hitboxes.RemoveAt(hitboxes.Count - 1);
            inKnockback = false;
            return false;
        }
        else
            return true;
    }

    /// <summary>
    /// Used for KoiBoss, Charges a dash for 1 second
    /// </summary>
    /// <param name="time">Current time</param>
    /// <returns></returns>
    private bool KoiDashCharge(ref float time)
    {
        //Stop motion at begining of charge
        if (time == 0)
            StopMotion();

        //Look towards player
        destination = new Vector3(PlayerPosition().x, transform.position.y, PlayerPosition().z);
        rotation = Quaternion.RotateTowards(rotation, Quaternion.LookRotation(destination - transform.position), 3.0f);

        if (time >= 1.0f)
            return false;
        else
            return true;
    }

    /// <summary>
    /// Used for KoiBoss, Dashes at player for 1 second
    /// </summary>
    /// <param name="time">Current Time</param>
    /// <returns></returns>
    private bool KoiDashAttack(ref float time)
    {
        //Add hitbox and start dash
        if (time == 0.0f)
        {
            hitboxes.Add(CreateHitbox(transform.position + transform.forward * 3.0f, new Vector3(1, 1, 1), HitboxType.EnemyHitbox, ramingDamage));
            ApplyMoveForce(transform.forward, 30.0f * speed, 1.0f);
        }

        if (!inKnockback)
        {
            if (playerCollision || obsticalCollision)
            {
                //Go into knockback
                StopMotion();
                inKnockback = true;
                time = 0.7f;
                ApplyMoveForce(-transform.forward, 2.0f * speed, 0.3f);
            }
            //ApplyFriction(0.25f);
        }
        //Do knockback if there was a hit
        else
        {
            //ApplyConstantMoveForce(-transform.forward, 2.0f * speed, 0.3f);
            //ApplyFriction(0.25f);
        }

        if (time >= 1.0f)
        {
            GameObject.Destroy(hitboxes[hitboxes.Count - 1]);
            hitboxes.RemoveAt(hitboxes.Count - 1);
            inKnockback = false;
            StopMotion();
            return false;
        }
        else
            return true;
    }

    /// <summary>
    /// Used for KoiBoss, charges bubble blast attack for 2 seconds
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    private bool KoiBubbleBlastCharge(ref float time)
    {
        if (time < 1.5f)
        {
            //Stop motion at start
            if (time == 0)
                StopMotion();

            //Look towards player
            destination = new Vector3(PlayerPosition().x, transform.position.y, PlayerPosition().z);
            rotation = Quaternion.RotateTowards(rotation, Quaternion.LookRotation(destination - transform.position), 3.0f);
        }

        if (time >= 2.0f)
            return false;
        else
            return true;
    }

    /// <summary>
    /// Used for KoiBoss, sends out 7 projectiles in an arc
    /// </summary>
    /// <param name="time">Current time</param>
    /// <returns></returns>
    private bool KoiBubbleBlastAttack(ref float time)
    {
        //Spawn projectiles
        SpawnProjectile(new Vector3(0, 0, (5 * lengthMult / 6)), 0.5f, 10, 3.0f, MovementPattern.Forward);
        SpawnProjectile(new Vector3(-0.10f, 0, (5 * lengthMult / 6) - 0.25f), 0.5f, 10, 3.0f, MovementPattern.Forward);
        SpawnProjectile(new Vector3(-0.25f, 0, (5 * lengthMult / 6) - 0.75f), 0.5f, 10, 3.0f, MovementPattern.Forward);
        SpawnProjectile(new Vector3(-0.50f, 0, (5 * lengthMult / 6) - 1.50f), 0.5f, 10, 3.0f, MovementPattern.Forward);
        SpawnProjectile(new Vector3(0.10f, 0, (5 * lengthMult / 6) - 0.25f), 0.5f, 10, 3.0f, MovementPattern.Forward);
        SpawnProjectile(new Vector3(0.25f, 0, (5 * lengthMult / 6) - 0.75f), 0.5f, 10, 3.0f, MovementPattern.Forward);
        SpawnProjectile(new Vector3(0.50f, 0, (5 * lengthMult / 6) - 1.50f), 0.5f, 10, 3.0f, MovementPattern.Forward);

        return false;
    }

    /// <summary>
    /// Used for KoiBoss, sends out one projectile towards player
    /// </summary>
    /// <param name="time">Current time</param>
    /// <returns></returns>
    private bool KoiBubbleAttack(ref float time)
    {
        SpawnProjectile(new Vector3(0, 0, 5 * lengthMult / 6), 1.0f, 10, 3.0f, MovementPattern.Forward);

        return false;
    }

    /// <summary>
    /// Used for KoiBoss, Koi dashes three times at player going above and bellow the water
    /// </summary>
    /// <param name="time">Current time</param>
    /// <returns></returns>
    private bool KoiUnderwaterDash(ref float time)
    {
        //Start dashing
        if (time == 0.0f)
        {
            hitboxes.Add(CreateHitbox(transform.position + transform.forward * 3.0f, new Vector3(1, 1, 1), HitboxType.EnemyHitbox, ramingDamage));
            gravity = ApplyArcForce(transform.forward, 30.0f * speed, 2f * transform.localScale.y, 1.0f);
        }

        if (!inKnockback)
        {
            //If monster hits player, stop special
            if (playerCollision || obsticalCollision)
            {
                inKnockback = true;
                velocity.x = 0;
                velocity.z = 0;
            }
            ApplyForce(gravity);
            //ApplyFriction(0.25f);
        }
        //Do knockback if there was a hit
        else
        {
            ApplyConstantMoveForce(-transform.forward, 5.0f * speed, 0.3f);
            ApplyForce(gravity);

            //Stop moving fish if it goes below its original height
            if (transform.position.y <= initalPos)
            {
                ReturnToInitalPosition();
                time = 1.0f;
            }
        }

        if (time >= 1.0f)
        {
            StopMotion();
            GameObject.Destroy(hitboxes[hitboxes.Count - 1]);
            hitboxes.RemoveAt(hitboxes.Count - 1);
            inKnockback = false;
            return false;
        }
        else
            return true;
    }

    /// <summary>
    /// Used for KoiBoss, Koi comes above water for player to get a chance
    /// to attack, then goes back under.
    /// </summary>
    /// <param name="time">Current Time</param>
    /// <returns></returns>
    private bool KoiUnderwaterDashReturn(ref float time)
    {
        //Move fish above water
        if (time < 1.0f)
        {
            ApplyConstantMoveForce(Vector3.up, 1.5f * transform.localScale.y, 1.0f);
        }
        else if (time < 4.0f)
        {
            StopMotion();
            //Do nothing, give player chance to attack
        }
        //Move fish back underwater
        else if (transform.position.y >= initalPos)
        {
            ApplyConstantMoveForce(Vector3.down, 2f * transform.localScale.y, 1.0f);
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
        //Move fish out of water
        ApplyConstantMoveForce(Vector3.up, 1.5f * transform.localScale.y, 1.0f);

        if (time >= 1.0f)
        {
            StopMotion();
            return false;
        }
        else
            return true;
    }

    /// <summary>
    /// Used for KoiBoss, returns koi underwater after bubble blast attack
    /// Also gives player a chance to attack for 1.5 seconds
    /// </summary>
    /// <param name="time">Current time</param>
    /// <returns></returns>
    private bool KoiBubbleBlastReturn(ref float time)
    {
        if (time < 1.5f)
        {
            //Do nothing, give player chance to attack
        }
        else if (transform.position.y > initalPos)
        {
            //Move fish back down
            ApplyConstantMoveForce(Vector3.down, 2f * transform.localScale.y, 1.0f);
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
        if (time < 4.0f)
        {
            //Calculate net force
            Vector3 netForce = Seek(new Vector3(PlayerPosition().x, transform.position.y, PlayerPosition().z));
            netForce += new Vector3(transform.forward.x, 0, transform.forward.z).normalized * maxSpeed;

            //Check for collision
            /*if (CheckCollision() || playerDistance < 5.0f)
            {
                netForce = Vector3.Cross(Vector3.up, netForce);
            }*/
            if (CheckObstacle())
            {
                ApplyForce(Steer(AvoidObstacle()) * 2.0f);
            }

            //Rotate in towards direction of velocity
            //rotation = Quaternion.LookRotation(velocity);
            if (velocity != Vector3.zero)
                rotation = Quaternion.RotateTowards(rotation, Quaternion.LookRotation(velocity), 4.0f);

            ApplyForce(netForce);
            ApplyFriction(0.25f);

            //If fish is right under player, change to attack
            if (playerDistance <= 3.1f)
            {
                StopMotion();
                time = 4.0f;
            }
        }

        if (time >= 4.5f)
        {
            StopMotion();
            return false;
        }
        else
            return true;
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
        if (time == 0.0f)
        {
            hitboxes.Add(CreateHitbox(transform.position, new Vector3(1, 2.3f, 6), HitboxType.EnemyHitbox, ramingDamage));
            gravity = ApplyArcForce(Vector3.up, 0.0f, 15.0f, 1.0f);
        }

        if (time <= 0.94f)
        {
            if (!inKnockback)
            {
                //If monster hits player, stop special
                if (playerCollision || obsticalCollision)
                {
                    inKnockback = true;
                }
                ApplyForce(gravity);
            }
            //Do knockback if there was a hit
            else
            {
                //Stop moving fish if it goes below its original height
                if (transform.position.y <= initalPos)
                    time = 3.9f;
                else
                {
                    ApplyConstantMoveForce(Vector3.down, 1.5f * transform.localScale.y, 1.0f);
                }
            }
        }
        //At the end of the attack, stop motion and remove hitbox
        if (time > 0.94 && hitboxes.Count > 0)
        {
            StopMotion();
            Destroy(hitboxes[hitboxes.Count - 1]);
            hitboxes.RemoveAt(hitboxes.Count - 1);
        }

        //If player was not hit, make the koi go back under water
        if (time >= 3.9 && !inKnockback)
        {
            //Move fish down until its back in original position
            if (transform.position.y > initalPos)
            {
                ApplyConstantMoveForce(Vector3.down, 1.5f * transform.localScale.y, 1.0f);
            }
            else
            {
                StopMotion();
                ReturnToInitalPosition();
                inKnockback = true;
            }
        }
        //Finish attack
        else if (time >= 3.9f)
        {
            StopMotion();
            inKnockback = false;
            return false;
        }

        return true;
    }
}