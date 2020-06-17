using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
/// <summary>
/// Left/Right -> way it goes out
/// F (faster), S (Slower)
/// </summary>
public class UIAnimMethods : MonoBehaviour
{
    //Fast Transition
    public void SlideLeftF(GameObject gObj)
    {
        gObj.transform.DOLocalMoveX(gObj.transform.localPosition.x - 1505f, .65f).SetUpdate(true);
    }
    public void SlideRightF(GameObject gObj)
    {
        gObj.transform.DOLocalMoveX(gObj.transform.localPosition.x + 1505f, .65f).SetUpdate(true);
    }

    //Normal Transition
    public void SlideLeft(GameObject gObj)
    {
        gObj.transform.DOLocalMoveX(gObj.transform.localPosition.x - 1505f, .7f).SetUpdate(true);
    }
    public void SlideRight(GameObject gObj)
    {
        gObj.transform.DOLocalMoveX(gObj.transform.localPosition.x + 1505f, .7f).SetUpdate(true);
    }

    //Slow Transition   
    public void SlideLeftS(GameObject gObj)
    {
        gObj.transform.DOLocalMoveX(gObj.transform.localPosition.x - 1505f, .75f).SetUpdate(true);
    }
    public void SlideRightS(GameObject gObj)
    {
        gObj.transform.DOLocalMoveX(gObj.transform.localPosition.x + 1505f, .75f).SetUpdate(true);
    }

    //Slowest Transition
    public void SlideLeftVS(GameObject gObj)
    {
        gObj.transform.DOLocalMoveX(gObj.transform.localPosition.x - 1505f, .8f).SetUpdate(true);
    }
    public void SlideRightVS(GameObject gObj)
    {
        gObj.transform.DOLocalMoveX(gObj.transform.localPosition.x + 1505f, .8f).SetUpdate(true);
    }

}
