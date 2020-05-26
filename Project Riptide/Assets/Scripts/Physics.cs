using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Physics : MonoBehaviour
{
    [SerializeField]
    protected float _mass;
    [SerializeField]
    protected float _maxSpeed;

    protected Vector3 _position;
    protected Vector3 _velocity;
    protected Vector3 _acceleration;
    protected Quaternion _rotation;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        _position = transform.position;
        _velocity = Vector3.zero;
        _acceleration = Vector3.zero;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        _velocity += _acceleration * Time.deltaTime;
        _position += _velocity * Time.deltaTime;

        transform.position = _position;
        transform.rotation = _rotation;

        _acceleration = Vector3.zero;
    }

    //Applys a force to the body
    protected void ApplyForce(Vector3 force)
    {
        _acceleration += force / _mass;
    }

    //Applys friction to a body, takes coefficient of friction
    protected void ApplyFriction(float coeff)
    {
        ApplyForce(_velocity * -coeff * _mass);
    }

    //Steers the body to a desired velocity
    protected Vector3 Steer(Vector3 desiredVelocity)
    {
        //Set up desired velocity
        desiredVelocity = desiredVelocity.normalized;
        desiredVelocity *= _maxSpeed;

        //Calc steering force
        Vector3 steeringForce = desiredVelocity - _velocity;

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
        _velocity = Vector3.zero;
        _acceleration = Vector3.zero;
    }
}
