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
    private float maxShipSpeed = 3f;
    private float maxRotationalVelocity = 1.0f;
    private float minRotationalVelocity = 0.20f;
    private float rotationalVeloctiy = 0.20f;
    private float rotationalAcceleration = 0.6f;
    private float maxLinearVelocity = 2;
	private float linearAccelerationScale = 1;

    public float linearVelocity;
    public ShipUpgrades shipUpgradeScript;
    private bool rotatePositive = true;

    private Hitbox playerHurtbox;

    protected override void Start()
    {
        camera = Camera.main.GetComponent<CameraController>();
        playerHurtbox = transform.GetComponentInChildren<Hitbox>();
        playerHurtbox.OnStay += OnObsticalCollision;
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
        if (rotation != lookRotation && speedScale > 0.05f)
        {
            //Increase rotational 
            if(Quaternion.Angle(rotation, lookRotation) < 45.0f)
            {
                rotationalVeloctiy += rotationalVeloctiy * -0.80f * Time.deltaTime;
                if (rotationalVeloctiy < minRotationalVelocity)
                    rotationalVeloctiy = minRotationalVelocity;
            }
            else
            {
                rotationalVeloctiy += rotationalAcceleration * Time.deltaTime;
                if (rotationalVeloctiy > maxRotationalVelocity)
                    rotationalVeloctiy = maxRotationalVelocity;
            }

            rotation = Quaternion.RotateTowards(rotation, lookRotation, rotationalVeloctiy);
        }
        //Reset velocity when not rotating
        else
            rotationalVeloctiy = minRotationalVelocity;

        //Check if rotating direction has changed, if so reset rotation velocity
        if((Vector3.Cross(transform.forward, moveDirection).y < 0 && rotatePositive) || (Vector3.Cross(transform.forward, moveDirection).y > 0 && !rotatePositive))
        {
            rotationalVeloctiy = minRotationalVelocity;
            rotatePositive = !rotatePositive;
        }

        //Calculate force moving towards desired location
        Vector3 netForce = Vector3.zero;
        if (speedScale > 0.05f)
        {
            netForce = GetConstantMoveForce(moveDirection, maxShipSpeed * speedScale, 1.0f);
            //Add force moving forwards
            netForce += new Vector3(transform.forward.x, 0, transform.forward.z) * maxShipSpeed * 1.5f * speedScale;
        }
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

    public void OnObsticalCollision(GameObject obstical)
    { 
        if(obstical.tag == "Obstical")
        {
            StopMotion();
            Vector3 backForce = transform.position - obstical.transform.position;
            backForce.Normalize();
            backForce *= 20.0f;
            ApplyForce(backForce);
        }
    }
}
