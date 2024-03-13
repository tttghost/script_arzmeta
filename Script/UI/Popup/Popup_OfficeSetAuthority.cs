using FrameWork.UI;
using MEC;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup_OfficeSetAuthority : PopupBase
{
    private Dictionary<OfficeAuthority, Toggle> dicToggles = new Dictionary<OfficeAuthority, Toggle>();

    public OfficeAuthority userAuthority { get; private set; }

    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        dicToggles.Clear();

        GetUI_TxtmpMasterLocalizing("txtmp_Set_Admin", new MasterLocalData("office_getting_enter"));
        GetUI_TxtmpMasterLocalizing("txtmp_Set_AdminSub", new MasterLocalData("office_getting_enter"));
        GetUI_TxtmpMasterLocalizing("txtmp_Set_Normal", new MasterLocalData("office_getting_enter"));
        GetUI_TxtmpMasterLocalizing("txtmp_Set_Speaker", new MasterLocalData("office_getting_enter"));
        GetUI_TxtmpMasterLocalizing("txtmp_Set_Audience", new MasterLocalData("office_getting_enter"));
        GetUI_TxtmpMasterLocalizing("txtmp_Set_Observer", new MasterLocalData("office_getting_enter"));

        // 오피스 권한에 따른 토글들 Dictionary 에 담기
        OfficeAuthority authority = OfficeAuthority.관리자;
        ToggleGroup togg_Authority = GetUI<ToggleGroup>(nameof(togg_Authority));
        if(togg_Authority != null)
        {
            for(int i = 0; i < togg_Authority.transform.childCount; ++i)
            {
                Transform child = togg_Authority.transform.GetChild(i);
                Toggle toggle = child.GetComponent<Toggle>();
                if(toggle != null)
                    dicToggles.Add(authority, toggle);

                authority += 1;
            }
        }

        foreach(KeyValuePair<OfficeAuthority, Toggle> pair in dicToggles)
        {
            pair.Value.onValueChanged.AddListener(
                      delegate { OnValueChanged_Authority(pair.Value.isOn, pair.Key); });
        }
    }

    private void OnValueChanged_Authority(bool isOn, OfficeAuthority authority)
    {
        if(isOn)
        {
            userAuthority = authority;
            DEBUG.LOG("권한팝업 : userAuthority = " + authority);
        }
    }

    protected override void Start()
    {
        GetUI_Button("btn_Close", () =>
       {
           SceneLogic.instance.Back();
            //SceneLogic.instance.PopPopup();
        });
    }

    #region Toggle 활성화, 비활성화 하는 함수들
    public void SetToggleState(OfficeAuthority myAuthority, OfficeAuthority targetAuthority)
    {
        userAuthority = targetAuthority;

        // 타겟 유저의 권한 Toggle 활성화
        if(dicToggles.ContainsKey(targetAuthority))
            dicToggles[targetAuthority].isOn = true;

        SetInteractable_AllToggle(false);

        if(Util.UtilOffice.IsLectureRoom())
        {
            SetActive_ToggleAll(true);
            SetActive_Toggle(OfficeAuthority.일반참가자, false);
        }
        else
        {
            SetActive_ToggleAll(false);
            SetActive_Toggle(OfficeAuthority.관리자, true);
            SetActive_Toggle(OfficeAuthority.일반참가자, true);
        }

        // 나의 권한이 무엇인지
        switch(myAuthority)
        {
        case OfficeAuthority.관리자:
            {
                if(Util.UtilOffice.IsLectureRoom())
                {
                    switch(targetAuthority)
                    {
                    case OfficeAuthority.관리자:
                    default:
                        {
                            SetInteractable_AllToggle(false);
                        }
                        break;

                    case OfficeAuthority.부관리자:
                        {
                            SetInteractable_AllToggle(true);
                            ToggleIsOn(dicToggles[OfficeAuthority.관리자]);
                        }
                        break;

                    case OfficeAuthority.발표자:
                        {
                            SetInteractable_AllToggle(true);
                            ToggleIsOn(dicToggles[OfficeAuthority.관리자]);
                        }
                        break;

                    case OfficeAuthority.청중:
                        {
                            SetInteractable_AllToggle(true);
                            ToggleIsOn(dicToggles[OfficeAuthority.관리자]);
                        }
                        break;

                    case OfficeAuthority.관전자:
                        {
                            SetInteractable_AllToggle(true);
                            ToggleIsOn(dicToggles[OfficeAuthority.관리자]);
                        }
                        break;
                    }
                }
                else
                {
                    switch(targetAuthority)
                    {
                    case OfficeAuthority.관리자:
                    default:
                        {
                            SetInteractable_AllToggle(false);
                        }
                        break;

                    case OfficeAuthority.일반참가자:
                        {
                            SetInteractable_Toggle(OfficeAuthority.관리자, true);
                            SetInteractable_Toggle(OfficeAuthority.일반참가자, false);
                        }
                        break;
                    }
                }
            }
            break;

        case OfficeAuthority.부관리자: // 내가 부관리자
            {
                switch(targetAuthority)
                {
                case OfficeAuthority.관리자:
                default:
                    {
                        SetInteractable_AllToggle(false);
                    }
                    break;

                case OfficeAuthority.부관리자:
                    {
                        SetInteractable_Toggle(OfficeAuthority.관리자, false);
                        SetInteractable_Toggle(OfficeAuthority.부관리자, false);
                        SetInteractable_Toggle(OfficeAuthority.발표자, true);
                        SetInteractable_Toggle(OfficeAuthority.청중, true);
                        SetInteractable_Toggle(OfficeAuthority.관전자, true);
                    }
                    break;

                case OfficeAuthority.발표자:
                    {
                        SetInteractable_Toggle(OfficeAuthority.관리자, false);
                        SetInteractable_Toggle(OfficeAuthority.부관리자, false);
                        SetInteractable_Toggle(OfficeAuthority.발표자, true);
                        SetInteractable_Toggle(OfficeAuthority.청중, true);
                        SetInteractable_Toggle(OfficeAuthority.관전자, true);
                    }
                    break;

                case OfficeAuthority.청중:
                    {
                        SetInteractable_Toggle(OfficeAuthority.관리자, false);
                        SetInteractable_Toggle(OfficeAuthority.부관리자, false);
                        SetInteractable_Toggle(OfficeAuthority.발표자, true);
                        SetInteractable_Toggle(OfficeAuthority.청중, false);
                        SetInteractable_Toggle(OfficeAuthority.관전자, true);
                    }
                    break;

                case OfficeAuthority.관전자:
                    {
                        SetInteractable_Toggle(OfficeAuthority.관리자, false);
                        SetInteractable_Toggle(OfficeAuthority.부관리자, false);
                        SetInteractable_Toggle(OfficeAuthority.발표자, true);
                        SetInteractable_Toggle(OfficeAuthority.청중, true);
                        SetInteractable_Toggle(OfficeAuthority.관전자, false);
                    }
                    break;
                }
            }
            break;

        default:    // 관리자, 부관리자가 아닌 경우
            {
                SetInteractable_AllToggle(false);
            }
            break;
        }
    }

    private void SetActive_Toggle(OfficeAuthority authority, bool isActive)
    {
        if(dicToggles.ContainsKey(authority) == false)
            return;

        dicToggles[authority].gameObject.SetActive(isActive);
    }

    private void SetActive_ToggleAll(bool isActive)
    {
        foreach(KeyValuePair<OfficeAuthority, Toggle> pair in dicToggles)
        {
            pair.Value.gameObject.SetActive(isActive);
        }
    }

    private void SetInteractable_Toggle(OfficeAuthority authority, bool isInteractable)
    {
        if(dicToggles.ContainsKey(authority) == false)
            return;

        dicToggles[authority].interactable = isInteractable;
    }

    private void SetInteractable_AllToggle(bool isInteractable)
    {
        foreach(KeyValuePair<OfficeAuthority, Toggle> pair in dicToggles)
        {
            pair.Value.interactable = isInteractable;
        }
    }
    #endregion
}
/**************************************************************************************
 * <내가 관리자 일 경우>

관리자(호스트)
	회의실
		관리자		    X
		일반참가자		X
		
		화면권한		O		
		채팅권한		O		
		음성권한		O		
		화상권한		O		
	
	강의실
		관리자		    X
		부관리자		X
		발표자		    X
		청중			X
		관전자		    X
		
		화면권한		X
		채팅권한		O		
		음성권한		O		
		화상권한		O		
	
부관리자
	회의실
		관리자		    O
		일반참가자		O
		
		화면권한		O		
		채팅권한		O		
		음성권한		O		
		화상권한		O		
	
	강의실
		관리자		    O
		부관리자		X
		발표자	    	O
		청중			O
		관전자		    O
		
		화면권한		O
		채팅권한		O
		음성권한		O
		화상권한		O
	
일반참가자
	회의실
		관리자		    O
		일반참가자		X
		
발표자
	강의실
		관리자		    O
		부관리자		O
		발표자		    X
		청중			O
		관전자		    O
청중
	강의실
		관리자		    O
		부관리자		O
		발표자		    O
		청중			X
		관전자		    O
관전자
	강의실
		관리자		    O
		부관리자		O
		발표자		    O
		청중			O
		관전자		    X

**************************************************************************/
