using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBallBehaviorScript : MonoBehaviour
{
    public int damageDealt;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < -2)
        {
            Destroy(this.gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        //Test if collision occured with enemy
        if (collision.gameObject.tag == "Enemy" && collision.relativeVelocity.magnitude > 2)
        {
            //Deal damage to enemy and delete projectile
            collision.gameObject.GetComponentInParent<Enemy>().TakeDamage(damageDealt);
            Destroy(this.gameObject);
            Debug.Log("Hit");
        }
    }
}
