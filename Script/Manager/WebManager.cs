using CryptoWebRequestSample;
using Cysharp.Threading.Tasks;
using FrameWork.UI;
using MEC;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public enum eHttpVerb
{
    GET,
    POST,
    PUT,
    DELETE,
}

namespace FrameWork.Network
{
    #region 웹매니저 데이터 클래스
    public class SendRequestData
    {
        public eHttpVerb httpVerb = eHttpVerb.GET;

        public string mainUrl = string.Empty;
        public string subUrl = string.Empty;
        public object packet = null;

        public bool dim = true;
        public bool isErrorPopup = true;

        public SendRequestData(eHttpVerb httpVerb, string mainUrl, string subUrl, object packet = null, bool dim = true, bool isErrorPopup = true)
        {
            this.httpVerb = httpVerb;

            this.mainUrl = mainUrl;
            this.subUrl = subUrl;
            this.packet = packet;

            this.dim = dim;
            this.isErrorPopup = isErrorPopup;
        }

        public string GetURL() => mainUrl + subUrl;
    }

    /// <summary>
    /// 멀티 폼 데이터용 클래스
    /// </summary>
    public class SendRequestData_MultiPartForm : SendRequestData
    {
        public string thumbnailPath = "";
        public Texture2D iosTex = null;

        public SendRequestData_MultiPartForm(eHttpVerb httpVerb, string mainUrl, string subUrl, object packet = null, bool dim = true, bool isErrorPopup = true, string thumbnailPath = "", Texture2D iosTex = null)
            : base(httpVerb, mainUrl, subUrl, packet, dim, isErrorPopup)
        {
            this.thumbnailPath = thumbnailPath;
            this.iosTex = iosTex;
        }
    }
    #endregion
    
    public class WebManager : Singleton<WebManager>
    {
        #region Unity 기본 메소드
        private void Update()
        {

        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        protected override void OnApplicationQuit()
        {
            base.OnApplicationQuit();

            Timing.KillCoroutines();
            StopAllCoroutines();
            Resources.UnloadUnusedAssets();
        }
        #endregion

        #region 변수
        public bool IsGateWayInfoResDone { get; private set; }

        private string GatewayInfoURL = "https://arzmetasta.blob.core.windows.net/arzmeta-container/gateway/gatewayInfo.txt";

        #region GateWayData
        public GatewayInfo GatewayInfo { get; private set; }

        private ServerInfo ServerInfo => GatewayInfo.Gateway.ServerType.ServerInfo;
        // Url
        public string AccountServerUrl => ServerInfo.accountServerUrl;
        public string PCAccountServerUrl => ServerInfo.pcAccountServerUrl;
        public string AgoraServerUrl => ServerInfo.agoraServerUrl;
        public string ContentServerUrl => ServerInfo.contentServerUrl;
        public string LobbyServerUrl => ServerInfo.lobbyServerUrl;
        public int GameServerPort => ServerInfo.gameServerPort;
        public string StorageUrl => ServerInfo.storageUrl;
        public string HomepageUrl => ServerInfo.homepageUrl;
        public string WebviewUrl => ServerInfo.webviewUrl;
        public string LinuxServerIp => ServerInfo.linuxServerIp;
        public int LinuxServerPort => ServerInfo.linuxServerPort;
        public int LinuxHttpPort => ServerInfo.linuxHttpPort;
        public string HomepageBackendUrl => ServerInfo.homepageBackendUrl;
        public string WebSocketUrl => ServerInfo.webSocketUrl;
        public string YoutubeDlUrl => ServerInfo.youtubeDlUrl;

        // Make 요청할 때 사용하는 Url,Port
        // 점핑매칭
        public string MatchingServerUrl => ServerInfo.matchingServerUrl + ":" + MatchingServerPort + Cons.RequestMakeRoomStr;
        public int MatchingServerPort => ServerInfo.matchingServerPort;

        // OX 퀴즈
        public string OXServerUrl => ServerInfo.OXServerUrl + ":" + MatchingServerPort + Cons.RequestMakeRoomStr;
        public int OXServerPort => ServerInfo.OXServerPort;

        // 오피스
        public string MeetingRoomServerUrl => ServerInfo.meetingRoomServerUrl + ":" + MeetingRoomServerport + Cons.RequestMakeRoomStr;
        public int MeetingRoomServerport => ServerInfo.meetingRoomServerPort;

        // 상담실
        public string MedicineRoomServerUrl => ServerInfo.medicineUrl + ":" + MedicineRoomServerport + Cons.RequestMakeRoomStr;
        public int MedicineRoomServerport => ServerInfo.medicinePort;

        // 마이룸
        public string MyRoomServerUrl => ServerInfo.myRoomServerUrl + ":" + MyRoomServerport + Cons.RequestMakeRoomStr;
        public int MyRoomServerport => ServerInfo.myRoomServerPort;
        #endregion
        #endregion

        #region 초기화
        public void Initialize()
        {
            SetUpGateWayInfo();
        }

        /// <summary>
        /// 게이트웨이 데이터 가져오는 메소드. 맨 처음 호출해줘야함
        /// </summary>
        private async void SetUpGateWayInfo()
        {
            IsGateWayInfoResDone = false;
            var mainURL = string.Empty;

            SendRequest<string>(new SendRequestData(eHttpVerb.GET, GatewayInfoURL, string.Empty), (res) => mainURL = SetMainUrl(res));

            await UniTask.WaitUntil(() => !string.IsNullOrEmpty(mainURL));

            var packet = new
            {
                osType = GetOsType(),
                appVersion = Application.version,
            };

            SendRequest<string>(new SendRequestData(eHttpVerb.POST, mainURL, string.Empty, packet), SerializeGatewayInfo);
        }

        /// <summary>
        /// 현재 플랫폼에 따른 OsType 리턴 
        /// </summary>
        /// <returns></returns>
        private OS_TYPE GetOsType()
        {
#if UNITY_EDITOR || !UNITY_ANDROID && !UNITY_IOS
            return OS_TYPE.Window;
#elif UNITY_ANDROID
            return OS_TYPE.Android;
#elif UNITY_IOS
            return OS_TYPE.IOS;
#endif
        }

        /// <summary>
        /// mainURL 세팅
        /// </summary>
        /// <param name="result"></param>
        private string SetMainUrl(string result)
        {
            if (string.IsNullOrEmpty(result))
            {
                OpenReturnToLogoPopup();
                return null;
            }

            return WebPacket.Gateway(ClsCrypto.DecryptByAES(result));
        }

        /// <summary>
        /// 게이트웨이 데이터 파싱 및 상태에 따른 처리
        /// </summary>
        /// <param name="result"></param>
        private void SerializeGatewayInfo(string result)
        {
            if (string.IsNullOrEmpty(result))
            {
                OpenReturnToLogoPopup();
                return;
            }

            var jobj = (JObject)JsonConvert.DeserializeObject(result);
            foreach (var x in jobj)
            {
                switch (x.Key)
                {
                    case "Gateway":
                        GatewayInfo = JsonConvert.DeserializeObject<GatewayInfo>(result);

                        CheckAppVersionDataReset(GatewayInfo.Gateway.appVersion);

                        switch ((SERVER_STATE)GatewayInfo.Gateway.ServerState.state)
                        {
                            case SERVER_STATE.ACTIVATE: IsGateWayInfoResDone = true; break;
                            case SERVER_STATE.INACTIVATE: SetGatewayPopup(GatewayInfo.Gateway.StateMessage.message, () => Util.QuitApplication()); break;
                            case SERVER_STATE.TEST: Debug.Log("Gateway: 테스트 수락된 유저만 진입할 수 있습니다. 본 기능은 아직 개발되지 않았습니다."); break;
                            case SERVER_STATE.NEED_UPDATE: SetGatewayPopup(GatewayInfo.Gateway.StateMessage.message, () => UpdateVersion(GatewayInfo.Gateway.OsType.storeUrl)); break;
                            default: break;
                        }
                        break;
                    default:
                        GatewayInfo_Update gatewayInfo_Update = JsonConvert.DeserializeObject<GatewayInfo_Update>(result);
                        SetGatewayPopup(gatewayInfo_Update.StateMessage.message, () => UpdateVersion(gatewayInfo_Update.OsType.storeUrl));
                        break;
                }
                break;
            }
        }

        /// <summary>
        /// 앱버전이 설치되어있던 것과 다를 시 로그아웃
        /// </summary>
        private void CheckAppVersionDataReset(string appVersion)
        {
            if (PlayerPrefs.GetString("AppVersion", "0.0.0") != appVersion)
            {
                LocalPlayerData.ResetData();
            }
            PlayerPrefs.SetString("AppVersion", appVersion);
        }

        private void SetGatewayPopup(string message, UnityAction action = null)
        {
            SceneLogic.instance.PushPopup<Popup_Basic>()
                     .ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.Confirm, masterLocalDesc: new MasterLocalData(message)))
                     .ChainPopupAction(new PopupAction(action));
        }

        public void UpdateVersion(string storeUrl)
        {
            Application.OpenURL(storeUrl);
            Application.Quit(); // 강종
        }
        #endregion

        #region [코어 메소드] Request&Response
        /// <summary>
        /// 데이터 리퀘스트
        /// </summary>
        public async void SendRequest<T_Res>(SendRequestData data, Action<T_Res> _res, Action<DefaultPacketRes> _error = null)
        {
            await Co_SendWebRequest(data, _res, _error ?? Error_Res);
        }

