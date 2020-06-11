using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBallBehavior : MonoBehaviour
{
    public int damageDealt;
    public GameObject hitbox;

    private GameObject projHitbox;

    // Start is called before the first frame update
    void Start()
    {
        projHitbox = Instantiate(hitbox, transform);
        projHitbox.GetComponent<Hitbox>().SetHitbox(gameObject, Vector3.zero, new Vector3(1, 1, 1), HitboxType.PlayerHitbox, damageDealt);
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
        if (hit.tag == "Obstical")
            Destroy(gameObject);
        if (hit.tag == "Enemy")
            Destroy(gameObject);
    }
}
