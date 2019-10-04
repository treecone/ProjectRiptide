using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Enemy : MonoBehaviour
{
    /// <summary>
    /// Charges dash attack for 2 seconds
    /// </summary>
    /// <param name="time">Current time</param>
    /// <returns></returns>
    private bool DashCharge(float time)
    {
        //Track player
        destination = new Vector3(GameObject.FindGameObjectWithTag("Player").transform.position.x, transform.position.y, GameObject.FindGameObjectWithTag("Player").transform.position.z);
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
        if (!inKnockback)
        {
            //Find local forward vector
            Vector3 forward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
            CheckCollision();
            //If monster hits player, stop special
            if (playerCollision || obsticalCollision)
                inKnockback = true;
            transform.Translate(new Vector3(forward.x, 0, forward.z) * speed / 2);
        }
        else
        {
            //Find local forward vector
            Vector3 forward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
            specialTimer[0] += Time.deltaTime;
            transform.Translate(new Vector3(-forward.x, 0, -forward.z) * speed / 6);
        }

        if (time >= 1.0f)
        {
            inKnockback = false;
            return false;
        }
        else
            return true;
    }
}
