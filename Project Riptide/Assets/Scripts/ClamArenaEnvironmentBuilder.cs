using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClamArenaEnvironmentBuilder : MonoBehaviour
{
    [SerializeField]
    private GameObject[] environmentObjects;

    [SerializeField]
    private float minRadius;

    [SerializeField]
    private float maxRadius;

    [SerializeField]
    private float objectCount;

    [SerializeField]
    private float scale;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < objectCount; i++)
        {
            GameObject newObj = Instantiate(environmentObjects[Random.Range(0, environmentObjects.Length)], Random.onUnitSphere * Random.Range(minRadius, maxRadius), Quaternion.identity, transform);
            newObj.transform.position -= new Vector3(0, newObj.transform.position.y, 0);
            newObj.transform.localScale *= scale;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
