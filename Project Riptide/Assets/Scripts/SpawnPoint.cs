using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;

    public GameObject EnemyPrefab
    {
        get
        {
            if(_enemyPrefab != null)
            {
                return _enemyPrefab;
            }
            else
            {
                Debug.LogError("Spawn point not set to an enemy.");
                return null;
            }
        }
    }

    /// <summary>
    /// On gizmos draw the enemy's mesh where the enemy will spawn
    /// </summary>
    private void OnDrawGizmos()
    {
        if (_enemyPrefab != null)
        {
            Gizmos.color = new Color32(0, 200, 0, 100);
            foreach (SkinnedMeshRenderer meshRenderer in _enemyPrefab.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                Gizmos.DrawMesh(meshRenderer.sharedMesh, transform.position + meshRenderer.transform.position - _enemyPrefab.transform.position, transform.rotation * meshRenderer.transform.rotation, meshRenderer.transform.lossyScale);
            }
        }
    }
}
