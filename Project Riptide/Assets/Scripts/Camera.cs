using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Mira Antolovich
/// 3/7/2019
/// Camera movement
/// </summary>
public class Camera : MonoBehaviour
{
    //fields
    public GameObject player;
    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - player.transform.position;
    }

    //called once per frame after update
    void LateUpdate()
    {
        transform.position = player.transform.position + offset;
    }
}
