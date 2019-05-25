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
        if (playerDistance > wanderRadius)
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
                transform.Translate(new Vector3(-forward.x, 0, -forward.z) * speed / 4);
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
