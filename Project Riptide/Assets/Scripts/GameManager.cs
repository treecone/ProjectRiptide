using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Start()
    {
        Screen.orientation = ScreenOrientation.Landscape;
        DontDestroyOnLoad(gameObject);
        Application.targetFrameRate = 60;
    }
}
