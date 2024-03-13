using System;
using System.Collections;
using System.Collections.Generic;
using FrameWork.UI;
using UnityEngine;
using UnityEngine.UI;

public class Panel_OfficeUpgrade : PanelBase
{
    [SerializeField] private View_OfficeUpgrade viewOfficeUpgrade;

    private void OnValidate()
    {
        if (!viewOfficeUpgrade) viewOfficeUpgrade = GetComponentInChildren<View_OfficeUpgrade>();
    }

    protected override void Start()
    {
        base.Start();
        viewOfficeUpgrade.onClose.AddListener(Close);
    }

    public override void OpenStartAct()
    {
        base.OpenStartAct();
        ChangeView<View_OfficeUpgrade>();
    }

    public void Open()
    {
        Show(true);
        ChangeView<View_OfficeUpgrade>();
    }
    public void Close()
    {
        Show(false);
    }
    
}
