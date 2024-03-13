/****************************************************************************************************
 * 
 *					DebugManager.cs
 *					
 *						- 디버그 로그를 시각적/관리적 측면으로 보기 위함
 *						
 *						- 구분 색상표 > 
 *							지수의 임의로 선정하였습니다. 변경가능!
 *							scene : red
 *							ui : orange
 *							singleton : purple
 *							web : yellow
 *							data : green
 *							photon : blue_photon
 *							sound : pink_sound
 *							
 *							- 사용방법 > 
 *								사용방법1 : DebugManager.Log("어쩌구 저쩌구", eColor.red);   ->   parameter 2개, 최초 띄어쓰기를 기준으로 앞에부분에 강조됨(색상변경/볼드체)
 *								사용방법2 : DebugManager.Log("어쩌구", "저쩌구", eColor.red);   ->   parameter 3개, 강조(색상변경/볼드체)할 문구를 맨앞에, 뒤에 부분은 일반글씨
 ****************************************************************************************************/
using System.Text;
using UnityEngine;
using System.Diagnostics;

/// <summary>
/// 매니저격 컬러 바로 지정
/// </summary>
public enum eColorManager
{
    none,
    SCENE,
    UI,
    DATA,
    WEB,
    AGORA,
    SOUND,
    SINGLE,
    STACK,
    REALTIME,
    Web_Request,
    Web_Response,
    GAMEPOTSDK,
    Scheduler,
    MediaPlayer,
    Debuging,
}

public class DEBUG
{
    public static bool isDebug = true;

    public static string[] hexColorManager = new string[]{
        "" ,      //none
        "#FF0000",//sceen_red
        "#FFA500",//ui_orange
        "#00FF00",//data_green
        "#FFFF00",//web_yellow
        "#099DFD",//Agora_Blue
        "#FFC0CB",//sound_pink
        "#800080",//single_purple
        "#9B0065",//stack_purple
        "#56CBF5",//realtime_blue
        "#56CBF5",//request_blue
        "#56CBF5",//response_blue
        "#FFA500",//gamepot_orange
        "#00E676",//schaduler_green
        "#FF0000",//mediaplayer_red
        "#FF0000",//debuging_red
        };

#if !UNITY_EDITOR
    [Conditional( "DEBUG_LOG" ), Conditional( "DEVELOPMENT_BUILD" )]
#endif
    public static void LOG(string str, eColorManager eColorManager = eColorManager.none)
    {
        if (isDebug) UnityEngine.Debug.Log(GetSbManager(str, eColorManager));
    }
    
    public static void LOGWARNING(string str, eColorManager eColorManager = eColorManager.none)
    {
        if (isDebug) UnityEngine.Debug.LogWarning(GetSbManager(str, eColorManager));
    }
    
    public static void LOGERROR(string str, eColorManager eColorManager = eColorManager.none)
    {
        if (isDebug) UnityEngine.Debug.LogError(GetSbManager(str, eColorManager));
    }
    
    private static StringBuilder GetSbManager(string str, eColorManager eColorManager = eColorManager.none)
    {
        StringBuilder sb = new StringBuilder();
        if (eColorManager == eColorManager.none)
        {
            sb.Append(str);
        }
        else
        {
            string head = $"<color={hexColorManager[(int)eColorManager]}><b>[";
            string middle = eColorManager.ToString();
            string tail = "]</b></color> ";
            sb.Append(head);
            sb.Append(middle);
            sb.Append(tail);
            sb.Append(str);
        }
        return sb;
    }
}



