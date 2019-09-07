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
        GameObject objectHit = collision.gameObject;
        HealthBar healthScript = objectHit.GetComponent<HealthBar>();
        if(healthScript != null)
        {
            healthScript.TakeDamage(damageDealt);
            Debug.Log("hit");
        }
    }
}
