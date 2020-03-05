using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMovementScript : MonoBehaviour
{
    private Rigidbody rb;
	public Vector3 Target { get; set;}
    public float maxRotationalVelocity;
    public float maxLinearVelocity;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
		//find the vector pointing from our position to the target
		Vector3 _direction = (Target - transform.position).normalized;

		//create the rotation we need to be in to look at the target
		Quaternion _lookRotation = Quaternion.LookRotation(_direction);

		//rotate us over time according to speed until we are in the required rotation
		transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * maxRotationalVelocity);

		if(Vector3.Distance(Target, transform.position) >= 0.1f)
		{
			transform.position = Vector3.Lerp(transform.position, Target, Time.deltaTime * maxLinearVelocity);
		}
	}

    public Vector3 GetPosition()
    {
        return transform.position;
    }
}
