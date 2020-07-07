using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBubbeBroth : MonoBehaviour
{
    private float _maxLifeSpan = 15.0f;

    private float _lifeSpan = 0.0f;
    private bool _destroying = false;
    private float _debuffDuration;

    // Start is called before the first frame update
    void Start()
    {
        GetComponentInChildren<Hitbox>().OnTrigger += SlowEnemy;
    }

    // Update is called once per frame
    void Update()
    {
        _lifeSpan += Time.deltaTime;
        if (_lifeSpan >= _maxLifeSpan && !_destroying)
        {
            GetComponentInChildren<ParticleSystem>().Stop();
            GameObject.Destroy(transform.Find("Hitbox").gameObject);
            _destroying = true;
        }
    }

    /// <summary>
    /// Sets values for bubble broth
    /// </summary>
    /// <param name="lifeSpan">Max life span</param>
    /// <param name="debuffDuration">Debuff Length</param>
    public void SetBubbleBroth(float lifeSpan, float debuffDuration)
    {
        _maxLifeSpan = lifeSpan;
        _debuffDuration = debuffDuration;
    }

    /// <summary>
    /// Slows player on entering bubble field
    /// </summary>
    /// <param name="other"></param>
    private void SlowEnemy(GameObject other)
    {
        if (other.tag == "Enemy")
        {
            //Slow enemy
            //other.GetComponent<StatusEffects>().AddStatus(StatusType.ShipSpeed, _debuffDuration, -0.75f);
            Debug.Log("Enemy would be slowed for " + _debuffDuration + " seconds.");
        }
    }
}
