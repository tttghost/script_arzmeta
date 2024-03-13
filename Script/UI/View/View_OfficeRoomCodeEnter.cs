//using System.Collections.Generic;
//using System.Linq;
//using FrameWork.UI;
//using Network;
//using TMPro;
//using UnityEngine.UI;

//namespace Office
//{
//    // 사용 X
//    public class View_OfficeRoomCodeEnter : UIBase
//    {
//        private TMP_Text txtmp_InputRoomCode, txtmp_EnterCodeCaution, txtmp_EnterRoom;
//        private TMP_InputField input_RoomCode;
//        private Button btn_EnterRoom;

//        private Panel_OfficeRoom _panelOfficeRoom;
//        private MeetingModuleData moduleData = null;

//        protected override void SetMemberUI()
//        {
//            base.SetMemberUI();

//            txtmp_InputRoomCode =
//                GetUI_Txtmp("txtmp_InputRoomCode", new LocalData(Cons.Local_Arzmeta, "1022")); //룸 코드 입력
//            txtmp_EnterCodeCaution =
//                GetUI_Txtmp("txtmp_EnterCodeCaution", new LocalData(Cons.Local_Arzmeta, "1176")); // 올바른 룸 코드를 입력해 주세요
//            txtmp_EnterRoom = GetUI_Txtmp("txtmp_EnterRoom", new LocalData(Cons.Local_Arzmeta, "1007")); // 입장하기

//            input_RoomCode = GetUI_TMPInputField("input_RoomCode", OnValueChangedRoomCode);
//            input_RoomCode.contentType = TMP_InputField.ContentType.IntegerNumber; // 룸코드는 숫자만 넣을 수 있음
//            input_RoomCode.characterLimit = 8;

//            btn_EnterRoom = GetUI_Button("btn_EnterRoom", OnclickEnterRoom);
//        }

//        protected override void Start()
//        {
//            base.Start();
//            _panelOfficeRoom = SceneLogic.instance.GetPanel<Panel_OfficeRoom>(Cons.Panel_OfficeRoom);
//        }

//        private void OnEnable()
//        {
//            PanelInit();
//        }

//        private void PanelInit()
//        {
//            txtmp_EnterCodeCaution.gameObject.SetActive(false);
//            input_RoomCode.text = string.Empty;
//            btn_EnterRoom.interactable = false;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="str"></param>
//        private void OnValueChangedRoomCode(string str)
//        {
//            if (str.Length == 0)
//            {
//                txtmp_EnterCodeCaution.gameObject.SetActive(false);
//                btn_EnterRoom.interactable = false;
//                return;
//            }

//            txtmp_EnterCodeCaution.gameObject.SetActive(str.Length < 8);
//            btn_EnterRoom.interactable = !(str.Length < 8);
//        }

//        /// <summary>
//        /// 초대 코드를 입력해야되는데 서버한테 물어봐야되나? 언제 가지고 온 데이터로 물어봐야되징?
//        /// </summary>
//        private void OnclickEnterRoom()
//        {
//            NetworkUtils.GetServerInfo(_panelOfficeRoom.isCloud, Cons.ServerType_Meeting, CompareRoomCode, ErrorCallback);
//        }

//        /// <summary>
//        /// 룸 코드 비교
//        /// 서버한테 받은 회의실 정보로 RoomID 비교하고 그 ServerInfo에서 비밀번호가 존재한지 판단 후 존재하면 비밀번호 팝업 활성화
//        /// 비밀번호를 서버한테 패킷으로 보내고 맞는 비밀번호를 보냈는지 응답 받고 경고창 or 입장하도록 구현하기
//        /// </summary>
//        /// <param name="infos"></param>
//        private void CompareRoomCode(List<ServerInfoRes> infos)
//        {
//            string inputCode = input_RoomCode.text.PadLeft(8, '0');
//            txtmp_EnterCodeCaution.gameObject.SetActive(false);
            
//            ServerInfoRes info = infos.FirstOrDefault(info => info.RoomId.PadLeft(8, '0') == inputCode);
//            _panelOfficeRoom.PasswordCheckEnterRoom(info, () =>
//            {
//                // 동일한 방이 없었기 때문에 조건 활성화 하기
//                txtmp_EnterCodeCaution.gameObject.SetActive(true);
//                btn_EnterRoom.interactable = false;
//            });
//        }

//        private void ErrorCallback(string str, int res)
//        {
//            txtmp_EnterCodeCaution.gameObject.SetActive(true);
//        }
//    }
//}