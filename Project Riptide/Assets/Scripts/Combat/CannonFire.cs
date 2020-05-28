using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonFire : MonoBehaviour
{
    public GameObject cannonBall;
    public Upgrades shipUpgradeScript;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            //Debug.Log("Fire");
            Fire(-transform.right);
        }
    }

    public void Fire(string debugShotType)
    {
        switch(debugShotType)
        {
            case "right":
                Fire(transform.right);
                break;
        }
    }

    public void Fire(string debugShotType, Vector3 targetDir)
    {
        switch (debugShotType)
        {
            case "right":
                Fire(transform.right, targetDir);
                break;
        }
    }

    public void Fire(Vector3 direction)
    {
        CannonShot rightShot = new CannonShot(direction, 90, shipUpgradeScript.masterUpgrade);
        CannonShot leftShot = new CannonShot(direction, -90, shipUpgradeScript.masterUpgrade);
        leftShot.Fire(cannonBall, gameObject);
        rightShot.Fire(cannonBall, gameObject);
    }

    public void Fire(Vector3 direction, Vector3 targetDir)
    {
        float angle = Mathf.Acos(Vector3.Dot(targetDir.normalized, transform.forward.normalized));
        Vector3 cross = Vector3.Cross(targetDir, transform.forward);
        if (Vector3.Dot(Vector3.up, cross) < 0)
        { // Or > 0
            angle = -angle;
        }
        angle *= Mathf.Rad2Deg;

        if(angle > 0 && angle < 45)
        {
            angle = 45;
        }
        if(angle > 135)
        {
            angle = 135;
        }
        if(angle < 0 && angle > -45)
        {
            angle = -45;
        }
        if(angle < -135)
        {
            angle = -135;
        }

        CannonShot rightShot = new CannonShot(direction, -angle, shipUpgradeScript.masterUpgrade);
        //CannonShot leftShot = new CannonShot(direction, -90, shipUpgradeScript.masterUpgrade);
        //leftShot.Fire(cannonBall, gameObject, cannonBallSizeScale);
        rightShot.Fire(cannonBall, gameObject);
    }

    public class CannonShot
    {
        private int damage;
        private Vector3 direction;
        private float fireSpeed;
        private float fireAngle;
        private float verticalRatio;
        private int count;
        private float spreadAngle;
        private float size;

        public CannonShot(Vector3 direction, float fireAngle, Upgrade upgrade)
        {
            this.count = 1 + (int)upgrade["count"];
            this.damage = 1 + (int)upgrade["damage"];
            this.direction = direction;
            this.fireSpeed = 40 + upgrade["fireSpeed"];
            this.fireAngle = fireAngle;
            this.verticalRatio = 0.1f + upgrade["verticalRatio"];
            this.spreadAngle = 20;
            this.size = 0.5f + upgrade["shotSize"];
        }

        public void Fire(GameObject cannonBall, GameObject ship)
        {
            float trueSpreadAngle;
            if (count == 1)
            {
                trueSpreadAngle = 0;
            }
            else
            {
                trueSpreadAngle = spreadAngle / (count - 1);
            }

            Quaternion angle = Quaternion.Euler(0, (-trueSpreadAngle * (count - 1) / 2) + ship.transform.rotation.eulerAngles.y + fireAngle, 0); //Quaternion.AngleAxis(-spreadAngle * (count - 1) / 2, Vector3.up) * direction;
            for (int i = 0; i < count; i++)
            {
                GameObject ball = Instantiate(cannonBall, ship.transform.position + (ship.transform.localScale.x / 2) * direction, Quaternion.identity);
                ball.transform.localScale = new Vector3(size, size, size);
                ball.SetActive(true);

                //ball.GetComponent<Rigidbody>().velocity = angle * fireSpeed;
                

                ball.transform.rotation = angle;

                
				ball.GetComponent<Rigidbody>().velocity = (Vector3.up * fireSpeed * (verticalRatio / (verticalRatio + 1.0f))) + (ball.transform.forward * fireSpeed * (1.0f / (verticalRatio + 1.0f))) + (/*ship.GetComponent<ShipMovement>().linearVelocity*/2 * 5 * ship.transform.forward);
                ball.GetComponent<CannonBallBehavior>().damageDealt = damage;

                angle *= Quaternion.Euler(0, trueSpreadAngle, 0);
            }
            
        }
    }
}
