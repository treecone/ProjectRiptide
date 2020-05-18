using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FireType { Right, Left, Both, Big, Tri, Target};

public class CannonFireScript : MonoBehaviour
{
    public GameObject cannonBall;
    public ShipUpgrades shipUpgradeScript;
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
            Debug.Log("Fire");
            Fire(-transform.right);
        }
    }

    public void Fire(Vector3 direction)
    {
        CannonShot shot = new CannonShot(direction, shipUpgradeScript.UpgradesOfType(Upgrade.UpgradeType.Shot));
        shot.Fire(cannonBall, transform);
    }

    public void Fire(FireType shotType)
    {
        switch(shotType)
        {
            case FireType.Big:
                CannonShot oneBig = new CannonShot(1, 10, transform.right, 40, 0.1f, 60);
                oneBig.Fire(cannonBall, transform);
                break;
            case FireType.Tri:
                CannonShot triShot = new CannonShot(3, 5, transform.right, 40, 0.1f, 30);
                triShot.Fire(cannonBall, transform);
                break;
			case FireType.Both:
				CannonShot left = new CannonShot(1, 5, -transform.right, 40, 0.1f, 30);
				CannonShot right = new CannonShot(1, 5, transform.right, 40, 0.1f, 30);
				left.Fire(cannonBall, transform);
				right.Fire(cannonBall, transform);
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
				shot.Fire(cannonBall, transform);
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

        public CannonShot(Vector3 direction)
        {
            this.count = 1;
            this.damage = 1;
            this.direction = direction;
            this.fireSpeed = 40;
            this.verticalRatio = 0.1f;
            this.spreadAngle = 0;
        }

        public CannonShot(Vector3 direction, List<Upgrade> upgrades)
        {
            CannonShot defaultShot = new CannonShot(direction);
            foreach(Upgrade u in upgrades)
            {
                defaultShot += (ShotUpgrade)u;
            }
            this.count = defaultShot.count;
            this.damage = defaultShot.damage;
            this.direction = defaultShot.direction;
            this.fireSpeed = defaultShot.fireSpeed;
            this.verticalRatio = defaultShot.verticalRatio;
            this.spreadAngle = defaultShot.spreadAngle;
        }

        public static CannonShot operator +(CannonShot shot, ShotUpgrade upgrade)
        {
            return new CannonShot(
                shot.count + upgrade.count,
                shot.damage + upgrade.damage,
                shot.direction,
                shot.fireSpeed + upgrade.fireSpeed,
                shot.verticalRatio,
                shot.spreadAngle
                );
        }

        public void Fire(GameObject cannonBall, Transform shipTransform)
        {
            Quaternion angle = Quaternion.Euler(0, -spreadAngle * (count - 1) / 2, 0); //Quaternion.AngleAxis(-spreadAngle * (count - 1) / 2, Vector3.up) * direction;
            for (int i = 0; i < count; i++)
            {
                GameObject ball = Instantiate(cannonBall, shipTransform.position + (shipTransform.localScale.x / 2) * direction, Quaternion.identity);
                ball.transform.localScale = new Vector3(20 * damage, 20 * damage, 20 * damage);
                ball.SetActive(true);

                //ball.GetComponent<Rigidbody>().velocity = angle * fireSpeed;
                angle *= Quaternion.Euler(0, spreadAngle, 0);

                ball.transform.rotation = angle;

				ball.GetComponent<Rigidbody>().velocity = (Vector3.up * fireSpeed * verticalRatio) + ((direction) /2 * fireSpeed);
                ball.GetComponent<CannonBallBehaviorScript>().damageDealt = damage;
            }
            
        }
    }
}