        /// <summary>
        /// 코어 메소드
        /// </summary>
        /// <typeparam name="T_Res"></typeparam>
        /// <param name="data"></param>
        /// <param name="_res"></param>
        /// <param name="_error"></param>
        /// <returns></returns>
        async Task Co_SendWebRequest<T_Res>(SendRequestData data, Action<T_Res> _res, Action<DefaultPacketRes> _error = null)
        {
            string responseJson = string.Empty;
            byte[] jsonBytes = null;

            if (data.dim) Single.Scene.SetDimOn(1f);

            SetIsErrorPopup(data.subUrl, data.isErrorPopup);

            #region 데이터 패킹 및 세팅
            if (data.packet != null)
            {
                var requestJson = JsonConvert.SerializeObject(data.packet, Formatting.Indented);
                jsonBytes = Encoding.UTF8.GetBytes(requestJson);
                DEBUG.LOG(data.subUrl + "\n" + requestJson + "\n", eColorManager.Web_Request);
            }

            var uwr = new UnityWebRequest(data.GetURL(), data.httpVerb.ToString())
            {
                uploadHandler = new UploadHandlerRaw(jsonBytes),
                downloadHandler = new DownloadHandlerBuffer()
            };

            uwr.SetRequestHeader("Content-Type", "application/json");
            SetRequestHeader_jwt(uwr);

            // uwr.timeout = 10; // 타임 아웃을 UniTask에서 처리하도록 수정. BKK
            var cts = new CancellationTokenSource();
            cts.CancelAfterSlim(TimeSpan.FromSeconds(10)); // 타임 아웃 시간. BKK
            #endregion

            #region 데이터 샌드 및 리스폰스
            try
            {
                await uwr.SendWebRequest().WithCancellation(cts.Token);
            }
            catch (UnityWebRequestException exception)
            {
                if (exception.Result == UnityWebRequest.Result.ConnectionError ||
                                       exception.Result == UnityWebRequest.Result.ProtocolError ||
                                       exception.Result == UnityWebRequest.Result.DataProcessingError)
                {
                    DEBUG.LOG(data.subUrl + " " + exception.Error, eColorManager.Web_Response);
                    DEBUG.LOG(exception.Text, eColorManager.Web_Response);

                    if (GetIsErrorPopup(data.subUrl)) await HandleError(exception.Error, exception.Text);

                    if (SceneLogic.instance.GetSceneType() == SceneName.Scene_Base_Title) RefrashTitle();
                }
            }
            // 타임 아웃 예외 처리
            catch (OperationCanceledException exception)
            {
                if (exception.CancellationToken == cts.Token)
                {
                    Debug.Log("TimeOut!!");
                    await HandleError("Request timeout", "{ error = 0, errorMessage = \"\"}");
                }
            }
            finally
            {
                await UniTask.WaitUntil(() => uwr.isDone);

                if (data.dim) Single.Scene.SetDimOff(1f);

                responseJson = Util.Beautify(uwr.downloadHandler.text);
                DEBUG.LOG(data.subUrl + "\n" + responseJson + "\n", eColorManager.Web_Response);

                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    T_Res resObj = (typeof(T_Res) == typeof(string)) ? (T_Res)(object)responseJson : JsonConvert.DeserializeObject<T_Res>(responseJson);

                    if (resObj != null)
                    {
                        _res?.Invoke(resObj);
                    }
                    else
                    {
                        SceneLogic.instance.PushPopup<Popup_Basic>()
                            .ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.Confirm, masterLocalDesc: new MasterLocalData("common_error_network_01")));
                    }
                }
                else
                {
                    var resObj = JsonConvert.DeserializeObject<DefaultPacketRes>(responseJson);
                    if (resObj != null) _error?.Invoke(resObj);
                }
            }
            #endregion
        }

        /// <summary>
        /// 멀티파트 폼데이터 리퀘스트
        /// </summary>
        /// <typeparam name="T_Res"></typeparam>
        /// <param name="data"></param>
        /// <param name="res"></param>
        /// <param name="error"></param>
        public async void SendWebRequest_MultiPartFormData<T_Res>(SendRequestData_MultiPartForm data, Action<T_Res> res, Action<DefaultPacketRes> error = null)
        {
            await Co_SendPostRequest_MultiPartFormData(data, res, error ?? Error_Res);
        }

        /// <summary>
        /// 멀티파트 폼데이터
        /// </summary>
        /// <typeparam name="T_Res"></typeparam>
        /// <param name="res"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        async Task Co_SendPostRequest_MultiPartFormData<T_Res>(SendRequestData_MultiPartForm data, Action<T_Res> res, Action<DefaultPacketRes> error = null)
        {
            string requestJson;

            if (data.dim) Single.Scene.SetDimOn(1f);

            #region 데이터 패킹 및 세팅
            //1. 멀티파트 폼데이터 셋업
            List<IMultipartFormSection> formData = new List<IMultipartFormSection>();

            if (data.thumbnailPath != "")
            {
#if UNITY_IOS
                byte[] image = data.iosTex.EncodeToPNG();
#else
                byte[] image = File.ReadAllBytes(data.thumbnailPath);
#endif
                string fileName = Uri.EscapeUriString(Path.GetFileName(data.thumbnailPath));
#if UNITY_IOS
                fileName = Util.ConvertIOSExtension(fileName);
#endif
                string extension = "image/" + Path.GetExtension(fileName).Split('.').Last();
                formData.Add(new MultipartFormFileSection("image", image, fileName, extension));
            }


            requestJson = JsonConvert.SerializeObject(data.packet, Formatting.Indented);

            DEBUG.LOG(data.subUrl + "\n" + requestJson + "\n", eColorManager.Web_Request);

            Dictionary<string, string> jsonDic = JsonConvert.DeserializeObject<Dictionary<string, string>>(requestJson);
            foreach (var item in jsonDic)
            {
                //Debug.Log("item.Key : " + item.Key);
                //Debug.Log("item.Value : " + item.Value);
                formData.Add(new MultipartFormDataSection(item.Key, item.Value));
            }

            //2. 폼데이터에 맞게 바운더리 작업
            byte[] boundary = UnityWebRequest.GenerateBoundary();
            byte[] formSections = UnityWebRequest.SerializeFormSections(formData, boundary);//3. 폼데이터에 맞게 업로드 핸들러

            //uwr = UnityWebRequest.Post(url, formData);
            UnityWebRequest uwr = new UnityWebRequest(data.GetURL(), data.httpVerb.ToString())
            {
                timeout = 10,
                uploadHandler = new UploadHandlerRaw(formSections),
                downloadHandler = new DownloadHandlerBuffer()
            };

            uwr.SetRequestHeader("Content-Type", $"multipart/form-data; boundary={Encoding.UTF8.GetString(boundary)}"); //4. 헤더에 바운더리 폼데이터이기때문에 바운더리 값 추가
            SetRequestHeader_jwt(uwr);

            #endregion

            #region 데이터 샌드 및 리스폰스
            try
            {
                await uwr.SendWebRequest(); //쏜다
            }
            catch (UnityWebRequestException exception)
            {
                if (exception.Result == UnityWebRequest.Result.ConnectionError ||
                         exception.Result == UnityWebRequest.Result.ProtocolError ||
                         exception.Result == UnityWebRequest.Result.DataProcessingError)
                {
                    DEBUG.LOG(exception.Error, eColorManager.Web_Response);
                    DEBUG.LOG(exception.Text, eColorManager.Web_Response);

                    await HandleError(exception.Error, exception.Text);

                    Debug.Log(exception.Error);
                }
            }
            finally
            {
                if (data.dim)
                {
                    Single.Scene.SetDimOff(1f);
                    await UniTask.WaitWhile(() => Single.Scene.isDim);
                }
                await UniTask.WaitUntil(() => uwr.isDone);

                string responseJson = Util.Beautify(uwr.downloadHandler.text);
                DEBUG.LOG(data.subUrl + "\n" + responseJson + "\n", eColorManager.Web_Response);

                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    var resObj = JsonConvert.DeserializeObject<T_Res>(responseJson);
                    if (resObj != null)
                    {
                        res.Invoke(resObj);
                    }
                    else
                    {
                        SceneLogic.instance.PushPopup<Popup_Basic>()
                            .ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.Confirm, masterLocalDesc: new MasterLocalData("common_error_network_01")));
                    }
                }
                else
                {
                    var resObj = JsonConvert.DeserializeObject<DefaultPacketRes>(responseJson);
                    if (resObj != null) error?.Invoke(resObj);
                }
                uwr.Dispose(); //해제
            }
            #endregion
        }

        /// <summary>
        /// 에러 발생 시 처리
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        private async Task HandleError(string errorMessage, string response)
        {
            DefaultPacketRes responseError = JsonConvert.DeserializeObject<DefaultPacketRes>(response);

            // 아예 서버 응답이 없을 경우, responseError 가 null 이 들어옴
            if (responseError == null)
            {
                OpenReturnToLogoPopup();
            }
            else
            {
                SceneLogic.instance.GetPopup<Popup_Basic>().CheckHttpResponseError(errorMessage); // http 서버 에러
                SceneLogic.instance.GetPopup<Popup_Basic>().CheckResponseError(responseError.error); // 웹 서버 에러
            }

            await UniTask.DelayFrame(1);
        }

        public void Error_Res(DefaultPacketRes res)
        {
            if (Single.Scene.isDim) Single.Scene.SetDimOff();

            Debug.Log("errorCode : " + res.error + ", errorMessage : " + res.errorMessage);
        }

        /// <summary>
        /// API 사용 시 헤더 추가
        /// 로그인 이전에 호출되는 API 외에 대부분 해당 헤더가 사용된다
        /// </summary>
        /// <param name="uwr"></param>
        private void SetRequestHeader_jwt(UnityWebRequest uwr)
        {
            if (!string.IsNullOrEmpty(LocalPlayerData.JwtAccessToken))
            {
                uwr.SetRequestHeader("jwtAccessToken", ClsCrypto.EncryptByAES(LocalPlayerData.JwtAccessToken));
            }
            if (!string.IsNullOrEmpty(LocalPlayerData.SessionID))
            {
                uwr.SetRequestHeader("sessionId", ClsCrypto.EncryptByAES(LocalPlayerData.SessionID));
            }
        }

        #region IsErrorPopup
        // 에러 시 자동으로 뜨는 팝업을 비/활성화 할 수 있는 옵션.
        // API 호출 시 isErrorPopup 파라미터를 작성하면 된다. true: 활성화, false: 비활성화
        // true가 기본값

        private Dictionary<string, bool> IsErrorPopupDic = new Dictionary<string, bool>();

        public void SetIsErrorPopup(string key, bool isErrorPopup)
        {
            if (!IsErrorPopupDic.ContainsKey(key))
            {
                IsErrorPopupDic.Add(key, default);
            }
            IsErrorPopupDic[key] = isErrorPopup;
        }

        private bool GetIsErrorPopup(string key)
        {
            if (IsErrorPopupDic.ContainsKey(key))
            {
                return IsErrorPopupDic[key];
            }
            return true;
        }
        #endregion

        #endregion

        #region API

        // 계정 Account
        public WebAccount account = new WebAccount();

        // 사용자 Member
        public WebMember member = new WebMember();

        // 친구 Friend
        public WebFriend friend = new WebFriend();

        // 투표 Vote
        public WebVote vote = new WebVote();

        // 오피스 Office
        public WebOffice office = new WebOffice();

        // 마이룸 MyRoom
        public WebMyRoom myRoom = new WebMyRoom();

        // 타인의 정보 Others
        public WebOthers others = new WebOthers();

        // 랭킹(무한의 코드) Ranking
        public WebRanking ranking = new WebRanking();

        // 우편함 PostBox
        public WebPostbox webPostbox = new WebPostbox();

        // 선택 투표(KTMF) SelectVote
        public WebSelectVote selectVote = new WebSelectVote();

        // 광고 컨텐츠(재화, 코인) AD-Contents
        public WebADContents adContents = new WebADContents();

        // 유학박람회 CSAF - Chongro Study Abroad Fair
        public WebCSAF CSAF = new WebCSAF();

        // 숏링크
        public WebShortLink shortLink = new WebShortLink();

        #region 보류
        #region RealtimeTest

        private void SendPostRequest_Test(string mainUrl, object obj, Action res, Action<string, int> error, bool dim = true)
        {
            Timing.RunCoroutine(CoSendWebRequest_Test(mainUrl, UnityWebRequest.kHttpVerbPOST, obj, res, error, dim));
        }

        IEnumerator<float> CoSendWebRequest_Test(string mainUrl, string method, object obj, Action res, Action<string, int> error, bool dim = true)
        {
            string sendUrl = mainUrl;
            string requestJson = string.Empty;
            string responeJson = string.Empty;
            byte[] jsonBytes = null;

            if (dim) Single.Scene.SetDimOn(1f);

            if (obj != null)
            {

                string jsonStr = JsonConvert.SerializeObject(obj, Formatting.Indented);
                jsonBytes = Encoding.UTF8.GetBytes(jsonStr);
            }

            DEBUG.LOG(mainUrl + "\n" + requestJson + "\n", eColorManager.Web_Request);

            var uwr = new UnityWebRequest(sendUrl, method);
            uwr.timeout = 10;
            uwr.uploadHandler = new UploadHandlerRaw(jsonBytes);
            uwr.downloadHandler = new DownloadHandlerBuffer();
            uwr.SetRequestHeader("Content-Type", "application/json");
            SetRequestHeader_jwt(uwr);

            try
            {
                uwr.SendWebRequest();
            }

            catch (UnityWebRequestException exception)
            {
                if (exception.Result == UnityWebRequest.Result.ConnectionError ||
                    exception.Result == UnityWebRequest.Result.ProtocolError ||
                    exception.Result == UnityWebRequest.Result.DataProcessingError)
                {
                    DEBUG.LOG(exception.Error, eColorManager.Web_Response);
                    DEBUG.LOG(exception.Text, eColorManager.Web_Response);

                    if (dim) Single.Scene.SetDimOff(2f);

                    Timing.RunCoroutine(HandleError_Test(exception.Error, exception.Text, error));
                }

                Debug.Log(exception.Error);
            }
            finally
            {
                Timing.WaitUntilTrue(() => uwr.isDone);

                if (dim) Single.Scene.SetDimOff(1f);

                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    responeJson = Util.Beautify(uwr.downloadHandler.text);

                    DEBUG.LOG(mainUrl + "\n" + responeJson + "\n", eColorManager.Web_Response);
                }

                else
                {

                }
            }
            yield return Timing.WaitForOneFrame;
        }

        public IEnumerator<float> HandleError_Test(string errorMessage, string response, Action<string, int> _error)
        {
            DefaultPacketRes responseError = JsonConvert.DeserializeObject<DefaultPacketRes>(response);
            // 아예 서버 응답이 없을 경우, responseError 가 null 이 들어옴
            if (responseError == null)
            {
                OpenReturnToLogoPopup();
                yield break;
            }

            SceneLogic.instance.GetPopup<Popup_Basic>().CheckHttpResponseError(errorMessage); // http 서버 에러
            SceneLogic.instance.GetPopup<Popup_Basic>().CheckResponseError(responseError.error); // 웹 서버 에러

            yield return Timing.WaitForSeconds(1f);

            _error?.Invoke(errorMessage, responseError.error);
        }

        #endregion

        #region 휴면계정
        /// <summary>
        /// 휴면계정 이메일 인증
        /// </summary>
        /// <param name="email"></param>
        /// <param name="_res"></param>
        public void DormantCheckEmail(string email, Action<CheckEmailPacketRes_Old> _res, Action<DefaultPacketRes> _error = null)
        {
            CheckEmailPacketReq packet = new CheckEmailPacketReq()
            {
                email = ClsCrypto.EncryptByAES(email)
            };

            SendRequest(new SendRequestData(eHttpVerb.POST, AccountServerUrl, WebPacket.DormantCheckEmail, packet), _res, _error);
        }

        /// <summary>
        /// 휴면계정 이메일 인증 확인
        /// </summary>
        /// <param name="email"></param>
        /// <param name="authCode"></param>
        /// <param name="_res"></param>
        public void DormantConfirmEmail(string email, string authCode, Action<DefaultPacketRes> _res, Action<DefaultPacketRes> _error = null)
        {
            AuthEmailPacketReq packet = new AuthEmailPacketReq()
            {
                email = ClsCrypto.EncryptByAES(email),
                //authCode = ClsCrypto.EncryptByAES(authCode)
            };

           SendRequest(new SendRequestData(eHttpVerb.POST, AccountServerUrl, WebPacket.DormantConfirmEmail, packet), _res, _error);
        }

        /// <summary>
        /// 휴면 계정 해제
        /// </summary>
        /// <param name="email"></param>
        /// <param name="_res"></param>
        public void DormantAccount(string email, Action<CheckEmailPacketRes_Old> _res, Action<DefaultPacketRes> _error = null)
        {
            CheckEmailPacketReq packet = new CheckEmailPacketReq()
            {
                email = ClsCrypto.EncryptByAES(email)
            };

           SendRequest(new SendRequestData(eHttpVerb.POST, AccountServerUrl, WebPacket.DormantAccount, packet), _res, _error);
        }
        #endregion

        #region 퀘스트
        /// <summary>
        /// 테스트코드
        /// </summary>
        private void Test_Quest()
        {
            //if (Input.GetKeyDown(KeyCode.Keypad1))
            //{
            //    Quest_GetMemberInfo_Req(Quest_GetMemberInfo_Res); //내 퀘스트현황 가져오기
            //}
            //if (Input.GetKeyDown(KeyCode.Keypad2))
            //{
            //    Quest_Process_Req(10, Quest_Process_Res); //퀘스트 10회달성의 퀘스트 그룹은 10, 1회씩 축적
            //}
            //if (Input.GetKeyDown(KeyCode.Keypad3))
            //{
            //    Quest_ReceiveReward_Req(15, Quest_ReceiveReward_Res); //퀘스트 10회달성의 퀘스트아이디는 15
            //}
        }


        /// <summary>
        /// 하루한번 로그인시 리워드
        /// </summary>
        /// <param name="isFirstLogin"></param>
        public void Quest_LoginReward(int isFirstLogin)
        {
            //Quest_GetMemberInfo_Req((res) =>
            //{
            //    Quest_GetMemberInfo_Res(res);
            //    if (isFirstLogin == 1) //퍼스트로그인 체커
            //    {
            //        Quest_Process_Req(1, Quest_Process_Res); //로그인 퀘스트 프로세스
            //    }
            //});
        }

        /// <summary>
        /// 내 퀘스트현황 가져오기 리퀘스트
        /// </summary>
        /// <param name="_res"></param>
        public void Quest_GetMemberInfo_Req(Action<QuestGetMemberInfoRes> _res, Action<string> _error = null)
        {
            //Util.RunCoroutine(Co_PostPacketRes<QuestGetMemberInfoRes>(contentServerUrl, "/api/quest/getMemberInfo", GetDefaultPacketReq(), (res) => _res.Invoke(res), (error) => _error?.Invoke(error)), "Quest_GetMemberInfo_Req");
        }

        /// <summary>
        /// 내 퀘스트현황 가져오기 리스판스
        /// </summary>
        /// <param name="res"></param>
        public void Quest_GetMemberInfo_Res(QuestGetMemberInfoRes res)
        {
            ////퀘스트데이터 비교하기 위해 저장
            //LocalPlayerData.memberQuest = (MemberQuest[])res.memberQuest.Clone();

            ////퀘스트보상여부 판단(퀘스트 뉴 마크 활성화 여부)
            //LocalPlayerData.existReward = false;
            //List<MemberQuest> memberQuestList = res.memberQuest.ToList();
            //for (int i = 0; i < memberQuestList.Count; i++)
            //{
            //    MemberQuest memberQuest = memberQuestList[i];
            //    if (memberQuest.isCompleted == 1)
            //    {
            //        LocalPlayerData.existReward = true;
            //        break;
            //    }
            //}
            //Panel_HUD panel_HUD = SceneLogic.instance.GetPanel<Panel_HUD>(Cons.Panel_HUD);
            //if (panel_HUD != null)
            //{
            //    panel_HUD.Menu.go_QuestNew.SetActive(LocalPlayerData.existReward);
            //}

        }

        /// <summary>
        /// 퀘스트 진행하기 리퀘스트
        /// </summary>
        /// <param name="_res"></param>
        public void Quest_Process_Req(int questConditionType, Action<QuestProcessRes> _res, Action<string> _error = null)
        {
            ////해야할(남아있는) 퀘스트 여부 체크
            //bool todoQuest = false;


            //for (int i = 0; i < LocalPlayerData.memberQuest.Length; i++)
            //{
            //    int questId = LocalPlayerData.memberQuest[i].questId;
            //    int master_questConditionType = Single.MasterData.dataQuest.GetData(questId).questConditionType;

            //    if (questConditionType == master_questConditionType)
            //    {
            //        todoQuest = true;
            //        break;
            //    }
            //}

            //if (!todoQuest)
            //{
            //    return;
            //}

            ////익명클래스
            //var questProcessReq = new
            //{
            //    memberId = ClsCrypto.EncryptByAES(LocalPlayerData.MemberID),
            //    jwtAccessToken = ClsCrypto.EncryptByAES(Token),
            //    sessionId = ClsCrypto.EncryptByAES(LocalPlayerData.SessionID),
            //    questConditionType = questConditionType,
            //};

            //Util.RunCoroutine(Co_PostPacketRes<QuestProcessRes>(contentServerUrl, "/api/quest/process", questProcessReq, (res) => _res.Invoke(res), (error) => _error?.Invoke(error)), "Quest_Process_Req");
        }

        /// <summary>
        /// 퀘스트 진행하기 리스판스
        /// </summary>
        /// <param name="res"></param>
        public void Quest_Process_Res(QuestProcessRes res)
        {
            //if (res.error == WebError.NET_E_SUCCESS)
            //{
            //    if (res.MemberQuest != null && res.MemberQuest.isCompleted == 1)
            //    {
            //        //토스트팝업으로 퀘스트완료 알림 호출
            //        int questNameType = Single.MasterData.dataQuest.GetData(res.MemberQuest.questId).questNameType;
            //        string questName = Single.MasterData.dataQuestNameType.GetData(questNameType).nameId.ToString();
            //        SceneLogic.instance.PushToastPopup(Cons.ToastPopupHorizontal, new PopupToastData(TOASTTYPE.Current, new LocalData(Cons.Local_Arzmeta, "20015", new LocalData(Cons.Local_Quest, questName))));

            //        //이미 완료한 그룹의 퀘스트를 재 Quest_Process_Req 하지 않기 위함
            //        Quest_GetMemberInfo_Req(Quest_GetMemberInfo_Res);
            //    }
            //}
        }

        /// <summary>
        /// 퀘스트 보상받기 리퀘스트
        /// </summary>
        /// <param name="_res"></param>
        public void Quest_ReceiveReward_Req(int questId, Action<QuestReceiveRewardRes> _res, Action<string> _error = null)
        {
            //익명클래스
            //var receiveRewardReq = new
            //{
            //    memberId = ClsCrypto.EncryptByAES(LocalPlayerData.MemberID),
            //    jwtAccessToken = ClsCrypto.EncryptByAES(LocalPlayerData.JwtAccessToken),
            //    sessionId = ClsCrypto.EncryptByAES(LocalPlayerData.SessionID),
            //    questId = questId,
            //};

            //Util.RunCoroutine(Co_PostPacketRes<QuestReceiveRewardRes>(contentServerUrl, "/api/quest/receiveReward", receiveRewardReq, (res) => _res.Invoke(res), (error) => _error?.Invoke(error)));
        }

        /// <summary>
        /// 퀘스트 보상받기 리스판스
        /// </summary>
        /// <param name="res"></param>
        public void Quest_ReceiveReward_Res(QuestReceiveRewardRes res)
        {
            //리워드받기 성공시
            //if (res.error == WebError.NET_E_SUCCESS)
            //{
            //    //리워드상품이 있을 시
            //    if (res.memberAvatarInven != null)
            //    {
            //        //로컬인벤 널체크
            //        if (LocalPlayerData.avatarInvens == null)
            //        {
            //            LocalPlayerData.avatarInvens = new List<AvatarInvens>();
            //        }

            //        //내 로컬인벤에 넣어준다.
            //        LocalPlayerData.avatarInvens.Add(new AvatarInvens { avatarPartsId = res.memberAvatarInven.avatarPartsId });
            //    }
            //}
        }
        #endregion 

        #region IAP 인앱결제 영수증(구글, 애플)
        public void IAP_Receipt_Req(IAP_GoogleReceiptReq iAP_GoogleReceiptReq, Action<IAP_GoogleReceiptRes> callback)
        {
            string preUrl = "https://dev-api-homepage.arzmeta.net";

            SendRequest(new SendRequestData(eHttpVerb.POST, preUrl, "/api/payment/iap/google", iAP_GoogleReceiptReq), callback, Error_Res);
        }

        public void IAP_ReceiptSubscription_Req(IAP_GoogleReceiptReq iAP_GoogleReceiptReq, Action<IAP_GoogleReceiptRes> callback)
        {
            string preUrl = "https://dev-api-homepage.arzmeta.net";

            SendRequest(new SendRequestData(eHttpVerb.POST, preUrl, "/api/payment/iap/google/billing", iAP_GoogleReceiptReq), callback, Error_Res);
        }
        #endregion

        #region 실시간 서버 
        /// <summary>
        /// 실시간 서버 정보 가져오기 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="isCloud"></param>
        /// <param name="type"></param>
        /// <param name="_res"></param>
        /// <param name="_error"></param>
        public void GetServerInfo<T, T1>(bool isCloud, string type = "", Action<T> _res = null, Action<T1> _error = null, bool isDim = false)
        {
            //var url = string.Empty;

            //if (isCloud)
            //{
            //    url = Single.Web.LobbyServerUrl + Cons.QUERY_SERVER_TYPE + type;

            //    var packet = new
            //    {
            //        memberId = ClsCrypto.EncryptByAES(LocalPlayerData.MemberID),
            //        jwtAccessToken = ClsCrypto.EncryptByAES(LocalPlayerData.JwtAccessToken),
            //        sessionId = ClsCrypto.EncryptByAES(LocalPlayerData.SessionID)
            //    };

            //    SendPostRequest<T, T1>(url, string.Empty, packet, _res, _error, isDim);
            //}
            //else
            //{
            //    url = Cons.RequestInnerServerUrl;
            //    SendGetRequest<T, T1>(url, string.Empty, _res, _error, isDim);
            //}
        }


        /// <summary>
        /// 오피스 서버 생성 요청 할 때
        /// </summary>
        /// <param name="data"></param>
        /// <param name="_res"></param>
        //public void CreateOfficeRoom(OfficeRoomDataTest data, Action<CreatedRoomRes> _res, Action<string, int> _error = null)
        //{
        //    List<ModuleReq> moduleLists = new List<ModuleReq>();

        //    BaseModuleReq baseModuleReq = new BaseModuleReq("Base", data.scenes, data.interval);
        //    moduleLists.Add(baseModuleReq);

        //    ChatModuleReq chatModuleReq = new ChatModuleReq("Chat");
        //    moduleLists.Add(chatModuleReq);

        //    OfficeModuleReq meetingModuleReq = new OfficeModuleReq("Meeting", data);
        //    moduleLists.Add(meetingModuleReq);

        //    //MakeRooms packet = new MakeRooms("test");
        //    //SendPostRequest<CreatedRoomRes>(data.url, string.Empty, packet, (res) => _res.Invoke(res), (str, res) => _error?.Invoke(str, res));
        //}

        /// <summary>
        /// 게임룸 생성 요청할 때 사용
        /// </summary>
        /// <param name="data"></param>
        /// <param name="_res"></param>
        //public void CreateGameRoom(MakeGameRoomData data, Action<CreatedRoomRes> _res, Action<string, int> _error = null)
        //{
        //    List<ModuleReq> moduleLists = new List<ModuleReq>();
        //    BaseModuleReq baseModuleReq = new BaseModuleReq("Base", data.scenes, data.interval);

        //    moduleLists.Add(baseModuleReq);

        //    GameModuleReq gameModuleReq = new GameModuleReq(data.serverType, data.roomName, data.maxPlayerNumber);
        //    moduleLists.Add(gameModuleReq);

        //    //MakeRooms packet = new MakeRooms("test");

        //    //Debug.Log(data.serverUrl);
        //    //SendPostRequest<CreatedRoomRes>(data.serverUrl, string.Empty, packet, (res) => _res.Invoke(res), (str, res) => _error?.Invoke(str, res));
        //}

        /// <summary>
        /// 마이룸 생성 요청할 때 사용
        /// </summary>
        /// <param name="data"></param>
        /// <param name="_res"></param>
        //public void CreateRoom(RoomData data, Action<CreatedRoomRes> _res, Action<string, int> _error = null)
        //{
        //    List<ModuleReq> moduleLists = new List<ModuleReq>();
        //    //BaseModuleReq baseModuleReq = new BaseModuleReq("Base", data.scenes, data.interval);
        //    //moduleLists.Add(baseModuleReq);

        //    //MyroomModuleReq myroomModuleReq = new MyroomModuleReq("Myroom", data.roomCode);
        //    //moduleLists.Add(myroomModuleReq);

        //    ChatModuleReq chatModuleReq = new ChatModuleReq("Chat");
        //    moduleLists.Add(chatModuleReq);

        //    //MakeRooms packet = new MakeRooms("test");

        //    //SendPostRequest<CreatedRoomRes>("", string.Empty, packet, (res) => _res.Invoke(res), (str, res) => _error?.Invoke(str, res));
        //}

        #endregion

        #region 심심이_SimSimi

        /// <summary>
        /// 심심이 메세지 전송
        /// </summary>
        /// <param name="utext"></param>
        /// <param name="_res"></param>
        /// <param name="_error"></param>
        public async void SendSimSimi(string utext, Action<ResSimSimi> _res, Action<ErrorSimSimi> _error)
        {
            //method(get, post, put...)
            string method = UnityWebRequest.kHttpVerbPOST;

            //header
            var header = new List<(string, string)>();
            header.Add(("Content-Type", "application/json"));
            header.Add(("x-api-key", "d~9wXsekDYKO2tpXUCrirlmL7FThVaobL8B4AG~P"));

            //url
            string version = "190410";
            string url = $"https://wsapi.simsimi.com/{version}/talk";

            //utext
            string lang = AppGlobalSettings.Instance.language == Language.Korean ? "ko" : "en";
            ReqSimSimi reqSimSimi = new ReqSimSimi(utext, lang);

            await AsyncRequest(method, header, url, reqSimSimi, _res, _error);
        }
        
        public async void SendChatGPT(string message, Action<ResChatGPT> _res, Action<ErrorSimSimi> _error)
        {
            //method(get, post, put...)
            string method = UnityWebRequest.kHttpVerbPOST;

            //header
            var header = new List<(string, string)>();
            header.Add(("Content-Type", "application/json"));
            header.Add(("secret", "secret"));

            //url
            string url = "https://api.openai.com/v1/chat/completions";

            //message
            ReqChatGPT reqChatGPT = new ReqChatGPT(message);

            await AsyncRequest(method, header, url, reqChatGPT, _res, _error);
        }



        /// <summary>
        /// 비동기 리퀘스트
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="method"></param>
        /// <param name="header"></param>
        /// <param name="url"></param>
        /// <param name="obj"></param>
        /// <param name="res"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        private async UniTask AsyncRequest<T1, T2>(string method, List<(string, string)> header, string url, object obj, Action<T1> res, Action<T2> error)
        {
            string requestJson = string.Empty;
            string responseJson = string.Empty;

            //데이터
            byte[] jsonBytes = null;
            if (obj != null)
            {
                requestJson = JsonConvert.SerializeObject(obj, Formatting.Indented);
                jsonBytes = Encoding.UTF8.GetBytes(requestJson);
            }

            DEBUG.LOG(url + "\n" + requestJson + "\n", eColorManager.Web_Request);

            UnityWebRequest uwr = new UnityWebRequest(url, method);
            uwr.timeout = 10;
            uwr.uploadHandler = new UploadHandlerRaw(jsonBytes);
            uwr.downloadHandler = new DownloadHandlerBuffer();

            //헤더
            foreach (var item in header)
            {
                uwr.SetRequestHeader(item.Item1, item.Item2);
            }

            try
            {
                await uwr.SendWebRequest();
            }
            catch (UnityWebRequestException exception)
            {

                switch (exception.Result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.ProtocolError:
                    case UnityWebRequest.Result.DataProcessingError:
                        {
                            DEBUG.LOG($"error : {exception.Error}, text : {exception.Text}", eColorManager.Web_Response);
                            //await HandleError(exception.Error, exception.Text);
                        }
                        break;
                    default:
                        break;
                }
            }
            finally
            {
                await UniTask.WaitUntil(() => uwr.isDone);

                responseJson = Util.Beautify(uwr.downloadHandler.text);
                DEBUG.LOG(url + "\n" + responseJson + "\n", eColorManager.Web_Response);

                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        T1 resObj = JsonConvert.DeserializeObject<T1>(responseJson);
                        if (resObj != null)
                        {
                            res?.Invoke(resObj);
                        }
                        else
                        {
                            SceneLogic.instance.PushPopup<Popup_Basic>()
                                .ChainPopupData(new PopupData(POPUPICON.WARNING, BTN_TYPE.Confirm, masterLocalDesc: new MasterLocalData("common_error_network_01")));
                        }
                    }
                    catch
                    {
                        DEBUG.LOGERROR("url 에러" + url);
                    }
                }
                else
                {
                    T2 resObj = JsonConvert.DeserializeObject<T2>(responseJson);
                    if (resObj != null)
                    {
                        error?.Invoke(resObj);
                    }
                }
            }
        }
        #endregion
        #endregion

        #endregion

        #region 기능 메소드
        /// <summary>
        /// 가입하는 플랫폼에 따른 RegPathType 리턴 
        /// </summary>
        /// <returns></returns>
        public REGPATH_TYPE GetRegPathType()
        {
#if UNITY_EDITOR || !UNITY_ANDROID && !UNITY_IOS
            return REGPATH_TYPE.Etc;
#elif UNITY_ANDROID
            return REGPATH_TYPE.Android;
#elif UNITY_IOS
            return REGPATH_TYPE.IOS;
#endif
        }

        private void OpenReturnToLogoPopup()
        {
            SceneLogic.instance.PushPopup<Popup_Basic>()
                .ChainPopupData(new PopupData(POPUPICON.WARNING, BTN_TYPE.Confirm, masterLocalDesc: new MasterLocalData(Cons.Local_Arzmeta, "common_error_server_02")))
                .ChainPopupAction(new PopupAction(Util.ReturnToLogo));
        }

        /// <summary>
        /// 현재 씬이 타이틀일 시 에러가 발생했을 때 타이틀 패널 리프레쉬
        /// </summary>
        private void RefrashTitle()
        {
            var panel_Title = SceneLogic.instance.GetPanel<Panel_Title>();
            if (panel_Title != null)
            {
                panel_Title.RefreshUI(true);
                Single.Scene.SetDimOff();
            }
        }
        #endregion
    }
}


