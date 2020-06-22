using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TentacleMode { StationarySlap = 0, TrackingSlap = 1, RisingAttack = 2}
public enum TentacleState { Rising = 0, Tracking = 1, Attacking = 2, Returning = 3, Destroy = 4}
public enum TentacleAnim { Rise = 0, Slam = 1, Return = 2, SpeedScale = 3 }

public class ClamTentacle : Physics
{
    [SerializeField]
    protected GameObject _hitboxPrefab;
    [SerializeField]
    protected GameObject _telegraphPrefab;
    [SerializeField]
    protected GameObject _particles;

    protected const float HEIGHT = 10.0f;

    protected TentacleMode _mode;
    protected TentacleState _state;
    protected float _time;
    protected Vector3 _particlePosition;
    protected float _risingTime;
    protected float _trackingTime;
    protected float _attackingTime;
    protected GetVector PlayerPosition;
    protected Animator _animator;
    protected int[] _animParm;
    protected GameObject _hitbox;
    protected GameObject _telegraph;
    protected float _damage;
    protected float _speedScale;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        _particles.transform.position = _particlePosition;
        if(_telegraph != null)
        {
            _telegraph.transform.position = new Vector3(_telegraph.transform.position.x, _particles.transform.position.y, _telegraph.transform.position.z);
        }

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
    /// <param name="damage">Damage dealt to player</param>
    /// <param name="risingTime">Time it take tentacale to rise from water</param>
    /// <param name="attackingTime">Time it takes tentacle to preform an attack</param>
    /// <param name="trackingTime">Time spent tracking the player</param>
    /// <param name="speedScale">Scale of speed applied to each of the time values</param>
    /// <param name="sizeScale">Size of tentacle, only effects x and z axis</param>
    public void SetTentacle(TentacleMode mode, GetVector PlayerPos, float damage, float risingTime, float trackingTime, float attackingTime, float speedScale, float sizeScale)
    {
        _mode = mode;
        _damage = damage;
        _speedScale = speedScale;
        PlayerPosition = PlayerPos;
        _risingTime = risingTime * _speedScale;
        _trackingTime = trackingTime * _speedScale;
        _attackingTime = attackingTime * _speedScale;

        _animator = GetComponentInChildren<Animator>();
        _animParm = new int[4] {
            Animator.StringToHash("rise"),
            Animator.StringToHash("slam"),
            Animator.StringToHash("return"),
            Animator.StringToHash("speedScale")
        };

        transform.localScale = new Vector3(transform.localScale.x * sizeScale, transform.localScale.y, transform.localScale.z * sizeScale);

        _animator.SetFloat(_animParm[(int)TentacleAnim.SpeedScale], 1 / _speedScale);

        _particlePosition = _particles.transform.position;

        //Set inital state
        _state = TentacleState.Rising;
        if (_mode == TentacleMode.RisingAttack)
        {
            _state = TentacleState.Tracking;
        }
        else
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + HEIGHT, transform.position.z);
        }

        _particles.transform.position = _particlePosition;
    }

    /// <summary>
    /// Tentacle stays still before slapping player
    /// </summary>
    private void StationarySlapBehavior()
    {
        switch(_state)
        {
            case TentacleState.Rising:
                if(_time == 0)
                {
                    _animator.Play(_animParm[(int)TentacleAnim.Rise]);                   
                }
                //Rise out of the water to prepare for attacking
                if(_time <= _risingTime)
                {
                    //ApplyConstantMoveForce(Vector3.up, HEIGHT, _risingTime);
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
                if (_time == 0)
                {
                    if (DoTelegraphs())
                    {
                        CreateTelegraph(new Vector3(0, _particles.transform.localPosition.y, 6 / transform.localScale.z), new Vector3(3.0f, 1.0f, 12.0f));

                    }
                }
                //For tracking, just stall for time
                _time += Time.deltaTime;

                if (_time > _trackingTime)
                {
                    _time = 0;
                    ClearTelegraph();
                    _state = TentacleState.Attacking;
                }
                break;
            case TentacleState.Attacking:
                if(_time == 0)
                {
                    //Play attacking animation and spawn hitbox
                    _animator.Play(_animParm[(int)TentacleAnim.Slam]);
                    _hitbox = Instantiate(_hitboxPrefab, transform);
                    _hitbox.GetComponent<Hitbox>().SetHitbox(gameObject, new Vector3(0, 0, 3), new Vector3(2, 2, 9), HitboxType.EnemyHitbox, _damage, new Vector2(90, 0), 500);
                }
                if (_time <= _attackingTime)
                {
                    //Move hitbox down with animation
                    _hitbox.transform.Translate(Vector3.down * HEIGHT * (1 / 0.8f) * (1 / _speedScale) * Time.deltaTime);
                    _time += Time.deltaTime;
                }

                //Instantiate hitbox when needed
                if (_time > _attackingTime)
                {
                    if (!_animator.IsInTransition(0))
                    {
                        Destroy(_hitbox);
                        _time = 0;
                        _state = TentacleState.Returning;
                    }
                }
                break;
            case TentacleState.Returning:
                //Go back underwater
                if(_time == 0)
                {
                    _animator.Play(_animParm[(int)TentacleAnim.Return]);
                    _particles.GetComponent<ParticleSystem>().Stop();
                }

                if (_time <= _risingTime)
                {
                    _time += Time.deltaTime;
                    ApplyConstantMoveForce(-Vector3.up, HEIGHT, _risingTime);
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
                if(_time == 0)
                {
                    _animator.Play(_animParm[(int)TentacleAnim.Rise]);
                }
                //Rise out of the water to prepare for attacking
                if (_time <= _risingTime)
                {
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
                //Track player
                if(_time == 0)
                {
                    if (DoTelegraphs())
                    {
                        CreateTelegraph(new Vector3(0, _particles.transform.localPosition.y, 6 / transform.localScale.z), new Vector3(3.0f, 1.0f, 12.0f));
                    }
                }

                if (_time < _trackingTime)
                {
                    _time += Time.deltaTime;
                    Vector3 destination = new Vector3(PlayerPosition().x, transform.position.y, PlayerPosition().z);
                    Quaternion desiredRotation = Quaternion.LookRotation(destination - transform.position);
                    _rotation = Quaternion.RotateTowards(_rotation, desiredRotation, 3.0f);
                }
                else
                {
                    ClearTelegraph();
                    _time = 0;
                    _state = TentacleState.Attacking;
                }
                break;
            case TentacleState.Attacking:
                if(_time == 0)
                {
                    _animator.Play(_animParm[(int)TentacleAnim.Slam]);
                    _hitbox = Instantiate(_hitboxPrefab, transform);
                    _hitbox.GetComponent<Hitbox>().SetHitbox(gameObject, new Vector3(0, 0, 3), new Vector3(2, 2, 9 ), HitboxType.EnemyHitbox, _damage, new Vector2(90, 0), 500);
                }
                if (_time <= _attackingTime)
                {
                    //Move hitbox down with animation
                    _hitbox.transform.Translate(Vector3.down * HEIGHT * (1 / 0.8f) * (1 / _speedScale) * Time.deltaTime);
                    _time += Time.deltaTime;
                }
                //Instantiate hitbox when needed
                if (_time > _attackingTime)
                {
                    if (!_animator.IsInTransition(0))
                    {
                        Destroy(_hitbox);
                        _time = 0;
                        _state = TentacleState.Returning;
                    }
                }
                break;
            case TentacleState.Returning:
                //Go back underwater
                if(_time == 0)
                {
                    _animator.Play(_animParm[(int)TentacleAnim.Return]);
                    _particles.GetComponent<ParticleSystem>().Stop();
                }

                if (_time <= _risingTime)
                {
                    _time += Time.deltaTime;
                    ApplyConstantMoveForce(-Vector3.up, HEIGHT, _risingTime);
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
                    _hitbox = Instantiate(_hitboxPrefab, transform);
                    _hitbox.GetComponent<Hitbox>().SetHitbox(gameObject, new Vector3(0, -6, 0), new Vector3(1.2f, 8, 1.2f), HitboxType.EnemyHitbox, _damage, new Vector2(90, 0), 500);
                }
                if (_time <= _risingTime)
                {
                    _time += Time.deltaTime;
                    ApplyConstantMoveForce(Vector3.up, HEIGHT, _risingTime);
                }
                else
                {
                    //After tentacle has risen, stop motion and start tracking
                    StopMotion();
                    _time = 0;
                    Destroy(_hitbox);
                    _state = TentacleState.Returning;
                }
                break;
            case TentacleState.Tracking:
                _time += Time.deltaTime;
                //For tracking, just stall for time
                if (_time > _trackingTime)
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
                if (_time <= _risingTime * 2)
                {
                    _time += Time.deltaTime;
                    ApplyConstantMoveForce(-Vector3.up, HEIGHT, _risingTime * 2);
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
    /// Creates a telegraph and adds it to the telegraph
    /// </summary>
    /// <param name="position">Position relative to enemy</param>
    /// <param name="scale">Local scale</param>
    protected void CreateTelegraph(Vector3 position, Vector3 scale)
    {
        GameObject temp = Instantiate(_telegraphPrefab, transform.position, transform.rotation, transform);
        temp.transform.localPosition = position;
        temp.transform.localScale = scale;

        _telegraph = temp;
    }

    /// <summary>
    /// Clears current telegraph
    /// </summary>
    protected void ClearTelegraph()
    {
        _telegraph.GetComponentInChildren<ParticleSystem>().Stop();
    }

    protected bool DoTelegraphs()
    {
        return true;
    }
}
