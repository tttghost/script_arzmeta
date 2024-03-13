using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
public class CanvasScalerSetter : MonoBehaviour
{
    private Canvas canvas;
    private CanvasScaler canvasScaler;

    private void Awake()
    {
        canvasScaler = GetComponent<CanvasScaler>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void Init()
    {
        SetProperties();
        SetMatch();
    }

    private void SetProperties()
    {
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920f, 1080f);
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        //_canvasScaler.matchWidthOrHeight = match;
        canvasScaler.referencePixelsPerUnit = 100;
    }
    
    public void SetMatch()
    {
        float match = 1;
        if (Screen.width > Screen.height)
        {
            match = 1.0f - ((float) Screen.height / (float) Screen.width);
        }
        else
        {
            match = 1.0f - ((float) Screen.width / (float) Screen.height);
        }
         
        canvasScaler.matchWidthOrHeight = match;
    }
}