//using CryptoWebRequestSample;
//using FrameWork.UI;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using UnityEngine;
//using UnityEngine.Networking;
//using System.Threading.Tasks;
//using Cysharp.Threading.Tasks;
//using MEC;
//using Newtonsoft.Json;
//using UnityEngine.Events;
//using Newtonsoft.Json.Linq;
//using System.Threading;
//using System.IO;
//using Assets._Launching.DEV.Script.Framework.Network.WebPacket;
//using System.Linq;
//using Gpm.Ui.Sample;
//using Protocol;

//namespace FrameWork.Network
//{
//    public class WebManager : Singleton<WebManager>
//    {
//        #region Unity 기본 메소드
//        private void Update()
//        {
//            if (Input.GetKeyDown(KeyCode.P))
//            {
//                //member.CheckNickName("한쏘매액", null);
//            }
//        }

//        protected override void OnDestroy()
//        {
//            base.OnDestroy();

//            webPostbox = null;
//        }

//        protected override void OnApplicationQuit()
//        {
//            base.OnApplicationQuit();

//            Timing.KillCoroutines();
//            StopAllCoroutines();
//            Resources.UnloadUnusedAssets();
//        }
//        #endregion

//        #region 변수
//        public bool IsGateWayInfoResDone { get; private set; }

