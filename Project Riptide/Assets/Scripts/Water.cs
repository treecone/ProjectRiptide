using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{   
    [SerializeField]
    private Vector3 waterOffset;
    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = player.transform.position + waterOffset;
    }
}
