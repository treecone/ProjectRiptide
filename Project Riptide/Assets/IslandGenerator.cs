using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandGenerator : MonoBehaviour
{
    [System.Serializable]
    private class DecoObject
    {
        public GameObject gameObject;
        public bool rotatesToGround;
        public float verticalOffset;
    }
    private Mesh _mesh;
    [SerializeField]
    private List<DecoObject> objectsToAdd;
    [SerializeField]
    private GameObject _water;
    // Start is called before the first frame update
    void Start()
    {
        _mesh = gameObject.GetComponent<MeshFilter>().mesh;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            for(int i = 0; i < 100; i++)
            {
                RandomTriPoint();
            }
            
        }
    }

    private Vector3 RandomTriPoint()
    {
        int numTris = _mesh.triangles.Length / 3;
        Vector3 result = new Vector3(0, _water.transform.position.y - 1, 0);
        Vector3 normal = Vector3.zero;
        while(result.y <= _water.transform.position.y)
        {
            int randIndex = Random.Range(0, numTris);
            Vector3 v1 = _mesh.vertices[_mesh.triangles[randIndex * 3]];
            Vector3 v2 = _mesh.vertices[_mesh.triangles[randIndex * 3 + 1]];
            Vector3 v3 = _mesh.vertices[_mesh.triangles[randIndex * 3 + 2]];

            Vector3 b1 = v2 - v1;
            Vector3 b2 = v3 - v1;
            normal = Vector3.Cross(b1, b2).normalized;
            float r1 = Random.Range(0.0f, 1.0f);
            float r2 = Random.Range(0.0f, 1.0f);
            if (r1 + r2 > 1.0f)
            {
                r1 = 1.0f - r1;
                r2 = 1.0f - r2;
            }

            result = v1 + r1 * b1 + r2 * b2;
            result = new Vector3(result.x * transform.localScale.x, result.y * transform.localScale.y, result.z * transform.localScale.z);
            result += transform.position;
        }
        DecoObject obj = objectsToAdd[Random.Range(0, objectsToAdd.Count)];
        if(!obj.rotatesToGround)
        {
            normal = Vector3.up;
        }
        GameObject newObject = Instantiate(obj.gameObject, result, Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0));
        newObject.transform.up = normal;
        newObject.transform.position += normal * obj.verticalOffset;
        return result;
    }
}