//        private enum dimdim
//        {
//            on,
//            off,
//            auto,
//            onoff,
//        }

//        private bool isReceived;
//        private string mainURL;
//        private string GatewayInfoURL = "https://arzmetasta.blob.core.windows.net/arzmeta-container/gateway/gatewayInfo.txt";

//        #region GateWayData
//        public GatewayInfo GatewayInfo { get; private set; }

//        private ServerInfo ServerInfo => GatewayInfo.Gateway.ServerType.ServerInfo;
//        // Url
//        public string AccountServerUrl => ServerInfo.accountServerUrl;
//        public string PCAccountServerUrl => ServerInfo.pcAccountServerUrl;
//        public string AgoraServerUrl => ServerInfo.agoraServerUrl;
//        public string ContentServerUrl => ServerInfo.contentServerUrl;
//        public string LobbyServerUrl => ServerInfo.lobbyServerUrl;
//        public int GameServerPort => ServerInfo.gameServerPort;
//        public string StorageUrl => ServerInfo.storageUrl;
//        public void SetStorageUrl(string str)
//        {
//            ServerInfo.storageUrl = str;
//        }
//        public string HomepageUrl => ServerInfo.homepageUrl;
//        public string WebviewUrl => ServerInfo.webviewUrl;
//        public string LinuxServerIp => ServerInfo.linuxServerIp;
//        public int LinuxServerPort => ServerInfo.linuxServerPort;
//        public int LinuxHttpPort => ServerInfo.linuxHttpPort;
//        public string HomepageBackendUrl => ServerInfo.homepageBackendUrl;
//        public string WebSocketUrl => ServerInfo.webSocketUrl;

//        // Make 요청할 때 사용하는 Url,Port
//        // 점핑매칭
//        public string MatchingServerUrl => ServerInfo.matchingServerUrl + ":" + MatchingServerPort + Cons.RequestMakeRoomStr;
//        public int MatchingServerPort => ServerInfo.matchingServerPort;
//        // OX 퀴즈
//        public string OXServerUrl => ServerInfo.OXServerUrl + ":" + MatchingServerPort + Cons.RequestMakeRoomStr;
//        public int OXServerPort => ServerInfo.OXServerPort;
//        // 오피스
//        public string MeetingRoomServerUrl => ServerInfo.meetingRoomServerUrl + ":" + MeetingRoomServerport + Cons.RequestMakeRoomStr;
//        public int MeetingRoomServerport => ServerInfo.meetingRoomServerPort;

