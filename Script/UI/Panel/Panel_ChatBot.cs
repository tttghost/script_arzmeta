using FrameWork.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Newtonsoft.Json;
using UnityEngine.Networking;
using System.Linq;

public class Panel_ChatBot : PanelBase
{
    #region 변수 및 이넘
    private GameObject go_ChatlBot;
    private GameObject go_ChatBotView;
    private GameObject go_ChatBotContent;
    private GameObject go_ChatBot;
    private GameObject go_Player;
    private GameObject chatBotLog;
    private GameObject chatPlayerLog;
    public GameObject chatBotLogPrefab;
    public GameObject chatBotPlayerLogPrefab;

    private TMP_InputField input_ChatBotInput;

    private TMP_Text txtmp_ChatBotName;

    private Button btn_enter;
    private Button btn_ChatBotBack;

    private Scrollbar scrollbar_ChatBot;

    private List<GameObject> chatList = new List<GameObject>(); //챗봇 대화 리스트
    List<string> emptyReplaceChat = new List<string>() { "나에게 물어볼 것 없어?", "오늘 날씨가 좋네", "다른 질문은 없어?", "활기찬 하루를 보내자", "띠용!" };

    [HideInInspector] public string chatBotUrl = "http://35.202.2.242:5000/send";/*"https://asia-east1-hancomapi.cloudfunctions.net/hancom-API/send";*/
    enum eType
    {
        ChatBot,
        Player,
    }
    #endregion

    #region 초기화 
    protected override void SetMemberUI()
    {
        base.SetMemberUI();
        btn_ChatBotBack = GetUI_Button(nameof(btn_ChatBotBack), Back); // 버튼 누를 시 챗봇 종료
        btn_enter = GetUI_Button(nameof(btn_enter), Send);
        go_ChatlBot = GetChildGObject(nameof(go_ChatlBot));
        go_ChatBotView = GetChildGObject(nameof(go_ChatBotView));
        go_ChatBotContent = GetChildGObject(nameof(go_ChatBotContent));
        scrollbar_ChatBot = GetUI<Scrollbar>(nameof(scrollbar_ChatBot));
        input_ChatBotInput = GetUI_TMPInputField(nameof(input_ChatBotInput));
        txtmp_ChatBotName = GetUI_TxtmpMasterLocalizing(nameof(txtmp_ChatBotName), "AI 비비안");
        input_ChatBotInput.onSubmit.AddListener(delegate { SendDone(input_ChatBotInput); });
        gameObject.SetActive(false);
    }

    protected override void Start()
    {
        base.Start();
    }
    private void OnDestroy()
    {
        chatList.Clear();
        StopAllCoroutines();
    }
    private void OnEnable()
    {
        CreateChatLog(eType.ChatBot, chatBotLog, "안녕! 나는 챗봇 AI 비비안이야! 나에게 하고 싶은 말이 있으면 언제든지 말해줘");
    }
    private void OnDisable()
    {
        for (int i = 0; i < chatList.Count; i++)
        {
            Destroy(chatList[i].gameObject);
        }
        EnableInput(true);
    }

    #endregion

    #region 업데이트 
    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Return)) // 엔터 키 ( 채팅 전송 및 스크롤바 하단 위치 )
        {
            ChatSend(input_ChatBotInput.text);
            AIChatbotReq aIChatbotReq = new AIChatbotReq();
            aIChatbotReq.texts = new string[1]; //총 6개의 스토리가 들어갈 수 있지만 대화내용이 하나라면 한개만 보내도됨
            aIChatbotReq.texts[0] = input_ChatBotInput.text;
            string jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(aIChatbotReq);
            if (string.IsNullOrEmpty(input_ChatBotInput.text) || input_ChatBotInput.text.Trim().Length == 0)
            {
                return;
            }
            else
            {
                StartCoroutine(RequestChatBot(jsonStr));
            }
            ChatClear();
        }
