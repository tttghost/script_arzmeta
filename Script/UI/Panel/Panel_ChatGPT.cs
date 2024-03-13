using FrameWork.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Unity.Linq;

public class Panel_ChatGPT : PanelBase
{
    private int chatCnt = 100;
    private Queue<GameObject> chatbotQueue = new Queue<GameObject>();

    private ChatGPT chatGPT = null;

    //private TMP_Text txtmp_Title;

    private GameObject go_Content;
    private GameObject item_SimSimiPlayer;
    private GameObject item_SimSimiBot;

    private TMP_InputField input_SimSimi;
    private Button btn_Send;
    private Button btn_Back;
    private Button btn_Reset;
    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        //txtmp_Title = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Title), new MasterLocalData("simsim_name"));

        go_Content = GetChildGObject(nameof(go_Content));
        item_SimSimiPlayer = Single.Resources.Load<GameObject>(Cons.Path_Prefab_Item + nameof(item_SimSimiPlayer));
        item_SimSimiBot = Single.Resources.Load<GameObject>(Cons.Path_Prefab_Item + nameof(item_SimSimiBot));

        input_SimSimi = GetUI_TMPInputField(nameof(input_SimSimi), submitAction: OnSubmit_SendSimSimi, masterLocalData: new MasterLocalData("simsim_request_input"));
        btn_Send = GetUI_Button(nameof(btn_Send), OnClick_Send);
        btn_Back = GetUI_Button(nameof(btn_Back), Back);
        btn_Reset = GetUI_Button(nameof(btn_Reset), OnClick_Reset);

        FindChatGPT();
    }

    private void OnClick_Reset()
    {
        LocalPlayerData.currentMessage.Clear();
        chatbotQueue.Clear();
        go_Content.Children().Destroy();
    }

    private void FindChatGPT()
    {
        GameObject go_ChatGPT = GameObject.Find(nameof(go_ChatGPT));
        if (go_ChatGPT)
        {
            chatGPT = go_ChatGPT.GetComponent<ChatGPT>();
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        Focus();
        MyPlayer.instance.EnableInput(false);
        chatGPT.SetOutline(false);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        chatGPT?.ChangeSimSimiState(eSimSimiState.isexciting, false);
        MyPlayer.instance.EnableInput(true);
        chatGPT.SetOutline(true);
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
    /// <param name="message"></param>
    private void ChatBot(string message)
    {
        Util.ProcessQueue("chatgpt"
            , () =>
            {
                chatGPT?.ChangeSimSimiState(eSimSimiState.isexciting, true);

                GameObject bot = Instantiate(item_SimSimiBot, go_Content.transform);
                ChatCnt();
                bot.GetComponentInChildren<TMP_Text>().text = ". . ."; //더미문구
                chatbotQueue.Enqueue(bot);

                //GPT대화 리퀘스트
                Single.Web.SendChatGPT(message, ResChatGPT, ErrorSimSImi);

                Util.RefreshLayout(gameObject, nameof(go_Content));
            }
            , 0.5f);
    }



    /// <summary>
    /// 심심이대화 리스판스
    /// </summary>
    /// <param name="resChatGPT"></param>
    private void ResChatGPT(ResChatGPT resChatGPT)
    {
        string content = string.Empty;
        if (resChatGPT.@object == "chat.completion")
        {
            content = resChatGPT.choices.First().message.content;
            LocalPlayerData.currentMessage.Add(new Message("assistant", content));
        }


        //var jObject = JsonConvert.DeserializeObject(resChatGPT) as JObject;

        //var dic = jObject.ToObject<Dictionary<string, object>>();

        //string key_object = "object";
        //if (dic.ContainsKey(key_object))
        //{
        //    if(dic[key_object] as string == "chat.completion")
        //    {
        //        string key_choices = "choices";
        //        if (dic.ContainsKey(key_choices))
        //        {
        //            var choices = dic[key_choices] as Choice[];
        //            content = choices.First().message.content;
        //            LocalPlayerData.currentMessage.Add(new Message("assistant", content));
        //        }
        //    }
        //}

        ChatBotResult(content);


    }

    /// <summary>
    /// 챗봇 결과
    /// </summary>
    /// <param name="message"></param>
    private void ChatBotResult(string message)
    {
        chatGPT?.ChangeSimSimiState(eSimSimiState.isexciting, false);

        if (chatbotQueue.Count > 0)
        {
            GameObject go = chatbotQueue.Dequeue();
            go.GetComponentInChildren<TMP_Text>().text = message;
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
