using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonFireScript : MonoBehaviour
{
    public float cannonBallSizeScale;
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
    public void Fire(Vector3 direction)
    {
        CannonShot shot = new CannonShot(direction, shipUpgradeScript.masterUpgrade);
        shot.Fire(cannonBall, gameObject, cannonBallSizeScale);
    }

	public class CannonShot
    {
        private int damage;
        private Vector3 direction;
        private float fireSpeed;
        private float verticalRatio;
        private int count;
        private float spreadAngle;
        
        //this doesn't really get used but it'll stay in just in case for now
        public CannonShot(int count, int damage, Vector3 direction, float fireSpeed, float verticalRatio, float spreadAngle)
        {
            this.count = count;
            this.damage = damage;
            this.direction = direction;
            this.fireSpeed = fireSpeed;
            this.verticalRatio = verticalRatio;
            this.spreadAngle = spreadAngle;
        }

        public CannonShot(Vector3 direction, Upgrade upgrade)
        {
            this.count = 1 + (int)upgrade["count"];
            this.damage = 1 + (int)upgrade["damage"];
            this.direction = direction;
            this.fireSpeed = 40 + upgrade["fireSpeed"];
            this.verticalRatio = 0.1f + upgrade["verticalRatio"];
            this.spreadAngle = 20;
        }

        public void Fire(GameObject cannonBall, GameObject ship, float cannonBallSizeScale)
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
            //currently only shoots right, gotta fix to actually account for direction
            Quaternion angle = Quaternion.Euler(0, (-trueSpreadAngle * (count - 1) / 2) + ship.transform.rotation.eulerAngles.y + 90, 0); //Quaternion.AngleAxis(-spreadAngle * (count - 1) / 2, Vector3.up) * direction;
            for (int i = 0; i < count; i++)
            {
                GameObject ball = Instantiate(cannonBall, ship.transform.position + (ship.transform.localScale.x / 2) * direction, Quaternion.identity);
                ball.transform.localScale = new Vector3(cannonBallSizeScale * damage, cannonBallSizeScale * damage, cannonBallSizeScale * damage);
                ball.SetActive(true);

                //ball.GetComponent<Rigidbody>().velocity = angle * fireSpeed;
                

                ball.transform.rotation = angle;

                
				ball.GetComponent<Rigidbody>().velocity = (Vector3.up * fireSpeed * verticalRatio) + (ball.transform.forward * fireSpeed) + (/*ship.GetComponent<ShipMovementScript>().linearVelocity*/2 * 5 * ship.transform.forward);
                ball.GetComponent<CannonBallBehaviorScript>().damageDealt = damage;

                angle *= Quaternion.Euler(0, trueSpreadAngle, 0);
            }
            
        }
    }
}
