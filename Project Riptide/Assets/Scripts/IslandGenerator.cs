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
        public float radius;
        public float verticalOffset;
        public float scale;
        
        public DecoObject(DecoObject other)
        {
            this.gameObject = other.gameObject;
            this.rotatesToGround = other.rotatesToGround;
            this.radius = other.radius;
            this.verticalOffset = other.verticalOffset;
            this.scale = other.scale;
        }
        public bool CollidesWith(DecoObject other)
        {
            float sqrDist = (gameObject.transform.position.x - other.gameObject.transform.position.x) * (gameObject.transform.position.x - other.gameObject.transform.position.x) +
                (gameObject.transform.position.z - other.gameObject.transform.position.z) * (gameObject.transform.position.z - other.gameObject.transform.position.z);
            return (sqrDist < (radius + other.radius) * (radius + other.radius));
        }
    }
    private Mesh _mesh;
    [SerializeField]
    private List<DecoObject> urbanObjects;
    [SerializeField]
    private List<DecoObject> environmentalObjects;
    [SerializeField]
    private GameObject _water;

    [SerializeField]
    private GameObject debugSphere;
    [SerializeField]
    private GameObject debugSphere2;

    [SerializeField]
    private int _numUrbanCenters;
    private List<Vector3> _urbanCenters;

    [SerializeField]
    private int _numEnvironmentalCenters;
    private List<Vector3> _environmentalCenters;

    private List<DecoObject> _decoObjects;

    // Start is called before the first frame update
    void Start()
    {
        _decoObjects = new List<DecoObject>();
        _mesh = gameObject.GetComponent<MeshFilter>().mesh;

        _urbanCenters = new List<Vector3>();
        _environmentalCenters = new List<Vector3>();
        for(int i = 0; i < _numUrbanCenters; i++)
        {
            Vector3 randPoint = new Vector3(0, _water.transform.position.y - 1, 0);
            while(randPoint.y <= _water.transform.position.y)
            {
                randPoint = _mesh.vertices[Random.Range(0, _mesh.vertices.Length)];
                randPoint = new Vector3(randPoint.x * transform.localScale.x, randPoint.y * transform.localScale.y, randPoint.z * transform.localScale.z);
                randPoint += transform.position;
            }
            
            

            _urbanCenters.Add(randPoint);
            
            Instantiate(debugSphere, _urbanCenters[i], Quaternion.identity);
        }
        for (int i = 0; i < _numEnvironmentalCenters; i++)
        {
            Vector3 randPoint = new Vector3(0, _water.transform.position.y - 1, 0);
            while (randPoint.y <= _water.transform.position.y)
            {
                randPoint = _mesh.vertices[Random.Range(0, _mesh.vertices.Length)];
            }
            randPoint = new Vector3(randPoint.x * transform.localScale.x, randPoint.y * transform.localScale.y, randPoint.z * transform.localScale.z);
            randPoint += transform.position;
            _environmentalCenters.Add(randPoint);
            Instantiate(debugSphere2, _environmentalCenters[i], Quaternion.identity);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            for(int i = 0; i < 100; i++)
            {
                CreateDecoObject();
            }
            
        }
    }

    private void CreateDecoObject()
    {
        bool built = false;
        DecoObject obj = null;
        Vector3 result = new Vector3(0,0,0);
        Vector3 normal = new Vector3(0, 0, 0);
        //while(!built)
        //{
            int numTris = _mesh.triangles.Length / 3;
            result = new Vector3(0, _water.transform.position.y - 1, 0);
            normal = Vector3.zero;
            while (result.y <= _water.transform.position.y)
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

            float urbanness = Urbanness(result);
            float randomValue = Random.Range(0.0f, 1.0f);
            DecoObject randomObj;

            if (randomValue < urbanness)
            {
                randomObj = new DecoObject(urbanObjects[Random.Range(0, urbanObjects.Count)]);
            }
            else
            {
                randomObj = new DecoObject(environmentalObjects[Random.Range(0, environmentalObjects.Count)]);
            }

            obj = new DecoObject(randomObj);
            if (!obj.rotatesToGround)
            {
                normal = Vector3.up;
            }
            built = true;
            foreach (DecoObject existingObject in _decoObjects)
            {
                if (existingObject.CollidesWith(obj))
                {
                    built = false;
                }
            }
        Debug.Log(built);
        //}
        Instantiate(obj.gameObject);
        obj.gameObject.transform.position = result;
        obj.gameObject.transform.rotation = Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0);
        obj.gameObject.transform.localScale = Vector3.one * obj.scale;
        obj.gameObject.transform.up = normal;
        obj.gameObject.transform.position += normal * obj.verticalOffset;


        for (float i = 0; i < Mathf.PI * 2; i += Mathf.PI / 8f)
        {
            Debug.DrawLine(result + new Vector3(Mathf.Cos(i), 0, Mathf.Sin(i)) * obj.radius,
                           result + new Vector3(Mathf.Cos(i + Mathf.PI / 8f), 0, Mathf.Sin(i + Mathf.PI / 8f)) * obj.radius,
                           Color.cyan, 100f);
        }
        Debug.Log("placed object");
        _decoObjects.Add(obj);
        
    }

    private float Urbanness(Vector3 pos) {
        float urbanTotal = 0;
        float environmentalTotal = 0;
        foreach(Vector3 urbanPoint in _urbanCenters)
        {
            urbanTotal += (urbanPoint - pos).sqrMagnitude;
        }
        foreach (Vector3 environmentalPoint in _environmentalCenters)
        {
            environmentalTotal += (environmentalPoint - pos).sqrMagnitude;
        }
        //Debug.Log("" + urbanTotal + " - " + environmentalTotal + " - " + urbanTotal / (urbanTotal + environmentalTotal));
        float urbanness = urbanTotal / (urbanTotal + environmentalTotal);
        if (urbanness >= 0.5f) {
            urbanness = 0.5f + Mathf.Pow((urbanness - 0.5f) / 4, 1.0f / 3.0f);
        } else
        {
            urbanness = 0.5f - Mathf.Pow(-((urbanness - 0.5f) / 4), 1.0f / 3.0f);
        }

        
        return urbanness;
    }
}
