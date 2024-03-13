using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ToggleBehaviour : MonoBehaviour
{
    private Toggle toggle;
    private ColorBlock colors;

    private Color normalColor;
    private Color selectedColor;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
        normalColor = toggle.colors.normalColor;
        selectedColor = toggle.colors.selectedColor;
        colors = toggle.colors;
        
        toggle.onValueChanged.AddListener(OnValueChanged);
    }

    private void OnValueChanged(bool enable)
    {
        if (enable)
        {
            colors.normalColor = selectedColor;
            colors.selectedColor = selectedColor;
            toggle.colors = colors;
        }
        else
        {
            colors.normalColor = normalColor;
            colors.selectedColor = normalColor;
            toggle.colors = colors;
        }
    }
}
