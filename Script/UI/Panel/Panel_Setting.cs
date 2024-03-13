using FrameWork.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class Panel_Setting : PanelBase
{
    #region 변수
    private ToggleGroup toggleGroup;
    private List<TogglePlus> toggleList = new List<TogglePlus>();
    public string forceView;
    #endregion

    protected override void SetMemberUI()
    {
        base.SetMemberUI();
        #region Button
        GetUI_Button("btn_Back", Back);
        #endregion

        #region TMP_Text
        GetUI_TxtmpMasterLocalizing("txtmp_SettingTitle", new MasterLocalData("9000"));
        GetUI_TxtmpMasterLocalizing("txtmp_System", new MasterLocalData("9004"));
        GetUI_TxtmpMasterLocalizing("txtmp_Account", new MasterLocalData("9001"));
        #endregion

        #region ToggleGroup
        toggleGroup = GetUI<ToggleGroup>("togg_Tab");
        toggleGroup.enabled = false;
        int count = toggleGroup.transform.childCount;
        for (int i = 0; i < count; i++)
        {
            TogglePlus tog = toggleGroup.transform.GetChild(i).GetComponent<TogglePlus>();
            string viewName = tog.name.Replace("togplus", "View");
            tog.SetToggleOnAction(() => { ChangeView(viewName); });
            toggleList.Add(tog);
        }
        toggleGroup.enabled = true;
        #endregion

        #region etc
        var go_PcManual = toggleList.FirstOrDefault(x => x.name == "togplus_PcManual").gameObject;
        var view_PcManual = GetView<View_PcManual>().gameObject;

#if UNITY_STANDALONE || UNITY_EDITOR
        if(view_PcManual != null) view_PcManual.SetActive(true);
        if (go_PcManual != null) go_PcManual.SetActive(true);

        GetUI_TxtmpMasterLocalizing("txtmp_PcManual", new MasterLocalData("arzphone_setting_title_shortcutkey"));
#elif UNITY_ANDROID || UNITY_IOS
        if(view_PcManual != null) view_PcManual.SetActive(false);
        if (go_PcManual != null) go_PcManual.SetActive(false);
#endif
        #endregion
    }

    #region 초기화
    protected override void OnEnable()
    {
        // default : View_System
        // 지정한 뷰로 강제로 뷰 및 토글 지정
        SetOpenStartCallback(() =>
        {
            string togName = forceView.Replace("View", "togplus");
            TogglePlus toggle = toggleList.FirstOrDefault(x => x.name == togName);
            if (toggle != null)
            {
                toggle.SetToggleIsOn(true);
            }
            forceView = Cons.View_System;
        });
    }
    #endregion
}
