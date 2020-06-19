using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{   
    [SerializeField]
    private Vector3 waterOffset;
    [SerializeField]
    private GameObject splashParticle;

    private GameObject player;
    private bool frameOne = true;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = player.transform.position + waterOffset;
        frameOne = false;
    }

    /*private void OnTriggerEnter(Collider other)
    {
        if (!frameOne && other.gameObject.tag == "Hitbox")
        {
            Hitbox hitbox = other.gameObject.GetComponent<Hitbox>();
            if (hitbox.Type == HitboxType.EnemyHurtbox || hitbox.Type == HitboxType.PlayerHurtbox)
            {
                GameObject particle = Instantiate(splashParticle, new Vector3(other.transform.position.x, transform.position.y, other.transform.position.z), splashParticle.transform.rotation);
                particle.transform.localScale = new Vector3(
                    particle.transform.localScale.x * other.transform.localScale.x,
                    particle.transform.localScale.y * other.transform.localScale.x,
                    particle.transform.localScale.z * other.transform.localScale.x);
            }
        }
    }*/

    /*private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Hitbox")
        {
            Physics obj = other.gameObject.GetComponent<Hitbox>().AttachedObject.GetComponent<Physics>();
            if (obj != null)
            {
                float yVelocity = obj.Velocity.y;
                if (yVelocity != 0)
                {
                    Debug.Log("Play splash at " + obj.Position);
                }
            }
        }
    }*/
}
