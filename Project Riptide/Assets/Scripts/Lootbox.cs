using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lootbox : MonoBehaviour
{
    [SerializeField]
    private List<Mesh> _meshes;
    [SerializeField]
    private Material _boxMaterial;

    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;
    // Start is called before the first frame update
    void Start()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _meshRenderer = GetComponent<MeshRenderer>();
        SetShape();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetShape()
    {
        _meshRenderer.material = _boxMaterial;
        _meshFilter.mesh = _meshes[Random.Range(0, _meshes.Count)];
    }
}
