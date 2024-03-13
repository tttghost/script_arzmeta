using System;
using System.Collections;
using System.Collections.Generic;
using Lean.Touch;
using UnityEngine;

public class LeanPinchCameraResetter : MonoBehaviour
{
    public LeanPinchCamera pinch;
    private void Awake()
    {
        if (!pinch) pinch = GetComponent<LeanPinchCamera>();
    }

    private void OnDisable()
    {
        pinch.Zoom = pinch.ClampMax;
    }
}
