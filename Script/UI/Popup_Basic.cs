
/****************************************************************************************************
 * 
 *					Popup_Basic.cs - 타이틀, 본문, 버튼등으로 이루어진 기본 팝업 템플릿
 * 
 ****************************************************************************************************/
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using Assets._Launching.DEV.Script.Framework.Network.WebPacket;
using Assets._Launching.DEV.Script.Framework.Network.WebSocket;
using UnityEngine.Localization.Components;
using System.Collections.Generic;
using MEC;
using GamePotUnity;

namespace FrameWork.UI
{
    /// <summary>
    /// 타이틀, 본문, 버튼등으로 이루어진 기본 팝업 템플릿
    /// </summary>
    public class Popup_Basic : PopupBase
    {


        #region 변수
        protected GameObject go_Icon = null;
        protected GameObject go_Title = null;
        protected GameObject go_Desc = null;
        protected GameObject go_Btns = null;

        protected TMP_Text txtmp_Title = null;
        protected TMP_Text txtmp_Desc = null;
        protected TMP_Text txtmp_Confirm = null;
        protected TMP_Text txtmp_Cancel = null;

        protected Image img_Icon = null;

        protected bool bConfirm = true; // 어떤버튼이 눌렸는지 체크
        protected Button btn_Confirm = null; // 확인버튼
        protected Button btn_Cancel = null; // 취소버튼

        protected UnityEvent event_Confirm = new UnityEvent();
        protected UnityEvent event_Cancel = new UnityEvent();

        protected Sprite img_Check;
        protected Sprite img_Email;
        protected Sprite img_Warning;

        protected MasterLocalData defaultLocalConfirm = new MasterLocalData("common_ok");
        protected MasterLocalData defaultLocalCancel = new MasterLocalData("common_cancel");
        #endregion


