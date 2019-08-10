﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonFireScript : MonoBehaviour
{
    public GameObject cannonBall;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("a"))
        {
            Fire("debugOneBig");
        }
        if(Input.GetKeyDown("d"))
        {
            Fire("debugTriShot");
        }
    }

    public void Fire(string shotType)
    {
        switch(shotType)
        {
            case "debugOneBig":
                CannonShot oneBig = new CannonShot(1, 10, transform.right, 10, 60, 0);
                oneBig.Fire(cannonBall, transform);
                break;
            case "debugTriShot":
                CannonShot triShot = new CannonShot(3, 5, transform.right, 10, 30, 0);
                triShot.Fire(cannonBall, transform);
                break;
        }
    }

    private class CannonShot
    {
        public float fireSpeedHoriz = 10;
        public float fireSpeedVert = 4;
        private int damage;
        private Vector3 direction;
        private float fireSpeed;
        private int count;
        private float spreadAngle;
        private float spreadDisplacement;

        public CannonShot(int count, int damage, Vector3 direction, float fireSpeed, float spreadAngle, float spreadDisplacement)
        {
            this.count = count;
            this.damage = damage;
            this.direction = direction;
            this.fireSpeed = fireSpeed;
            this.spreadAngle = spreadAngle;
            this.spreadDisplacement = spreadDisplacement;
        }

        public void Fire(GameObject cannonBall, Transform shipTransform)
        {
            Vector3 angle = Quaternion.AngleAxis(-spreadAngle * (count - 1) / 2, Vector3.up) * direction;
            for(int i = 0; i < count; i++)
            {
                GameObject ball = Instantiate(cannonBall, shipTransform.position + (shipTransform.localScale.x / 2) * direction, Quaternion.identity);

                //Added by Caden to be able to test player attacking fish, feel free to remove later
                ball.GetComponent<Rigidbody>().velocity = (Vector3.up * fireSpeedVert) + (shipTransform.right * fireSpeedHoriz);
                //

                //ball.GetComponent<Rigidbody>().velocity = angle * fireSpeed;
                angle = Quaternion.AngleAxis(spreadAngle, Vector3.up) * angle;

                ball.GetComponent<CannonBallBehaviorScript>().damageDealt = damage;
            }
            
        }
    }
}