//        // 상담실
//        public string MedicineRoomServerUrl => ServerInfo.medicineUrl + ":" + MedicineRoomServerport + Cons.RequestMakeRoomStr;
//        public int MedicineRoomServerport => ServerInfo.medicinePort;

//        // 마이룸
//        public string MyRoomServerUrl => ServerInfo.myRoomServerUrl + ":" + MyRoomServerport + Cons.RequestMakeRoomStr;
//        public int MyRoomServerport => ServerInfo.myRoomServerPort;
//        #endregion
//        #endregion

//        #region 초기화
//        public void Initialize()
//        {
//            SetUpGateWayInfo();
//        }

//        /// <summary>
//        /// 게이트웨이 데이터 가져오는 메소드.맨 처음 호출해줘야함
//        /// </summary>
//        private async void SetUpGateWayInfo()
//        {
//            IsGateWayInfoResDone = false;
//            mainURL = string.Empty;

//            await WebRequestWithOutJsonPasing(GatewayInfoURL, UnityWebRequest.kHttpVerbGET, null, SetMainUrl);

//            await UniTask.WaitUntil(() => mainURL != string.Empty);

//            var packet = new
//            {
//                osType = GetOsType(),
//                appVersion = Application.version,
//            };

//            await WebRequestWithOutJsonPasing(mainURL, UnityWebRequest.kHttpVerbPOST, packet, SerializeGatewayInfo);
//        }

//        /// <summary>
//        /// 현재 플랫폼에 따른 OsType 리턴 
//        /// </summary>
//        /// <returns></returns>
//        private OS_TYPE GetOsType()
//        {
//#if UNITY_EDITOR || !UNITY_ANDROID && !UNITY_IOS
//            return OS_TYPE.Window;
//#elif UNITY_ANDROID
//            return OS_TYPE.Android;
//#elif UNITY_IOS
//            return OS_TYPE.IOS;
//#endif
//        }

//        /// <summary>
//        /// mainURL 세팅
//        /// </summary>
//        /// <param name="result"></param>
//        private void SetMainUrl(string result)
//        {
//            if (string.IsNullOrEmpty(result))
//            {
//                OpenReturnToLogoPopup();
//                return;
//            }

//            mainURL = WebPacket.Gateway(ClsCrypto.DecryptByAES(result));
//        }

//        /// <summary>
//        /// 게이트웨이 데이터 파싱 및 상태에 따른 처리
//        /// </summary>
//        /// <param name="result"></param>
//        private void SerializeGatewayInfo(string result)
//        {
//            if (string.IsNullOrEmpty(result))
//            {
//                OpenReturnToLogoPopup();
//                return;
//            }

//            var jobj = (JObject)JsonConvert.DeserializeObject(result);
//            foreach (var x in jobj)
//            {
//                switch (x.Key)
//                {
//                    case "Gateway":
//                        GatewayInfo = JsonConvert.DeserializeObject<GatewayInfo>(result);

//                        CheckAppVersionDataReset(GatewayInfo.Gateway.appVersion);

//                        switch ((SERVER_STATE)GatewayInfo.Gateway.ServerState.state)
//                        {
//                            case SERVER_STATE.ACTIVATE: IsGateWayInfoResDone = true; break;
//                            case SERVER_STATE.INACTIVATE: SetGatewayPopup(GatewayInfo.Gateway.StateMessage.message, () => Util.QuitApplication()); break;
//                            case SERVER_STATE.TEST: Debug.Log("Gateway: 테스트 수락된 유저만 진입할 수 있습니다. 본 기능은 아직 개발되지 않았습니다."); break;
//                            case SERVER_STATE.NEED_UPDATE: SetGatewayPopup(GatewayInfo.Gateway.StateMessage.message, () => UpdateVersion(GatewayInfo.Gateway.OsType.storeUrl)); break;
//                            default: break;
//                        }
//                        break;
//                    default:
//                        GatewayInfo_Update gatewayInfo_Update = JsonConvert.DeserializeObject<GatewayInfo_Update>(result);
//                        SetGatewayPopup(gatewayInfo_Update.StateMessage.message, () => UpdateVersion(gatewayInfo_Update.OsType.storeUrl));
//                        break;
//                }
//                break;
//            }
//        }

//        /// <summary>
//        /// 앱버전이 설치되어있던 것과 다를 시 로그아웃
//        /// </summary>
//        private void CheckAppVersionDataReset(string appVersion)
//        {
//            if (PlayerPrefs.GetString("AppVersion", "0.0.0") != appVersion)
//            {
//                LocalPlayerData.ResetData();
//            }
//            PlayerPrefs.SetString("AppVersion", appVersion);
//        }

//        private void SetGatewayPopup(string message, UnityAction action = null)
//        {
//            SceneLogic.instance.PushPopup<Popup_Basic>()
//                     .ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.Confirm, masterLocalDesc: new MasterLocalData(message)))
//                     .ChainPopupAction(new PopupAction(action));
//        }

//        /// <summary>
//        /// GatewayInfoURL 받아오는 웹리퀘스트 (json 파싱 안함)
//        /// </summary>
//        /// <param name="mainUrl"></param>
//        /// <param name="res"></param>
//        /// <returns></returns>
//        private async Task WebRequestWithOutJsonPasing(string mainUrl, string method, object obj, Action<string> res = null)
//        {
//            string responeJson = string.Empty;
//            byte[] jsonBytes = null;

//            #region 데이터 패킹 및 세팅
//            if (obj != null)
//            {
//                var requestJson = JsonConvert.SerializeObject(obj, Formatting.Indented);
//                jsonBytes = Encoding.UTF8.GetBytes(requestJson);
//                DEBUG.LOG(mainUrl + "\n" + requestJson + "\n", eColorManager.Web_Request);
//            }

//            using var uwr = new UnityWebRequest(mainUrl, method);
//            uwr.timeout = 10;
//            uwr.uploadHandler = new UploadHandlerRaw(jsonBytes);
//            uwr.downloadHandler = new DownloadHandlerBuffer();
//            uwr.SetRequestHeader("Content-Type", "application/json");
//            #endregion

//            try
//            {
//                await uwr.SendWebRequest();
//            }
//            catch (UnityWebRequestException exception)
//            {
//                if (exception.Result == UnityWebRequest.Result.ConnectionError || exception.Result == UnityWebRequest.Result.ProtocolError)
//                {
//                    DEBUG.LOG(exception.Error, eColorManager.Web_Response);
//                    DEBUG.LOG(exception.Text, eColorManager.Web_Response);

//                    if (SceneLogic.instance.GetPopup<Popup_Basic>().CheckHttpResponseError(exception.Error)) return;
//                }
//            }
//            finally
//            {
//                await UniTask.WaitUntil(() => uwr.isDone);

//                if (uwr.result == UnityWebRequest.Result.Success)
//                {
//                    responeJson = uwr.downloadHandler.text;
//                    DEBUG.LOG(mainUrl + "\n" + Util.Beautify(responeJson) + "\n", eColorManager.Web_Response);
//                    res?.Invoke(responeJson);
//                }
//            }
//        }

//        public void UpdateVersion(string storeUrl)
//        {
//            Application.OpenURL(storeUrl);
//            Application.Quit(); // 강종
//        }
//        #endregion

//        #region [코어 메소드] Request&Response
//        public class SendRequestData<T_Res, T_Error>
//        {
//            public string mainUrl = string.Empty;
//            public string subUrl = string.Empty;
//            public object packet = null;

//            public Action<T_Res> _res = null;
//            public Action<T_Error> _error = null;

//            public bool dim = true;
//            public bool isErrorPopup = true;

//            public SendRequestData(string mainUrl, string subUrl, object packet = null, Action<T_Res> _res = null, Action<T_Error> _error = null, bool dim = true, bool isErrorPopup = true)
//            {
//                this.mainUrl = mainUrl;
//                this.subUrl = subUrl;
//                this.packet = packet;

//                this._res = _res;
//                this._error = _error;

//                this.dim = dim;
//                this.isErrorPopup = isErrorPopup;
//            }
//        }

//        /// <summary>
//        ///  Post 리퀘스트
//        /// </summary>
//        public async Task Co_PostPacketRes<T_Res, T_Error>(string mainUrl, string subUrl, object packet, Action<T_Res> _res, Action<T_Error> _error, bool dim = true, bool isErrorPopup = true)
//        {
//            isReceived = false;

//            await UniTask.Delay(200);

//            SendPostRequest<T_Res, T_Error>(mainUrl, subUrl, packet, (res) => _res.Invoke(res), (res) => _error?.Invoke(res), dim, isErrorPopup);

//            await UniTask.WaitUntil(() => isReceived);
//        }

//        /// <summary>
//        /// Post 리퀘스트
//        /// </summary>
//        public async void SendPostRequest<T_Res, T_Error>(string mainUrl, string subUrl, object obj, Action<T_Res> res, Action<T_Error> error, bool dim = true, bool isErrorPopup = true)
//        {
//            await CoSendWebRequest(mainUrl, subUrl, UnityWebRequest.kHttpVerbPOST, obj, res, error, dim, isErrorPopup);
//        }

//        /// <summary>
//        /// Get 리퀘스트
//        /// </summary>
//        public async void SendGetRequest<T_Res, T_Error>(string mainUrl, string subUrl, Action<T_Res> res, Action<T_Error> error, bool dim = true, bool isErrorPopup = true)
//        {
//            await CoSendWebRequest(mainUrl, subUrl, UnityWebRequest.kHttpVerbGET, null, res, error, dim, isErrorPopup);
//        }

//        /// <summary>
//        /// Put 리퀘스트
//        /// </summary>
//        public async void SendPutRequest<T_Res, T_Error>(string mainUrl, string subUrl, object obj, Action<T_Res> res, Action<T_Error> error, bool dim = true, bool isErrorPopup = true)
//        {
//            await CoSendWebRequest(mainUrl, subUrl, UnityWebRequest.kHttpVerbPUT, obj, res, error, dim, isErrorPopup);
//        }

//        /// <summary>
//        /// Delete 리퀘스트
//        /// </summary>
//        public async void SendDeleteRequest<T_Res, T_Error>(string mainUrl, string subUrl, Action<T_Res> res, Action<T_Error> error, bool dim = true, bool isErrorPopup = true)
//        {
//            await CoSendWebRequest(mainUrl, subUrl, UnityWebRequest.kHttpVerbDELETE, null, res, error, dim, isErrorPopup);
//        }

//        async Task CoSendWebRequest<T_Res, T_Error>(string mainUrl, string subUrl, string method, object obj, Action<T_Res> res, Action<T_Error> error, bool dim = true, bool isErrorPopup = true)
//        {
//            string sendUrl = mainUrl + subUrl;
//            string responseJson = string.Empty;
//            byte[] jsonBytes = null;

//            if (dim) Single.Scene.SetDimOn(1f);

//            SetIsErrorPopup(subUrl, isErrorPopup);

//            #region 데이터 패킹 및 세팅
//            if (obj != null)
//            {
//                var requestJson = JsonConvert.SerializeObject(obj, Formatting.Indented);
//                jsonBytes = Encoding.UTF8.GetBytes(requestJson);
//                DEBUG.LOG(subUrl + "\n" + requestJson + "\n", eColorManager.Web_Request);
//            }

//            var uwr = new UnityWebRequest(sendUrl, method)
//            {
//                uploadHandler = new UploadHandlerRaw(jsonBytes),
//                downloadHandler = new DownloadHandlerBuffer()
//            };

//            uwr.SetRequestHeader("Content-Type", "application/json");
//            if (!string.IsNullOrEmpty(LocalPlayerData.JwtAccessToken))
//            {
//                uwr.SetRequestHeader("jwtAccessToken", ClsCrypto.EncryptByAES(LocalPlayerData.JwtAccessToken));
//            }
//            if (!string.IsNullOrEmpty(LocalPlayerData.SessionID))
//            {
//                uwr.SetRequestHeader("sessionId", ClsCrypto.EncryptByAES(LocalPlayerData.SessionID));
//            }

//            // uwr.timeout = 10; // 타임 아웃을 UniTask에서 처리하도록 수정. BKK
//            var cts = new CancellationTokenSource();
//            cts.CancelAfterSlim(TimeSpan.FromSeconds(10)); // 타임 아웃 시간. BKK
//            #endregion

