using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KoiBubbleBroth : MonoBehaviour
{
    private const float MAX_LIFESPAN = 15.0f;

    private float _lifeSpan = 0.0f;
    private bool _destroying = false;

    // Start is called before the first frame update
    void Start()
    {
        GetComponentInChildren<Hitbox>().OnTrigger += SlowPlayer;
        GetComponentInChildren<Hitbox>().OnExit += RemoveSlowExit;
        GetComponentInChildren<Hitbox>().OnDestruction += RemoveSlow;
    }

    // Update is called once per frame
    void Update()
    {
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
            //Deal 2 damage per second for 5 seconds
            other.GetComponent<StatusEffects>().AddStatus(StatusType.ShipSpeed, 5.0f, -0.75f);
        }
    }

    /// <summary>
    /// Removes slow effect when player leaves bubble field
    /// </summary>
    /// <param name="other"></param>
    private void RemoveSlowExit(GameObject other)
    {

    }

    /// <summary>
    /// Removes slow effect when bubble field disappears
    /// </summary>
    private void RemoveSlow()
    {

    }
}
