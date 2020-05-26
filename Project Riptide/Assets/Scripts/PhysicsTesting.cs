using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsTesting : Physics
{
    Vector3 gravity = Vector3.down * 10.0f;
    float time = 0.0f;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (transform.position.y < 0)
            StopMotion();

        if (Input.GetKeyDown(KeyCode.P))
        {
            gravity = ApplyArcForce(transform.forward, 10.0f, 20.0f, 2.0f);
        }

        if (transform.position.y > 0)
        {
            ApplyForce(gravity);
            time += Time.deltaTime;
            Debug.Log(time);
            Debug.Log(transform.position);
        }

        base.Update();
    }

    protected void ApplyArcForce(Vector3 dir, float dist, float time, Vector3 gravity)
    {
        float xForce = _mass * (dist / (time * Time.deltaTime));
        //float yForce = ((2 * mass * yMax) / (time * Time.deltaTime)) - ((gravity * time) / (4 * Time.deltaTime));
        float yForce = (-gravity.y * time) / (2 * Time.deltaTime);
        Vector3 netForce = dir * xForce;
        netForce += yForce * Vector3.up;
        ApplyForce(netForce);
    }

    protected Vector3 ApplyArcForce(Vector3 dir, float dist, float yMax, float time)
    {
        float xForce = _mass * (dist / (time * Time.deltaTime));
        float gravity = (-8 * _mass * yMax) / (time * time);
        float yForce = (-gravity * time) / (2 * Time.deltaTime);
        Vector3 netForce = dir * xForce;
        netForce += yForce * Vector3.up;
        ApplyForce(netForce);
        return Vector3.up * gravity;
    }
}
