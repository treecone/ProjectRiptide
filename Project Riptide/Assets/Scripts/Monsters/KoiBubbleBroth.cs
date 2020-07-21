using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KoiBubbleBroth : MonoBehaviour
{
    private const float MAX_LIFESPAN = 15.0f;

    private float _lifeSpan = 0.0f;
    private bool _destroying = false;
    private bool _maxSize = false;
    private Vector3 _hitboxSize;
    private Vector3 _telegraphSize;
    private GameObject _telegraph;
    private GameObject _hitbox;
    private StatusEffects _playerStatus;

    // Start is called before the first frame update
    void Start()
    {
        _hitbox = GetComponentInChildren<Hitbox>().gameObject;
        _hitbox.GetComponent<Hitbox>().OnTrigger += SlowPlayer;
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
        if(_lifeSpan < 2.0f)
        {
            _hitbox.transform.localScale = _hitboxSize * (_lifeSpan / 2.0f);
            _telegraph.transform.localScale = _telegraphSize * (_lifeSpan / 2.0f);
        }
        else if(!_maxSize)
        {
            _maxSize = true;
            _hitbox.transform.localScale = _hitboxSize;
            _telegraph.transform.localScale = _telegraphSize;
        }

        _lifeSpan += Time.deltaTime;
        if(_lifeSpan >= MAX_LIFESPAN && !_destroying)
        {
            GetComponentInChildren<ParticleSystem>().Stop();
            GameObject.Destroy(transform.Find("Hitbox").gameObject);
            _destroying = true;
        }
    }

    /// <summary>
    /// Slows player on entering bubble field
    /// </summary>
    /// <param name="other"></param>
    private void SlowPlayer(GameObject other)
    {
        if (other.tag == "Player")
        {
            if (!other.GetComponent<StatusEffects>().CheckStatus("BubbleBrothLasting"))
            {
                //Deal 2 damage per second for 5 seconds
                other.GetComponent<StatusEffects>().AddStatus(StatusType.MovementSpeed, "BubbleBroth", 99.0f, -0.75f);
                _playerStatus = other.GetComponent<StatusEffects>();
            }
        }
    }

    /// <summary>
    /// Removes slow effect when player leaves bubble field
    /// </summary>
    /// <param name="other"></param>
    private void RemoveSlowExit(GameObject other)
    {
        if(other.tag == "Hitbox" && other.transform.parent.tag == "Player")
        {
            other.GetComponent<Hitbox>().AttachedObject.GetComponent<StatusEffects>().RemoveStatus("BubbleBroth");
            if (!other.GetComponent<Hitbox>().AttachedObject.GetComponent<StatusEffects>().CheckStatus("BubbleBrothLasting"))
            {
                other.GetComponent<Hitbox>().AttachedObject.GetComponent<StatusEffects>().AddStatus(StatusType.MovementSpeed, "BubbleBrothLasting", 3.0f, -0.75f);
            }
            _playerStatus = null;
        }
    }

    /// <summary>
    /// Removes slow effect when bubble field disappears
    /// </summary>
    private void RemoveSlow()
    {
        if (_playerStatus != null)
        {

            _playerStatus.RemoveStatus("BubbleBroth");
            _playerStatus.AddStatus(StatusType.MovementSpeed, "BubbleBrothLasting", 3.0f, -0.75f);
        }
    }
}
