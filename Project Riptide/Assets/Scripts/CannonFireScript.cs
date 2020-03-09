using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FireType { Right, Left, Both, Big, Tri, Target};

public class CannonFireScript : MonoBehaviour
{
    public GameObject cannonBall;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
		if(Input.GetKeyDown(KeyCode.Space))
		{
			Fire(FireType.Both);
		}
    }

    public void Fire(FireType shotType)
    {
        switch(shotType)
        {
            case FireType.Big:
                CannonShot oneBig = new CannonShot(1, 10, transform.right, 10, 60, 0);
                oneBig.Fire(cannonBall, transform);
                break;
            case FireType.Tri:
                CannonShot triShot = new CannonShot(3, 5, transform.right, 10, 30, 0);
                triShot.Fire(cannonBall, transform);
                break;
			case FireType.Both:
				CannonShot left = new CannonShot(1, 5, -transform.right, 10, 30, 0);
				CannonShot right = new CannonShot(1, 5, transform.right, 10, 30, 0);
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
				CannonShot shot = new CannonShot(1, 5, direction.normalized, 10, 30, 0);
				shot.Fire(cannonBall, transform);
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
            Quaternion angle = Quaternion.Euler(0, -spreadAngle * (count - 1) / 2, 0); //Quaternion.AngleAxis(-spreadAngle * (count - 1) / 2, Vector3.up) * direction;
            for (int i = 0; i < count; i++)
            {
                GameObject ball = Instantiate(cannonBall, shipTransform.position + (shipTransform.localScale.x / 2) * direction, Quaternion.identity);
                ball.SetActive(true);

                //ball.GetComponent<Rigidbody>().velocity = angle * fireSpeed;
                angle *= Quaternion.Euler(0, spreadAngle, 0);

                ball.transform.rotation = angle;

                ball.GetComponent<Rigidbody>().velocity = (Vector3.up * fireSpeedVert) + ((direction * 2 + shipTransform.forward)/2 * fireSpeedHoriz);
                ball.GetComponent<CannonBallBehaviorScript>().damageDealt = damage;
            }
            
        }
    }
}
