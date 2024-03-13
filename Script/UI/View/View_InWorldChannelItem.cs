using System.Collections.Generic;
using FrameWork.Network;
using FrameWork.UI;
using MEC;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 월드씬 Panel_Setting 안에 있는 채널 변경 View 버튼 / 메인 서버에서만 사용함 
/// </summary>
public class View_InWorldChannelItem : View_ChannelItem
{
    public View_InWorldChannel viewInWorldChannel = null;
    //private Scene scene;

    protected override void Start()
    {
        base.Start();

        if (viewInWorldChannel.curChannel == channelName)
        {
            btn_channel.interactable = false;
        }
    }

    /// <summary>
    /// 버튼 눌렀을 때 실행할 메소드
    /// </summary>
    protected override void PopupOpen()
    {
        // 인원 수 초과했을 때 
        if (playerNumber >= maxEnterCount)
        {
            PushPopup<Popup_Basic>()
                .ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.Confirm, masterLocalDesc: new MasterLocalData("5006")));
        }
        else
        {

            // UI/UX 피드백 적용 완료
            PushPopup<Popup_Basic>()
                .ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.ConfirmCancel, masterLocalDesc: new MasterLocalData("9302")))
                .ChainPopupAction(new PopupAction(() => Util.RunCoroutine(ServerConnect())));
        }
    }


	/// <summary>
	/// Player 60이하, 채널 선택 Yes 눌렀을 때 실행 
	/// </summary>
	protected override IEnumerator<float> ServerConnect()
	{
		Single.Scene.SetDimOn();

		string sceneName = SceneManager.GetActiveScene().name;

		//Single.RealTime.SwitchConnection(
		//	new Connect(sessionType, string.Empty, serverInfo),
		//	new Connect(sessionType, sceneName, serverInfo)
		//);

        //Single.RealTime.SwitchConnection(
        //    new Connect(sessionType, serverInfo, string.Empty),
        //    new Connect(sessionType, serverInfo, sceneName, Success, Error)
        //);

        yield return Timing.WaitForOneFrame;
	}
}
