using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBallBehavior : MonoBehaviour
{
    public int damageDealt;
    public GameObject hitbox;

    private GameObject projHitbox;
    private HitboxEnter _onTrigger;
    public HitboxEnter OnTrigger
    {
        set { _onTrigger = value; }
    }

    [SerializeField]
    private GameObject _impactParticles;

    // Start is called before the first frame update
    void Start()
    {
        projHitbox = Instantiate(hitbox, transform);
        projHitbox.GetComponent<Hitbox>().SetHitbox(gameObject, Vector3.zero, new Vector3(1, 1, 1), HitboxType.PlayerHitbox, damageDealt);
        projHitbox.GetComponent<Hitbox>().OnTrigger += PlayParticles;
        projHitbox.GetComponent<Hitbox>().OnTrigger += DestroyProj;
        projHitbox.GetComponent<Hitbox>().OnTrigger += _onTrigger;
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < -2)
        {
            SoundManager.instance.PlaySound("Splash");
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

    void PlayParticles(GameObject other)
    {
        if (other.tag == "Enemy")
        {
            GameObject.Instantiate(_impactParticles, transform.position, Quaternion.identity, other.transform);
        }
    }
}
