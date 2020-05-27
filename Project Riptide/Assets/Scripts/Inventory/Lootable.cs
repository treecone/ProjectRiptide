using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lootable : MonoBehaviour
{
    public Item itemStored;
    public Color lightColor;
    public string itemName;
    public float waterElevation;
    public Mesh [] diffrentMeshes;

    private Rigidbody _rb;
    private GameObject _player;

    void Start()
    {
        //Changing Meshes
        if(itemStored.Rarity == 0)
        {
            gameObject.GetComponent<MeshFilter>().mesh = diffrentMeshes[0];
            gameObject.GetComponent<MeshCollider>().sharedMesh = diffrentMeshes[0];
        }
        else if(itemStored.Rarity < 3)
        {
            gameObject.GetComponent<MeshFilter>().mesh = diffrentMeshes[1];
            gameObject.GetComponent<MeshCollider>().sharedMesh = diffrentMeshes[1];
        }
        else
        {
            gameObject.GetComponent<MeshFilter>().mesh = diffrentMeshes[2];
            gameObject.GetComponent<MeshCollider>().sharedMesh = diffrentMeshes[2];
        }

        _player = GameObject.FindGameObjectWithTag("Player");
        //transform.GetChild(0).GetComponent<SpriteRenderer>().color = lightColor;
        itemName = itemStored.Name;
        _rb = gameObject.GetComponent<Rigidbody>();
        _rb.AddTorque(new Vector3(Random.Range(0, 0.25f), Random.Range(0, 0.25f), Random.Range(0, 0.25f)) * 1);
    }

    void Update()
    {
        if(gameObject.transform.position.y < waterElevation)
        {
            _rb.AddForce(Vector3.up * 30);
            _rb.AddForce(_rb.velocity * -1 * 3);
        }

        if (Vector3.Distance(this.transform.position, _player.transform.position) < 2.0f)
        {
            GameObject.Find("Canvas").transform.Find("InventoryWindows").GetComponent<Inventory>().AddItem(itemStored.Name, 8);
            Destroy(gameObject);
        }
    }
}
