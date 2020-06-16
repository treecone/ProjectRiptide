using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DentedPixel;

public class UIAnimMethods : MonoBehaviour
{
    public void Start()
    {
        LeanTween.cancelAll();
        LeanTween.reset();
    }

    public void SlideLeft(GameObject gObj)
    {
        LeanTween.moveLocalX(gObj, 0, 1f);
        Debug.Log("SlideL");
    }

    public void SlideRight(GameObject gObj)
    {
        LeanTween.moveLocalX(gObj, -1050f, 1f);
        LeanTween.update();
        Debug.Log("SlideR");
    }

}
