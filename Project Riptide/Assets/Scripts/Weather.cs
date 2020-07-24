using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weather : MonoBehaviour
{
    // This is from 0-1
    [SerializeField]
    private int cloudAmount;
    [SerializeField]
    private int rainAmount;
    [SerializeField]
    private int windAmount;

    public void changeWeather (int wantedAmount, int lerpAmount)
    {
        cloudAmount = (int)Mathf.Lerp(cloudAmount, wantedAmount, lerpAmount);
    }

    public void UpdateWeather ()
    {
        ParticleSystem rain = transform.Find("Rain").GetComponent<ParticleSystem>();
        rain.emissionRate = cloudAmount;
    }
}
