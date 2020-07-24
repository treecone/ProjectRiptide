using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPiece : MonoBehaviour
{
    private Map _map;

    // Start is called before the first frame update
    void Start()
    {
        _map = GameObject.FindGameObjectWithTag("GameManager").GetComponent<Map>();
        GetComponentInChildren<Hitbox>().OnTrigger += TakeMapPiece;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void TakeMapPiece(GameObject other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Map Exposed");
            _map.ExposeCurrentChunk();
            Destroy(gameObject);
        }
    }
}
