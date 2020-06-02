using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShotIndicator : MonoBehaviour
{
    private float MAX_DURATION = 0.5f;
    private float _currDuration = 0.0f;
    private Image _image;

    private void Start()
    {
        _image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, _image.color.a - Time.deltaTime);
        if(_image.color.a <= 0)
        {
            Destroy(gameObject);
        }
    }
}
