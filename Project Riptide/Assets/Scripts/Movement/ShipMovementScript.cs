using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMovementScript : PhysicsScript
{
    public CameraController camera;
	private Vector3 Target;
	public Vector3 TargetDirection { get; set;}
    public float speedScale = 1.0f;
    private float maxShipSpeed = 5.0f;
    private float maxRotationalVelocity = 1.2f;
    private float rotationalVeloctiy = 0.0f;
    private float rotationalAcceleration = 0.6f;
    private float maxLinearVelocity = 2;
	private float linearAccelerationScale = 1;

    public float linearVelocity;
    public ShipUpgrades shipUpgradeScript;
    private bool rotatePositive = true;

    protected override void Start()
    {
        camera = Camera.main.GetComponent<CameraController>();
        base.Start();
    }

    // Update is called once per frame
    override protected void Update()
    {
		float distance = TargetDirection.magnitude;
		if (distance > 2f) 
			Target = transform.position + TargetDirection.normalized * 5f;
		else
			Target = transform.position + TargetDirection.normalized * 0.1f;

		//find the vector pointing from our position to the target
		Vector3 moveDirection = (Target - transform.position).normalized;

        //create the rotation we need to be in to look at the target
        Quaternion lookRotation = rotation;
        if(moveDirection.sqrMagnitude != 0)
		    lookRotation = Quaternion.LookRotation(moveDirection);

        //Rotate based on target location
        if (rotation != lookRotation)
        {
            //Increase rotational velocity
            rotationalVeloctiy += rotationalAcceleration * Time.deltaTime;
            if (rotationalVeloctiy > maxRotationalVelocity)
                rotationalVeloctiy = maxRotationalVelocity;
            rotation = Quaternion.RotateTowards(rotation, lookRotation, rotationalVeloctiy);
        }
        //Reset velocity when not rotating
        else
            rotationalVeloctiy = 0.25f;

        //Check if rotating direction has changed, if so reset rotation velocity
        if((Vector3.Cross(transform.forward, moveDirection).y < 0 && rotatePositive) || (Vector3.Cross(transform.forward, moveDirection).y > 0 && !rotatePositive))
        {
            rotationalVeloctiy = 0.25f;
            rotatePositive = !rotatePositive;
        }

        //Calculate force moving towards desired location
        Vector3 netForce = GetConstantMoveForce(moveDirection, maxShipSpeed * speedScale, 1.0f);
        //Add force moving forwards
        netForce += new Vector3(transform.forward.x, 0, transform.forward.z) * 3.0f;
        Debug.DrawLine(position, position + netForce, Color.blue);
        Debug.DrawLine(position, position + moveDirection * 10.0f, Color.black);

        //Apply net force
        ApplyForce(netForce);
        //Apply Friction
        ApplyFriction(0.75f);
        //Apply force against the side of the ship, reduces drift
        ApplyCounterSideForce(0.99f);
        base.Update();
        //Update camera
        camera.UpdateCamera();
        Debug.DrawLine(position, position + velocity, Color.green);
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    private Vector3 GetConstantMoveForce(Vector3 dir, float dist, float time)
    {
        float moveForce = (2 * mass * dist) / (time * time);
        Vector3 netForce = dir * moveForce;
        return netForce;
    }

    private void ApplyCounterSideForce(float strength)
    {
        Vector3 localVel = transform.worldToLocalMatrix * velocity;
        Vector3 counterVec = new Vector3(-localVel.x, 0, 0);
        Debug.DrawLine(position, position + (Vector3)(transform.localToWorldMatrix * counterVec * strength), Color.red);
        ApplyForce(transform.localToWorldMatrix * counterVec * strength * mass);
    }
}
