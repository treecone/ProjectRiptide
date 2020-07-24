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
    [SerializeField]
    private Sprite _exposedImage;
    [SerializeField]
    private Sprite _highlightedHidden;
    [SerializeField]
    private Sprite _hidden;

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


    //MAP ANIMATIONS
    /// <summary>
    /// used for both exposing the chunk originally and when you leave the chunk
    /// </summary>
    /// <param name="gObj"></param>
    public void ExposeChunk(Image gObj)
    {
        gObj.sprite = _exposedImage;
        //gObj.DOColor(new Color(114, 88, 54, 255), .1f).SetUpdate(true);
        gObj.color = new Color32(173, 32, 35, 255);
    }
    public void InExposedChunk(Image gObj)
    {
        //gObj.DOColor(new Color(173, 32, 35, 255), .1f).SetUpdate(true);
        gObj.color = new Color32(173, 32, 35, 255);
    }
    public void LeaveExposedChunk(Image gObj)
    {
        //gObj.DOColor(new Color(114, 88, 54, 255), .1f).SetUpdate(true);
        gObj.color = new Color32(114, 88, 54, 255);
    }
    public void InHiddenChunk(Image gObj)
    {
        gObj.sprite = _highlightedHidden;
    }
    public void LeaveHiddenChunk(Image gObj)
    {
        gObj.sprite = _hidden;
    }

    //BUTTON ANIMATIONS
    public void ChooseButton(GameObject gObj)
    {
        gObj.transform.DOMoveX(gObj.transform.position.x - 32f, .25f).SetUpdate(true);
        gObj.GetComponent<Image>().color = Color.white;
        gObj.GetComponent<Button>().interactable = false;
    }

    /// <summary>
    /// Used to set values of buttons to reset ship, vault, or buying
    /// </summary>
    /// <param name="button"></param>
    public void ResetButton(GameObject gObj)
    {
        gObj.GetComponent<Button>().interactable = true;
        gObj.transform.DOMoveX(gObj.transform.position.x + 32f, .25f).SetUpdate(true);
        gObj.GetComponent<Image>().color = new Color32(180, 180, 180, 255);
    }



}