//            try
//            {
//                await uwr.SendWebRequest().WithCancellation(cts.Token);
//            }
//            catch (UnityWebRequestException exception)
//            {
//                if (exception.Result == UnityWebRequest.Result.ConnectionError ||
//                                       exception.Result == UnityWebRequest.Result.ProtocolError ||
//                                       exception.Result == UnityWebRequest.Result.DataProcessingError)
//                {
//                    DEBUG.LOG(subUrl + " " + exception.Error, eColorManager.Web_Response);
//                    DEBUG.LOG(exception.Text, eColorManager.Web_Response);

//                    isReceived = false;

//                    if (GetIsErrorPopup(subUrl)) await HandleError(exception.Error, exception.Text);

//                    if (SceneLogic.instance.GetSceneType() == SceneName.Scene_Base_Title) RefrashTitle();
//                }
//            }
//            // 타임 아웃 예외 처리
//            catch (OperationCanceledException exception)
//            {
//                if (exception.CancellationToken == cts.Token)
//                {
//                    Debug.Log("TimeOut!!");
//                    isReceived = false;

//                    await HandleError("Request timeout", "{ error = 0, errorMessage = \"\"}");
//                }
//            }
//            finally
//            {
//                await UniTask.WaitUntil(() => uwr.isDone);

//                if (dim) Single.Scene.SetDimOff(1f);

//                responseJson = Util.Beautify(uwr.downloadHandler.text);
//                DEBUG.LOG(subUrl + "\n" + responseJson + "\n", eColorManager.Web_Response);

//                if (uwr.result == UnityWebRequest.Result.Success)
//                {
//                    T_Res resObj = (typeof(T_Res) == typeof(string)) ? (T_Res)(object)responseJson : JsonConvert.DeserializeObject<T_Res>(responseJson);

//                    if (resObj != null)
//                    {
//                        res?.Invoke(resObj);
//                    }
//                    else
//                    {
//                        SceneLogic.instance.PushPopup<Popup_Basic>()
//                            .ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.Confirm, masterLocalDesc: new MasterLocalData("common_error_network_01")));
//                    }

//                    isReceived = true;
//                }
//                else
//                {
//                    var resObj = JsonConvert.DeserializeObject<T_Error>(responseJson);

//                    if (resObj != null)
//                    {
//                        error?.Invoke(resObj);
//                    }
//                }
//            }
//        }

//        public async void SendWebRequest_MultiPartFormData<T_Res, T_Error>(string mainUrl, string subUrl, string method, object req, Action<T_Res> res, Action<T_Error> error, bool dim = true, string thumbnailPath = "", Texture2D iosTex = null)
//        {
//            await Co_SendPostRequest_MultiPartFormData(mainUrl, subUrl, method, req, res, error, dim, thumbnailPath, iosTex);
//        }

//        /// <summary>
//        /// 멀티파트 폼데이터
//        /// </summary>
//        /// <typeparam name="T_Res"></typeparam>
//        /// <param name="mainUrl"></param>
//        /// <param name="subUrl"></param>
//        /// <param name="req"></param>
//        /// <param name="res"></param>
//        /// <param name="error"></param>
//        /// <param name="dim"></param>
//        /// <param name="thumbnailPath"></param>
//        /// <returns></returns>
//        async Task Co_SendPostRequest_MultiPartFormData<T_Res, T_Error>(string mainUrl, string subUrl, string method, object req, Action<T_Res> res, Action<T_Error> error, bool dim = true, string thumbnailPath = "", Texture2D iosTex = null)
//        {
//            string requestJson;
//            string url = mainUrl + subUrl;

//            if (dim) Single.Scene.SetDimOn(1f);

//            #region 데이터 패킹 및 세팅
//            //1. 멀티파트 폼데이터 셋업
//            List<IMultipartFormSection> formData = new List<IMultipartFormSection>();

//            if (thumbnailPath != "")
//            {
//#if UNITY_IOS
//                byte[] image = iosTex.EncodeToPNG();
//#else
//                byte[] image = File.ReadAllBytes(thumbnailPath);
//#endif
//                string fileName = Uri.EscapeUriString(Path.GetFileName(thumbnailPath));
//#if UNITY_IOS
//                fileName = Util.ConvertIOSExtension(fileName);
//#endif
//                string extension = "image/" + Path.GetExtension(fileName).Split('.').Last();
//                formData.Add(new MultipartFormFileSection("image", image, fileName, extension));
//            }


//            requestJson = JsonConvert.SerializeObject(req, Formatting.Indented);

//            DEBUG.LOG(subUrl + "\n" + requestJson + "\n", eColorManager.Web_Request);

//            Dictionary<string, string> jsonDic = JsonConvert.DeserializeObject<Dictionary<string, string>>(requestJson);
//            foreach (var item in jsonDic)
//            {
//                //Debug.Log("item.Key : " + item.Key);
//                //Debug.Log("item.Value : " + item.Value);
//                formData.Add(new MultipartFormDataSection(item.Key, item.Value));
//            }

//            //2. 폼데이터에 맞게 바운더리 작업
//            byte[] boundary = UnityWebRequest.GenerateBoundary();
//            byte[] formSections = UnityWebRequest.SerializeFormSections(formData, boundary);//3. 폼데이터에 맞게 업로드 핸들러

//            //uwr = UnityWebRequest.Post(url, formData);
//            UnityWebRequest uwr = new UnityWebRequest(url, method)
//            {
//                timeout = 10,
//                uploadHandler = new UploadHandlerRaw(formSections),
//                downloadHandler = new DownloadHandlerBuffer()
//            };

//            uwr.SetRequestHeader("Content-Type", $"multipart/form-data; boundary={Encoding.UTF8.GetString(boundary)}"); //4. 헤더에 바운더리 폼데이터이기때문에 바운더리 값 추가

//            if (!string.IsNullOrEmpty(LocalPlayerData.JwtAccessToken))
//            {
//                uwr.SetRequestHeader("jwtAccessToken", ClsCrypto.EncryptByAES(LocalPlayerData.JwtAccessToken));
//            }
//            if (!string.IsNullOrEmpty(LocalPlayerData.SessionID))
//            {
//                uwr.SetRequestHeader("sessionId", ClsCrypto.EncryptByAES(LocalPlayerData.SessionID));
//            }
//            #endregion

//            try
//            {
//                await uwr.SendWebRequest(); //쏜다
//            }
//            catch (UnityWebRequestException exception)
//            {
//                if (exception.Result == UnityWebRequest.Result.ConnectionError ||
//                         exception.Result == UnityWebRequest.Result.ProtocolError ||
//                         exception.Result == UnityWebRequest.Result.DataProcessingError)
//                {
//                    DEBUG.LOG(exception.Error, eColorManager.Web_Response);
//                    DEBUG.LOG(exception.Text, eColorManager.Web_Response);

//                    isReceived = false;

//                    await HandleError(exception.Error, exception.Text);

//                    Debug.Log(exception.Error);
//                }
//            }
//            finally
//            {
//                if (dim)
//                {
//                    Single.Scene.SetDimOff(1f);
//                    await UniTask.WaitWhile(() => Single.Scene.isDim);
//                }
//                await UniTask.WaitUntil(() => uwr.isDone);

//                string responseJson = Util.Beautify(uwr.downloadHandler.text);
//                DEBUG.LOG(subUrl + "\n" + responseJson + "\n", eColorManager.Web_Response);

//                if (uwr.result == UnityWebRequest.Result.Success)
//                {
//                    var resObj = JsonConvert.DeserializeObject<T_Res>(responseJson);
//                    if (resObj != null)
//                    {
//                        res.Invoke(resObj);
//                    }
//                    else
//                    {
//                        SceneLogic.instance.PushPopup<Popup_Basic>()
//                            .ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.Confirm, masterLocalDesc: new MasterLocalData("common_error_network_01")));
//                    }

//                    isReceived = true;
//                }
//                else
//                {
//                    var resObj = JsonConvert.DeserializeObject<T_Error>(responseJson);

//                    if (resObj != null)
//                    {
//                        error?.Invoke(resObj);
//                    }
//                }
//                uwr.Dispose(); //해제
//            }
//        }

//        private async Task HandleError(string errorMessage, string response)
//        {
//            DefaultPacketRes responseError = JsonConvert.DeserializeObject<DefaultPacketRes>(response);

//            // 아예 서버 응답이 없을 경우, responseError 가 null 이 들어옴
//            if (responseError == null)
//            {
//                OpenReturnToLogoPopup();
//            }
//            else
//            {
//                // http 서버 에러
//                // 웹 서버 에러
//                if (SceneLogic.instance.GetPopup<Popup_Basic>().CheckHttpResponseError(errorMessage) ||
//                    SceneLogic.instance.GetPopup<Popup_Basic>().CheckResponseError(responseError.error))
//                {
//                    member.StopHeartBeat();
//                }
//            }

//            await UniTask.DelayFrame(1);
//        }

//        public void Error_Res(DefaultPacketRes res)
//        {
//            if (Single.Scene.isDim) Single.Scene.SetDimOff();

//            Debug.Log("errorCode : " + res.error + ", errorMessage : " + res.errorMessage);
//        }

//        #region IsErrorPopup
//        // 에러 시 자동으로 뜨는 팝업을 비/활성화 할 수 있는 옵션.
//        // API 호출 시 isErrorPopup 파라미터를 작성하면 된다. true: 활성화, false: 비활성화
//        // true가 기본값

//        private Dictionary<string, bool> IsErrorPopupDic = new Dictionary<string, bool>();

//        public void SetIsErrorPopup(string key, bool isErrorPopup)
//        {
//            if (!IsErrorPopupDic.ContainsKey(key))
//            {
//                IsErrorPopupDic.Add(key, default);
//            }
//            IsErrorPopupDic[key] = isErrorPopup;
//        }

//        private bool GetIsErrorPopup(string key)
//        {
//            if (IsErrorPopupDic.ContainsKey(key))
//            {
//                return IsErrorPopupDic[key];
//            }
//            return false;
//        }
//        #endregion

//        #endregion

//        #region API

//        #region 계정 Account
//        public WebAccount account = new WebAccount();
//        #endregion

//        #region 사용자 Member
//        public WebMember member = new WebMember();
//        #endregion

//        #region 친구 Friend
//        public WebFriend friend = new WebFriend();
//        #endregion

//        #region 투표 Vote
//        public WebVote vote = new WebVote();
//        #endregion

//        #region 오피스 Office
//        public WebOffice office = new WebOffice();
//        #endregion

//        #region 마이룸 MyRoom
//        public WebMyRoom myRoom = new WebMyRoom();
//        #endregion

//        #region 타인의 정보 Others
//        public WebOthers others = new WebOthers();
//        #endregion

//        #region 랭킹(무한의 코드) Ranking
//        public WebRanking ranking = new WebRanking();
//        #endregion

//        #region 우편함 PostBox
//        public WebPostbox webPostbox = new WebPostbox();
//        #endregion

//        #region 선택 투표(KTMF) SelectVote
//        public WebSelectVote selectVote = new WebSelectVote();
//        #endregion

//        #region 광고 컨텐츠(재화, 코인) AD-Contents
//        public WebADContents adContents = new WebADContents();
//        #endregion

//        #region 유학박람회 CSAF - Chongro Study Abroad Fair
//        public WebCSAF CSAF = new WebCSAF();
//        #endregion

//        #region 숏링크
//        public WebShortLink shortLink = new WebShortLink();
//        #endregion

//        #region 보류
//        #region RealtimeTest

//        private void SendPostRequest_Test(string mainUrl, object obj, Action res, Action<string, int> error, bool dim = true)
//        {
//            Timing.RunCoroutine(CoSendWebRequest_Test(mainUrl, UnityWebRequest.kHttpVerbPOST, obj, res, error, dim));
//        }

//        IEnumerator<float> CoSendWebRequest_Test(string mainUrl, string method, object obj, Action res, Action<string, int> error, bool dim = true)
//        {
//            string sendUrl = mainUrl;
//            string requestJson = string.Empty;
//            string responeJson = string.Empty;
//            byte[] jsonBytes = null;

