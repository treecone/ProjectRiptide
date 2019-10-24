﻿using System.Collections;
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
    public Transform player;
    public Vector3 offset;

    public float smoothSpeed = 10f;

    void Start ()
    {
        if(player == null)
        {
            player = GameObject.FindWithTag("Player").transform;
            Debug.LogWarning("No player was assigned to the camera. Default: Player Tag. ");
        }
    }

    void FixedUpdate()
    {
        //position the camera needs to be in
        Vector3 smoothingPosition = player.position + offset;
        //position the camera is in to get to the needed position, takes time to reach that point
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, smoothingPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        transform.LookAt(player);
    }
}
