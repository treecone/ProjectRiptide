using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
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
        public int weight;
        
        public DecoObject(DecoObject other)
        {
            this.gameObject = other.gameObject;
            this.rotatesToGround = other.rotatesToGround;
            this.radius = other.radius;
            this.verticalOffset = other.verticalOffset;
            this.scale = other.scale;
            this.weight = other.weight;
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
    private List<DecoObject> _urbanObjects;
    [SerializeField]
    private List<DecoObject> _environmentalObjects;
    [SerializeField]
    private float _waterHeight;

    [SerializeField]
    private int _numUrbanCenters;
    private List<Vector3> _urbanCenters;

    [SerializeField]
    private int _numEnvironmentalCenters;
    private List<Vector3> _environmentalCenters;

    [SerializeField]
    private int _numDecoObjects;
    [Header("Island generation tools - check box to use")]
    [SerializeField]
    private bool _setup;
    [SerializeField]
    private bool _generate;
    [SerializeField]
    private bool _clear;
    
    private List<DecoObject> _decoObjects;
    private List<GameObject> _clones;
    private int _totalUrbanWeight;
    private int _totalEnviromentalWeight;
 
    // Start is called before the first frame update
    void Start()
    {
        _clones = new List<GameObject>();
        _decoObjects = new List<DecoObject>();
        _mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;

          
    }

    // Update is called once per frame
    void Update()
    {
        if (_setup)
        {
            _setup = false;
            Setup();
        }
        if(_generate)
        {
            _totalUrbanWeight = 0;
            //Calculate total weights of urban and enviroment
            for (int i = 0; i < _urbanObjects.Count; i++)
            {
                _totalUrbanWeight += _urbanObjects[i].weight;
            }
            _totalEnviromentalWeight = 0;
            for (int i = 0; i < _environmentalObjects.Count; i++)
            {
                _totalEnviromentalWeight += _environmentalObjects[i].weight;
            }

            _mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
            _generate = false;
            for(int i = 0; i < _numDecoObjects; i++)
            {
                CreateDecoObject();
            }
            
        }
        if(_clear)
        {
            _clear = false;
            ClearIsland();
        }
    }

    private void Setup()
    {
        _waterHeight = 0.9f;
        _urbanCenters = new List<Vector3>();
        _environmentalCenters = new List<Vector3>();
        for (int i = 0; i < _numUrbanCenters; i++)
        {
            Vector3 randPoint = new Vector3(0, _waterHeight - 1, 0);
            while (randPoint.y <= _waterHeight)
            {
                randPoint = _mesh.vertices[Random.Range(0, _mesh.vertices.Length)];
                randPoint = new Vector3(randPoint.x * transform.localScale.x, randPoint.y * transform.localScale.y, randPoint.z * transform.localScale.z);
                randPoint += transform.position;
            }

            _urbanCenters.Add(randPoint);
        }
        for (int i = 0; i < _numEnvironmentalCenters; i++)
        {
            Vector3 randPoint = new Vector3(0, _waterHeight - 1, 0);
            while (randPoint.y <= _waterHeight)
            {
                randPoint = _mesh.vertices[Random.Range(0, _mesh.vertices.Length)];
                randPoint = new Vector3(randPoint.x * transform.localScale.x, randPoint.y * transform.localScale.y, randPoint.z * transform.localScale.z);
                randPoint += transform.position;
            }
            _environmentalCenters.Add(randPoint);
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
            result = new Vector3(0, _waterHeight - 1, 0);
            normal = Vector3.zero;
            while (result.y <= _waterHeight)
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
                randomObj = new DecoObject(_urbanObjects[GetWeightedRandom(_urbanObjects, _totalUrbanWeight)]);
            }
            else
            {
                randomObj = new DecoObject(_environmentalObjects[GetWeightedRandom(_environmentalObjects, _totalEnviromentalWeight)]);
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
        //}
        GameObject clone = Instantiate(obj.gameObject);
        clone.transform.position = result;
        clone.transform.rotation = Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0);
        clone.transform.localScale = Vector3.one * obj.scale;
        clone.transform.up = normal;
        clone.transform.position += normal * obj.verticalOffset;
        
        clone.transform.SetParent(transform);
        _clones.Add(clone);
        /*for (float i = 0; i < Mathf.PI * 2; i += Mathf.PI / 8f)
        {
            Debug.DrawLine(result + new Vector3(Mathf.Cos(i), 0, Mathf.Sin(i)) * obj.radius,
                           result + new Vector3(Mathf.Cos(i + Mathf.PI / 8f), 0, Mathf.Sin(i + Mathf.PI / 8f)) * obj.radius,
                           Color.cyan, 100f);
        }*/
        _decoObjects.Add(obj);
        
    }

    private void ClearIsland()
    {
        for(int i = 0; i < _clones.Count; i++)
        {
            DestroyImmediate(_clones[i]);
        }
        _decoObjects.Clear();
        _clones.Clear();
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

    private int GetWeightedRandom(List<DecoObject> objs, int maxTotalWeight)
    {
        int weightSum = maxTotalWeight;

        for(int i = 0; i < objs.Count; i++)
        {
            if (Random.Range(0, weightSum) < objs[i].weight)
            {
                return i;
            }

            weightSum -= objs[i].weight;
        }

        return 0;
    }
}
