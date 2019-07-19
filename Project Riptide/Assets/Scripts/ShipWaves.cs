﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipWaves : MonoBehaviour
{
    public GameObject myPrefab;
    private List<GameObject> waves;
    private double timer;
    private GameObject ship;

    // Start is called before the first frame update
    void Start()
    {
        waves = new List<GameObject>();
        ship = GameObject.Find("Ship");
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 1.5)
        {
            if((ship.GetComponent<Rigidbody>().velocity.magnitude > .1 && ship.GetComponent<Rigidbody>().velocity.magnitude < 2.4f) ||
            (ship.GetComponent<Rigidbody>().velocity.magnitude > 2.6f && ship.GetComponent<Rigidbody>().velocity.magnitude < 4.9f)){
                timer = 0;
                waves.Add(Instantiate(myPrefab, new Vector3(0, 0, 0), Quaternion.Euler(90, 0, 0)));
                waves[waves.Count - 1].transform.position = GameObject.Find("Ship").transform.position - GameObject.Find("Ship").transform.TransformDirection(new Vector3(0, 0, 1)) + new Vector3(0, .3f, 0);
                if (waves.Count > 1)
                {
                    Destroy(waves[0]);
                    waves.RemoveAt(0);
                }
            }
            else
            {
                foreach (GameObject w in waves)
                {
                    Destroy(w);
                    waves = new List<GameObject>();
                }
            }
        }
        foreach(GameObject w in waves)
        {
            waves[waves.Count - 1].transform.position = GameObject.Find("Ship").transform.position - GameObject.Find("Ship").transform.TransformDirection(new Vector3(0, 0, 1)) + new Vector3(0, .3f, 0);
            if(ship.GetComponent<Rigidbody>().velocity.magnitude >2.5f)
            {
                w.transform.localScale = new Vector3(w.transform.localScale.x * 1.005f, w.transform.localScale.y * 1.005f, w.transform.localScale.z);
            }
            else if(ship.GetComponent<Rigidbody>().velocity.magnitude > 0f)
            {
                w.transform.localScale = new Vector3(w.transform.localScale.x * 1.001f, w.transform.localScale.y * 1.001f, w.transform.localScale.z);
            }

        }
    }
}
