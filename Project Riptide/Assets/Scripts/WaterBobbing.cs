using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBobbing : MonoBehaviour
{
    [SerializeField]
    private float _bobAmount;


    private float _startY;
    private float _bobOffset;
    // Start is called before the first frame update
    void Start()
    {
        _bobOffset = Random.Range(0, Mathf.PI * 2);
        _startY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, _startY + Mathf.Sin(Time.time + _bobOffset) * _bobAmount, transform.position.z);
    }
}
