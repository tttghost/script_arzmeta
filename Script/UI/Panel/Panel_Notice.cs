//using FrameWork.UI;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine.UI;

//public class Panel_Notice : PanelBase
//{
//    #region 초기화
//    protected override void SetMemberUI()
//    {
//        base.SetMemberUI();

//        #region Button
//        GetUI_Button( "btn_Close", Back );
//        GetUI_Button( "btn_Back", Back );
//        GetUI_Button( "btn_Notice", OnNotice_GamePot );
//        GetUI_Button( "btn_Event", OnEvent_GamePot );
//        #endregion

//        #region TMP_Text
//        GetUI_TxtmpMasterLocalizing( "txtmp_Notice", new MasterLocalData( "9005" ) );
//        GetUI_TxtmpMasterLocalizing( "txtmp_Event", new MasterLocalData( "9002" ) );
//        #endregion
//    }
//    #endregion

//    #region Button Callback
//    private void OnNotice_GamePot()
//	{
//        Single.WebView.OpenWebView(new WebViewData(
//            new WebDataSetting(WEBVIEWTYPE.URL, "https://noticetest.oopy.io"),
//            new WebSetting(webview_size: WEBVIEW_SIZE.WINDOW)));
////#if UNITY_EDITOR_WIN || UNITY_EDITOR_OSX || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
////#else
////        GamePotManager.ShowNotice(false);
////        //GamePotManager.ShowEvent( "notice" );
////#endif
//    }

//    private void OnEvent_GamePot()
//    {
//        Single.WebView.OpenWebView(new WebViewData(
//    new WebDataSetting(WEBVIEWTYPE.URL, "https://noticetest.oopy.io"),
//    new WebSetting(webview_size: WEBVIEW_SIZE.WINDOW)));
////#if UNITY_EDITOR_WIN || UNITY_EDITOR_OSX || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
////#else
////        GamePotManager.ShowEvent( "event" );
////#endif
//    }
//    #endregion
//}




using FrameWork.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class Panel_Notice : PanelBase
{
    #region 초기화
    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        #region Button
        GetUI_Button("btn_Close", Back);
        GetUI_Button("btn_Back", Back);
        GetUI_Button("btn_Notice", OnNotice);
        GetUI_Button("btn_Event", OnEvent);
        #endregion

        #region TMP_Text
        GetUI_TxtmpMasterLocalizing("txtmp_Notice", new MasterLocalData("9005"));
        GetUI_TxtmpMasterLocalizing("txtmp_Event", new MasterLocalData("9002"));
        #endregion
    }
    #endregion

    #region Button Callback
    private void OnNotice()
    {
        LocalPlayerData.Method.OpenNoticeOrEvent(eNoticeType.공지사항);
    }

    private void OnEvent()
    {
        LocalPlayerData.Method.OpenNoticeOrEvent(eNoticeType.이벤트);
    }
    
    #endregion
}


