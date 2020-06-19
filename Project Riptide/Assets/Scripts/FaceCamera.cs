using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    [SerializeField]
    private Camera _camera;
    // Start is called before the first frame update
    void Start()
    {
        if(_camera == null)
        {
            _camera = Camera.main;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.forward = _camera.transform.forward;
    }
}
