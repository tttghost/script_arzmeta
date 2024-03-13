namespace Gpm.Ui.Sample
{
    using FrameWork.UI;
    using Gpm.Ui;
    using TMPro;
    using UnityEngine;
    using UnityEngine.EventSystems;

    #region 데이터 클래스
    public class Item_ChatDefault : InfiniteScrollData
    {
        public string message;
        public Color32 color;
    }

    // 월드 채팅
    public class Item_C_ChatMessage : Item_ChatDefault
    {
        public string roomCode;
        public string roomName;
    }
    public class Item_S_ChatMessage : Item_C_ChatMessage
    {
        public string sendNickName;
    }

    // 1:1 채팅
    public class Item_C_ChatDM : Item_ChatDefault
    {
        public string recvNickName;
    }
    public class Item_S_ChatDM : Item_C_ChatDM
    {
        public string sendNickName;
    }

    // 시스템 채팅
    public class Item_S_ChatSystem : Item_ChatDefault
    {
    }

    // 1:1 채팅 유저 리스트
    public class Item_UserList : InfiniteScrollData
    {
        public string NickName;
        public Color32 color;
    }
    #endregion

    public class Item_Chat : InfiniteScrollItem, IPointerClickHandler
    {
        private View_Chat viewChat;
        private ChatModeController chatModeController;

        private TMP_Text txtmp_Chat;
        private RectTransform rect;

        private string chatText = string.Empty;
        private Color32 chatColor = default;

        // 메세지를 보낼 수 없는 상대일 때, 클릭 이벤트 막기 위한 bool 값
        private bool isTarget = true;
        // 로컬라이징이 필요한 메세지를 구분하기 위한 bool 값
        private bool isMasterLocalData = false;

        public override void UpdateData(InfiniteScrollData _scrollData)
        {
            base.UpdateData(_scrollData);

            Init();

            // 월드 채팅
            if (_scrollData is Item_S_ChatMessage item_S_ChatMessage)
            {
                if (string.IsNullOrEmpty(item_S_ChatMessage.sendNickName))
                {
                    isTarget = false;
                    isMasterLocalData = true;
                    chatText = item_S_ChatMessage.message;
                }
                else
                {
                    chatText = item_S_ChatMessage.sendNickName + ":" + item_S_ChatMessage.message;
                }
                chatColor = item_S_ChatMessage.color;
            }
            // 1:1 채팅
            else if (_scrollData is Item_S_ChatDM item_S_ChatDM)
            {
                // 1:1 채팅 시스템 메세지일 경우, 
                if (string.IsNullOrEmpty(item_S_ChatDM.sendNickName) && string.IsNullOrEmpty(item_S_ChatDM.recvNickName))
                {
                    isTarget = false;
                    isMasterLocalData = true;
                    chatText = item_S_ChatDM.message;
                }
                // 내가 보낸 채팅 메세지의 경우,
                else if (string.IsNullOrEmpty(item_S_ChatDM.sendNickName))
                {
                    isTarget = false;
                    chatText = $"[@{item_S_ChatDM.recvNickName}]:{item_S_ChatDM.message}";
                }
                // 내가 받은 채팅 메세지의 경우,
                else
                {
                    chatText = item_S_ChatDM.sendNickName + ":" + item_S_ChatDM.message;
                }

                chatColor = item_S_ChatDM.color;
            }
            // 시스템 채팅
            else if (_scrollData is Item_S_ChatSystem item_S_ChatSystem)
            {
                isTarget = false;
                isMasterLocalData = true;
                chatText = item_S_ChatSystem.message;
                chatColor = item_S_ChatSystem.color;
            }
            // 1:1 채팅 유저 리스트
            else if (_scrollData is Item_UserList item_UserList)
            {
                chatText = item_UserList.NickName;
                chatColor = item_UserList.color;
            }

            if (isMasterLocalData)
                Util.SetMasterLocalizing(txtmp_Chat, new MasterLocalData(chatText));
            else
                Util.SetMasterLocalizing(txtmp_Chat, chatText);
            txtmp_Chat.color = chatColor;

            // 채팅 길이에 맞게 Item 높이 조절
            SetItemHeight();

#if UNITY_EDITOR
            // inputField에 Focusing
            viewChat.GetChatInputField().ActivateInputField();
#endif
        }

        private void Init()
        {
            viewChat = SceneLogic.instance.GetPanel<Panel_HUD>().GetView<View_Chat>();
            chatModeController = viewChat.GetComponent<ChatModeController>();

            txtmp_Chat = Util.Search<TMP_Text>(gameObject, nameof(txtmp_Chat));

            rect = transform.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, 50f);
        }

        private void SetItemHeight()
        {
            float textHeight = txtmp_Chat.preferredHeight;
            Vector2 sizeDelta = new Vector2(rect.sizeDelta.x, textHeight);
            SetSize(sizeDelta);
        }

        /// <summary>
        /// 채팅 로그 있을 때, 채팅 모드에 따른 기능 추가
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerClick(PointerEventData eventData)
        {
            // 기본 모드일 때, 채팅 모드로 전환
            if (chatModeController.ChatMode == CHAT_MODE.STANDARD)
                chatModeController.ChangeChatMode();

            // 채팅 모드일 때,
            if (chatModeController.ChatMode == CHAT_MODE.CHATTING)
            {
                // 본인의 채팅 로그이거나 1:1 채팅 안내문구인 경우, return;
                if (txtmp_Chat.text.StartsWith(LocalPlayerData.NickName) || !isTarget)
                    return;
                //  1:1 채팅방으로 전환
                else
                {
                    if (viewChat.ToggleType != CHAT_TYPE.WHISPER)
                        viewChat.ToggleType = CHAT_TYPE.WHISPER;

                    // 인풋필드에 타겟 닉네임 세팅
                    TMP_InputField input = viewChat.GetChatInputField();
                    string targetNickName = txtmp_Chat.text.Split(':')[0];
                    input.text = $"@{targetNickName}:";

                    // 캐럿 끝으로 이동
                    input.selectionAnchorPosition = input.selectionFocusPosition;
                    input.MoveTextEnd(false);

#if UNITY_EDITOR
                    // inputField에 Focusing
                    input.ActivateInputField();
#endif
                }
            }
        }
    }
}

