using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FrameWork.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.Android;

public static partial class Util
{
    public static class UtilOffice
    {
        private static Sprite sprite_Password = Resources.Load<Sprite>(Cons.Path_Image + Define.ICON_LOCK);
        private static Sprite sprite_NoPassword = Resources.Load<Sprite>(Cons.Path_Image + Define.ICON_UNLOCK);
        private static Sprite sprite_Normal = Resources.Load<Sprite>(Cons.Path_Image + Define.ICON_INTEREST_NORMAL);
        private static Sprite sprite_Heart = Resources.Load<Sprite>(Cons.Path_Image + Define.ICON_INTEREST_HEART);

        private static Sprite sprite_Meeting = Resources.Load<Sprite>(Cons.Path_ArzPhoneIcon + Define.ICON_MEETING);
        private static Sprite sprite_Lecture = Resources.Load<Sprite>(Cons.Path_ArzPhoneIcon + Define.ICON_LECTURE);

        #region [Camera, Microphone 디바이스가 있는지 체크]
        public static bool CheckCameraDevices()
        {
            return WebCamTexture.devices.Length > 0 ? true : false;
        }

        public static bool CheckMicrophoneDevices()
        {
            return Microphone.devices.Length > 0 ? true : false;
        }
        #endregion

        #region [Camera, Microphone 관련 하드웨어 접근 권한이 있는지 체크]
        public static bool CheckCameraPermission()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.Camera);
#elif UNITY_IOS && !UNITY_EDITOR
            return Application.HasUserAuthorization(UserAuthorization.WebCam);
#else
            return true;
#endif
        }

        public static bool CheckMicrophonePermission()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.Microphone);
#elif UNITY_IOS && !UNITY_EDITOR
            return Application.HasUserAuthorization(UserAuthorization.Microphone);
#else
            return true;