#endif
    }

    #endregion

    #region 핵심 함수 ( 채팅 로그 생성 , Req, Res )
    /// <summary>
    /// 채팅 타입에 따른 채팅 목록 생성
    /// </summary>
    private void CreateChatLog(eType _type, GameObject _obj, string _message)
    {
        switch (_type)
        {
            case eType.ChatBot:
                {
                    _obj = Instantiate(chatBotLogPrefab, go_ChatBotContent.transform);
                }
                break;
            case eType.Player:
                {
                    _obj = Instantiate(chatBotPlayerLogPrefab, go_ChatBotContent.transform);
                }
                break;
            default:
                break;
        }
        TextMeshProUGUI text = Util.Search<TextMeshProUGUI>(_obj, "txtmp_chatLog");
        text.text = _message;
        scrollbar_ChatBot.value = 0f;
        chatList.Add(_obj);
    }

    /// <summary>
    /// AI 챗봇 답변 
    /// </summary>
    /// <returns></returns>
    public IEnumerator RequestChatBot(string json)
    {
        using (UnityWebRequest req = UnityWebRequest.Post("http://35.202.2.242:5000/send", json))
        {
            EnableInput(false);
            yield return new WaitForSeconds(0.5f);
            Exception();
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(jsonToSend);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            //req.SetRequestHeader("Authorization", "Bearer eyJhbGciOiJSUzI1NiIsImtpZCI6IjIwOWMwNTdkM2JkZDhjMDhmMmQ1NzM5Nzg4NjMyNjczZjdjNjI0MGYiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2FjY291bnRzLmdvb2dsZS5jb20iLCJhenAiOiIzMjU1NTk0MDU1OS5hcHBzLmdvb2dsZXVzZXJjb250ZW50LmNvbSIsImF1ZCI6IjMyNTU1OTQwNTU5LmFwcHMuZ29vZ2xldXNlcmNvbnRlbnQuY29tIiwic3ViIjoiMTAwODAzNzg0NDAxNTYyODIwMzYxIiwiZW1haWwiOiJmcm9udGlzaHViQGdtYWlsLmNvbSIsImVtYWlsX3ZlcmlmaWVkIjp0cnVlLCJhdF9oYXNoIjoiMlFDU0hKTG1QSll4eVB4X3lsUTBMdyIsImlhdCI6MTY2MzczOTU1NCwiZXhwIjoxNjYzNzQzMTU0fQ.OLYtrwxIF5tfkyZ9HPgii3VLwBsNNYmCp59Qsdni_XXXdWYLkRLB1nD_IfEN9Ez6GCxy7tBCQj32tQtxca9WpUB1oGZ0bZ92Fm9HynLeF7u4hnhtZAQtzjNFd-lZ4b0O6qEmF4Uok69pM-CDYeU9In1vxqYZ8rHVarDECOhm7hvFywHx1rS8KblFMBBjQNWxNKVEyQSl9REhjnuE1-wzkLtTc8XQDYp3rMAJVr2wpnpqSBEL5TogRWooSCDpzhhJOFxpvLK1GXw47c79ajBvWYtdet94MX_S3nfBAZCQ7gb-w_kiC8PBPMaItZrJcHQ8aCLMmiCrTZvVaW8zgTy2Dg");
            yield return req.SendWebRequest();
            if (req.result == UnityWebRequest.Result.ConnectionError || req.result == UnityWebRequest.Result.ProtocolError)
            {
                DEBUG.LOG("chatBotError: " + req.error, eColorManager.WEB);
            }
            else
            {

                if (string.IsNullOrEmpty(req.downloadHandler.text) || req.downloadHandler.text.Trim().Length == 0)
                {

                    System.Random r = new System.Random();
                    int index = r.Next(emptyReplaceChat.Count);
                    string randomString = emptyReplaceChat[index];

                    yield return new WaitForSeconds(0.5f);
                    Destroy(chatBotLog);
                    CreateChatLog(eType.ChatBot, chatBotLog, randomString);
                    //DEBUG.LOG("chatBotSucess empty: " + req.downloadHandler.text, eColorManager.WEB);
                }
                else
                {
                    yield return new WaitForSeconds(0.5f);
                    Destroy(chatBotLog);
                    CreateChatLog(eType.ChatBot, chatBotLog, req.downloadHandler.text);

                    //DEBUG.LOG("chatBotSucess: " + req.downloadHandler.text,eColorManager.WEB);
                }
                EnableInput(true);
            }
        }
    }

    #endregion

    #region 버튼 ( 채팅 보내기, 닫기 ) 
    /// <summary>
    /// 채팅 타입에 따라 보내기
    /// </summary>
    private void ChatSend(string message)
    {
        if (string.IsNullOrEmpty(message) || message.Trim().Length == 0)
            return;
        else
        {
            CreateChatLog(eType.Player, chatPlayerLog, message);
        }
    }
    /// <summary>
    /// 인풋 초기화 및 포커스
    /// </summary>
    private void ChatClear()
    {
        input_ChatBotInput.text = string.Empty;
#if UNITY_EDITOR
        input_ChatBotInput.ActivateInputField();
#endif
    }

    /// <summary>
    /// 안드로이드,ios 기기에서 메세지 보내는 함수
    /// </summary>
    private void SendDone(TMP_InputField input)
    {
        if (Application.isEditor || string.IsNullOrEmpty(input.text) || input.text.Trim().Length == 0)
        {
            return;
        }
        if (input.text.Length > 0) // 인풋이 0보다 크면
        {
            AIChatbotReq aIChatbotReq = new AIChatbotReq();
            aIChatbotReq.texts = new string[1]; //총 6개의 스토리가 들어갈 수 있지만 대화내용이 하나라면 한개만 보내도됨
            aIChatbotReq.texts[0] = input_ChatBotInput.text;
            string jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(aIChatbotReq);
            StartCoroutine(RequestChatBot(jsonStr));
            ChatSend(input.text); //채팅 보내기
            ChatClear();
        }
    }
    /// <summary>
    /// 챗봇한테 메세지 보내기 버튼 
    /// </summary>
    private void Send()
    {
        if (string.IsNullOrEmpty(input_ChatBotInput.text) || input_ChatBotInput.text.Trim().Length == 0)
            return;
        else
        {
            ChatSend(input_ChatBotInput.text);
            AIChatbotReq aIChatbotReq = new AIChatbotReq();
            aIChatbotReq.texts = new string[1]; //총 6개의 스토리가 들어갈 수 있지만 대화내용이 하나라면 한개만 보내도됨
            aIChatbotReq.texts[0] = input_ChatBotInput.text;
            string jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(aIChatbotReq);
            StartCoroutine(RequestChatBot(jsonStr));
            ChatClear();
        }
    }

    /// <summary>
    /// 챗봇 종료
    /// </summary>
    private void Back()
    {
        SceneLogic.instance.PopPanel();
        EnableInput(true);
    }
    #endregion

    #region 챗봇 예외 처리 
    /// <summary>
    /// 챗봇 대화 기다릴시 (···) 예외처리
    /// </summary>
    private void Exception()
    {
        chatBotLog = Instantiate(chatBotLogPrefab, go_ChatBotContent.transform);
        TextMeshProUGUI text = Util.Search<TextMeshProUGUI>(chatBotLog, "txtmp_chatLog");
        text.text = "응답을 기다리는중입니다.";
        chatList.Add(chatBotLog);
    }
    /// <summary>
    /// 인풋 활성화 / 비활성화
    /// </summary>
    private void EnableInput(bool enabled)
    {
        input_ChatBotInput.enabled = enabled;
    }
    public TMP_InputField GetChatBotInputField()
    {
        return input_ChatBotInput;
    }

    #endregion
}
