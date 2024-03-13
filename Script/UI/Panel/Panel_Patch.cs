/*
 * 
 *			PanelProgress_Patching
 * 
 */
using UnityEngine;
using UnityEngine.UI;
using FrameWork.UI;

public class Panel_Patch : Panel_Progress
{
    [HideInInspector] public GameObject go_Anchor1 = null;
    [HideInInspector] public GameObject go_Anchor2 = null;
    [HideInInspector] public Button btn_click = null;
    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        btn_click = GetUI<Button>("btn_click");
        btn_click.onClick.AddListener(() =>
        {
            SetAnchor(0);
            //((Scene_Patch)(SceneLogic.instance)).StartPatch();
        });
        go_Anchor1 = GetChildGObject("go_Anchor1");
        go_Anchor2 = GetChildGObject("go_Anchor2");
        SetAnchor(1);

    }


    public void SetAnchor(int idx)
    {
        if (idx == 0)
        {
            go_Anchor1.SetActive(false);
            go_Anchor2.SetActive(false);
        }
        else if (idx == 1)
        {
            go_Anchor1.SetActive(true);
            go_Anchor2.SetActive(false);
        }
        else
        {
            go_Anchor1.SetActive(false);
            go_Anchor2.SetActive(true);
        }
    }
}
