using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Scrollbar))]
public class TutorialScrollChanger : MonoBehaviour
{
    private Scrollbar scrollbar;

    private void Awake()
    {
        scrollbar = GetComponent<Scrollbar>();
    }

    public void SetZero()
    {
        scrollbar.value = 0;
    }

    public void SetOne()
    {
        scrollbar.value = 1;
    }
}
