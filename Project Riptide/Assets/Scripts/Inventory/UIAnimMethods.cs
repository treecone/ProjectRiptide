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
        gObj.transform.DOLocalMoveX(gObj.transform.localPosition.x - 2700f, .55f).SetUpdate(true);
    }
    public void SlideRightF(GameObject gObj)
    {
        gObj.transform.DOLocalMoveX(gObj.transform.localPosition.x + 2700f, .55f).SetUpdate(true);
    }

    //Normal Transition
    public void SlideLeft(GameObject gObj)
    {
        gObj.transform.DOLocalMoveX(gObj.transform.localPosition.x - 2700f, .6f).SetUpdate(true);
    }
    public void SlideRight(GameObject gObj)
    {
        gObj.transform.DOLocalMoveX(gObj.transform.localPosition.x + 2700f, .6f).SetUpdate(true);
    }

    //Slow Transition   
    public void SlideLeftS(GameObject gObj)
    {
        gObj.transform.DOLocalMoveX(gObj.transform.localPosition.x - 2700f, .65f).SetUpdate(true);
    }
    public void SlideRightS(GameObject gObj)
    {
        gObj.transform.DOLocalMoveX(gObj.transform.localPosition.x + 2700f, .65f).SetUpdate(true);
    }

    //Slowest Transition
    public void SlideLeftVS(GameObject gObj)
    {
        gObj.transform.DOLocalMoveX(gObj.transform.localPosition.x - 2700f, .7f).SetUpdate(true);
    }
    public void SlideRightVS(GameObject gObj)
    {
        gObj.transform.DOLocalMoveX(gObj.transform.localPosition.x + 2700f, .7f).SetUpdate(true);
    }


    //IENUMERATORS
    IEnumerator DisablePanel(GameObject gObj)
    {
        gObj.GetComponent<Image>().DOFade(0, .7f).SetUpdate(true);
        yield return new WaitForSecondsRealtime(.65f);
        gObj.SetActive(false);
    }

    IEnumerator EnableNormalUI(GameObject gObj)
    {
        yield return new WaitForSecondsRealtime(.05f);
        gObj.SetActive(true);
    }

    /// <summary>
    /// disables panel after fading and waiting .5 seconds
    /// </summary>
    /// <param name="gObj"></param>
    public void DisablePanelAnim(GameObject gObj)
    {
        StartCoroutine(DisablePanel(gObj));
    }
    /// <summary>
    /// enables normal UI panel after .5 seconds
    /// </summary>
    /// <param name="gObj"></param>
    public void EnableNormalUIAnim(GameObject gObj)
    {
        StartCoroutine(EnableNormalUI(gObj));
    }

    public void FadePanel(Image gObj)
    {
        gObj.DOFade(0f, .5f).SetUpdate(true);
    }

    public void EnableInventoryPanel(Image gObj)
    {    
        gObj.DOFade(.5f, .5f).SetUpdate(true);
    }

    public void EnableMarketPanel(Image gObj)
    {
        gObj.DOFade(1f, .5f).SetUpdate(true);
    }

}
