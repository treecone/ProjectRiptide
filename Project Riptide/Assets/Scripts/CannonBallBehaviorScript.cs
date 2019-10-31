using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBallBehaviorScript : MonoBehaviour
{
    public int damageDealt;
    public GameObject hitbox;

    private GameObject projHitbox;

    // Start is called before the first frame update
    void Start()
    {
        projHitbox = Instantiate(hitbox, transform);
        projHitbox.GetComponent<Hitbox>().SetHitbox(gameObject, transform.position, new Vector3(0.015f, 0.015f, 0.015f), HitboxType.PlayerHitbox, damageDealt);
        projHitbox.GetComponent<Hitbox>().OnTrigger += DestroyProj;
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < -2)
        {
            Destroy(this.gameObject);
        }
    }

    //Destroy projectile upon hitbox activation
    void DestroyProj(GameObject hit)
    {
        Destroy(gameObject);
    }
}
