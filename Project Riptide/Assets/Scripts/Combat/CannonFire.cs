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

    private float _shotAngle = 90;
    public float ShotAngle => _shotAngle;

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

        //Clamp shot angle
        float shotClamp = (180f - _shotAngle) / 2;
        if(angle > 0 && angle < shotClamp)
        {
            angle = shotClamp;
        }
        if(angle > 180 - shotClamp)
        {
            angle = 180 - shotClamp;
        }
        if(angle < 0 && angle > -shotClamp)
        {
            angle = -shotClamp;
        }
        if(angle < -180 + shotClamp)
        {
            angle = -180 + shotClamp;
        }

        //Camera camera = Camera.main;
        //camera.transform.Translate(0f, 2.0f, 2.0f);
        CannonShot rightShot = new CannonShot(direction, -angle + offset, _shipUpgradeScript.masterUpgrade);
        rightShot.Fire(_cannonBall, gameObject, _cannonSmoke);

        /*if (offset == 15.0f)
        {
            CannonShot rightShot = new CannonShot(direction, 90 + offset, _shipUpgradeScript.masterUpgrade);
            rightShot.Fire(_cannonBall, gameObject, _cannonSmoke);
        }
        else
        {
            CannonShot leftShot = new CannonShot(direction, -90 + offset, _shipUpgradeScript.masterUpgrade);
            leftShot.Fire(_cannonBall, gameObject, _cannonSmoke);
        }*/
        return angle;
    }

    public float Fire(Vector3 direction, Vector3 targetDir, float offset, Upgrade upgrade)
    {
        //Calculate angle to fire shot
        float angle = Mathf.Acos(Vector3.Dot(targetDir.normalized, transform.forward.normalized));
        Vector3 cross = Vector3.Cross(targetDir, transform.forward);
        if (Vector3.Dot(Vector3.up, cross) < 0)
        { // Or > 0
            angle = -angle;
        }
        angle *= Mathf.Rad2Deg;

        //Clamp shot angle
        float shotClamp = (180f - _shotAngle) / 2;
        if (angle > 0 && angle < shotClamp)
        {
            angle = shotClamp;
        }
        if (angle > 180 - shotClamp)
        {
            angle = 180 - shotClamp;
        }
        if (angle < 0 && angle > -shotClamp)
        {
            angle = -shotClamp;
        }
        if (angle < -180 + shotClamp)
        {
            angle = -180 + shotClamp;
        }

        //Camera camera = Camera.main;
        //camera.transform.Translate(0f, 2.0f, 2.0f);
        CannonShot rightShot = new CannonShot(direction, -angle + offset, upgrade);
        rightShot.Fire(_cannonBall, gameObject, _cannonSmoke);
        return angle;
    }

    public float Fire(Vector3 direction, Vector3 targetDir, float offset, Upgrade upgrade, HitboxEnter onTrigger)
    {
        //Calculate angle to fire shot
        float angle = Mathf.Acos(Vector3.Dot(targetDir.normalized, transform.forward.normalized));
        Vector3 cross = Vector3.Cross(targetDir, transform.forward);
        if (Vector3.Dot(Vector3.up, cross) < 0)
        { // Or > 0
            angle = -angle;
        }
        angle *= Mathf.Rad2Deg;

        //Clamp shot angle
        float shotClamp = (180f - _shotAngle) / 2;
        if (angle > 0 && angle < shotClamp)
        {
            angle = shotClamp;
        }
        if (angle > 180 - shotClamp)
        {
            angle = 180 - shotClamp;
        }
        if (angle < 0 && angle > -shotClamp)
        {
            angle = -shotClamp;
        }
        if (angle < -180 + shotClamp)
        {
            angle = -180 + shotClamp;
        }

        //Camera camera = Camera.main;
        //camera.transform.Translate(0f, 2.0f, 2.0f);
        CannonShot rightShot = new CannonShot(direction, -angle + offset, upgrade, onTrigger);
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
        private HitboxEnter _onTrigger;

        public CannonShot(Vector3 direction, float fireAngle, Upgrade upgrade)
        {
            this._count = 1 + (int)upgrade[StatusType.Count];
            this._damage = 3 + (int)upgrade[StatusType.Damage];
            this._direction = direction;
            this._fireSpeed = 40 + upgrade[StatusType.FireSpeed];
            this._fireAngle = fireAngle;
            this._verticalRatio = 0.1f + upgrade[StatusType.VerticalRatio];
            this._spreadAngle = 20 + upgrade[StatusType.SpreadAngle];
            this._size = 0.5f + upgrade[StatusType.ShotSize];
        }

        public CannonShot(Vector3 direction, float fireAngle, Upgrade upgrade, HitboxEnter onTrigger)
        {
            this._count = 1 + (int)upgrade[StatusType.Count];
            this._damage = 3 + (int)upgrade[StatusType.Damage];
            this._direction = direction;
            this._fireSpeed = 40 + upgrade[StatusType.FireSpeed];
            this._fireAngle = fireAngle;
            this._verticalRatio = 0.1f + upgrade[StatusType.VerticalRatio];
            this._spreadAngle = 20 + upgrade[StatusType.SpreadAngle];
            this._size = 0.5f + upgrade[StatusType.ShotSize];
            _onTrigger = onTrigger;
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
                if (_onTrigger != null)
                {
                    ball.GetComponent<CannonBallBehavior>().OnTrigger = _onTrigger;
                }

                angle *= Quaternion.Euler(0, trueSpreadAngle, 0);
            }
            SoundManager.instance.PlaySound("CannonFire");
        }
    }
}
