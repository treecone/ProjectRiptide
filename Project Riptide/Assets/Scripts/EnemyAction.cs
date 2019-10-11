using System.Collections;
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
        if (actionQueue.Peek()(currTime))
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
    private bool DashCharge(float time)
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
    private bool DashAttack(float time)
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
}
