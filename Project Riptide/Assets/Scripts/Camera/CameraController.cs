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
    [SerializeField]
    private float _cameraShakeAmount;

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
        /*if (allTargets.Count > 0)
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
            SetOffScreenIndicators();
        }
        else
        {*/
            //Not in combat
            _farthestTarget = null;
            _offset = Vector3.Lerp(_offset, _minZoom, 0.01f);
            _movePosition = _player.position + _offset;
        //}
        transform.position = Vector3.SmoothDamp(transform.position, _movePosition, ref _moveVelocity, _smoothTime);
        //transform.LookAt(_movePosition-_offset);

    }

    /// <summary>
    /// Sets screen indicators for target monsters off screen
    /// </summary>
    /*private void SetOffScreenIndicators()
    {
        for(int i = 0; i < _inputManager.TargetEnemies.Count; i++)
        {
            //If target enemy is not on the screen
            if(!_inputManager.TargetEnemies[i].IsVisible)
            {
                //If enemy does not have indicator create one
                if(_inputManager.TargetEnemies[i].OffScreenIndicator == null)
                {
                    _inputManager.TargetEnemies[i].OffScreenIndicator = Instantiate(_offScreenIndPrefab, _inputManager.transform.position, _inputManager.transform.rotation, _inputManager.transform);
                }
                if(!_inputManager.TargetEnemies[i].OffScreenIndicator.activeSelf)
                {
                    _inputManager.TargetEnemies[i].OffScreenIndicator.SetActive(true);
                }

                //Set position and rotation of indicator
                Vector3 enemyDir3D = _inputManager.TargetEnemies[i].Position - _player.position;
                Vector2 enemyDir = new Vector2(enemyDir3D.z, -enemyDir3D.x);
                enemyDir.Normalize();
                Vector2 indPosition = enemyDir * _screenLength;

                if(indPosition.x > _maxScreenX)
                {
                    indPosition.y *= _maxScreenX / indPosition.x;
                    indPosition.x *= _maxScreenX / indPosition.x;
                }
                else if(indPosition.x < -_maxScreenX)
                {
                    indPosition.y *= -_maxScreenX / indPosition.x;
                    indPosition.x *= -_maxScreenX / indPosition.x;
                }
                if(indPosition.y > _maxScreenY)
                {
                    indPosition.x *= _maxScreenY / indPosition.y;
                    indPosition.y *= _maxScreenY / indPosition.y;
                }
                else if(indPosition.y < -_maxScreenY)
                {
                    indPosition.x *= -_maxScreenY / indPosition.y;
                    indPosition.y *= -_maxScreenY / indPosition.y;
                }
                float angle = Mathf.Atan2(indPosition.y, indPosition.x) * Mathf.Rad2Deg;
                indPosition.x += _inputManager.ScreenCorrect.x;
                indPosition.y += _inputManager.ScreenCorrect.y;
                Debug.Log(indPosition);

                _inputManager.TargetEnemies[i].OffScreenIndicator.transform.position = indPosition;
                _inputManager.TargetEnemies[i].OffScreenIndicator.transform.localRotation = Quaternion.Euler(0,0,angle);

            }
            //Turn off indicator if enemy becomes visible
            else if(_inputManager.TargetEnemies[i].IsVisible && _inputManager.TargetEnemies[i].OffScreenIndicator != null && _inputManager.TargetEnemies[i].OffScreenIndicator.activeSelf)
            {
                _inputManager.TargetEnemies[i].OffScreenIndicator.SetActive(false);
            }
        }
    }*/

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(_movePosition, 0.5f);
    }

    void DynamicZoom ()
    {
        if(_farthestTarget != null)
        _offset = Vector3.Lerp(_minZoom, _maxZoom, (_player.transform.position - _farthestTarget.transform.position).sqrMagnitude / Mathf.Pow(_inputManager.MaxCombatRange,2) - 0.1f);
    }

    public void CameraShake (Vector3 dir)
    {
        gameObject.transform.Translate(dir * _cameraShakeAmount);
    }
}
