using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UrbanCenter : MonoBehaviour
{
    [SerializeField]
    private float _radius;

    public float Radius => _radius;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}
