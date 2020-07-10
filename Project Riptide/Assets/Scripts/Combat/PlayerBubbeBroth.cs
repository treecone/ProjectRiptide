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
    private List<StatusEffects> _enemyStatus;

    // Start is called before the first frame update
    void Start()
    {
        _enemyStatus = new List<StatusEffects>();

        _hitbox = GetComponentInChildren<Hitbox>().gameObject;
        _hitbox.GetComponent<Hitbox>().OnTrigger += SlowEnemy;
        _hitbox.GetComponent<Hitbox>().OnExit += RemoveSlowExit;
        _hitbox.GetComponent<Hitbox>().OnDestruction += RemoveSlow;
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
            //GameObject.Destroy(transform.Find("Hitbox").gameObject);
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
            if (!other.GetComponent<StatusEffects>().CheckStatus("PlayerBubbles"))
            {
                other.GetComponent<StatusEffects>().AddStatus(StatusType.MovementSpeed, "PlayerBubbles", 9999.0f, -0.9f);
                _enemyStatus.Add(other.GetComponent<StatusEffects>());
            }
        }
    }

    /// <summary>
    /// Removes slow effect when enemy leaves bubble field
    /// </summary>
    /// <param name="other"></param>
    private void RemoveSlowExit(GameObject other)
    {
        if (other.tag == "Hitbox" && other.transform.parent.tag == "Enemy")
        {
            other.GetComponent<Hitbox>().AttachedObject.GetComponent<StatusEffects>().RemoveStatus("PlayerBubbles");
        }
    }

    /// <summary>
    /// Removes slow effect when bubble field disappears
    /// </summary>
    private void RemoveSlow()
    {
        if (_enemyStatus.Count > 0)
        {
            for(int i = 0; i < _enemyStatus.Count; i++)
            {
                _enemyStatus[i].RemoveStatus("PlayerBubbles");
            }
        }
    }
}
