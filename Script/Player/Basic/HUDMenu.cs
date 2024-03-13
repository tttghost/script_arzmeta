using UnityEngine;
using UnityEngine.UI;
using FrameWork.UI;
using System;
using TMPro;

[Serializable]
public class HUDMenu
{
	[HideInInspector] public GameObject go_QuestNew;
	[HideInInspector] public View_HUD_TopLeft viewHudTopLeft;

    Transform view_HUD_TopRight;
    Transform view_HUD_TopCenter;

    Transform go_Velocity;
    TMP_Text txtmp_Velocity;


    public HUDMenu(Panel_HUD _panel)
	{
		viewHudTopLeft = _panel.GetView<View_HUD_TopLeft>();
		viewHudTopLeft.gameObject.SetActive(viewHudTopLeft != null ? true : false);

        view_HUD_TopRight = _panel.transform.Search(nameof(view_HUD_TopRight));
        view_HUD_TopCenter = _panel.transform.Search(nameof(view_HUD_TopCenter));

        go_Velocity = _panel.transform.Search(nameof(go_Velocity));
        txtmp_Velocity = _panel.transform.Search(nameof(txtmp_Velocity)).GetComponent<TMP_Text>();
	}

    public void RoomButtonEvent()
    {
        SceneLogic.instance.PushPopup<Popup_Basic>()
            .ChainPopupData(new PopupData(POPUPICON.CHECK, BTN_TYPE.ConfirmCancel, null, new MasterLocalData("40010")))
            .ChainPopupAction(new PopupAction(RoomPopupAction));
    }
    
    public void RoomPopupAction()
    {
        LocalPlayerData.Method.roomCode = string.Empty;
        LocalPlayerData.Method.roomOwnerName = string.Empty;

        Single.RealTime.EnterRoom(RoomType.MyRoom, LocalPlayerData.MemberCode);
    }

    public void MapButtonEvent() => SceneLogic.instance.PushPanel<Panel_Map>(false);
    //public void SNSButtonEvent() => SceneLogic.instance.PushPanel<Panel_SNS>(Cons.Panel_SNS);
    public void QuestButtonEvent() => SceneLogic.instance.PushPanel<Panel_Quest>();

    public void SettingButtonEvent(bool _show = true)
    {
        Panel_Setting panel_Setting = SceneLogic.instance.GetPanel<Panel_Setting>();
        panel_Setting.ChangeView<View_Account>();
        SceneLogic.instance.PushPanel<Panel_Setting>(_show);
    }


    public void EnableTopRight(bool _show)
    {
        var canvasGroup = view_HUD_TopRight.GetComponent<CanvasGroup>();

        var alpha = _show ? 1f : 0f;

        FadeUtils.FadeCanvasGroup(canvasGroup, alpha);
    }

    public void EnableTopCenter(bool _show)
    {
        var canvasGroup = view_HUD_TopCenter.GetComponent<CanvasGroup>();

        var alpha = _show ? 1f : 0f;

        FadeUtils.FadeCanvasGroup(canvasGroup, alpha);
    }

    public void EnableVelocity(bool _show)
    {
        var canvasGroup = go_Velocity.GetComponent<CanvasGroup>();

        var alpha = _show ? 1f : 0f;

        FadeUtils.FadeCanvasGroup(canvasGroup, alpha);
    }

    public void SetVelocity(float _velocity)
	{
        txtmp_Velocity.text = Mathf.Abs(_velocity).ToString("N0") + " <size=40>KM/H</size>";
    }
}