#endif
        }
        #endregion

        #region [카메라, 마이크 하드웨어 접근권한 요청 함수]
        public static void RequestOfficePermission()
        {
            if (Application.isEditor)
                return;

#if     UNITY_ANDROID
            RequestPermission_AOS();

#elif   UNITY_IOS
            RequestPermission_iOS();
#endif
        }

        public static async void RequestPermission_AOS()
        {
            await Cysharp.Threading.Tasks.UniTask.NextFrame();
            await Cysharp.Threading.Tasks.UniTask.WaitUntil(() => Application.isFocused);

            var callbacks = new PermissionCallbacks();
            callbacks.PermissionDenied += PermissionCallbacks_PermissionDenied;
            callbacks.PermissionGranted += PermissionCallbacks_PermissionGranted;
            callbacks.PermissionDeniedAndDontAskAgain += PermissionCallbacks_PermissionDeniedAndDontAskAgain;

            UnityEngine.Android.Permission.RequestUserPermission(
                UnityEngine.Android.Permission.Camera, callbacks);
        }

        public static void PermissionCallbacks_PermissionDeniedAndDontAskAgain(string permissionName)
        {
            DEBUG.LOG($"{permissionName} PermissionDeniedAndDontAskAgain", eColorManager.AGORA);

            RequestPermissionMicrophone_AOS();
        }

        internal static void PermissionCallbacks_PermissionGranted(string permissionName)
        {
            DEBUG.LOG($"{permissionName} PermissionCallbacks_PermissionGranted", eColorManager.AGORA);

            RequestPermissionMicrophone_AOS();
        }

        internal static void PermissionCallbacks_PermissionDenied(string permissionName)
        {
            DEBUG.LOG($"{permissionName} PermissionCallbacks_PermissionDenied", eColorManager.AGORA);

            RequestPermissionMicrophone_AOS();
        }

        public static async void RequestPermissionMicrophone_AOS()
        {
            await Cysharp.Threading.Tasks.UniTask.NextFrame();
            await Cysharp.Threading.Tasks.UniTask.WaitUntil(() => Application.isFocused);

            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.Microphone);
        }

        /// <summary>
        /// iOS 카메라, 마이크 하드웨어 접근권한 요청 
        /// </summary>
        public static async void RequestPermission_iOS()
        {
            await Cysharp.Threading.Tasks.UniTask.NextFrame();
            await Cysharp.Threading.Tasks.UniTask.WaitUntil(() => Application.isFocused);

            if (CheckCameraDevices())
            {
                await Application.RequestUserAuthorization(UserAuthorization.WebCam);
            }

            if (CheckMicrophoneDevices())
            {
                await Application.RequestUserAuthorization(UserAuthorization.Microphone);
            }
        }
        #endregion

        public static RoomType GetRoomType(int _modeType)
        {
            return _modeType == 1 ? RoomType.Meeting : RoomType.Lecture;
        }

        public static string GetModeType(int _modeType)
        {
            var masterId = Single.MasterData.dataOfficeModeType.GetData(_modeType).name;

            return masterId;
        }

        public static string GetTopicType(int _topicType)
        {
            var masterId = Single.MasterData.dataOfficeTopicType.GetData(_topicType).name;

            return masterId;
        }

        public static (string repeat, string normal) GetStartDate(int _repeat, DateTime _dateTime)
        {
            string repeat = Util.Int2DayOfWeek(_repeat);
            string normal = string.Format("{0:D4}-{1:D2}-{2:D2}", _dateTime.Year, _dateTime.Month, _dateTime.Day);

            return (repeat, normal);
        }

        public static (string date, string time) GetDateTime(string _dateTime)
        {
            DateTime parsedDateTime = DateTime.Parse(_dateTime);

            string date = parsedDateTime.ToString("yyyy.MM.dd");
            string time = parsedDateTime.ToString("tt hh:mm");

            return (date, time);
        }

        public static (string date, string time) GetDateTime(DateTime dateTime)
        {
            string date = dateTime.ToString("yyyy-MM-dd");
            string time = dateTime.ToString("tt hh:mm:ss");

            return (date, time);
        }

        public static (Sprite sprite, string path) GetSpaceThumbnail(string _spaceInfoId)
        {
            var filename = Single.MasterData.dataOfficeSpaceInfo.GetData(int.Parse(_spaceInfoId)).thumbnail;
            var path = Cons.Path_OfficeThumbnail + filename;

            return (Single.Resources.Load<Sprite>(path), path);
        }

        public static bool CheckEnterAvailable(DateTime _start, DateTime _end)
        {
            return DateTime.Now >= _start.AddMinutes(-30) && DateTime.Now <= _end.AddMinutes(30);
        }

        public static Sprite GetInterest(string _roomCode)
        {
            return IsInterest(_roomCode) ? sprite_Heart : sprite_Normal;
        }

        public static Sprite GetInterest(bool _isInterest)
        {
            return _isInterest ? sprite_Heart : sprite_Normal;
        }

        public static bool IsInterest(string _roomCode)
        {
            var interests = ArzMetaReservationController.Instance.Interests;
            var repeats = ArzMetaReservationController.Instance.InterestRepeats;

            return interests.Any(info => info.info.roomCode == _roomCode) || repeats.Any(info => info.info.roomCode == _roomCode);
        }

        public static Sprite GetLockType(bool _isPassword)
        {
            return _isPassword ? sprite_Password : sprite_NoPassword;
        }

        public static Sprite GetLockType(int _isPassword)
        {
            return Convert.ToBoolean(_isPassword) ? sprite_Password : sprite_NoPassword;
        }

        public static Sprite GetOfficeType(int _topicType)
        {
            return _topicType == 0 ? sprite_Meeting : sprite_Lecture;
        }

        public static string GetSceneName(string _spaceInfoId)
        {
            var sceneName = Single.MasterData.dataOfficeSpaceInfo.GetData(int.Parse(_spaceInfoId)).sceneName;

            return sceneName;
        }

        public static string GetOfficeSpaceTitle(OfficeRoomReservationInfo _reservationInfo)
        {
            //int year = _reservationInfo.reservationDateTime.Year;
            //int month = _reservationInfo.reservationDateTime.Month;
            //int day = _reservationInfo.reservationDateTime.Day;

            //DateTime dateValue = new DateTime(year, month, day);
            //string dayOfWeek = Util.GetDayOfWeek(dateValue.DayOfWeek);

            //return string.Format("{0:D4}. {1:D2}. {2:D2} ({3})", year, month, day, dayOfWeek);

            return Util.GetMasterLocalizing(new MasterLocalData("1008"));
        }

        public static string GetOfficeSpaceCount(List<OfficeRoomReservationInfo> reservationInfos)
        {
            int meeting = 0;
            int lecture = 0;

            for (int i = 0; i < reservationInfos.Count; i++)
            {
                if (reservationInfos[i].info.modeType == 1) meeting++;

                else lecture++;
            }

            var txtmp_meeting = Util.GetMasterLocalizing(new MasterLocalData("office_room_topic_meeting"));
            var txtmp_lecture = Util.GetMasterLocalizing(new MasterLocalData("office_room_topic_lecture"));

            return string.Format(txtmp_meeting + " : {0:D1},   " + txtmp_lecture + " : {1:D1}", meeting, lecture);
        }

        public static void DestroyIfNotOffice(GameObject _gameObject)
        {
            if (Single.RealTime.roomType.current == RoomType.Lecture || Single.RealTime.roomType.current == RoomType.Meeting)
            {
                UnityEngine.Object.Destroy(_gameObject);
            }
        }

        public static bool IsOffice()
        {
            bool isRet = false;

            switch (Single.RealTime.roomType.current)
            {
                case RoomType.Meeting:
                case RoomType.Lecture:
                    isRet = true;
                    break;

                default:
                    isRet = false;
                    break;
            }

            return isRet;
        }

        public static Color HexToColor(string _hexCode)
        {
            Color color = Color.white;

            if (ColorUtility.TryParseHtmlString(_hexCode, out color))
            {
                return color;
            }

            return color;
        }

        public static RoomType GetOfficeRoomType(string _topicType)
        {
            return _topicType == "1" ? RoomType.Meeting : RoomType.Lecture;
        }

        public static Dictionary<OfficeAuthority, Sprite> officeAuthorityIcons =
                new Dictionary<OfficeAuthority, Sprite>();

        public static void EnableHUDLayer(bool enable)
        {
            if (enable)
            {
                Camera.main.cullingMask |= 1 << LayerMask.NameToLayer("HUD");
            }
            else
            {
                Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("HUD"));
            }
        }

        public static Dictionary<string, Sprite> thumbnail_OfficeRoom = new Dictionary<string, Sprite>();
        public static async UniTask<Sprite> GetThumbnail_Office(string roomCode, string thumbnailPath)
        {

            Sprite thumbnailSprite = null;

            if (!string.IsNullOrEmpty(thumbnailPath)) //스토리지에 썸네일 존재한다.
            {
                string commonPath = Path.Combine("office", roomCode, thumbnailPath);
                string localPath = Path.Combine(Application.persistentDataPath, commonPath);
                string remotePath = Path.Combine(Single.Web.StorageUrl, commonPath);

                if (!File.Exists(localPath)) //로컬에 썸네일파일 없음
                {
                    thumbnailSprite = await Co_LoadRemoteAsyncSprite(remotePath); //다운
                    Sprite2Image(localPath, thumbnailSprite); //저장
                }
                else if (!thumbnail_OfficeRoom.ContainsKey(commonPath)) //딕셔너리에 썸네일 없음
                {
                    thumbnailSprite = await Co_LoadLocalAsyncSprite(localPath);
                }
                else
                {
                    thumbnailSprite = thumbnail_OfficeRoom[commonPath];
                }


                if (!thumbnail_OfficeRoom.ContainsKey(commonPath)) //딕셔너리에 썸네일 없음
                {
                    thumbnail_OfficeRoom.Add(commonPath, thumbnailSprite);
                }
                else
                {
                    thumbnail_OfficeRoom[commonPath] = thumbnailSprite;
                }
            }

            return thumbnailSprite;
        }

        public static string GetMasterLocal_OfficeAutority(int autority)
        {
            return GetMasterLocal_OfficeAutority((OfficeAuthority)autority);
        }

        public static string GetMasterLocal_OfficeAutority(OfficeAuthority autority)
        {
            string tempText = string.Empty;

            switch (autority)
            {
                case OfficeAuthority.관리자:
                    tempText = "office_participant_type_manager";
                    break;

                case OfficeAuthority.부관리자:
                    tempText = "office_participant_type_assistant_manager";
                    break;

                case OfficeAuthority.일반참가자:
                    tempText = "office_participant_type_participant";
                    break;

                case OfficeAuthority.발표자:
                    tempText = "office_participant_type_presenter";
                    break;

                case OfficeAuthority.청중:
                    tempText = "office_participant_type_audience";
                    break;

                case OfficeAuthority.관전자:
                    tempText = "office_participant_type_observer";
                    break;

                default:
                    tempText = "권한없음";
                    break;
            }

            return Util.GetMasterLocalizing(tempText);
        }

        public static Sprite GetAuthoritySprite(OfficeAuthority authority)
        {
            if (officeAuthorityIcons.Count == 0)
                Load_Sprites();

            Sprite sprIcon = null;
            if (officeAuthorityIcons.TryGetValue(authority, out sprIcon))
                return sprIcon;

            return null;
        }

        public static Sprite GetAuthoritySprite(int authority)
        {
            return GetAuthoritySprite((OfficeAuthority)authority);
        }

        public static void Load_Sprites()
        {
            if (officeAuthorityIcons.Count > 0)
                return;

            const string path = "Addressable/Image/";

            officeAuthorityIcons = new SerializableDictionary<OfficeAuthority, Sprite>
                {
                    {OfficeAuthority.관리자, CommonUtils.Load<Sprite>(path + "icon_office_host_02")},
                    {OfficeAuthority.부관리자, CommonUtils.Load<Sprite>(path + "icon_office_manager_02")},
                    {OfficeAuthority.일반참가자, CommonUtils.Load<Sprite>(path + "icon_office_guest_02")},
                    {OfficeAuthority.발표자, CommonUtils.Load<Sprite>(path + "icon_office_speaker_02")},
                    {OfficeAuthority.청중, CommonUtils.Load<Sprite>(path + "icon_office_audience_02")},
                    {OfficeAuthority.관전자, CommonUtils.Load<Sprite>(path + "icon_office_observer_02")}
                };
        }

        public static bool IsExposition()
        {
            bool isExpositionBooth = false;
            switch (SceneLogic.instance.GetSceneType())
            {
                case SceneName.Scene_Room_Exposition_Booth: 
                    isExpositionBooth = true; break;
                default: break;
            }
            return isExpositionBooth;
        }

        public static bool IsLectureRoom()
        {
            bool isLectureRoom = false;

            switch (SceneLogic.instance.GetSceneType())
            {
                case SceneName.Scene_Room_Lecture:
                case SceneName.Scene_Room_Lecture_22Christmas:
                    isLectureRoom = true;
                    break;

                default:
                    isLectureRoom = false;
                    break;
            }

            return isLectureRoom;
        }

        public static void OpenWarningPopup(string message)
        {
            // TODO : 마스터에서 로컬라이징 키 가져오기
            string localizedKey = "로컬라이징키";
            switch (message)
            {
                case "SUCCESS":

                    return;

                // 잘못된 클라이언트 ID 입니다.
                case "WRONG_CLIENT_ID":
                    localizedKey = "office_error_unableid";
                    break;

                // 관리자가 2명 이상 입니다.
                case "TOO_MANY_HOST":
                    localizedKey = "office_error_overmanager";
                    break;

                // 관리자가 없는 경우
                case "NO_HOST":
                    localizedKey = "office_error_nonmanager";
                    break;

                // 화면권한이 2명 이상입니다.
                case "TOO_MANY_SCREEN_PERMISSION":
                    localizedKey = "office_error_overscreencontrol";
                    break;

                // 관전자가 정원 이상인 경우
                case "TOO_MANY_OBSERVER":
                    localizedKey = "office_error_overobserver";
                    break;

                // 관전자 아닌 사람의 정원이 꽉차서, 관전자로 바꾸지 못하는 경우
                case "TOO_MANY_NON_OBSERVER":
                    localizedKey = "office_error_overcapacity";
                    break;

                default:
                    break;
            }

            SceneLogic.instance.PushPopup<Popup_Basic>()
                .ChainPopupData(new PopupData(POPUPICON.WARNING, BTN_TYPE.Confirm, null, new MasterLocalData(localizedKey)))
                .ChainPopupAction(null);
        }
    }
}
