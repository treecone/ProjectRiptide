using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SoundManager.instance.PlaySound("CannonFire");
        }
        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            SoundManager.instance.PlaySound("TripleShot");
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SoundManager.instance.PlaySound("Splash");
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SoundManager.instance.PlaySound("Cannon3");
        }
    }
}
