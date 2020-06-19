using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //fields
    [SerializeField]
    private Transform _player;
    [SerializeField]
    private Vector3 _offset = new Vector3(35, 35 , 0);
    [SerializeField]
    private float _smoothTime = 2;
    private Vector3 _moveVelocity;
    private Vector3 _movePosition;
    [SerializeField]
    private List<Enemy> allTargets;
    [SerializeField]
    private float _playerWeight = 0.3f;

    //Zoom
    private Enemy _farthestTarget;
    [SerializeField]
    private Vector3 _minZoom;
    [SerializeField]
    private Vector3 _maxZoom;
    private InputManager _inputManager;


    void Start ()
    {
        if(_player == null)
        {
            _player = GameObject.FindWithTag("Player").transform;
            _inputManager = GameObject.Find("Canvas").GetComponent<InputManager>();
            allTargets = _inputManager.TargetEnemies;
            Debug.LogWarning("No player was assigned to the camera. Default: Player Tag. ");
        }
    }

    void Update()
    {

    }

    public void UpdateCamera()
    {
        if (allTargets.Count > 0)
        {
            DynamicZoom();
            _movePosition = Vector3.zero;
            //In combat
            for (int i = 0; i < allTargets.Count; i++)
            {
                if (_farthestTarget == null)
                {
                    _farthestTarget = allTargets[i];
                }
                else if(allTargets.Count > 1)
                {
                    if (_farthestTarget.transform.position.sqrMagnitude - _player.transform.position.sqrMagnitude < allTargets[i].transform.position.sqrMagnitude - _player.transform.position.sqrMagnitude) //Distance
                    {
                        _farthestTarget = allTargets[i];
                    }
                }
                _movePosition += allTargets[i].transform.position;
            }
            _movePosition /= allTargets.Count;
            _movePosition = Vector3.Lerp(_player.transform.position, _movePosition, _playerWeight);
            _movePosition += _offset;
        }
        else
        {
            //Not in combat
            _farthestTarget = null;
            _offset = Vector3.Lerp(_offset, _minZoom, 0.01f);
            _movePosition = _player.position + _offset;
        }
        transform.position = Vector3.SmoothDamp(transform.position, _movePosition, ref _moveVelocity, _smoothTime);
        //transform.LookAt(_movePosition-_offset);

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(_movePosition, 0.5f);
    }

    void DynamicZoom ()
    {
        if(_farthestTarget != null)
        _offset = Vector3.Lerp(_minZoom, _maxZoom, (_player.transform.position - _farthestTarget.transform.position).sqrMagnitude / Mathf.Pow(_inputManager.MaxCombatRange,2) - 0.1f);
    }

    public void ToggleCombatView(bool on)
    {
        /*if(on)
        {
            _offset = new Vector3(30, 30, 0);
        }
        else
        {
            _offset = new Vector3(35, 35, 0);
        }
        */
    }
}
