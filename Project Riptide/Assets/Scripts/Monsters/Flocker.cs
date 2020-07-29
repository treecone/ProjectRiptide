using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flocker : Physics
{
    private const float SEPERATION_DISTANCE = 3.0f;
    private const float COHESION_DISTANCE = 5.0f;
    private Animator _animator;

    public Animator FlockerAnimator => _animator;

    // Start is called before the first frame update
    protected override void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        foreach (Hitbox hitbox in GetComponentsInChildren<Hitbox>())
        {
            hitbox.OnStay += OnObsticalCollision;
        }
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if(_velocity.x == float.NaN)
        {
            _velocity = Vector3.zero;
        }
        if (_velocity.x != 0 && _velocity.z != 0)
        {
            Quaternion desiredRotation = Quaternion.LookRotation(new Vector3(_velocity.x, 0, _velocity.z));
            _rotation = Quaternion.RotateTowards(_rotation, desiredRotation, 2.0f * 60 * Time.deltaTime);
        }
        base.Update();
    }

    /// <summary>
    /// Align to flock's desired direction
    /// </summary>
    /// <param name="flockDirection">Flock direction</param>
    public void Alignment(Vector3 flockDirection)
    {
        ApplyForce(Steer(new Vector3(flockDirection.x, 0, flockDirection.z)) * 5.0f);
    }

    /// <summary>
    /// Cohere to the flock center
    /// </summary>
    /// <param name="flockCenter">Flock Center</param>
    public void Cohesion(Vector3 flockCenter)
    {
        float distance = Vector3.SqrMagnitude(flockCenter - _position);
        ApplyForce(Seek(new Vector3(flockCenter.x, transform.position.y, flockCenter.z)) * (distance / (COHESION_DISTANCE * COHESION_DISTANCE)));
    }

    /// <summary>
    /// Seperate from closest neighbor
    /// </summary>
    /// <param name="closest"></param>
    public void Seperation(Vector3 closest)
    {
        //If closest neighbor is close enough, seperate
        float sqrDistance = Vector3.SqrMagnitude(closest - transform.position);
        if (sqrDistance < SEPERATION_DISTANCE * SEPERATION_DISTANCE)
        {
            Vector3 avoidDirection = (transform.position - closest) + transform.position;
            ApplyForce(Seek(new Vector3(avoidDirection.x, transform.position.y, avoidDirection.z)) * (SEPERATION_DISTANCE * SEPERATION_DISTANCE / sqrDistance));
        }
    }

    /// <summary>
    /// Called when inside an obstacle
    /// Move enemy after the obstacle
    /// </summary>
    /// <param name="obstical">GameObject colliding with</param>
    public void OnObsticalCollision(GameObject obstical)
    {
        if (obstical.tag == "Obstical")
        {
            StopHorizontalMotion();
            Vector3 backForce = transform.position - obstical.transform.position;
            backForce = new Vector3(backForce.x, 0, backForce.z);
            backForce.Normalize();
            backForce *= 200.0f;
            ApplyForce(backForce);
        }
        else if (obstical.tag == "Hitbox" && obstical.transform.parent != null && obstical.transform.parent.tag == "Enemy")
        {
            GameObject attached = obstical.GetComponent<Hitbox>().AttachedObject;
            if (attached != transform.parent.gameObject)
            {
                Vector3 backForce = transform.position - obstical.transform.position;
                backForce = new Vector3(backForce.x, 0, backForce.z);
                backForce.Normalize();
                backForce *= 5.0f * 5.0f;
                ApplyForce(backForce);
            }
        }
        else if (obstical.tag == "Hitbox" && obstical.transform.parent != null && obstical.transform.parent.tag == "Player")
        {
            Vector3 backForce = transform.position - obstical.transform.position;
            backForce = new Vector3(backForce.x, 0, backForce.z);
            backForce.Normalize();
            backForce *= 5.0f * 5.0f;
            ApplyForce(backForce);
        }
    }
}
