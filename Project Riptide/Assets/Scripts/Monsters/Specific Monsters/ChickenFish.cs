using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenFish : Flocker
{
    private float _minY = -1.3f;

    public float MinY
    {
        set { _minY = value; }
    }

    private Vector3 _gravity;

    // Start is called before the first frame update
    protected override void Start()
    {
        _gravity = ApplyArcForce(Vector3.forward, 0, 3.5f, 1.5f);
        _acceleration = Vector3.zero;
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    /// <summary>
    /// Moves fish up and down as it swims
    /// </summary>
    public void MoveUpAndDown()
    {
        if(transform.position.y < _minY)
        {
            ApplyArcForce(Vector3.up, 0, 3.5f, 1.5f);
        }
        ApplyForce(_gravity);
    }
}