        #region 초기화
        /// <summary>
        /// 멤버UI 등록
        /// </summary>
        protected override void SetMemberUI()
        {
            base.SetMemberUI();
            //GameObject
            go_Icon = GetChildGObject(nameof(go_Icon));
            go_Title = GetChildGObject(nameof(go_Title));
            go_Desc = GetChildGObject(nameof(go_Desc));
            go_Btns = GetChildGObject(nameof(go_Btns));

            //TMP_Text
            txtmp_Title = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Title));
            txtmp_Desc = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Desc));
            txtmp_Confirm = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Confirm));
            txtmp_Cancel = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Cancel));

            //Image
            img_Icon = GetUI_Img(nameof(img_Icon));
            img_Check = Resources.Load<Sprite>(Cons.Path_Image + "Icon_check");
            img_Email = Resources.Load<Sprite>(Cons.Path_Image + "Icon_email");
            img_Warning = Resources.Load<Sprite>(Cons.Path_Image + "Icon_warning");

            //Button
            btn_Confirm = GetUI_Button(nameof(btn_Confirm), OnConfirm); // 확인버튼
            btn_Confirm.gameObject.SetActive(true);
            btn_Cancel = GetUI_Button(nameof(btn_Cancel), OnCancel);   // 취소버튼
        }
        #endregion


        #region 버튼 이벤트 등록 메소드
        /// <summary>
        /// 확인(Confirm)
        /// </summary>
        protected override void OnConfirm()
        {
            bConfirm = true;
            base.OnConfirm();
        }

        /// <summary>
        /// 취소(Cancel)
        /// </summary>
        protected override void OnCancel()
        {
            bConfirm = false;
            base.OnCancel();
        }

        /// <summary>
        /// 팝업 액션
        /// </summary>
        protected override void PopupAction()
        {
            base.PopupAction();
            if (bConfirm)
            {
                event_Confirm?.Invoke();
            }
            else
            {
                event_Cancel?.Invoke();
            }
            event_Confirm.RemoveAllListeners();
            event_Cancel.RemoveAllListeners();
        }
        #endregion


        #region 이벤트 등록
        /// <summary>
        /// 체인 팝업데이터
        /// </summary>
        /// <param name="popupData"></param>
        /// <returns></returns>
        public Popup_Basic ChainPopupData(PopupData popupData)
        {
            SetPopupData(popupData);
            return this;
        }

        /// <summary>
        /// 체인 팝업액션
        /// </summary>
        /// <param name="popupAction"></param>
        /// <returns></returns>
        public Popup_Basic ChainPopupAction(PopupAction popupAction)
        {
            SetPopupAction(popupAction);
            return this;
        }

        /// <summary>
        /// 팝업 데이터 및 형태 세팅 본체 메소드
        /// </summary>
        /// <param name="popupData"></param>
        public void SetPopupData(PopupData popupData = null)
        {
            if (popupData == null) return;

            #region 아이콘
            SetUpIcon(popupData.icon);
            #endregion

            #region 원버튼 / 투버튼
            bool isActive = bConfirm = popupData.btnType == BTN_TYPE.Confirm;
            btn_Cancel.gameObject.SetActive(!isActive);

            if (txtmp_Confirm != null)
            {
                MasterLocalData local = popupData.masterLocalConfirm ?? defaultLocalConfirm;
                Util.SetMasterLocalizing(txtmp_Confirm, local);
            }

            if (txtmp_Cancel != null)
            {
                MasterLocalData local = popupData.masterLocalCancel ?? defaultLocalCancel;
                Util.SetMasterLocalizing(txtmp_Cancel, local);
            }
            #endregion

            #region 팝업 효과음
            if (popupData.icon == POPUPICON.WARNING)
            {
                Single.Sound.PlayWarningPopupSound();
            }
            else
            {
                if (popupData.btnType == BTN_TYPE.Confirm)
                {
                    Single.Sound.PlayOneButtonPopupSound();
                }
                else
                {
                    Single.Sound.PlayTwoButtonPopupSound();
                }
            }
            #endregion

            #region 제목
            if (go_Title != null)
            {
                bool active = true;

                if (popupData.masterLocalTitle != null)
                {
                    Util.SetMasterLocalizing(txtmp_Title, popupData.masterLocalTitle);
                }
                else
                {
                    active = false;
                }
                go_Title.SetActive(active);
            }
            #endregion

            #region 내용
            if (go_Desc != null)
            {
                bool active = true;

                if (popupData.masterLocalDesc != null)
                {
                    Util.SetMasterLocalizing(txtmp_Desc, popupData.masterLocalDesc);
                }
                else
                {
                    active = false;
                }
                go_Desc.SetActive(active);
            }
            #endregion
        }

        /// <summary>
        /// 아이콘 세팅
        /// </summary>
        /// <param name="type"></param>
        private void SetUpIcon(POPUPICON type)
        {
            if (go_Icon != null)
            {
                go_Icon.SetActive(false);

                if (img_Icon != null)
                {
                    Sprite sprite = null;
                    switch (type)
                    {
                        case POPUPICON.CHECK: sprite = img_Check; break;
                        case POPUPICON.EMAIL: sprite = img_Email; break;
                        case POPUPICON.WARNING: sprite = img_Warning; break;
                        case POPUPICON.NONE:
                        default: go_Icon.SetActive(false); return;
                    }

                    img_Icon.sprite = sprite;
                    go_Icon.SetActive(true);
                }
            }
        }

        /// <summary>
        /// 확인, 취소 버튼에 실행할 메소드 등록
        /// </summary>
        /// <param name="_confirm"></param>
        /// <param name="_cancel"></param>
        public void SetPopupAction(PopupAction popupAction = null)
        {
            if (popupAction == null) return;

            event_Confirm.RemoveAllListeners();
            event_Cancel.RemoveAllListeners();

            if (popupAction.confirmAction != null)
            {
                event_Confirm.AddListener(popupAction.confirmAction);
            }

            if (popupAction.cancelAction != null)
            {
                event_Cancel.AddListener(popupAction.cancelAction);
            }
        }
        #endregion


        #region 공통 에러 팝업
        /// <summary>
        /// 공통 서버 에러 팝업
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public void CheckResponseError(int _error)
        {
            DEBUG.LOG(_error.ToString(), eColorManager.WEB);

            switch ((WEBERROR)_error)
            {
                // ===== [로그인 관련 에러] =====
                //case WEBERROR.NET_E_SUCCESS:
                case WEBERROR.NET_E_NOT_LOGINED: SetDataPushPopup("common_error_wrongapproach", Util.ReturnToLogin); break;                    // 로그인 되지 않음
                case WEBERROR.NET_E_EMPTY_TOKEN: SetDataPushPopup("common_error_token_01", Util.ReturnToLogin); break;                         // 토큰이 비어있음
                case WEBERROR.NET_E_EXPIRED_TOKEN: SetDataPushPopup("common_error_token_02", Util.ReturnToLogin); break;                       // 토큰 만료
                case WEBERROR.NET_E_INVALID_TOKEN: SetDataPushPopup("common_error_token_03", Util.ReturnToLogin); break;                       // 유효하지 않은 토큰

                // ===== [계정 관련 에러] =====
                //case WEBERROR.NET_E_IS_DORMANT_ACCOUNT:
                case WEBERROR.NET_E_DUPLICATE_LOGIN: SetDataPushPopup("common_error_concurrent_access", Util.QuitApplication); break;          // 중복 로그인
                case WEBERROR.NET_E_ALREADY_DELETE_USER_ID: SetDataPushPopup("common_error_withdrawal"); break;                          // 계정 탈퇴한 사용자 아이디
                case WEBERROR.NET_E_ALREADY_EXIST_EMAIL: SetDataPushPopup("common_error_duplicate_email_01"); break;                     // 이미 존재하는 이메일
                case WEBERROR.NET_E_ALREADY_EXIST_NICKNAME: SetDataPushPopup("common_error_duplicate_nickname"); break;                  // 이미 존재하는 닉네임
                case WEBERROR.NET_E_ALREADY_MY_NICKNAME: SetDataPushPopup("common_state_mynickname"); break;                             // 이미 나의 닉네임
                case WEBERROR.NET_E_NOT_MATCH_PASSWORD: SetDataPushPopup("common_error_wrongpassword_01"); break;                        // 패스워드 불일치
                case WEBERROR.NET_E_NOT_EXIST_USER: SetDataPushPopup("common_error_unregistered_uesr"); break;                           // 존재하지 않는 사용자
                case WEBERROR.NET_E_NOT_MATCH_EMAIL_AUTH_CODE: SetDataPushPopup("common_error_certified"); break;                        // 존재하지 않는 이메일 인증 코드
                case WEBERROR.NET_E_NOT_EXIST_EMAIL: SetDataPushPopup("common_error_unregistered_email"); break;                         // 존재하지 않는 이메일
                case WEBERROR.NET_E_NOT_AUTH_EMAIL: SetDataPushPopup("common_error_uncertified_email"); break;                           // 인증 되지 않은 이메일
                case WEBERROR.NET_E_ALREADY_EXIST_EMAIL_FOR_ARZMETA_LOGIN: SetDataPushPopup("common_error_duplicate_email_02"); break;   // 이미 자체 로그인 가입 된 이메일 계정 사용자
                case WEBERROR.NET_E_EMPTY_PASSWORD: SetDataPushPopup("common_error_blank_password"); break;                              // 패스워드 없음
                case WEBERROR.NET_E_CANNOT_UPDATED_EMAIL: SetDataPushPopup("common_error_email_01"); break;                              // 이메일 업데이트 불가 (1달에 1번)
                case WEBERROR.NET_E_SAME_PREVIOUS_EMAIL: SetDataPushPopup("common_error_email_02"); break;                               // 이메일이 변경 되지 않았음 (기존 이메일과 같다)
                case WEBERROR.NET_E_SOCIAL_LOGIN_USER: SetDataPushPopup("common_error_signup_02"); break;                                // 소셜 로그인 사용자 입니다.
                case WEBERROR.NET_E_INVALID_EMAIL: SetDataPushPopup("common_state_invalid_email"); break;                                // 유효하지 않은 이메일 입니다.
                case WEBERROR.NET_E_OVER_COUNT_EMAIL_AUTH: SetDataPushPopup("common_state_overcount_emailauth"); break;                  // 이메일 인증 횟수를 초과 하였습니다.
                case WEBERROR.NET_E_ALREADY_PROVIDER_TYPELINKED_ACCOUNT: SetDataPushPopup("setting_state_already_linked"); break;                     // 이미 연동된 계정 입니다.
                case WEBERROR.NET_E_CANNOT_RELEASE_LINKED_ACCOUNT: SetDataPushPopup("setting_error_unable_release"); break;              // 계정 연동 해제 불가.
                case WEBERROR.NET_E_MAX_OVER_BUSINESS_CARD: SetDataPushPopup("businesscard_state_overcont"); break;                      // 비지니스 명함 갯수 초과
                //case WEBERROR.NET_E_ERROR_BUSINESS_CARD_ID:                                                                                   // 비지니스 명함 아이디 에러
                //case WEBERROR.NET_E_ALREADY_ACCOUNT:                                                                                          // 이미 존재하는 계정  
                case WEBERROR.NET_E_ALREADY_LINKED_OTHER_ACCOUNT: SetDataPushPopup("setting_state_already_linked_other"); break;         // 이미 다른계정에 연동되어 있습니다.
                case WEBERROR.NET_E_NOT_EXIST_BUSINESS_CARD: SetDataPushPopup("businesscard_state_nonexistent"); break;                  // 존재하지 않는 비지니스 명함  

                // ===== [친구 관련 에러] =====
                case WEBERROR.NET_E_ALREADY_FRIEND: SetDataPushPopup("common_error_friend_registration"); break;                         // 이미 친구 입니다.
                case WEBERROR.NET_E_ALREADY_RECEIVED_FRIEND_REQUEST: SetDataPushPopup("common_error_friend_receive"); break;             // 이미 친구 요청을 받았습니다.
                case WEBERROR.NET_E_ALREADY_SEND_FRIEND_REQUEST: SetDataPushPopup("common_error_friend_send"); break;                    // 이미 친구 요청을 보냈습니다.
                case WEBERROR.NET_E_NOT_EXIST_RECEIVED_REQUEST: SetDataPushPopup("common_error_friend_norequest"); break;                // 이미 친구 요청을 보냈습니다.
                case WEBERROR.NET_E_NOT_EXIST_REQUEST: SetDataPushPopup("common_state_empty"); break;                                    // 보낸 요청이 없습니다.
                case WEBERROR.NET_E_MEMBER_IS_BLOCK: SetDataPushPopup("common_state_block"); break;                                      // 차단 된 사용자 입니다.
                case WEBERROR.NET_E_MY_FRIEND_MAX_COUNT: SetDataPushPopup("friend_reception_unable_add1"); break;                        // 나의 친구 수 초과  
                case WEBERROR.NET_E_TARGET_FRIEND_MAX_COUNT: SetDataPushPopup("friend_reception_unable_add2"); break;                    // 상대의 친구 수 초과
                case WEBERROR.NET_E_CANNOT_BLOCK_MYSELF: SetDataPushPopup("common_error_block_myself"); break;                           // 자기 자신은 차단 할 수 없음.  
                case WEBERROR.NET_E_CANNOT_REQUEST_MYSELF: SetDataPushPopup("common_error_addfriend_myself"); break;                     // 자기 자신은 친구 요청을 보낼 수 없음.

                // ===== [아이템 관련 에러] =====
                case WEBERROR.NET_E_NOT_HAVE_ITEM: SetDataPushPopup("common_state_item_noyhold"); break;                                 // 소유하지 않은 아이템
                case WEBERROR.NET_E_ITEM_OVER_COUNT: SetDataPushPopup("common_state_item_overcount"); break;                             // 아이템 갯수 초과
                //case WEBERROR.NET_E_NOT_MATCH_ITEM:                                                                                           // 아이템이 일치하지 않습니다.
                case WEBERROR.NET_E_ITEM_NOT_REMOVABLE: SetDataPushPopup("myroom_state_impossible_delete"); break;                       // 아이템을 배치 해제 할 수 없다.

                // ===== [오피스 관련 에러] =====
                case WEBERROR.NET_E_NOT_EXIST_OFFICE: SetDataPushPopup("216"); break;                                                     // 존재 하지 않는 오피스

                // ===== [마이룸 관련 에러] =====
                case WEBERROR.NET_E_ALREADY_EXIST_MYROOM_ITEM: SetDataPushPopup("myroom_notice_possess"); break;                         // 마이룸에 있는 아이템
                case WEBERROR.NET_E_NOT_EXIST_MYROOM_ITEM: SetDataPushPopup("myroom_notice_nonpossession_01"); break;                    // 마이룸에 없는 아이템
                case WEBERROR.NET_E_NOT_EXIST_FURNITURE_INVEN_ITEM: SetDataPushPopup("myroom_nonpossession_02"); break;                  // 가구 인벤에 없는 아이템
                case WEBERROR.NET_E_CANNOT_DELETE_ITEM: SetDataPushPopup("myroom_notice_deletion"); break;                               // 삭제 불가능 아이템

                // ===== [투표 관련 에러] =====
                case WEBERROR.NET_E_NOT_EXIST_NOTICE: SetDataPushPopup("common_error_post_06"); break;                                   // 존재 하지 않는 공지사항 입니다.
                case WEBERROR.NET_E_BAD_PASSWORD: SetDataPushPopup("common_error_wrongpassword_02"); break;                              // 잘못된 패스워드 형식 입니다.
                case WEBERROR.NET_E_CANNOT_VOTE: SetDataPushPopup("common_error_vote_01"); break;                                        // 투표 불가.
                case WEBERROR.NET_E_TOO_MANY_RESPONSE: SetDataPushPopup("common_error_answer_01"); break;                                // 투표 응답 갯수가 너무 많습니다.
                case WEBERROR.NET_E_ALREADY_VOTE: SetDataPushPopup("common_error_vote_02"); break;                                       // 이미 투표를 했습니다.
                case WEBERROR.NET_E_WRONG_RESPONSE: SetDataPushPopup("common_error_answer_02"); break;                                   // 잘못된 응답입니다.
                case WEBERROR.NET_E_NOT_EXIST_VOTE: SetDataPushPopup("common_error_vote_03"); break;                                     // 존재하지 않는 투표입니다.
                case WEBERROR.NET_E_NOT_EXIST_PROGRESS_VOTE: SetDataPushPopup("common_error_vote_04"); break;                            // 진행 중인 투표가 없습니다.

                // ===== [우편함 관련 에러] =====
                case WEBERROR.NET_E_CANNOT_RECEIVED_POST: SetDataPushPopup("post_notice_received"); break;                               // 우편을 수령할 수 없습니다.
                case WEBERROR.NET_E_NOT_EXIST_POST: SetDataPushPopup("post_notice_existent"); break;                                     // 존재하지 않는 우편입니다.

                // ===== [액자아이템 관련 에러] =====
                case WEBERROR.NET_E_NOT_EXIST_IMAGE_URL: SetDataPushPopup("img_notice_url"); break;                                      // 이미지 URL 이 없습니다.
                case WEBERROR.NET_E_NOT_EXIST_IMAGE_FILE: SetDataPushPopup("img_notice_file"); break;                                    // 이미지 파일이 없습니다.
                case WEBERROR.NET_E_BAD_IMAGE: SetDataPushPopup("img_notice_inappropriate"); break;                                      // 부적절한 이미지 입니다.

                // ===== [ NFT 관련 에러] =====
                case WEBERROR.NET_E_ALREADY_LINKED_SAME_WALLET_ADDR:                                                                            // 이미 연동된 지갑 주소와 같은 지갑 주소 입니다.
                //case WEBERROR.NET_E_ALREADY_EXISTS_LINKED_WALLET_ADDR:                                                                        // 나의 계정에 이미 지갑이 연동 되어 있는 경우.
                case WEBERROR.NET_E_ALREADY_EXISTS_LINKED_ACCOUNT: SetDataPushPopup("juri_notice_walletaddress"); break;                 // 지갑 주소가 이미 다른 계정과 연동되어 있는 경우
                //case WEBERROR.NET_E_NOT_EXISTS_LINKED_WALLET_ADDR:                                                                            // 연동된 지갑 주소가 없음.

                // ===== [ 유학박람회 관련 에러] =====
                case WEBERROR.NET_E_NOT_EXISTS_BOOTH: SetDataPushPopup("office_booth_notice_nonexistence"); break;                       // 존재 하지 않는 부스 입니다.
                case WEBERROR.NET_E_NOT_EXISTS_EVENT: SetDataPushPopup("office_booth_notice_event"); break;                              // 존재 하지 않는 이벤트 입니다.
                case WEBERROR.NET_E_HAVE_NOT_LICENSE_MEMBER: SetDataPushPopup("office_booth_notice_license"); break;                     // 라이선스를 보유지 않은 회원 입니다.
                case WEBERROR.NET_E_NOT_EXIST_FILE: SetDataPushPopup("office_booth_notice_file"); break;                                 // 파일이 없습니다.
                case WEBERROR.NET_E_NOT_EXIST_EVENT: SetDataPushPopup("office_booth_notice_event"); break;                               // 진행중인 행사가 없습니다.
                case WEBERROR.NET_E_UNAUTHORIZE_ADMIN: SetDataPushPopup("office_booth_notice_permission"); break;                        // 권한이 없습니다.

                // ===== [디비 관련 에러] =====
                case WEBERROR.NET_E_DB_FAILED: SetDataPushPopup("common_error_server_02", Util.ReturnToLogo); break;                      // 디비가 문제 있음
                case WEBERROR.NET_E_SERVER_INACTIVATE: SetDataPushPopup("common_error_server_03"); break;                                // 서버 비활성
                case WEBERROR.NET_E_NEED_UPDATE: SetDataPushPopup("common_error_update"); break;                                         // 업데이트 필요
                default:
                    {
                        SetDataPushPopup($"{Util.GetMasterLocalizing("common_error_retye")}\n\n<size=25><color=grey>error code : #{_error}</color></size>");
                    }
                    break;
            }
        }

        /// <summary>
        /// 서버 에러 예외처리 팝업
        /// </summary>
        /// <param name="_error"></param>
        /// <returns></returns>
        public void CheckHttpResponseError(string _error)
        {
            DEBUG.LOG(_error, eColorManager.Web_Response);

            switch (_error)
            {
                case "Request timeout": // 타임 아웃
                case "Cannot connect to destination host": // 호스트를 찾을 수 없음
                    SetDataPushPopup("common_error_server_02", Util.ReturnToLogo); break;
                default: break;
            }
        }

        /// <summary>
        /// 웹소켓 에러 예외 처리 팝업
        /// 무조건 콘텐츠 정지가 되어야 하면 return true, 해당 메소드 이후로 실행해야 할 코드가 있다면 return false
        /// </summary>
        /// <param name="_error"></param>
        /// <returns></returns>
        public bool CheckWebSocketResponseError(string _error)
        {
            DEBUG.LOG(_error, eColorManager.Web_Response);

            switch ((WebSocketError)int.Parse(_error))
            {
                case WebSocketError.DROP_PLAYER:                                                                                // 최신 세션아이디가 아닌 유저일 경우
                case WebSocketError.RECONNECT_FAILED:                                                                           // 클라이언트 재연결 시도 후에도 연결이 안 될 때
                    SetDataPushPopup(new MasterLocalData("common_error_network_03", _error), Util.ReturnToLogin); return true;
                case WebSocketError.NOT_FRIEND: SetDataPushPopup("friend_notice_unregistered"); return true;                    // 친구가 아닐 경우
                case WebSocketError.OFFLINE_FRIEND: SetDataPushPopup("friend_notice_offline"); return true;                     // 친구가 오프라인일 경우
                case WebSocketError.NOT_EXIST_USER_FRIEND: SetDataPushPopup("common_error_unregistered_uesr"); return true;     // 존재하지 않는 친구 사용자인 경우
                case WebSocketError.DO_NOT_ENTER_SCENE: Single.Socket.OpenErrorPopup_UnableFollow(); return true;               // 존재하지 않는 친구 사용자인 경우
                case WebSocketError.DUPLICATE: return true;
                default: return false;
            }
        }

        /// <summary>
        /// 팝업 종합 데이터 세팅 함수 (string)
        /// </summary>
        /// <param name="str"></param>
        /// <param name="action"></param>
        private void SetDataPushPopup(string masterLocalData, UnityAction action = null)
        {
            SetDataPushPopup(new MasterLocalData(masterLocalData), action);
        }

        /// <summary>
        /// 팝업 종합 데이터 세팅 함수 (MasterLocalData)
        /// </summary>
        /// <param name="masterLocalData"></param>
        /// <param name="action"></param>
        private void SetDataPushPopup(MasterLocalData masterLocalData, UnityAction action = null)
        {
            SceneLogic.instance.isUILock = false;
            ChainPopupData(new PopupData(POPUPICON.WARNING, BTN_TYPE.Confirm, masterLocalDesc: masterLocalData))
            .ChainPopupAction(new PopupAction(action));
            PushPopup<Popup_Basic>();
        }
        #endregion
    }


    #region PopupData, PopupAction
    /// <summary>
    /// 팝업의 데이터를 넣어줄때 사용한다.
    /// </summary>
    public class PopupData
    {
        public POPUPICON icon;

        public MasterLocalData masterLocalTitle;
        public MasterLocalData masterLocalDesc;
        public MasterLocalData masterLocalConfirm;
        public MasterLocalData masterLocalCancel;

        public BTN_TYPE btnType;

        /// <summary>
        /// 가장 최신 팝업데이터 셋업 오버로딩
        /// </summary>
        /// <param name="icon"></param>
        /// <param name="btnType"></param>
        /// <param name="masterLocalTitle"></param>
        /// <param name="masterLocalDesc"></param>
        /// <param name="masterLocalConfirm"></param>
        /// <param name="masterLocalCancel"></param>
        public PopupData(POPUPICON icon, BTN_TYPE btnType, MasterLocalData masterLocalTitle = null, MasterLocalData masterLocalDesc = null, MasterLocalData masterLocalConfirm = null, MasterLocalData masterLocalCancel = null)
        {
            InitPopupData();
            this.icon = icon;
            this.btnType = btnType;

            this.masterLocalTitle = masterLocalTitle;
            this.masterLocalDesc = masterLocalDesc;

            this.masterLocalConfirm = masterLocalConfirm;
            this.masterLocalCancel = masterLocalCancel;
        }

        public void InitPopupData()
        {
            this.icon = POPUPICON.NONE;
            this.btnType = BTN_TYPE.Confirm;
            this.masterLocalTitle = null;
            this.masterLocalDesc = null;
            this.masterLocalConfirm = null;
            this.masterLocalCancel = null;
        }
    }

    /// <summary>
    /// 팝업의 액션을 넣어줄때 사용한다.
    /// </summary>
    public class PopupAction
    {
        public UnityAction confirmAction;
        public UnityAction cancelAction;
        public PopupAction(UnityAction confirmAction, UnityAction cancelAction = null)
        {
            this.confirmAction = confirmAction;
            this.cancelAction = cancelAction;
        }
    }
    #endregion

}