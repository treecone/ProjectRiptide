﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Enemy : MonoBehaviour
{
    //General method for making the moster follow the player
    //Usually should be used for enemy AI when not in an action
    private void FollowPlayer()
    {
        //Track player
        destination = new Vector3(PlayerPosition().x, transform.position.y, PlayerPosition().z);
        //Find the direction the monster should be looking
        lookRotation = Quaternion.LookRotation(destination - transform.position);
        //Find local forward vector
        Vector3 forward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
        //When monster gets close circle player
        if (!CheckCollision() && playerDistance < 5.0f)
        {
            lookRotation = Quaternion.LookRotation(Vector3.Cross(Vector3.up, transform.forward));
        }

        //Rotate and move monster
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 0.4f);
        transform.Translate(new Vector3(forward.x, 0, forward.z) * speed / 40);
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
        //Track player
        destination = new Vector3(PlayerPosition().x, transform.position.y, PlayerPosition().z);
        //Find the direction the monster should be looking
        lookRotation = Quaternion.LookRotation(destination - transform.position);
        //Find local forward vector
        Vector3 forward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 1.0f);

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
        if(time == 0.0f)
        {
            hitboxes.Add(CreateHitbox(transform.position + transform.forward * 3.0f, new Vector3(1, 1, 1), HitboxType.PlayerHurtbox, ramingDamage));
        }

        if (!inKnockback)
        {
            //Find local forward vector
            Vector3 forward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
            CheckCollision();
            //If monster hits player, stop special
            if (playerCollision || obsticalCollision)
            {
                inKnockback = true;
                time = 0.7f;
            }
            transform.Translate(new Vector3(forward.x, 0, forward.z) * speed / 2);
        }
        //Do knockback if there was a hit
        else
        {
            //Find local forward vector
            Vector3 forward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
            transform.Translate(new Vector3(-forward.x, 0, -forward.z) * speed / 6);
        }

        if (time >= 1.0f)
        {
            GameObject.Destroy(hitboxes[hitboxes.Count - 1]);
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
        //Track player
        destination = new Vector3(PlayerPosition().x, transform.position.y, PlayerPosition().z);
        //Find the direction the monster should be looking
        lookRotation = Quaternion.LookRotation(destination - transform.position);
        //Find local forward vector
        Vector3 forward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 3.0f);

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
        if (time == 0.0f)
        {
            hitboxes.Add(CreateHitbox(transform.position + transform.forward * 3.0f, new Vector3(1, 1, 1), HitboxType.PlayerHurtbox, ramingDamage));
        }

        if (!inKnockback)
        {
            //Find local forward vector
            Vector3 forward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
            CheckCollision();
            //If monster hits player, stop special
            if (playerCollision || obsticalCollision)
            {
                inKnockback = true;
                time = 0.7f;
            }
            transform.Translate(new Vector3(forward.x, 0, forward.z) * speed / 3);
        }
        //Do knockback if there was a hit
        else
        {
            //Find local forward vector
            Vector3 forward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
            transform.Translate(new Vector3(-forward.x, 0, -forward.z) * speed / 4);
        }

        if (time >= 1.0f)
        {
            GameObject.Destroy(hitboxes[hitboxes.Count - 1]);
            inKnockback = false;
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
            //Track player
            destination = new Vector3(PlayerPosition().x, transform.position.y, PlayerPosition().z);
            //Find the direction the monster should be looking
            lookRotation = Quaternion.LookRotation(destination - transform.position);
            //Find local forward vector
            Vector3 forward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 3.0f);
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

    private bool KoiUnderwaterDash(ref float time)
    {
        if (time == 0.0f)
        {
            hitboxes.Add(CreateHitbox(transform.position + transform.forward * 3.0f, new Vector3(1, 1, 1), HitboxType.PlayerHurtbox, ramingDamage));
        }

        if (!inKnockback)
        {
            //Find local forward vector
            Vector3 forward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
            CheckCollision();
            //If monster hits player, stop special
            if (playerCollision || obsticalCollision)
            {
                inKnockback = true;
                if (time < 0.5f)
                    time = 1.0f - time;
            }
            transform.Translate(new Vector3(forward.x, 0, forward.z) * speed / 2);
            //Move fish parabolically
            transform.Translate(new Vector3(0, (-32 * time + 16) * Time.deltaTime, 0));
            shadow.transform.Translate(new Vector3(0, (32 * time - 16) * Time.deltaTime, 0), Space.World);
            heightMult += (32 * time - 16) * Time.deltaTime;
        }
        //Do knockback if there was a hit
        else
        {
            //Find local forward vector
            Vector3 forward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
            transform.Translate(new Vector3(-forward.x, 0, -forward.z) * speed / 4);

            //Stop moving fish if it goes below its original height
            if (transform.position.y <= initalPos)
                time = 1.0f;

             transform.Translate(new Vector3(0, (-32 * time + 16) * Time.deltaTime, 0));
             shadow.transform.Translate(new Vector3(0, (32 * time - 16) * Time.deltaTime, 0), Space.World);
             heightMult += (32 * time - 16) * Time.deltaTime;
        }

        if (time >= 1.0f)
        {
            GameObject.Destroy(hitboxes[hitboxes.Count - 1]);
            inKnockback = false;
            return false;
        }
        else
            return true;
    }

    private bool KoiBubbleBlastUnderwaterCharge(ref float time)
    {
        transform.Translate(Vector3.up * Time.deltaTime * 3);
        shadow.transform.Translate(Vector3.down * Time.deltaTime * 3, Space.World);
        heightMult += Vector3.down.y * Time.deltaTime * 3;

        if (time >= 1.0f)
            return false;
        else
            return true;
    }

    private bool KoiUnderwaterFollow(ref float time)
    {
        if (time < 4.0f)
        {
            //Track player
            destination = new Vector3(PlayerPosition().x, transform.position.y, PlayerPosition().z);
            //Find the direction the monster should be looking
            lookRotation = Quaternion.LookRotation(destination - transform.position);
            //Find local forward vector
            Vector3 forward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
            if (!CheckCollision())
            {
                lookRotation = Quaternion.LookRotation(Vector3.Cross(Vector3.up, transform.forward));
            }

            //Rotate and move monster
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 1.6f);
            transform.Translate(new Vector3(forward.x, 0, forward.z) * speed / 10);

            //If fish is right under player, change to attack
            if (playerDistance <= 3.1f)
                time = 4.0f;
        }

        if (time >= 4.5f)
            return false;
        else
            return true;
    }

    /// <summary>
    /// WORK IN PROGRESS, FINSIH KNOCKBACK CODING
    /// DOUBLE CHECK EVERYTHING
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    private bool KoiUnderwaterAttack(ref float time)
    {
        if (time == 0.0f)
        {
            hitboxes.Add(CreateHitbox(new Vector3(0, 0.26f, -0.5f), new Vector3(1, 2.3f, 6), HitboxType.PlayerHitbox, ramingDamage));
        }

        if (time <= 0.9f)
        {
            if (!inKnockback)
            {
                //Find local forward vector
                Vector3 forward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
                CheckCollision();
                //If monster hits player, stop special
                if (playerCollision || obsticalCollision)
                {
                    inKnockback = true;
                    if (time < 0.5f)
                        time = 1.0f - time;
                }
                transform.Translate(new Vector3(forward.x, 0, forward.z) * speed / 2);
                //Move Koi up and down parabolically
                transform.Translate(new Vector3(0, (-64 * time + 32) * Time.deltaTime, 0));
                shadow.transform.Translate(new Vector3(0, (64 * time - 32) * Time.deltaTime, 0), Space.World);
                heightMult += (64 * specialTimer[(int)Action.Extra] - 32) * Time.deltaTime;
            }
            //Do knockback if there was a hit
            else
            {
                //Stop moving fish if it goes below its original height
                if (transform.position.y <= initalPos)
                    time = 3.9f;

                transform.Translate(new Vector3(0, (-64 * time + 32) * Time.deltaTime, 0));
                shadow.transform.Translate(new Vector3(0, (64 * time - 32) * Time.deltaTime, 0), Space.World);
                heightMult += (64 * time - 32) * Time.deltaTime;
            }
        }

        if (time >= 3.9f)
        {
            GameObject.Destroy(hitboxes[hitboxes.Count - 1]);
            return false;
        }
        else
            return true;
    }
}
