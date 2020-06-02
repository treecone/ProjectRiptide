using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonFire : MonoBehaviour
{
    [SerializeField]
    private GameObject _cannonBall;
    [SerializeField]
    private Upgrades _shipUpgradeScript;
    [SerializeField]
    private GameObject _cannonSmoke;
    public GameObject CannonSmoke => _cannonSmoke;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*if(Input.GetKeyDown(KeyCode.Space))
        {
            //Debug.Log("Fire");
            Fire(-transform.right);
        }*/
    }


    public float Fire(Vector3 targetDir, float offset)
    {
        return Fire(transform.right, targetDir, offset);
    }

    public float Fire(Vector3 direction, Vector3 targetDir, float offset)
    {
        //Calculate angle to fire shot
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

        //Camera camera = Camera.main;
        //camera.transform.Translate(0f, 2.0f, 2.0f);
        CannonShot rightShot = new CannonShot(direction, -angle + offset, _shipUpgradeScript.masterUpgrade);
        rightShot.Fire(_cannonBall, gameObject, _cannonSmoke);
        return angle;
    }

    public class CannonShot
    {
        private int _damage;
        private Vector3 _direction;
        private float _fireSpeed;
        private float _fireAngle;
        private float _verticalRatio;
        private int _count;
        private float _spreadAngle;
        private float _size;

        public CannonShot(Vector3 direction, float fireAngle, Upgrade upgrade)
        {
            this._count = 1 + (int)upgrade["count"];
            this._damage = 1 + (int)upgrade["damage"];
            this._direction = direction;
            this._fireSpeed = 40 + upgrade["fireSpeed"];
            this._fireAngle = fireAngle;
            this._verticalRatio = 0.1f + upgrade["verticalRatio"];
            this._spreadAngle = 20;
            this._size = 0.5f + upgrade["shotSize"];
        }

        public void Fire(GameObject cannonBall, GameObject ship, GameObject cannonSmoke)
        {
            float trueSpreadAngle;
            if (_count == 1)
            {
                trueSpreadAngle = 0;
            }
            else
            {
                trueSpreadAngle = _spreadAngle / (_count - 1);
            }

            Quaternion angle = Quaternion.Euler(0, (-trueSpreadAngle * (_count - 1) / 2) + ship.transform.rotation.eulerAngles.y + _fireAngle, 0); //Quaternion.AngleAxis(-spreadAngle * (count - 1) / 2, Vector3.up) * direction;
            for (int i = 0; i < _count; i++)
            {
                GameObject ball = Instantiate(cannonBall, ship.transform.position + (ship.transform.localScale.x / 2) * _direction, Quaternion.identity);
                ball.transform.localScale = new Vector3(_size, _size, _size);
                ball.SetActive(true);

                //ball.GetComponent<Rigidbody>().velocity = angle * fireSpeed;

                ball.transform.rotation = angle;
                Instantiate(cannonSmoke, ship.transform.position, angle, ship.transform);
                
				ball.GetComponent<Rigidbody>().velocity = (Vector3.up * _fireSpeed * (_verticalRatio / (_verticalRatio + 1.0f))) + (ball.transform.forward * _fireSpeed * (1.0f / (_verticalRatio + 1.0f))) + (/*ship.GetComponent<ShipMovement>().linearVelocity*/2 * 5 * ship.transform.forward);
                ball.GetComponent<CannonBallBehavior>().damageDealt = _damage;

                angle *= Quaternion.Euler(0, trueSpreadAngle, 0);
            }
            
        }
    }
}
