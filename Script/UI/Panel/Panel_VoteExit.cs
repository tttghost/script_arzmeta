using FrameWork.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel_VoteExit : PanelBase
{
    [HideInInspector] public Button btn_Exit;
    VoteObjManager voteObjManager;

    protected override void SetMemberUI()
    {
        base.SetMemberUI();
        voteObjManager = GameObject.Find("VoteObjManager").gameObject.GetComponent<VoteObjManager>();

        btn_Exit = GetUI_Button("btn_Exit", () => voteObjManager.ZoomOut());
    }
}
