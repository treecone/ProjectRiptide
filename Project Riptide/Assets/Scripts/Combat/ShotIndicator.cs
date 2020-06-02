using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShotIndicator : MonoBehaviour
{
    private Image _image;

    private void Start()
    {
        _image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        //Decrease alpha value of indicator
        _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, _image.color.a - Time.deltaTime);
        //When alpha is gone, destory the indicator
        if(_image.color.a <= 0)
        {
            Destroy(gameObject);
        }
    }
}
