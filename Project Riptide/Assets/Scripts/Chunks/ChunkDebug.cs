using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ChunkDebug : MonoBehaviour
{
    [SerializeField]
    private float _waterLevel = 0.9f;
    [SerializeField]
    private Mesh _waterMesh;
    [SerializeField]
    private Material _waterMat;
    [SerializeField]
    private bool _drawWater = true;


    private void Start()
    {
       _drawWater = false;
    }

    private void OnDrawGizmos()
    {
        Vector3 scale = GetComponent<BoxCollider>().size;
        Gizmos.color = new Color32(0, 255, 0, 100);
        Gizmos.DrawWireCube(transform.position, scale);
        if (_drawWater && _waterMesh != null && _waterMat != null)
        { 
            _waterMat.SetPass(0);
            Graphics.DrawMeshNow(_waterMesh, Matrix4x4.Translate(new Vector3(transform.position.x, transform.position.y + _waterLevel, transform.position.z)) * Matrix4x4.Scale(new Vector3(10,1,10)) * Matrix4x4.Rotate(Quaternion.identity));
            //Gizmos.DrawMesh(_waterMesh, new Vector3(transform.position.x, transform.position.y + _waterLevel, transform.position.z), Quaternion.identity, new Vector3(10,10,10));
        }
    }

    /*private void DrawWater()
    {
        if (_drawWater && _waterMesh != null && _waterMat != null)
        {
            _waterMat.SetPass(0);
            Graphics.DrawMesh(_waterMesh, Matrix4x4.Translate(new Vector3(transform.position.x, transform.position.y + _waterLevel, transform.position.z)) * Matrix4x4.Scale(new Vector3(10, 1, 10)) * Matrix4x4.Rotate(Quaternion.identity), _waterMat, 1);
            //Graphics.DrawMeshNow(_waterMesh, Matrix4x4.Translate(new Vector3(transform.position.x, transform.position.y + _waterLevel, transform.position.z)) * Matrix4x4.Scale(new Vector3(10, 1, 10)) * Matrix4x4.Rotate(Quaternion.identity));
        }
    }*/
}
