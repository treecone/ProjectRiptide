using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Mira Antolovich
/// 3/7/2019
/// Camera movement
/// </summary>
public class CameraController : MonoBehaviour
{
    //fields
    [SerializeField]
    private Transform _player;
    [SerializeField]
    private Vector3 _offset = new Vector3(30, 30 , 0);
    [SerializeField]
    private float _smoothSpeed = 0.01f;

    void Start ()
    {
        if(_player == null)
        {
            _player = GameObject.FindWithTag("Player").transform;
            Debug.LogWarning("No player was assigned to the camera. Default: Player Tag. ");
        }
    }

    void Update()
    {

    }

    public void UpdateCamera()
    {
        //position the camera needs to be in
        Vector3 smoothingPosition = _player.position + _offset;
        //position the camera is in to get to the needed position, takes time to reach that point
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, smoothingPosition, _smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        transform.LookAt(_player);
    }
}
