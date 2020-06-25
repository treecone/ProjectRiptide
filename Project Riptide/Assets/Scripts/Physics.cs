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

    public Vector3 Position
    {
        get { return _position; }
        set { _position = value; transform.position = value; }
    }

    public Vector3 Velocity
    {
        get { return _velocity; }
        set { _velocity = value;}
    }

    public Quaternion Rotation
    {
        get { return _rotation; }
        set { _rotation = value; transform.rotation = _rotation; }
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        _position = transform.position;
        _velocity = Vector3.zero;
        _acceleration = Vector3.zero;
        _rotation = transform.rotation;
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

    /// <summary>
    /// Applies a force to the body
    /// </summary>
    /// <param name="force">Force to apply</param>
    public void ApplyForce(Vector3 force)
    {
        _acceleration += force / _mass;
    }

    /// <summary>
    /// Applies friction to the body
    /// </summary>
    /// <param name="coeff">Coefficent of friction</param>
    public void ApplyFriction(float coeff)
    {
        ApplyForce(_velocity * -coeff * _mass);
    }

    /// <summary>
    /// Steers the body towards a desired velocity
    /// </summary>
    /// <param name="desiredVelocity">Desired Velocity</param>
    /// <returns>Force to steer the body</returns>
    public Vector3 Steer(Vector3 desiredVelocity)
    {
        //Set up desired velocity
        desiredVelocity = desiredVelocity.normalized;
        desiredVelocity *= _maxSpeed;

        //Calc steering force
        Vector3 steeringForce = desiredVelocity - _velocity;

        //Return force
        return steeringForce;
    }

    /// <summary>
    /// Seeks a target destination
    /// </summary>
    /// <param name="target">Target to seek</param>
    /// <returns>Force to seek the target</returns>
    public Vector3 Seek(Vector3 target)
    {
        Vector3 desiredVelocity = target - transform.position;
        return Steer(desiredVelocity);
    }

    /// <summary>
    /// Stops motion of the body
    /// </summary>
    public void StopMotion()
    {
        _velocity = Vector3.zero;
        _acceleration = Vector3.zero;
    }

    /// <summary>
    /// Stops motion of the x and z axis of the body
    /// </summary>
    public void StopHorizontalMotion()
    {
        _velocity = new Vector3(0, _velocity.y, 0);
        _acceleration = new Vector3(0, _acceleration.y, 0);
    }

    /// <summary>
    /// Stops motion on the y axis of the body
    /// </summary>
    public void StopVerticalMotion()
    {
        _velocity = new Vector3(_velocity.x, 0, _velocity.z);
        _acceleration = new Vector3(_acceleration.x, 0, _acceleration.z);
    }

    /// <summary>
    /// Applys a force to move the enemy in an arc
    /// </summary>
    /// <param name="dir">Direction of movement</param>
    /// <param name="dist">Horizontal distance covered</param>
    /// <param name="time">Time that the arc takes place</param>
    /// <param name="gravity">Gravity being applied each frame</param>
    public void ApplyArcForce(Vector3 dir, float dist, float time, Vector3 gravity)
    {
        float deltaTime = 0;
        if (Time.deltaTime == 0)
        {
            deltaTime = 1f / 60f;
        }
        else
        {
            deltaTime = Time.deltaTime;
        }

        float xForce = _mass * (dist / (time * deltaTime));
        float yForce = (-gravity.y * time) / (2 * deltaTime);
        Vector3 netForce = dir * xForce;
        netForce += yForce * Vector3.up;
        ApplyForce(netForce);
    }

    /// <summary>
    /// Applys a force to move the enemy in an arc
    /// </summary>
    /// <param name="dir">Direction of movement</param>
    /// <param name="dist">Horizontal distance covered</param>
    /// <param name="yMax">Maximum vertical distance</param>
    /// <param name="time">Time that the arc takes place</param>
    /// <returns>Gravity to be applied each frame</returns>
    public Vector3 ApplyArcForce(Vector3 dir, float dist, float yMax, float time)
    {
        float deltaTime = 0;
        if (Time.deltaTime == 0)
        {
            deltaTime = 1f / 60f;
        }
        else
        {
            deltaTime = Time.deltaTime;
        }

        float xForce = _mass * (dist / (time * deltaTime));
        float gravity = (-8 * _mass * yMax) / (time * time);
        float yForce = (-gravity * time) / (2 * deltaTime);
        Vector3 netForce = dir * xForce;
        netForce += yForce * Vector3.up;
        ApplyForce(netForce);
        return Vector3.up * gravity;
    }

    /// <summary>
    /// Applies a force to move in a direction at a specified speed
    /// Applied only once
    /// </summary>
    /// <param name="dir">Direction of movment</param>
    /// <param name="dist">Distance moved over time frame</param>
    /// <param name="time">Time frame to move dstance</param>
    public void ApplyMoveForce(Vector3 dir, float dist, float time)
    {
        float deltaTime = 0;
        if (Time.deltaTime == 0)
        {
            deltaTime = 1f / 60f;
        }
        else
        {
            deltaTime = Time.deltaTime;
        }

        float moveForce = _mass * (dist / (time * Time.deltaTime));
        Vector3 netForce = dir * moveForce;
        ApplyForce(netForce);
    }

    /// <summary>
    /// Applies a force to move in a direction at a specified speed
    /// Needs to be applied each frame
    /// </summary>
    /// <param name="dir">Direction of movement</param>
    /// <param name="dist">Distance moved over time frame</param>
    /// <param name="time">Time frame to move distance</param>
    public void ApplyConstantMoveForce(Vector3 dir, float dist, float time)
    {
        float moveForce = (2 * _mass * dist) / (time * time);
        Vector3 netForce = dir * moveForce;
        ApplyForce(netForce);
    }
}
