using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMovementScript : MonoBehaviour
{
    private Rigidbody rb;
	public Vector3 TargetDirection { get; set;}
    public float maxRotationalVelocity;
    public float maxLinearVelocity;
	public float linearAccelerationScale = 0.95f;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

		Vector3 Target;
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
