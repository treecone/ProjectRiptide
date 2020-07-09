using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBubbeBroth : MonoBehaviour
{
    private float _maxLifeSpan = 15.0f;

    private float _lifeSpan = 0.0f;
    private bool _destroying = false;
    private float _debuffDuration;
    private bool _maxSize = false;
    private Vector3 _hitboxSize;
    private Vector3 _telegraphSize;
    private GameObject _telegraph;
    private GameObject _hitbox;

    // Start is called before the first frame update
    void Start()
    {
        _hitbox = GetComponentInChildren<Hitbox>().gameObject;
        _hitbox.GetComponent<Hitbox>().OnTrigger += SlowEnemy;
        _hitboxSize = _hitbox.transform.localScale;
        _hitbox.transform.localScale = Vector3.zero;

        _telegraph = transform.Find("Bubble Field").Find("BubbleFieldTelegraph").gameObject;
        _telegraphSize = _telegraph.transform.localScale;
        _telegraph.transform.localScale = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (_lifeSpan < 2.0f)
        {
            _hitbox.transform.localScale = _hitboxSize * (_lifeSpan / 2.0f);
            _telegraph.transform.localScale = _telegraphSize * (_lifeSpan / 2.0f);
        }
        else if (!_maxSize)
        {
            _maxSize = true;
            _hitbox.transform.localScale = _hitboxSize;
            _telegraph.transform.localScale = _telegraphSize;
        }

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
            other.GetComponent<StatusEffects>().AddStatus(StatusType.MovementSpeed, _debuffDuration, -0.8f);
        }
    }
}
