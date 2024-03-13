using FrameWork.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Panel_SimSimi : PanelBase
{
    private int chatCnt = 100;
    private Queue<GameObject> chatbotQueue = new Queue<GameObject>();

    private SimSimi simsimi = null;

    private TMP_Text txtmp_Title;

    private GameObject go_Content;
    private GameObject item_SimSimiPlayer;
    private GameObject item_SimSimiBot;
    
    private TMP_InputField input_SimSimi;
    private Button btn_Send;
    private Button btn_Back;
    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        txtmp_Title = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Title), new MasterLocalData("simsim_name"));

        go_Content = GetChildGObject(nameof(go_Content));
        item_SimSimiPlayer = Single.Resources.Load<GameObject>(Cons.Path_Prefab_Item + nameof(item_SimSimiPlayer));
        item_SimSimiBot = Single.Resources.Load<GameObject>(Cons.Path_Prefab_Item + nameof(item_SimSimiBot));

        input_SimSimi = GetUI_TMPInputField(nameof(input_SimSimi), submitAction: OnSubmit_SendSimSimi, masterLocalData: new MasterLocalData("simsim_request_input"));
        btn_Send = GetUI_Button(nameof(btn_Send), OnClick_Send);
        btn_Back = GetUI_Button(nameof(btn_Back), Back);

        FindSimSimi();
    }
    private void FindSimSimi()
    {
        GameObject go_SimSimi = GameObject.Find("go_SimSimi");
        if (go_SimSimi)
        {
            simsimi = go_SimSimi.GetComponent<SimSimi>();
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        Focus();
        MyPlayer.instance.EnableInput(false);
        simsimi.SetOutline(false);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        simsimi?.ChangeSimSimiState(eSimSimiState.isexciting, false);
        MyPlayer.instance.EnableInput(true);
        simsimi.SetOutline(true);
    }

    /// <summary>
    /// 인풋필드 포커싱
    /// </summary>
    private void Focus()
    {
#if UNITY_EDITOR
        input_SimSimi.ActivateInputField();
#endif
    }

    /// <summary>
    /// 엔터 입력, 심심이 메세지 전달
    /// </summary>
    private void OnClick_Send()
    {
        OnSubmit_SendSimSimi(input_SimSimi.text);
    }

    /// <summary>
    /// 심심이에게 메세지 전달
    /// </summary>
    /// <param name="utext"></param>
    private void OnSubmit_SendSimSimi(string utext)
    {

        if (string.IsNullOrEmpty(utext) || utext.Trim().Length == 0 || utext.Length == 0)
        {
            return;
        }

        //챗 플레이어 생성
        ChatPlayer(utext);

        //챗봇 생성
        ChatBot(utext);

        input_SimSimi.text = string.Empty;

        Focus();
    }


    /// <summary>
    /// 챗플레이어 생성
    /// </summary>
    /// <param name="utext"></param>
    private void ChatPlayer(string utext)
    {
        GameObject player = Instantiate(item_SimSimiPlayer, go_Content.transform);
        ChatCnt();
        player.GetComponentInChildren<TMP_Text>().text = utext;
        Util.RefreshLayout(gameObject, nameof(go_Content));

    }

    /// <summary>
    /// 채팅 개수 조절
    /// </summary>
    private void ChatCnt()
    {
        if (go_Content.transform.childCount > chatCnt)
        {
            Destroy(go_Content.transform.GetChild(0).gameObject);
        }
    }

    /// <summary>
    /// 챗봇 생성
    /// </summary>
    /// <param name="utext"></param>
    private void ChatBot(string utext)
    {
        Util.ProcessQueue("simsimi"
            , () =>
            {
                simsimi?.ChangeSimSimiState(eSimSimiState.isexciting, true);

                GameObject bot = Instantiate(item_SimSimiBot, go_Content.transform);
                ChatCnt();
                bot.GetComponentInChildren<TMP_Text>().text = ". . ."; //더미문구
                chatbotQueue.Enqueue(bot);

                //심심이대화 리퀘스트
                Single.Web.SendSimSimi(utext, ResSimSimi, ErrorSimSImi);

                Util.RefreshLayout(gameObject, nameof(go_Content));
            }
            , 0.5f);
    }



    /// <summary>
    /// 심심이대화 리스판스
    /// </summary>
    /// <param name="resSimSimi"></param>
    private void ResSimSimi(ResSimSimi resSimSimi)
    {
        string atext = string.Empty;
        switch ((eSimSimiStatus)resSimSimi.status)
        {
            case eSimSimiStatus.OK:
                    atext = resSimSimi.atext;
                break;
            //case eSimSimiStatus.ParameterRequired:
            //    break;
            case eSimSimiStatus.DoNotUnderstand:
                atext = "??????????????????";
                break;
            //case eSimSimiStatus.Unauthorized:
            //    break;
            case eSimSimiStatus.LimitExceeded:
                atext = "질문 사용한도 초과입니다.";
                break;
            //case eSimSimiStatus.Servererror:
            //    break;
            default:
                break;
        }
        ChatBotResult(atext);


    }

    /// <summary>
    /// 챗봇 결과
    /// </summary>
    /// <param name="atext"></param>
    private void ChatBotResult(string atext)
    {
        //Debug.Log("success : " + resSimSimi.atext);
        simsimi?.ChangeSimSimiState(eSimSimiState.isexciting, false);

        if (chatbotQueue.Count > 0)
        {
            GameObject go = chatbotQueue.Dequeue();
            go.GetComponentInChildren<TMP_Text>().text = atext;
            Util.RefreshLayout(gameObject, nameof(go_Content));
        }
    }

    /// <summary>
    /// 심심이대화 에러
    /// </summary>
    /// <param name="errorSimSimi"></param>
    private void ErrorSimSImi(ErrorSimSimi errorSimSimi)
    {
        ChatBotResult("Error Error Error Error Error");
    }

    public enum eSimSimiStatus
    {
        OK = 200,
        ParameterRequired = 227,
        DoNotUnderstand = 228,
        Unauthorized = 403,
        LimitExceeded = 429,
        Servererror = 500,
    }
}
