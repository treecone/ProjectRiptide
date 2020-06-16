using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UIAnimMethods : MonoBehaviour
{
    //SLIDING TITLE IN AND OUT
    public void SlideTLeft(GameObject gObj)
    {
        gObj.transform.DOLocalMoveX(-1505f, .7f).SetUpdate(true);
    }
    public void SlideTRight(GameObject gObj)
    {
        gObj.transform.DOLocalMoveX(0, .65f).SetUpdate(true);
    }

    //SLIDING BASE MATERIALS IN AND OUT
    public void SlideLeft(GameObject gObj)
    {
        gObj.transform.DOLocalMoveX(-1505f, .65f).SetUpdate(true);
    }
    public void SlideRight(GameObject gObj)
    {
        gObj.transform.DOLocalMoveX(0, .7f).SetUpdate(true);
    }

    //SLIDING CLOSE BUTTON IN AND OUT
    public void SlideCLeft(GameObject gObj)
    {
        gObj.transform.DOLocalMoveX(-897f, .65f).SetUpdate(true);
    }
    public void SlideCRight(GameObject gObj)
    {
        gObj.transform.DOLocalMoveX(608f, .7f).SetUpdate(true);
    }

    //SLIDING GOLD IN AND OUT
    public void SlideGLeft(GameObject gObj)
    {
        gObj.transform.DOLocalMoveX(-1977f, .7f).SetUpdate(true);
    }
    public void SlideGRight(GameObject gObj)
    {
        gObj.transform.DOLocalMoveX(-472.2f, .65f).SetUpdate(true);
    }


}
