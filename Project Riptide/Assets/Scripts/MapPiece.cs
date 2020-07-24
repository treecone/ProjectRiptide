using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPiece : MonoBehaviour
{
    [SerializeField]
    private int x;
    [SerializeField]
    private int y;

    private Map _map;

    // Start is called before the first frame update
    void Start()
    {
        _map = GameObject.FindGameObjectWithTag("GameManager").GetComponent<Map>();
        GetComponentInChildren<Hitbox>().OnTrigger += TakeMapPiece;
        //Don't spawn map piece if piece has already been collected
        if(_map.CheckMapExposed(x, y))
        {
            Destroy(gameObject);
        }
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
