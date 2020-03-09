using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lootable : MonoBehaviour
{
    public Item itemStored;
    public Color lightColor;
    public string itemName;
    public float waterElevation;
    private Rigidbody rb;
    public Mesh [] diffrentMeshes;

    private GameObject player;

    void Start()
    {
        //Changing Meshes
        if(itemStored.rarity == 0)
        {
            gameObject.GetComponent<MeshFilter>().mesh = diffrentMeshes[0];
            gameObject.GetComponent<MeshCollider>().sharedMesh = diffrentMeshes[0];
        }
        else if(itemStored.rarity < 3)
        {
            gameObject.GetComponent<MeshFilter>().mesh = diffrentMeshes[1];
            gameObject.GetComponent<MeshCollider>().sharedMesh = diffrentMeshes[1];
        }
        else
        {
            gameObject.GetComponent<MeshFilter>().mesh = diffrentMeshes[2];
            gameObject.GetComponent<MeshCollider>().sharedMesh = diffrentMeshes[2];
        }


        transform.GetChild(0).GetComponent<SpriteRenderer>().color = lightColor;
        itemName = itemStored.name;
        rb = gameObject.GetComponent<Rigidbody>();
        rb.AddTorque(new Vector3(Random.Range(0, 0.25f), Random.Range(0, 0.25f), Random.Range(0, 0.25f)) * 1);
    }

    void Update()
    {
        if(gameObject.transform.position.y < waterElevation)
        {
            rb.AddForce(Vector3.up * 10);
            rb.AddForce(rb.velocity * -1 * 3);
        }

        if (Vector3.Distance(this.transform.position, player.transform.position) < 0.2f)
        {
            GameObject.Find("Canvas").transform.Find("InventoryWindows").GetComponent<Inventory>().AddItem("Stone", 8);
        }
    }
}
