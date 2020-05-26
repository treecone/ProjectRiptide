using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMovement : Physics
{
    //Constants
    private const float MAX_SHIP_SPEED = 3f;
    private const float MAX_ROTATIONAL_VELOCITY = 1.0f;
    private const float MIN_ROTATIONAL_VELOCITY = 0.20f;

    public CameraController cameraControl;
	private Vector3 Target;
	public Vector3 TargetDirection { get; set;}
    public float speedScale = 1.0f;
    private float rotationalVelocity = 0.20f;
    private float rotationalAcceleration = 0.6f;

    public Upgrades shipUpgradeScript;
    private bool rotatePositive = true;

    private Hitbox playerHurtbox;

    protected override void Start()
    {
        cameraControl = Camera.main.GetComponent<CameraController>();
        playerHurtbox = transform.GetComponentInChildren<Hitbox>();
        //Add collision to hurt box
        playerHurtbox.OnStay += OnObsticalCollision;
        base.Start();
    }

    // Update is called once per frame
    override protected void Update()
    {
		//find the vector pointing from our position to the target
		Vector3 moveDirection = TargetDirection.normalized;

        //create the rotation we need to be in to look at the target
        Quaternion lookRotation = rotation;
        if(moveDirection.sqrMagnitude != 0)
		    lookRotation = Quaternion.LookRotation(moveDirection);

        //Rotate based on target location
        if (rotation != lookRotation && speedScale > 0.05f)
        {
            //If rotation is close to desired location, slow down rotation
            if(Quaternion.Angle(rotation, lookRotation) < 45.0f)
            {
                rotationalVelocity += rotationalVelocity * -0.80f * Time.deltaTime;
                //Make sure rotation stay's above minium value
                if (rotationalVelocity < MIN_ROTATIONAL_VELOCITY)
                    rotationalVelocity = MIN_ROTATIONAL_VELOCITY;
            }
            //Else speed up rotation
            else
            {
                rotationalVelocity += rotationalAcceleration * Time.deltaTime;
                //Make sure rotation stay's below maximum value
                if (rotationalVelocity > MAX_ROTATIONAL_VELOCITY)
                    rotationalVelocity = MAX_ROTATIONAL_VELOCITY;
            }

            //Update rotation
            rotation = Quaternion.RotateTowards(rotation, lookRotation, rotationalVelocity);
        }
        //Reset velocity when not rotating
        else
            rotationalVelocity = MIN_ROTATIONAL_VELOCITY;

        //Check if rotating direction has changed, if so reset rotation velocity
        if((Vector3.Cross(transform.forward, moveDirection).y < 0 && rotatePositive) || (Vector3.Cross(transform.forward, moveDirection).y > 0 && !rotatePositive))
        {
            rotationalVelocity = MIN_ROTATIONAL_VELOCITY;
            rotatePositive = !rotatePositive;
        }

        //Calculate force moving towards desired location
        Vector3 netForce = Vector3.zero;
        //Make sure speed is large enough to keep moving
        if (speedScale > 0.05f)
        {
            //Add force moving towards desired location based on ship speed and speed scale
            netForce = GetConstantMoveForce(moveDirection, MAX_SHIP_SPEED * speedScale, 1.0f);
            //Add force moving forwards
            netForce += new Vector3(transform.forward.x, 0, transform.forward.z) * MAX_SHIP_SPEED * 1.5f * speedScale * (1.0f + shipUpgradeScript.masterUpgrade["shipSpeed"]);
        }
        //Draw debug lines for net force and move direction
        Debug.DrawLine(position, position + netForce, Color.blue);
        Debug.DrawLine(position, position + moveDirection * 10.0f, Color.black);

        //Apply net force
        ApplyForce(netForce);
        //Apply Friction
        ApplyFriction(0.75f);
        //Apply force against the side of the ship, reduces drift
        ApplyCounterSideForce(98.0f);
        base.Update();
        //Update camera
        cameraControl.UpdateCamera();
        Debug.DrawLine(position, position + velocity, Color.green);
    }

    /// <summary>
    /// Returns the position of the player
    /// </summary>
    /// <returns>Position of player</returns>
    public Vector3 GetPosition()
    {
        return transform.position;
    }

    /// <summary>
    /// Creates a force that moves in a direction at a given speed
    /// Should be applied each frame to move at desired speed
    /// </summary>
    /// <param name="dir">Direction of movement</param>
    /// <param name="dist">Distance moved in given time frame</param>
    /// <param name="time">Time frame for moving distance</param>
    /// <returns>Force to be applied each frame</returns>
    private Vector3 GetConstantMoveForce(Vector3 dir, float dist, float time)
    {
        float moveForce = (2 * mass * dist) / (time * time);
        Vector3 netForce = dir * moveForce;
        return netForce;
    }

    /// <summary>
    /// Creates a force that counters ship's left and right momentum
    /// </summary>
    /// <param name="strength">Strength of reduction (0-1)</param>
    private void ApplyCounterSideForce(float strength)
    {
        //Calculate force that counters local velocity on the x axis
        Vector3 localVel = transform.worldToLocalMatrix * velocity;
        Vector3 counterVec = new Vector3(-localVel.x, 0, 0);
        //Draw debug line of counter force
        Debug.DrawLine(position, position + (Vector3)(transform.localToWorldMatrix * counterVec * strength), Color.red);
        ApplyForce(transform.localToWorldMatrix * counterVec * strength * mass);
    }

    /// <summary>
    /// Called while colliding with an obstical
    /// Pushes player out from the obstacle to stop collision
    /// </summary>
    /// <param name="obstical">GameObject player is colliding with</param>
    public void OnObsticalCollision(GameObject obstical)
    { 
        //Make sure collision is with an obstical
        if(obstical.tag == "Obstical")
        {
            //Stop motion
            StopMotion();

            //Create a force away from obstacle
            Vector3 backForce = transform.position - obstical.transform.position;
            backForce = new Vector3(backForce.x, 0, backForce.z);
            backForce.Normalize();
            backForce *= 20.0f;
            ApplyForce(backForce);
        }
    }

    /// <summary>
    /// Player takes knockback from an outside source
    /// </summary>
    /// <param name="knockback">Knockback Force</param>
    public void TakeKnockback(Vector3 knockback)
    {
        ApplyForce(knockback / (1.0f + shipUpgradeScript.masterUpgrade["hardiness"]));
    }
}
