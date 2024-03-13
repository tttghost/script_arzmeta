using FrameWork.UI;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
/// <summary>
/// 20230608 업데이트에서 해당 팝업 보류
/// </summary>
public class Popup_OfficeEnterSetting : PopupBase
{
    #region 변수
    private Dictionary<string, GameObject> dicBizCards = new Dictionary<string, GameObject>();

    private TMP_Text txtmp_Title;
    private TMP_Text txtmp_WarningNicknameCheck;
    private Button btn_Enter;
    private Button btn_Save;
    private TMP_InputField input_Nickname;
    private TogglePlus togplus_ShowBizCard;
    private Transform go_CardRig;
    private GameObject go_SelectBizCard;

    private BizCardInfo bizCardInfo = null;
    #endregion

    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        #region TMP_Text
        txtmp_Title = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Title));
        txtmp_WarningNicknameCheck = GetUI_TxtmpMasterLocalizing(nameof(txtmp_WarningNicknameCheck));

        GetUI_TxtmpMasterLocalizing("txtmp_Enter", new MasterLocalData("common_enter"));
        GetUI_TxtmpMasterLocalizing("txtmp_Save", new MasterLocalData("common_save"));
        GetUI_TxtmpMasterLocalizing("txtmp_NicknameTitle", new MasterLocalData("office_officialname"));
        GetUI_TxtmpMasterLocalizing("txtmp_ShowBizCard", new MasterLocalData("office_setting_card_ex"));
        GetUI_TxtmpMasterLocalizing("txtmp_ShowOn", new MasterLocalData("1100"));
        GetUI_TxtmpMasterLocalizing("txtmp_ShowOff", new MasterLocalData("1016"));
        #endregion

        #region Button
        btn_Enter = GetUI_Button(nameof(btn_Enter), OnClick_Enter);
        btn_Save = GetUI_Button(nameof(btn_Save), OnClick_Save);

        GetUI_Button("btn_Exit", OnClick_Exit);
        #endregion

        #region InputField
        input_Nickname = GetUI_TMPInputField(nameof(input_Nickname), OnValueChanged_Nickname);
        #endregion

        #region TogglePlus
        togplus_ShowBizCard = GetUI<TogglePlus>(nameof(togplus_ShowBizCard));
        if (togplus_ShowBizCard != null)
        {
            togplus_ShowBizCard.SetToggleAction(SetActiveSelectBizCard);
        }
        #endregion

        #region etc
        go_CardRig = GetChildGObject(nameof(go_CardRig)).transform;
        go_SelectBizCard = GetChildGObject(nameof(go_SelectBizCard));
        #endregion
    }

    #region 초기화
    protected override void Start()
    {
        if (txtmp_Title != null)
        {
            string local = IsOffice() ? "임시 로컬" : "office_entry_setting";
            Util.SetMasterLocalizing(txtmp_Title, new MasterLocalData(local));
        }

        SetActiveBtn();
    }

    protected override void OnEnable()
    {
        if (input_Nickname != null)
        {
            input_Nickname.Clear();
            Util.SetMasterLocalizing(input_Nickname.placeholder, LocalPlayerData.NickName);
        }

        if (go_CardRig != null)
        {
            InitCardRig();
            SetBizCard();
        }

        if (togplus_ShowBizCard != null)
        {
            togplus_ShowBizCard.SetToggleIsOn(true);
        }

        if (txtmp_WarningNicknameCheck != null)
        {
            Util.SetActive_Warning(txtmp_WarningNicknameCheck, null);
        }
    }

    /// <summary>
    /// 오피스 외/내부에 따라 입장하기 저장하기 버튼 비/활성화
    /// </summary>
    private void SetActiveBtn()
    {
        if (btn_Enter == null || btn_Save == null) return;

        btn_Enter.gameObject.SetActive(!IsOffice());
        btn_Save.gameObject.SetActive(IsOffice());
    }

    /// <summary>
    /// 카드 리그 하위 오브젝트 비활성화
    /// </summary>
    private void InitCardRig()
    {
        int count = dicBizCards.Count;
        if (count > 0)
        {
            List<GameObject> valueList = dicBizCards.Values.ToList();
            for (int i = 0; i < count; i++)
            {
                valueList[i].SetActive(false);
            }
        }
    }

    /// <summary>
    /// 노출 명함 세팅하기
    /// </summary>
    private void SetBizCard()
    {
        bizCardInfo = LocalPlayerData.Method.GetDefaultBizCard();
        GameObject obj = null;

        if (bizCardInfo != null)
        {
            obj = CreateOrGetObject(Util.GetBizPrefabName(bizCardInfo.templateId, BIZTEMPLATE_TYPE.MINI));
            if (obj != null)
            {
                BizCardBase bizCard = obj.GetComponent<BizCardBase>();
                bizCard.SetData(new BizCardData(bizCardInfo, LocalPlayerData.MemberCode, LocalPlayerData.AvatarInfo));
            }
        }
        else
        {
            obj = CreateOrGetObject(Cons.View_BizCard_Add_Mini);
            if (obj != null)
            {
                obj.GetComponent<View_BizCard_Add>().AddListener(OnClick_Add);
            }
        }

        if (obj != null)
        {
            Util.SetParentPosition(obj, go_CardRig);
            obj.SetActive(true);
        }
        else
        {
            SetActiveSelectBizCard(false);
        }
    }

    /// <summary>
    /// 프리팹 가져오기 및 로드
    /// </summary>
    /// <param name="prefabName"></param>
    /// <returns></returns>
    private GameObject CreateOrGetObject(string prefabName)
    {
        if (!string.IsNullOrEmpty(prefabName))
        {
            if (dicBizCards.TryGetValue(prefabName, out GameObject dicObj))
            {
                return dicObj;
            }
            else
            {
                GameObject loadObj = Single.Resources.Instantiate<GameObject>(Cons.Path_Prefab_UI_View + prefabName);
                dicBizCards.Add(prefabName, loadObj);

                return loadObj;
            }
        }
        return null;
    }
    #endregion

    #region 
    /// <summary>
    /// 명함 노출 토글 상태에 따른 노출 명함 오브젝트 비/활성화
    /// </summary>
    /// <param name="isActive"></param>
    private void SetActiveSelectBizCard(bool isActive)
    {
        if (go_SelectBizCard == null) return;

        go_SelectBizCard.SetActive(isActive);
    }

    /// <summary>
    /// 나가기
    /// </summary>
    private void OnClick_Exit()
    {
        PushPopup<Popup_Basic>()
            .ChainPopupData(new PopupData(POPUPICON.WARNING, BTN_TYPE.ConfirmCancel, null, new MasterLocalData("office_confirm_enter_cancel")))
            .ChainPopupAction(new PopupAction(() =>
            {
                var panel = GetPanel<Panel_OfficeWaitRoom>();

                if (panel.gameObject.activeSelf)
                {
                    SceneLogic.instance.isUILock = false;
                    SceneLogic.instance.PopPanel();
                }

                LocalContentsData.isOfficeEnter = false;

                Single.RealTime.KillConnect();

                SceneLogic.instance.isUILock = false;
                SceneLogic.instance.PopPopup();
            })
       );
    }

    /// <summary>
    /// 입장하기 - 오피스 외부
    /// </summary>
    private void OnClick_Enter()
    {
        string curNickname = CurNickname();
        if (string.IsNullOrEmpty(curNickname)) return;

        Single.Scene.SetDimOn();

        // 명함 여부, 명함 넘기기
        bool isOn = togplus_ShowBizCard.GetToggleIsOn();
        LocalContentsData.isOfficeEnter = isOn;
    }

    /// <summary>
    /// 저장하기 - 오피스 내부
    /// </summary>
    private void OnClick_Save()
    {
        Debug.Log("저장하기");
    }

    /// <summary>
    /// 명함 편집 패널 열기
    /// </summary>
    private void OnClick_Add()
    {
        SceneLogic.instance.PopPopup();

        Debug.Log("명함 편집 패널 열기");

        // 명함 편집 패널 열기
        // 명함 편집 백버튼에 다시 이 팝업 띄우게 하기
    }

    /// <summary>
    /// 닉네임 실시간 입력
    /// </summary>
    /// <param name="nickname"></param>
    private void OnValueChanged_Nickname(string nickname)
    {
        Util.SetActive_Warning(txtmp_WarningNicknameCheck, null);
    }
    #endregion

    #region 기능
    /// <summary>
    /// 올바른 닉네임 판단 및 반환
    /// </summary>
    /// <returns></returns>
    private string CurNickname()
    {
        string nickname = input_Nickname.text;

        if (!string.IsNullOrEmpty(nickname))
        {
            if (!Util.NicknameRestriction(nickname))
            {
                // 닉네임은 2~12자 이내 한글, 영문, 숫자로 입력해주세요!
                Util.SetActive_Warning(txtmp_WarningNicknameCheck, "4011");
                PushPopup<Popup_Basic>()
                    .ChainPopupData(new PopupData(POPUPICON.WARNING, BTN_TYPE.Confirm, null, new MasterLocalData("4011")));
            }
            else if (Util.CheckForbiddenWords(nickname))
            {
                // 금칙어가 포함되어있습니다
                Util.SetActive_Warning(txtmp_WarningNicknameCheck, "game_error_include_inapposite");
                PushPopup<Popup_Basic>()
                    .ChainPopupData(new PopupData(POPUPICON.WARNING, BTN_TYPE.Confirm, null, new MasterLocalData("game_error_include_inapposite")));
            }
            else
            {
                Util.SetActive_Warning(txtmp_WarningNicknameCheck, null);
                return nickname;
            }
        }
        else
        {
            return LocalPlayerData.NickName;
        }

        return null;
    }

    /// <summary>
    /// 오피스인지 여부
    /// </summary>
    /// <returns></returns>
    private bool IsOffice()
    {
        switch (SceneLogic.instance.GetSceneType())
        {
            case SceneName.Scene_Room_Lecture:
            case SceneName.Scene_Room_Lecture_22Christmas:
            case SceneName.Scene_Room_Meeting:
            case SceneName.Scene_Room_Meeting_22Christmas:
            case SceneName.Scene_Room_Meeting_Office:
            case SceneName.Scene_Room_Consulting:
                return true;
            default:
                return false;
        }
    }
    #endregion
}