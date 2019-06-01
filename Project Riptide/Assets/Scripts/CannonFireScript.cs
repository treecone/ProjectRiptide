using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonFireScript : MonoBehaviour
{
    public GameObject cannonBall;
    public float fireSpeedHoriz;
    public float fireSpeedVert;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("a"))
        {
            Fire(true);
        }
        if(Input.GetKeyDown("d"))
        {
            Fire(false);
        }
    }

    void Fire(bool side)
    {
        Vector3 direction = side ? -transform.right : transform.right;
        GameObject ball = Instantiate(cannonBall, transform.position + (transform.localScale.x / 2) * direction, Quaternion.identity);
        ball.GetComponent<Rigidbody>().velocity = direction * fireSpeedHoriz + transform.up * fireSpeedVert;
    }
}