//            if (dim) Single.Scene.SetDimOn(1f);

//            if (obj != null)
//            {

//                string jsonStr = JsonConvert.SerializeObject(obj, Formatting.Indented);
//                jsonBytes = Encoding.UTF8.GetBytes(jsonStr);
//            }

//            DEBUG.LOG(mainUrl + "\n" + requestJson + "\n", eColorManager.Web_Request);

//            var uwr = new UnityWebRequest(sendUrl, method);
//            uwr.timeout = 10;
//            uwr.uploadHandler = new UploadHandlerRaw(jsonBytes);
//            uwr.downloadHandler = new DownloadHandlerBuffer();
//            uwr.SetRequestHeader("Content-Type", "application/json");
//            if (!string.IsNullOrEmpty(LocalPlayerData.JwtAccessToken))
//            {
//                uwr.SetRequestHeader("jwtAccessToken", ClsCrypto.EncryptByAES(LocalPlayerData.JwtAccessToken));
//            }
//            if (!string.IsNullOrEmpty(LocalPlayerData.SessionID))
//            {
//                uwr.SetRequestHeader("sessionId", ClsCrypto.EncryptByAES(LocalPlayerData.SessionID));
//            }

//            try
//            {
//                uwr.SendWebRequest();
//            }

//            catch (UnityWebRequestException exception)
//            {
//                if (exception.Result == UnityWebRequest.Result.ConnectionError ||
//                    exception.Result == UnityWebRequest.Result.ProtocolError ||
//                    exception.Result == UnityWebRequest.Result.DataProcessingError)
//                {
//                    DEBUG.LOG(exception.Error, eColorManager.Web_Response);
//                    DEBUG.LOG(exception.Text, eColorManager.Web_Response);

//                    isReceived = false;

//                    if (dim) Single.Scene.SetDimOff(2f);

//                    Timing.RunCoroutine(HandleError_Test(exception.Error, exception.Text, error));
//                }

//                Debug.Log(exception.Error);
//            }
//            finally
//            {
//                Timing.WaitUntilTrue(() => uwr.isDone);

//                if (dim) Single.Scene.SetDimOff(1f);

//                if (uwr.result == UnityWebRequest.Result.Success)
//                {
//                    responeJson = Util.Beautify(uwr.downloadHandler.text);

//                    DEBUG.LOG(mainUrl + "\n" + responeJson + "\n", eColorManager.Web_Response);

//                    isReceived = true;
//                }

//                else
//                {

//                }
//            }

//            yield return Timing.WaitUntilTrue(() => isReceived);
//        }

//        public IEnumerator<float> HandleError_Test(string errorMessage, string response, Action<string, int> _error)
//        {
//            DefaultPacketRes responseError = JsonConvert.DeserializeObject<DefaultPacketRes>(response);
//            // 아예 서버 응답이 없을 경우, responseError 가 null 이 들어옴
//            if (responseError == null)
//            {
//                OpenReturnToLogoPopup();
//                yield break;
//            }

//            // http 서버 에러
//            // 웹 서버 에러
//            if (SceneLogic.instance.GetPopup<Popup_Basic>().CheckHttpResponseError(errorMessage) ||
//                SceneLogic.instance.GetPopup<Popup_Basic>().CheckResponseError(responseError.error))
//            {
//                member.StopHeartBeat();
//                yield break;
//            }

//            yield return Timing.WaitForSeconds(1f);

//            _error?.Invoke(errorMessage, responseError.error);
//        }

//        #endregion

//        #region 휴면계정
//        /// <summary>
//        /// 휴면계정 이메일 인증
//        /// </summary>
//        /// <param name="email"></param>
//        /// <param name="_res"></param>
//        public async void DormantCheckEmail(string email, Action<CheckEmailPacketRes_Old> _res, Action<DefaultPacketRes> _error = null)
//        {
//            CheckEmailPacketReq packet = new CheckEmailPacketReq()
//            {
//                email = ClsCrypto.EncryptByAES(email)
//            };

//            await Co_PostPacketRes(AccountServerUrl, WebPacket.DormantCheckEmail, packet, _res, _error);
//        }

//        /// <summary>
//        /// 휴면계정 이메일 인증 확인
//        /// </summary>
//        /// <param name="email"></param>
//        /// <param name="authCode"></param>
//        /// <param name="_res"></param>
//        public async void DormantConfirmEmail(string email, string authCode, Action<DefaultPacketRes> _res, Action<DefaultPacketRes> _error = null)
//        {
//            AuthEmailPacketReq packet = new AuthEmailPacketReq()
//            {
//                email = ClsCrypto.EncryptByAES(email),
//                //authCode = ClsCrypto.EncryptByAES(authCode)
//            };

//            await Co_PostPacketRes(AccountServerUrl, WebPacket.DormantConfirmEmail, packet, _res, _error);
//        }

//        /// <summary>
//        /// 휴면 계정 해제
//        /// </summary>
//        /// <param name="email"></param>
//        /// <param name="_res"></param>
//        public async void DormantAccount(string email, Action<CheckEmailPacketRes_Old> _res, Action<DefaultPacketRes> _error = null)
//        {
//            CheckEmailPacketReq packet = new CheckEmailPacketReq()
//            {
//                email = ClsCrypto.EncryptByAES(email)
//            };

//            await Co_PostPacketRes(AccountServerUrl, WebPacket.DormantAccount, packet, _res, _error);
//        }
//        #endregion

//        #region 퀘스트
//        /// <summary>
//        /// 테스트코드
//        /// </summary>
//        private void Test_Quest()
//        {
//            //if (Input.GetKeyDown(KeyCode.Keypad1))
//            //{
//            //    Quest_GetMemberInfo_Req(Quest_GetMemberInfo_Res); //내 퀘스트현황 가져오기
//            //}
//            //if (Input.GetKeyDown(KeyCode.Keypad2))
//            //{
//            //    Quest_Process_Req(10, Quest_Process_Res); //퀘스트 10회달성의 퀘스트 그룹은 10, 1회씩 축적
//            //}
//            //if (Input.GetKeyDown(KeyCode.Keypad3))
//            //{
//            //    Quest_ReceiveReward_Req(15, Quest_ReceiveReward_Res); //퀘스트 10회달성의 퀘스트아이디는 15
//            //}
//        }


//        /// <summary>
//        /// 하루한번 로그인시 리워드
//        /// </summary>
//        /// <param name="isFirstLogin"></param>
//        public void Quest_LoginReward(int isFirstLogin)
//        {
//            //Quest_GetMemberInfo_Req((res) =>
//            //{
//            //    Quest_GetMemberInfo_Res(res);
//            //    if (isFirstLogin == 1) //퍼스트로그인 체커
//            //    {
//            //        Quest_Process_Req(1, Quest_Process_Res); //로그인 퀘스트 프로세스
//            //    }
//            //});
//        }

//        /// <summary>
//        /// 내 퀘스트현황 가져오기 리퀘스트
//        /// </summary>
//        /// <param name="_res"></param>
//        public void Quest_GetMemberInfo_Req(Action<QuestGetMemberInfoRes> _res, Action<string> _error = null)
//        {
//            //Util.RunCoroutine(Co_PostPacketRes<QuestGetMemberInfoRes>(contentServerUrl, "/api/quest/getMemberInfo", GetDefaultPacketReq(), (res) => _res.Invoke(res), (error) => _error?.Invoke(error)), "Quest_GetMemberInfo_Req");
//        }

//        /// <summary>
//        /// 내 퀘스트현황 가져오기 리스판스
//        /// </summary>
//        /// <param name="res"></param>
//        public void Quest_GetMemberInfo_Res(QuestGetMemberInfoRes res)
//        {
//            ////퀘스트데이터 비교하기 위해 저장
//            //LocalPlayerData.memberQuest = (MemberQuest[])res.memberQuest.Clone();

//            ////퀘스트보상여부 판단(퀘스트 뉴 마크 활성화 여부)
//            //LocalPlayerData.existReward = false;
//            //List<MemberQuest> memberQuestList = res.memberQuest.ToList();
//            //for (int i = 0; i < memberQuestList.Count; i++)
//            //{
//            //    MemberQuest memberQuest = memberQuestList[i];
//            //    if (memberQuest.isCompleted == 1)
//            //    {
//            //        LocalPlayerData.existReward = true;
//            //        break;
//            //    }
//            //}
//            //Panel_HUD panel_HUD = SceneLogic.instance.GetPanel<Panel_HUD>(Cons.Panel_HUD);
//            //if (panel_HUD != null)
//            //{
//            //    panel_HUD.Menu.go_QuestNew.SetActive(LocalPlayerData.existReward);
//            //}

//        }

//        /// <summary>
//        /// 퀘스트 진행하기 리퀘스트
//        /// </summary>
//        /// <param name="_res"></param>
//        public void Quest_Process_Req(int questConditionType, Action<QuestProcessRes> _res, Action<string> _error = null)
//        {
//            ////해야할(남아있는) 퀘스트 여부 체크
//            //bool todoQuest = false;


//            //for (int i = 0; i < LocalPlayerData.memberQuest.Length; i++)
//            //{
//            //    int questId = LocalPlayerData.memberQuest[i].questId;
//            //    int master_questConditionType = Single.MasterData.dataQuest.GetData(questId).questConditionType;

//            //    if (questConditionType == master_questConditionType)
//            //    {
//            //        todoQuest = true;
//            //        break;
//            //    }
//            //}

//            //if (!todoQuest)
//            //{
//            //    return;
//            //}

//            ////익명클래스
//            //var questProcessReq = new
//            //{
//            //    memberId = ClsCrypto.EncryptByAES(LocalPlayerData.MemberID),
//            //    jwtAccessToken = ClsCrypto.EncryptByAES(Token),
//            //    sessionId = ClsCrypto.EncryptByAES(LocalPlayerData.SessionID),
//            //    questConditionType = questConditionType,
//            //};

//            //Util.RunCoroutine(Co_PostPacketRes<QuestProcessRes>(contentServerUrl, "/api/quest/process", questProcessReq, (res) => _res.Invoke(res), (error) => _error?.Invoke(error)), "Quest_Process_Req");
//        }

//        /// <summary>
//        /// 퀘스트 진행하기 리스판스
//        /// </summary>
//        /// <param name="res"></param>
//        public void Quest_Process_Res(QuestProcessRes res)
//        {
//            //if (res.error == WebError.NET_E_SUCCESS)
//            //{
//            //    if (res.MemberQuest != null && res.MemberQuest.isCompleted == 1)
//            //    {
//            //        //토스트팝업으로 퀘스트완료 알림 호출
//            //        int questNameType = Single.MasterData.dataQuest.GetData(res.MemberQuest.questId).questNameType;
//            //        string questName = Single.MasterData.dataQuestNameType.GetData(questNameType).nameId.ToString();
//            //        SceneLogic.instance.PushToastPopup(Cons.ToastPopupHorizontal, new PopupToastData(TOASTTYPE.Current, new LocalData(Cons.Local_Arzmeta, "20015", new LocalData(Cons.Local_Quest, questName))));

//            //        //이미 완료한 그룹의 퀘스트를 재 Quest_Process_Req 하지 않기 위함
//            //        Quest_GetMemberInfo_Req(Quest_GetMemberInfo_Res);
//            //    }
//            //}
//        }

//        /// <summary>
//        /// 퀘스트 보상받기 리퀘스트
//        /// </summary>
//        /// <param name="_res"></param>
//        public void Quest_ReceiveReward_Req(int questId, Action<QuestReceiveRewardRes> _res, Action<string> _error = null)
//        {
//            //익명클래스
//            //var receiveRewardReq = new
//            //{
//            //    memberId = ClsCrypto.EncryptByAES(LocalPlayerData.MemberID),
//            //    jwtAccessToken = ClsCrypto.EncryptByAES(LocalPlayerData.JwtAccessToken),
//            //    sessionId = ClsCrypto.EncryptByAES(LocalPlayerData.SessionID),
//            //    questId = questId,
//            //};

//            //Util.RunCoroutine(Co_PostPacketRes<QuestReceiveRewardRes>(contentServerUrl, "/api/quest/receiveReward", receiveRewardReq, (res) => _res.Invoke(res), (error) => _error?.Invoke(error)));
//        }

