using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    //Fields
    private Vector3 velocity;
    private int damage;
    private float speed;
    private float currLifeSpan;
    private float maxLifeSpan;

    // Start is called before the first frame update
    void Start()
    {
        currLifeSpan = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        //Increment lifespan
        currLifeSpan += Time.deltaTime;

        if(currLifeSpan >= maxLifeSpan)
        {
            GameObject.Destroy(gameObject);
        }
        else
        {
            transform.Translate(velocity);
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        //If projectile collides with an obstical destory it 
        if (collision.gameObject.tag == "Obstical")
            GameObject.Destroy(gameObject);
        else
            GameObject.Destroy(gameObject);
    }

    public void LoadProjectile(Vector3 velocity, float speed, int damage, float maxLifeSpan)
    {
        this.velocity = velocity;
        this.speed = speed;
        this.damage = damage;
        this.maxLifeSpan = maxLifeSpan;
    }
}
