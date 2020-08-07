using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteelMine : MonoBehaviour
{
    [SerializeField]
    private GameObject _explosionParticles;

    const float MAX_LIFE_SPAN = 60.0f;
    float _lifeSpan;

    // Start is called before the first frame update
    void Start()
    {
        GetComponentInChildren<Hitbox>().OnTrigger += MineTriggered;
    }

    // Update is called once per frame
    void Update()
    {
        _lifeSpan += Time.deltaTime;
        if(_lifeSpan > MAX_LIFE_SPAN)
        {
            Destroy(gameObject);
        }
    }

    public void SetMine(float damage)
    {
        GetComponentInChildren<Hitbox>().Damage = damage;
    }

    /// <summary>
    /// Mine triggered
    /// </summary>
    /// <param name="other"></param>
    void MineTriggered(GameObject other)
    {
        if(other.tag == "Enemy")
        {
            Instantiate(_explosionParticles, transform.position, _explosionParticles.transform.rotation);
            Destroy(gameObject);
        }
    }
}
