using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsScript : MonoBehaviour
{
    public float mass;
    public float maxSpeed;

    protected Vector3 position;
    protected Vector3 velocity;
    protected Vector3 acceleration;
    protected Quaternion rotation;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        position = transform.position;
        velocity = Vector3.zero;
        acceleration = Vector3.zero;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        velocity += acceleration * Time.deltaTime;
        position += velocity * Time.deltaTime;

        transform.position = position;
        transform.rotation = rotation;

        acceleration = Vector3.zero;
    }

    //Applys a force to the body
    protected void ApplyForce(Vector3 force)
    {
        acceleration += force / mass;
    }

    //Applys friction to a body, takes coefficient of friction
    protected void ApplyFriction(float coeff)
    {
        ApplyForce(velocity * -coeff * mass);
    }

    //Steers the body to a desired velocity
    protected Vector3 Steer(Vector3 desiredVelocity)
    {
        //Set up desired velocity
        desiredVelocity = desiredVelocity.normalized;
        desiredVelocity *= velocity.magnitude;

        //Calc steering force
        Vector3 steeringForce = desiredVelocity - velocity;

        //Return force
        return steeringForce;
    }

    protected Vector3 Seek(Vector3 target)
    {
        Vector3 desiredVelocity = target - transform.position;
        return Steer(desiredVelocity);
    }

    protected void StopMotion()
    {
        velocity = Vector3.zero;
        acceleration = Vector3.zero;
    }
}
