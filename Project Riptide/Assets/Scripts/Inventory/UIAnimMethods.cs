using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UIAnimMethods : MonoBehaviour
{
    public void Start()
    {
        LeanTween.cancelAll();
        LeanTween.reset();
    }

    public void SlideTLeft(GameObject gObj)
    {
        gObj.transform.DOLocalMoveX(-1505f, .7f).SetUpdate(true);
        Debug.Log("SlideL");
    }

    public void SlideTRight(GameObject gObj)
    {
        gObj.transform.DOLocalMoveX(0, .65f).SetUpdate(true);
        Debug.Log("SlideR");
    }

    public void SlideLeft(GameObject gObj)
    {
        gObj.transform.DOLocalMoveX(-1505f, .65f).SetUpdate(true);
        Debug.Log("SlideL");
    }

    public void SlideRight(GameObject gObj)
    {
        LeanTween.DOLocalMoveX(gObj, -1050f, 1f);
        LeanTween.update();
        Debug.Log("SlideR");
    }

    public void SlideCLeft(GameObject gObj)
    {
        gObj.transform.DOLocalMoveX(-897f, .65f).SetUpdate(true);
    }
    public void SlideCRight(GameObject gObj)
    {
        gObj.transform.DOLocalMoveX(608f, .7f).SetUpdate(true);
    }

}
