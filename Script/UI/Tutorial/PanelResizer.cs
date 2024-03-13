using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class PanelResizer : MonoBehaviour
{
    private RectTransform rectTransform;

    private RectTransform rootRect;
    
    private Vector2 rootSize;
    
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        rootRect = transform.root as RectTransform;
    }

    private void Update()
    {
        rootSize = new Vector2(rootRect.sizeDelta.x, rootRect.sizeDelta.y);
        rectTransform.sizeDelta = rootSize;
    }
}
