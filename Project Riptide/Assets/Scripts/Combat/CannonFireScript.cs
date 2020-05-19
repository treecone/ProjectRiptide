using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FireType { Right, Left, Both, Big, Tri, Target};

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
		/*if(Input.GetKeyDown(KeyCode.Space))
		{
			Fire(FireType.Both);
		}*/
        if(Input.GetKeyDown(KeyCode.Space))
        {
            //Debug.Log("Fire");
            //Fire(-transform.right);
        }
    }

    public void Fire(Vector3 direction)
    {
        CannonShot shot = new CannonShot(direction, shipUpgradeScript.masterUpgrade);
        shot.Fire(cannonBall, gameObject, cannonBallSizeScale);
    }

    public void Fire(FireType shotType)
    {
        switch(shotType)
        {
            case FireType.Big:
                CannonShot oneBig = new CannonShot(1, 10, transform.right, 40, 0.1f, 60);
                oneBig.Fire(cannonBall, gameObject, cannonBallSizeScale);
                break;
            case FireType.Tri:
                CannonShot triShot = new CannonShot(3, 5, transform.right, 40, 0.1f, 30);
                triShot.Fire(cannonBall, gameObject, cannonBallSizeScale);
                break;
			case FireType.Both:
				CannonShot left = new CannonShot(1, 5, -transform.right, 40, 0.1f, 30);
				CannonShot right = new CannonShot(1, 5, transform.right, 40, 0.1f, 30);
				left.Fire(cannonBall, gameObject, cannonBallSizeScale);
				right.Fire(cannonBall, gameObject, cannonBallSizeScale);
				break;
		}
	}
	public void Fire(FireType shotType, Vector3 target)
	{
		Vector3 direction = target - transform.position;
		float angle = Vector3.Angle(transform.forward, direction);
		if (angle < 15)
			return;
		switch (shotType)
		{
			case FireType.Target:
				CannonShot shot = new CannonShot(1, 5, direction.normalized, 40, 0.1f, 30);
				shot.Fire(cannonBall, gameObject, cannonBallSizeScale);
				break;
		}
	}

	public class CannonShot
    {

        private int damage;
        private Vector3 direction;
        private float fireSpeed;
        private float verticalRatio;
        private int count;
        private float spreadAngle;
        

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
            this.verticalRatio = 0.1f;
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
            Quaternion angle = Quaternion.Euler(0, (-trueSpreadAngle * (count - 1) / 2) + ship.transform.rotation.eulerAngles.y + 90, 0); //Quaternion.AngleAxis(-spreadAngle * (count - 1) / 2, Vector3.up) * direction;
            for (int i = 0; i < count; i++)
            {
                GameObject ball = Instantiate(cannonBall, ship.transform.position + (ship.transform.localScale.x / 2) * direction, Quaternion.identity);
                ball.transform.localScale = new Vector3(cannonBallSizeScale * damage, cannonBallSizeScale * damage, cannonBallSizeScale * damage);
                ball.SetActive(true);

                //ball.GetComponent<Rigidbody>().velocity = angle * fireSpeed;
                

                ball.transform.rotation = angle;

                
				ball.GetComponent<Rigidbody>().velocity = (Vector3.up * fireSpeed * verticalRatio) + (ball.transform.forward * fireSpeed) + (ship.GetComponent<ShipMovementScript>().linearVelocity * 5 * ship.transform.forward);
                ball.GetComponent<CannonBallBehaviorScript>().damageDealt = damage;

                angle *= Quaternion.Euler(0, trueSpreadAngle, 0);
            }
            
        }
    }
}