//        /// <summary>
//        /// 퀘스트 보상받기 리스판스
//        /// </summary>
//        /// <param name="res"></param>
//        public void Quest_ReceiveReward_Res(QuestReceiveRewardRes res)
//        {
//            //리워드받기 성공시
//            //if (res.error == WebError.NET_E_SUCCESS)
//            //{
//            //    //리워드상품이 있을 시
//            //    if (res.memberAvatarInven != null)
//            //    {
//            //        //로컬인벤 널체크
//            //        if (LocalPlayerData.avatarInvens == null)
//            //        {
//            //            LocalPlayerData.avatarInvens = new List<AvatarInvens>();
//            //        }

//            //        //내 로컬인벤에 넣어준다.
//            //        LocalPlayerData.avatarInvens.Add(new AvatarInvens { avatarPartsId = res.memberAvatarInven.avatarPartsId });
//            //    }
//            //}
//        }
//        #endregion 

//        #region IAP 인앱결제 영수증(구글, 애플)
//        public async void IAP_Receipt_Req(IAP_GoogleReceiptReq iAP_GoogleReceiptReq, Action<IAP_GoogleReceiptRes> callback)
//        {
//            string preUrl = "https://dev-api-homepage.arzmeta.net";

//            await Co_PostPacketRes<IAP_GoogleReceiptRes, DefaultPacketRes>(preUrl, "/api/payment/iap/google", iAP_GoogleReceiptReq, (res) => callback(res), Error_Res);
//        }
//        public async void IAP_ReceiptSubscription_Req(IAP_GoogleReceiptReq iAP_GoogleReceiptReq, Action<IAP_GoogleReceiptRes> callback)
//        {
//            string preUrl = "https://dev-api-homepage.arzmeta.net";

//            await Co_PostPacketRes<IAP_GoogleReceiptRes, DefaultPacketRes>(preUrl, "/api/payment/iap/google/billing", iAP_GoogleReceiptReq, (res) => callback(res), Error_Res);
//        }

//        #endregion

//        #region 실시간 서버 
//        /// <summary>
//        /// 실시간 서버 정보 가져오기 
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="isCloud"></param>
//        /// <param name="type"></param>
//        /// <param name="_res"></param>
//        /// <param name="_error"></param>
//        public void GetServerInfo<T, T1>(bool isCloud, string type = "", Action<T> _res = null, Action<T1> _error = null, bool isDim = false)
//        {
//            var url = string.Empty;

//            if (isCloud)
//            {
//                url = Single.Web.LobbyServerUrl + Cons.QUERY_SERVER_TYPE + type;

//                var packet = new
//                {
//                    memberId = ClsCrypto.EncryptByAES(LocalPlayerData.MemberID),
//                    jwtAccessToken = ClsCrypto.EncryptByAES(LocalPlayerData.JwtAccessToken),
//                    sessionId = ClsCrypto.EncryptByAES(LocalPlayerData.SessionID)
//                };

//                SendPostRequest<T, T1>(url, string.Empty, packet, _res, _error, isDim);
//            }
//            else
//            {
//                url = Cons.RequestInnerServerUrl;
//                SendGetRequest<T, T1>(url, string.Empty, _res, _error, isDim);
//            }
//        }


//        /// <summary>
//        /// 오피스 서버 생성 요청 할 때
//        /// </summary>
//        /// <param name="data"></param>
//        /// <param name="_res"></param>
//        //public void CreateOfficeRoom(OfficeRoomDataTest data, Action<CreatedRoomRes> _res, Action<string, int> _error = null)
//        //{
//        //    List<ModuleReq> moduleLists = new List<ModuleReq>();

//        //    BaseModuleReq baseModuleReq = new BaseModuleReq("Base", data.scenes, data.interval);
//        //    moduleLists.Add(baseModuleReq);

//        //    ChatModuleReq chatModuleReq = new ChatModuleReq("Chat");
//        //    moduleLists.Add(chatModuleReq);

//        //    OfficeModuleReq meetingModuleReq = new OfficeModuleReq("Meeting", data);
//        //    moduleLists.Add(meetingModuleReq);

//        //    //MakeRooms packet = new MakeRooms("test");
//        //    //SendPostRequest<CreatedRoomRes>(data.url, string.Empty, packet, (res) => _res.Invoke(res), (str, res) => _error?.Invoke(str, res));
//        //}

//        /// <summary>
//        /// 게임룸 생성 요청할 때 사용
//        /// </summary>
//        /// <param name="data"></param>
//        /// <param name="_res"></param>
//        //public void CreateGameRoom(MakeGameRoomData data, Action<CreatedRoomRes> _res, Action<string, int> _error = null)
//        //{
//        //    List<ModuleReq> moduleLists = new List<ModuleReq>();
//        //    BaseModuleReq baseModuleReq = new BaseModuleReq("Base", data.scenes, data.interval);

//        //    moduleLists.Add(baseModuleReq);

//        //    GameModuleReq gameModuleReq = new GameModuleReq(data.serverType, data.roomName, data.maxPlayerNumber);
//        //    moduleLists.Add(gameModuleReq);

//        //    //MakeRooms packet = new MakeRooms("test");

//        //    //Debug.Log(data.serverUrl);
//        //    //SendPostRequest<CreatedRoomRes>(data.serverUrl, string.Empty, packet, (res) => _res.Invoke(res), (str, res) => _error?.Invoke(str, res));
//        //}

//        /// <summary>
//        /// 마이룸 생성 요청할 때 사용
//        /// </summary>
//        /// <param name="data"></param>
//        /// <param name="_res"></param>
//        //public void CreateRoom(RoomData data, Action<CreatedRoomRes> _res, Action<string, int> _error = null)
//        //{
//        //    List<ModuleReq> moduleLists = new List<ModuleReq>();
//        //    //BaseModuleReq baseModuleReq = new BaseModuleReq("Base", data.scenes, data.interval);
//        //    //moduleLists.Add(baseModuleReq);

//        //    //MyroomModuleReq myroomModuleReq = new MyroomModuleReq("Myroom", data.roomCode);
//        //    //moduleLists.Add(myroomModuleReq);

//        //    ChatModuleReq chatModuleReq = new ChatModuleReq("Chat");
//        //    moduleLists.Add(chatModuleReq);

//        //    //MakeRooms packet = new MakeRooms("test");

//        //    //SendPostRequest<CreatedRoomRes>("", string.Empty, packet, (res) => _res.Invoke(res), (str, res) => _error?.Invoke(str, res));
//        //}

//        #endregion

//        #region 심심이_SimSimi

//        /// <summary>
//        /// 심심이 메세지 전송
//        /// </summary>
//        /// <param name="utext"></param>
//        /// <param name="_res"></param>
//        /// <param name="_error"></param>
//        public async void SendSimSimi(string utext, Action<ResSimSimi> _res, Action<ErrorSimSimi> _error)
//        {
//            //method(get, post, put...)
//            string method = UnityWebRequest.kHttpVerbPOST;

//            //header
//            var header = new List<(string, string)>();
//            header.Add(("Content-Type", "application/json"));
//            header.Add(("x-api-key", "d~9wXsekDYKO2tpXUCrirlmL7FThVaobL8B4AG~P"));

//            //url
//            string version = "190410";
//            string url = $"https://wsapi.simsimi.com/{version}/talk";

//            //utext
//            string lang = AppGlobalSettings.Instance.language == Language.Korean ? "ko" : "en";
//            ReqSimSimi reqSimSimi = new ReqSimSimi(utext, lang);

//            await AsyncRequest(method, header, url, reqSimSimi, _res, _error);
//        }



//        /// <summary>
//        /// 비동기 리퀘스트
//        /// </summary>
//        /// <typeparam name="T1"></typeparam>
//        /// <typeparam name="T2"></typeparam>
//        /// <param name="method"></param>
//        /// <param name="header"></param>
//        /// <param name="url"></param>
//        /// <param name="obj"></param>
//        /// <param name="res"></param>
//        /// <param name="error"></param>
//        /// <returns></returns>
//        private async UniTask AsyncRequest<T1, T2>(string method, List<(string, string)> header, string url, object obj, Action<T1> res, Action<T2> error)
//        {
//            string requestJson = string.Empty;
//            string responseJson = string.Empty;

//            //데이터
//            byte[] jsonBytes = null;
//            if (obj != null)
//            {
//                requestJson = JsonConvert.SerializeObject(obj, Formatting.Indented);
//                jsonBytes = Encoding.UTF8.GetBytes(requestJson);
//            }

//            DEBUG.LOG(url + "\n" + requestJson + "\n", eColorManager.Web_Request);

//            UnityWebRequest uwr = new UnityWebRequest(url, method);
//            uwr.timeout = 10;
//            uwr.uploadHandler = new UploadHandlerRaw(jsonBytes);
//            uwr.downloadHandler = new DownloadHandlerBuffer();

//            //헤더
//            foreach (var item in header)
//            {
//                uwr.SetRequestHeader(item.Item1, item.Item2);
//            }

//            try
//            {
//                await uwr.SendWebRequest();
//            }
//            catch (UnityWebRequestException exception)
//            {

//                switch (exception.Result)
//                {
//                    case UnityWebRequest.Result.ConnectionError:
//                    case UnityWebRequest.Result.ProtocolError:
//                    case UnityWebRequest.Result.DataProcessingError:
//                        {
//                            DEBUG.LOG($"error : {exception.Error}, text : {exception.Text}", eColorManager.Web_Response);

//                            isReceived = false;

//                            //await HandleError(exception.Error, exception.Text);
//                        }
//                        break;
//                    default:
//                        break;
//                }
//            }
//            finally
//            {
//                await UniTask.WaitUntil(() => uwr.isDone);

//                responseJson = Util.Beautify(uwr.downloadHandler.text);
//                DEBUG.LOG(url + "\n" + responseJson + "\n", eColorManager.Web_Response);

//                if (uwr.result == UnityWebRequest.Result.Success)
//                {
//                    T1 resObj = JsonConvert.DeserializeObject<T1>(responseJson);
//                    if (resObj != null)
//                    {
//                        res?.Invoke(resObj);
//                    }
//                    else
//                    {
//                        SceneLogic.instance.PushPopup<Popup_Basic>()
//                            .ChainPopupData(new PopupData(POPUPICON.WARNING, BTN_TYPE.Confirm, masterLocalDesc: new MasterLocalData("common_error_network_01")));
//                    }

//                    isReceived = true;
//                }
//                else
//                {
//                    T2 resObj = JsonConvert.DeserializeObject<T2>(responseJson);
//                    if (resObj != null)
//                    {
//                        error?.Invoke(resObj);
//                    }
//                }
//            }
//        }
//        #endregion
//        #endregion

//        #endregion

//        #region 기능 메소드
//        /// <summary>
//        /// 가입하는 플랫폼에 따른 RegPathType 리턴 
//        /// </summary>
//        /// <returns></returns>
//        public REGPATH_TYPE GetRegPathType()
//        {
//#if UNITY_EDITOR || !UNITY_ANDROID && !UNITY_IOS
//            return REGPATH_TYPE.Etc;
//#elif UNITY_ANDROID
//            return REGPATH_TYPE.Android;
//#elif UNITY_IOS
//            return REGPATH_TYPE.IOS;
//#endif
//        }

//        private void OpenReturnToLogoPopup()
//        {
//            SceneLogic.instance.PushPopup<Popup_Basic>()
//                .ChainPopupData(new PopupData(POPUPICON.WARNING, BTN_TYPE.Confirm, masterLocalDesc: new MasterLocalData(Cons.Local_Arzmeta, "common_error_server_02")))
//                .ChainPopupAction(new PopupAction(Util.ReturnToLogo));
//        }

//        /// <summary>
//        /// 현재 씬이 타이틀일 시 에러가 발생했을 때 타이틀 패널 리프레쉬
//        /// </summary>
//        private void RefrashTitle()
//        {
//            var panel_Title = SceneLogic.instance.GetPanel<Panel_Title>();
//            if (panel_Title != null)
//            {
//                panel_Title.RefreshUI(true);
//                Single.Scene.SetDimOff();
//            }
//        }
//        #endregion
//    }
//}
