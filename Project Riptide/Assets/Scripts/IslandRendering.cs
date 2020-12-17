using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandRendering : MonoBehaviour
{
    private const float ISLAND_RENDER_DISTANCE = 250.0f;

    [SerializeField]
    private List<GameObject> _islands;
    private GameObject _player;
    private float _dt;

    // Start is called before the first frame update
    void Start()
    {
        Transform enviroment = transform.Find("Enviroment");
        for(int i = 0; i < enviroment.childCount; i++)
        {
            _islands.Add(enviroment.GetChild(i).gameObject);
        }
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        _dt += Time.deltaTime;

        if(_dt >= 0.25f)
        {
            UpdateIslandRender();
            _dt = 0;
        }
    }

    /// <summary>
    /// Updates islands in chunck if they should be rendered
    /// </summary>
    void UpdateIslandRender()
    {
        for (int i = 0; i < _islands.Count; i++)
        {
            //Check is island is close enough to render
            if (Vector3.SqrMagnitude(new Vector3(_player.transform.position.x, 0, _player.transform.position.z) - new Vector3(_islands[i].transform.position.x, 0, _islands[i].transform.position.z)) < ISLAND_RENDER_DISTANCE * ISLAND_RENDER_DISTANCE)
            {
                _islands[i].SetActive(true);
            }
            else
            {
                _islands[i].SetActive(false);
            }
        }
    }
}
