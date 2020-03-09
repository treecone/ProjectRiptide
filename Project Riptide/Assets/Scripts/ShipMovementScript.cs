using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMovementScript : MonoBehaviour
{
	private Vector3 Target;
	public Vector3 TargetDirection { get; set;}
    private float maxRotationalVelocity = 9;
    private float maxLinearVelocity = 2;
	private float linearAccelerationScale = 1;
    // Update is called once per frame
    void FixedUpdate()
    {
		float distance = TargetDirection.magnitude;
		if (distance > 2f) 
			Target = transform.position + TargetDirection.normalized * 5f;
		else
			Target = transform.position + TargetDirection.normalized * 0.1f;

		//find the vector pointing from our position to the target
		Vector3 moveDirection = (Target - transform.position).normalized;

		//create the rotation we need to be in to look at the target
		Quaternion lookRotation = Quaternion.LookRotation(moveDirection);

		//rotate us over time according to speed until we are in the required rotation
		transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * maxRotationalVelocity);

		if (distance > 2f)
		{
			transform.position = Vector3.Lerp(transform.position, Target, Time.deltaTime * maxLinearVelocity * Mathf.Pow(linearAccelerationScale, -distance));	
		}
	}

    public Vector3 GetPosition()
    {
        return transform.position;
    }
}
