using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipMovement : Physics
{
    //Constants
    private const float MAX_SHIP_SPEED = 3f;
    private const float MAX_ROTATIONAL_VELOCITY = 1f;
    private const float MIN_ROTATIONAL_VELOCITY = 0.2f;
    private const float ROTATIONAL_ACCELERATION = 0.6f;
    private GameObject _canvas;
    private Camera _camera;
    private GameObject _shotIndicator;
    private CameraController _cameraControl;
	private Vector3 _target;
	public Vector3 TargetDirection { get; set;}
    private float _speedScale = 1.0f;
    private float _rotationalVelocity = 0.20f;

    [SerializeField]
    private Upgrades shipUpgradeScript;
    private bool rotatePositive = true;

    private Hitbox playerHurtbox;

    public float SpeedScale
    {
        get
        {
            return _speedScale;
        }
        set
        {
            if (value < 0)
            {
                _speedScale = 0;
            }
            else if (value > 1)
            {
                _speedScale = 1;
            }
            else
            {
                _speedScale = value;
            }
        }
    }

    public bool IndicatorActive
    {
        get { return _shotIndicator.activeSelf; }
        set { _shotIndicator.SetActive(value); }
    }

    protected override void Start()
    {
        _camera = Camera.main;
        _cameraControl = _camera.GetComponent<CameraController>();
        playerHurtbox = transform.GetComponentInChildren<Hitbox>();
        _canvas = transform.Find("Canvas").gameObject;
        _shotIndicator = _canvas.transform.Find("ShotIndicator").gameObject;
        //Add collision to hurt box
        playerHurtbox.OnStay += OnObsticalCollision;
        base.Start();
    }

    // Update is called once per frame
    override protected void Update()
    {
        //Rotate Canvas towards camera
        _canvas.transform.rotation = new Quaternion(_camera.transform.rotation.x, _camera.transform.rotation.y, _camera.transform.rotation.z, _camera.transform.rotation.w);

        //find the vector pointing from our position to the target
        Vector3 moveDirection = TargetDirection.normalized;

        //create the rotation we need to be in to look at the target
        Quaternion lookRotation = _rotation;
        if(moveDirection.sqrMagnitude != 0)
		    lookRotation = Quaternion.LookRotation(moveDirection);

        //Rotate based on target location
        if (_rotation != lookRotation && _speedScale > 0.05f)
        {
            //If rotation is close to desired location, slow down rotation
            if(Quaternion.Angle(_rotation, lookRotation) < 45.0f)
            {
                _rotationalVelocity += _rotationalVelocity * -0.80f * Time.deltaTime;
                //Make sure rotation stay's above minium value
                if (_rotationalVelocity < MIN_ROTATIONAL_VELOCITY)
                    _rotationalVelocity = MIN_ROTATIONAL_VELOCITY;
            }
            //Else speed up rotation
            else
            {
                _rotationalVelocity += ROTATIONAL_ACCELERATION * Time.deltaTime;
                //Make sure rotation stay's below maximum value
                if (_rotationalVelocity > MAX_ROTATIONAL_VELOCITY)
                    _rotationalVelocity = MAX_ROTATIONAL_VELOCITY;
            }

            //Update rotation
            _rotation = Quaternion.RotateTowards(_rotation, lookRotation, _rotationalVelocity * 60 * Time.deltaTime);
        }
        //Reset velocity when not rotating
        else
            _rotationalVelocity = MIN_ROTATIONAL_VELOCITY;

        //Check if rotating direction has changed, if so reset rotation velocity
        if((Vector3.Cross(transform.forward, moveDirection).y < 0 && rotatePositive) || (Vector3.Cross(transform.forward, moveDirection).y > 0 && !rotatePositive))
        {
            _rotationalVelocity = MIN_ROTATIONAL_VELOCITY;
            rotatePositive = !rotatePositive;
        }

        //Calculate force moving towards desired location
        Vector3 netForce = Vector3.zero;
        //Make sure speed is large enough to keep moving
        if (_speedScale > 0.05f)
        {
            //Add force moving towards desired location based on ship speed and speed scale
            netForce = GetConstantMoveForce(moveDirection, MAX_SHIP_SPEED * _speedScale, 1.0f);
            //Add force moving forwards
            netForce += new Vector3(transform.forward.x, 0, transform.forward.z) * MAX_SHIP_SPEED * 1.5f * _speedScale * (1.0f + shipUpgradeScript.masterUpgrade[StatusType.ShipSpeed]);
        }
        //Draw debug lines for net force and move direction
        Debug.DrawLine(_position, _position + netForce, Color.blue);
        Debug.DrawLine(_position, _position + moveDirection * 10.0f, Color.black);

        //Apply net force
        ApplyForce(netForce);
        ApplyFriction(0.75f);
        //Apply force against the side of the ship, reduces drift
        ApplyCounterSideForce(0.98f);
        base.Update();
        //Update camera
        _cameraControl.UpdateCamera();
        Debug.DrawLine(_position, _position + _velocity, Color.green);
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
    /// Returns the velocity of the player
    /// </summary>
    /// <returns>Velocity of player</returns>
    public Vector3 GetVelocity()
    {
        return _velocity;
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
        float moveForce = (2 * _mass * dist) / (time * time);
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
        Vector3 localVel = transform.worldToLocalMatrix * _velocity;
        Vector3 counterVec = new Vector3(-localVel.x, 0, 0);
        //Draw debug line of counter force
        Debug.DrawLine(_position, _position + (Vector3)(transform.localToWorldMatrix * counterVec * strength), Color.red);
        ApplyForce(transform.localToWorldMatrix * counterVec * strength * _mass);
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
            backForce *= 40.0f * (1f / 60f / Time.deltaTime);
            Debug.Log(backForce);
            Debug.Log((1f / 60f / Time.deltaTime));
            ApplyForce(backForce);
        }
        if(obstical.tag == "Hitbox" && obstical.transform.parent.tag == "Enemy" && obstical.GetComponent<Hitbox>().Type != HitboxType.EnemyHitbox)
        {
            //Create a force away from enemy
            Vector3 backForce = transform.position - obstical.transform.position;
            backForce = new Vector3(backForce.x, 0, backForce.z);
            backForce.Normalize();
            backForce *= 20.0f * (1f / 60f / Time.deltaTime);
            ApplyForce(backForce);
        }
    }

    /// <summary>
    /// Player takes knockback from an outside source
    /// </summary>
    /// <param name="knockback">Knockback Force</param>
    public void TakeKnockback(Vector3 knockback)
    {
        ApplyForce(knockback / (1.0f + shipUpgradeScript.masterUpgrade[StatusType.Hardiness]));
    }
}
