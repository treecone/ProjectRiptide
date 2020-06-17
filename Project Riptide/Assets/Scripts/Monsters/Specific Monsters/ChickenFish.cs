using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenFish : Physics
{
    private const float SEPERATION_DISTANCE = 3.0f;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    /// <summary>
    /// Align to flock's desired direction
    /// </summary>
    /// <param name="flockDirection">Flock direction</param>
    public void Alignment(Vector3 flockDirection)
    {
        ApplyForce(Steer(new Vector3(flockDirection.x, 0, flockDirection.z)));
    }

    /// <summary>
    /// Cohere to the flock center
    /// </summary>
    /// <param name="flockCenter">Flock Center</param>
    public void Cohesion(Vector3 flockCenter)
    {
        ApplyForce(Seek(new Vector3(flockCenter.x, transform.position.y, flockCenter.z)));
    }

    /// <summary>
    /// Seperate from closest neighbor
    /// </summary>
    /// <param name="closest"></param>
    public void Seperation(Vector3 closest)
    {
        //If closest neighbor is close enough, seperate
        if(Vector3.SqrMagnitude(closest - transform.position) < SEPERATION_DISTANCE * SEPERATION_DISTANCE)
        {
            Vector3 avoidDirection = (transform.position - closest) + transform.position;
            ApplyForce(Seek(new Vector3(avoidDirection.x, transform.position.y, avoidDirection.z)));
        }
    }
}
