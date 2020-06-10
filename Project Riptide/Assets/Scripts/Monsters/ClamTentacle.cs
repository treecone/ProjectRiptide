using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TentacleMode { StationarySlap = 0, TrackingSlap = 1, RisingAttack = 2}
public enum TentacleState { Rising = 0, Tracking = 1, Attacking = 2, Returning = 3, Destroy = 4}

public class ClamTentacle : Physics
{
    [SerializeField]
    protected GameObject _hitbox;
    [SerializeField]
    protected GameObject _particles;

    protected const float HEIGHT = 10.0f;

    protected TentacleMode _mode;
    protected TentacleState _state;
    protected float _time;
    protected Vector3 _particlePosition;
    protected float risingTime;
    protected float trackingTime;
    protected float attackingTime;
    protected GetVector PlayerPosition;


    // Start is called before the first frame update
    protected override void Start()
    {
        _particlePosition = _particles.transform.position;
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        _particles.transform.position = _particlePosition;

        switch(_mode)
        {
            case TentacleMode.StationarySlap:
                StationarySlapBehavior();
                break;
            case TentacleMode.TrackingSlap:
                TrackingSlapBehavior();
                break;
            case TentacleMode.RisingAttack:
                RisingAttackBehavior();
                break;
        }

        //Destory tentacle if marked for destruction
        if(_state == TentacleState.Destroy)
        {
            Destroy(gameObject);
        }

        base.Update();
    }

    /// <summary>
    /// Sets values for the tentacle
    /// </summary>
    /// <param name="mode">Attack mode of tentacle</param>
    /// <param name="PlayerPos">Method to get player position</param>
    public void SetTentacle(TentacleMode mode, GetVector PlayerPos, float risingTime, float trackingTime, float attackingTime, float speedScale)
    {
        _mode = mode;
        PlayerPosition = PlayerPos;

        this.risingTime = risingTime * speedScale;
        this.trackingTime = trackingTime * speedScale;
        this.attackingTime = attackingTime * speedScale;

        //Set inital state
        _state = TentacleState.Rising;
        if(_mode == TentacleMode.RisingAttack)
        {
            _state = TentacleState.Tracking;
        }
    }

    /// <summary>
    /// Tentacle stays still before slapping player
    /// </summary>
    private void StationarySlapBehavior()
    {
        switch(_state)
        {
            case TentacleState.Rising:
                //Rise out of the water to prepare for attacking
                if(_time <= risingTime)
                {
                    ApplyConstantMoveForce(Vector3.up, HEIGHT, risingTime);
                    _time += Time.deltaTime;
                }
                else
                {
                    //After tentacle has risen, stop motion and start tracking
                    StopMotion();
                    _time = 0;
                    _state = TentacleState.Tracking;
                }
                break;
            case TentacleState.Tracking:
                //For tracking, just stall for time
                _time += Time.deltaTime;

                if (_time > trackingTime)
                {
                    _time = 0;
                    _state = TentacleState.Attacking;
                }
                break;
            case TentacleState.Attacking:
                //Slap down and attack the player
                _time += Time.deltaTime;

                //Instantiate hitbox when needed
                if (_time > attackingTime)
                {
                    _time = 0;
                    _state = TentacleState.Returning;
                }
                break;
            case TentacleState.Returning:
                //Go back underwater
                if(_time == 0)
                {
                    _particles.GetComponent<ParticleSystem>().Stop();
                }

                if (_time <= risingTime)
                {
                    _time += Time.deltaTime;
                    ApplyConstantMoveForce(-Vector3.up, HEIGHT, risingTime);
                }
                else
                {
                    //After tentacle has risen, stop motion and start tracking
                    StopMotion();
                    _time = 0;
                    _state = TentacleState.Destroy;
                }
                break;
        }
    }

    /// <summary>
    /// Tentacle tracks players movements before striking the player
    /// </summary>
    private void TrackingSlapBehavior()
    {
        switch (_state)
        {
            case TentacleState.Rising:
                //Rise out of the water to prepare for attacking
                if (_time <= risingTime)
                {
                    _time += Time.deltaTime;
                    ApplyConstantMoveForce(Vector3.up, HEIGHT, risingTime);
                }
                else
                {
                    //After tentacle has risen, stop motion and start tracking
                    StopMotion();
                    _time = 0;
                    _state = TentacleState.Tracking;
                }
                break;
            case TentacleState.Tracking:
                //Track player
                if (_time < trackingTime)
                {
                    _time += Time.deltaTime;
                    Vector3 destination = new Vector3(PlayerPosition().x, transform.position.y, PlayerPosition().z);
                    Quaternion desiredRotation = Quaternion.LookRotation(destination - transform.position);
                    _rotation = Quaternion.RotateTowards(_rotation, desiredRotation, 2.0f);
                }
                else
                {
                    _time = 0;
                    _state = TentacleState.Attacking;
                }
                break;
            case TentacleState.Attacking:
                //Slap down and attack the player
                _time += Time.deltaTime;
                //Instantiate hitbox when needed
                if (_time > attackingTime)
                {
                    _time = 0;
                    _state = TentacleState.Returning;
                }
                break;
            case TentacleState.Returning:
                //Go back underwater
                if(_time == 0)
                {
                    _particles.GetComponent<ParticleSystem>().Stop();
                }

                if (_time <= risingTime)
                {
                    _time += Time.deltaTime;
                    ApplyConstantMoveForce(-Vector3.up, HEIGHT, risingTime);
                }
                else
                {
                    //After tentacle has risen, stop motion and start tracking
                    StopMotion();
                    _time = 0;
                    _state = TentacleState.Destroy;
                }
                break;
        }
    }

    /// <summary>
    /// Tentacle rises out of the water quickly to attack the player
    /// </summary>
    private void RisingAttackBehavior()
    {
        switch (_state)
        {
            case TentacleState.Rising:
                //Rise out of the water to prepare for attacking
                if(_time == 0)
                {
                    //Add hitbox
                }
                if (_time <= risingTime)
                {
                    _time += Time.deltaTime;
                    ApplyConstantMoveForce(Vector3.up, HEIGHT, risingTime);
                }
                else
                {
                    //After tentacle has risen, stop motion and start tracking
                    StopMotion();
                    _time = 0;
                    _state = TentacleState.Returning;
                }
                break;
            case TentacleState.Tracking:
                _time += Time.deltaTime;
                //For tracking, just stall for time
                if (_time > trackingTime)
                {
                    _time = 0;
                    _state = TentacleState.Rising;
                }
                break;
            case TentacleState.Returning:
                if (_time == 0)
                {
                    _particles.GetComponent<ParticleSystem>().Stop();
                }

                //Go back underwater
                if (_time <= risingTime * 2)
                {
                    _time += Time.deltaTime;
                    ApplyConstantMoveForce(-Vector3.up, HEIGHT, risingTime * 2);
                }
                else
                {
                    //After tentacle has risen, stop motion and start tracking
                    StopMotion();
                    _time = 0;
                    _state = TentacleState.Destroy;
                }
                break;
        }
    }
}
