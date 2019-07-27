using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMovementScript : MonoBehaviour
{
    private Rigidbody rb;

    /// <summary>
    /// The linear velocity of the ship
    /// </summary>
    private float linearVelocity;

    /// <summary>
    /// The rotational velocity of the ship
    /// </summary>
    private float rotationalVelocity;


    //-----Configs-----
    public float rotationalAcceleration;
    public float maxRotationalVelocity;
    private float minRotationalVelocity;
    public float rotationalDrag;

    public float linearAcceleration;
    public float maxLinearVelocity;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        minRotationalVelocity = -maxRotationalVelocity;
    }

    // Update is called once per frame
    void Update()
    {
        //Apply current rotational and linear velocities
        transform.Rotate(0, rotationalVelocity, 0, Space.Self);
        rb.velocity = transform.forward * linearVelocity;
    }

    public float RotationalVelocity
    {
        get
        {
            return rotationalVelocity;
        }
        set
        {
            rotationalVelocity = value;
            if (rotationalVelocity > maxRotationalVelocity) rotationalVelocity = maxRotationalVelocity;
            if (rotationalVelocity < minRotationalVelocity) rotationalVelocity = minRotationalVelocity;
        }
    }

    public float LinearVelocity
    {
        get
        {
            return linearVelocity;
        }
        set
        {
            linearVelocity = value;
            if (linearVelocity > maxLinearVelocity) linearVelocity = maxLinearVelocity;
        }
    }
}
