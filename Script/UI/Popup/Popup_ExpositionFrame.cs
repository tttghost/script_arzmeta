using Assets._Launching.DEV.Script.Framework.Network.WebPacket;
using Cysharp.Threading.Tasks;
using FrameWork.Network;
using FrameWork.UI;
using Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class Popup_ExpositionFrame : Popup_FrameBase
{
    #region 변수
    private GameObject go_Btn;
    private GameObject go_FrameParent;
    private TMP_InputField input_SiteURL;
    private TMP_Text txtmp_Recommended;
    
    private Button btn_SiteURL;

    private BannerInfo bannerInfo;
    private string image; //실제 이미지 주소
    
    private const int maxWidthHeight = 4200;
    private const int maxMB = 15;

    private bool isChangedImage = false;
    private bool isChangedSiteUrl = false;
    #endregion

    #region 함수

    #region 초기화, 셋데이터
    protected override void SetMemberUI()
    {
        base.SetMemberUI();
        go_Btn = GetChildGObject(nameof(go_Btn));
        go_FrameParent = GetChildGObject(nameof(go_FrameParent));

        input_SiteURL = GetUI_TMPInputField(nameof(input_SiteURL), (str) =>
        {
            isChangedSiteUrl = str != bannerInfo.interactionValue;
            SeButtontInteractable();
        });

        txtmp_Recommended = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Recommended));

        btn_SiteURL = GetUI_Button(nameof(btn_SiteURL), OnClick_SiteURL);
    }
    private void OnClick_SiteURL()
    {
        input_SiteURL.text = string.Empty;
    }

    /// <summary>
    /// 셋 데이터 최초
    /// </summary>
    /// <param name="num"></param>
    public async void SetData(BannerInfo bannerInfo, FRAME_KIND fRAME_KIND)
    {
        this.bannerInfo = bannerInfo.DeepCopy();

        input_SiteURL.text = bannerInfo.interactionValue == Cons.EMPTY ? string.Empty : bannerInfo.interactionValue;

        if (LocalContentsData.GetBannerInfo(bannerInfo.bannerId) != null) // 앨범에 저장된 이미지가 있으면 -> 로컬에 있으면 : 로컬꺼 사용, 로컬에 없으면 : 다운로드
        {
            await LoadFileOnData(bannerInfo);
        }
        else //앨범에 저장된 이미지가 없다면 -> 새로 저장
        {
            OnClick_TempDataDelete();
            togplus_AlbumAttach.SetToggleIsOn(true);
        }

        image = Cons.ORIGIN;

        ResetIsChanged();
        SeButtontInteractable();

        SetRecommendedSize(fRAME_KIND);
        SetFrame(fRAME_KIND);

        Util.RefreshLayout(gameObject, "go_Content");
        Util.RefreshLayout(gameObject, "go_Content");
        Util.RefreshLayout(gameObject, "go_Content");

        BackAction_Custom = OnClick_Back;
    }

    /// <summary>
    /// 배너사이즈 권고
    /// </summary>
    private void SetRecommendedSize(FRAME_KIND fRAME_KIND)
    {
        string recommended = Util.GetMasterLocalizing(new MasterLocalData("common_title_img_recommended"));
        string size = string.Empty;
        switch (fRAME_KIND)
        {
            case FRAME_KIND.b_p_sframea:
                size = "1024 X 639";
                break;
            case FRAME_KIND.b_p_sframeb:
                break;
            case FRAME_KIND.b_p_sframec:
                size = "1024 X 1976";
                break;
            default:
                break;
        }
        txtmp_Recommended.text = $"{recommended} : {size}";
    }

    /// <summary>
    /// 프레임사이즈 변경
    /// </summary>
    /// <param name="fRAME_KIND"></param>
    private void SetFrame(FRAME_KIND fRAME_KIND)
    {
        Vector2 frameScale = Vector2.zero;
        switch (fRAME_KIND)
        {
            case FRAME_KIND.b_p_sframea: frameScale = new Vector2(2f, 1.25f); break;
            case FRAME_KIND.b_p_sframeb: frameScale = new Vector2(1.5f, 1.5f); break;
            case FRAME_KIND.b_p_sframec: frameScale = new Vector2(1f, 2f); break;
        }
        go_FrameParent.transform.GetComponent<RectTransform>().sizeDelta = frameScale * 400f;

    }
    private void ResetIsChanged()
    {
        isChangedImage = false;
        isChangedSiteUrl = false;
    }


    /// <summary>
    /// 서버에 앨범데이터가 무조건 있기 때문에 로드 한다.
    /// </summary>
    private async Task LoadFileOnData(BannerInfo bannerInfo)
    {
        switch ((FRAMEIMAGEAPPEND_TYPE)bannerInfo.uploadType)
        {
            case FRAMEIMAGEAPPEND_TYPE.로컬이미지: togplus_AlbumAttach.SetToggleIsOn(true); break;
            case FRAMEIMAGEAPPEND_TYPE.URL이미지: togplus_URLAttach.SetToggleIsOn(true); break;
        }

        tempTex = await Util.Co_LoadExpositionFrame(bannerInfo, true); //첫로드, 없으면서버에서 이미지 다운

        OnData_Texture();
    }
    #endregion

    #region 텍스쳐 불러와서 임시저장
    /// <summary>
    /// 데이터 임시저장
    /// </summary>
    protected override async void OnClick_TempDataLoad()
    {
        base.OnClick_TempDataLoad();
        await Co_TempDataLoad();

        //가져온텍스트가 없다면? or 링크가 이미지가 아니라면?

        tempTex = await Util.Co_LoadExpositionFrame(bannerInfo, false); //더미로 템프이미지 가져오기때문에 저장하지 않음
        if (tempTex != null)
        {
            OnData_Texture();
        }
    }

    /// <summary>
    /// 어떤 타입인지에 따라 type과 imageName 추가
    /// </summary>
    /// <returns></returns>
    private async UniTask Co_TempDataLoad()
    {
        bannerInfo.uploadType = (int)fRAMEIMAGEAPPEND_TYPE;

        //텍스쳐 임시저장
        switch (fRAMEIMAGEAPPEND_TYPE)
        {
            case FRAMEIMAGEAPPEND_TYPE.로컬이미지:
                {
                    string imagePath = await Util.Co_FindLocalTexPath();
                    if(imagePath == string.Empty)
                    {
                        return;
                    }
                    bannerInfo.uploadValue = imagePath;
                    image = bannerInfo.uploadValue;
                }
                break; //로컬앨범 풀네임 이다. formdata용 path

            case FRAMEIMAGEAPPEND_TYPE.URL이미지:
                {
                    bannerInfo.uploadValue = input_ImageURL.text;
                    image = string.Empty;
                }
                break;
        }
    }
    #endregion

    #region 임시텍스쳐 삭제
    /// <summary>
    /// 임시저장 데이터 지우기
    /// </summary>
    protected override void OnClick_TempDataDelete()
    {
        tempTex = null;
        OnData_Texture();
    }
    #endregion

    #region 임시텍스쳐 서버에 저장
    /// <summary>
    /// 서버에 저장하기
    /// </summary>
    protected override void OnClick_UploadData()
    {
        base.OnClick_UploadData();

        if (tempTex != null)
        {
            bannerInfo.interactionValue = input_SiteURL.text == string.Empty ? Cons.EMPTY : input_SiteURL.text;

            Texture2D iosTex = null; //ios 이미지 확장자 "heic", "heif", "heix", "hevc" 대응용 tex
#if UNITY_IOS
            iosTex = tempTex;
#endif

            if (LocalContentsData.GetBannerInfo(bannerInfo.bannerId) == null) //등록
            {
                Single.Web.CSAF.CreateCSAFBanner(bannerInfo, iosTex, OnCSAFBanner_CreateEdit, image);
            }
            else //수정
            {
                Single.Web.CSAF.EditCSAFBanner(bannerInfo, iosTex, OnCSAFBanner_CreateEdit, image);
            }
        }
        else //삭제
        {
            bannerInfo.interactionValue = Cons.EMPTY;
            Single.Web.CSAF.DeleteCSAFBanner(bannerInfo, OnCSAFBanner_Delete);
        }
    }


    /// <summary>
    /// 박람회부스 배너 생성,수정 콜백
    /// </summary>
    /// <param name="res"></param>
    private async void OnCSAFBanner_CreateEdit(BannerInfoRes res)
    {
        switch ((WEBERROR)res.error)
        {
            case WEBERROR.NET_E_SUCCESS:
                tempTex = await Util.Co_LoadExpositionFrame(bannerInfo, true);
                SetPanelFrame(tempTex);
                break;
            case WEBERROR.NET_E_DB_FAILED:
            case WEBERROR.NET_E_BAD_IMAGE:
                PushPopup<Popup_Basic>()
                .ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.Confirm, masterLocalDesc: new MasterLocalData("myroom_error_append_anable")));
                break;
        }
    }

    /// <summary>
    /// 박람회부스 배너 삭제 콜백
    /// </summary>
    /// <param name="res"></param>
    private void OnCSAFBanner_Delete(DeleteBannerRes res)
    {
        switch ((WEBERROR)res.error)
        {
            case WEBERROR.NET_E_SUCCESS:
                SetPanelFrame(tempTex);
                break;
            case WEBERROR.NET_E_DB_FAILED:
            case WEBERROR.NET_E_BAD_IMAGE:
                PushPopup<Popup_Basic>()
                .ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.Confirm, masterLocalDesc: new MasterLocalData("myroom_error_append_anable")));
                break;
        }
    }


    /// <summary>
    /// 바뀐이미지 브로드캐스트&팝팝업&토스트메세지
    /// </summary>
    /// <param name="tex"></param>
    protected override void SetPanelFrame(Texture2D tex)
    {
        base.SetPanelFrame(tex);
        C_WILDCARD(bannerInfo.bannerId);
        GetPanel<Panel_ExpositionFrame>().SetSprite(Util.Tex2Sprite(tex));

        PopPopup();
        SceneLogic.instance.isUILock = false;

        OpenToast<Toast_Basic>()
            .ChainToastData(new ToastData_Basic(TOASTICON.None, new MasterLocalData("008"), 1.5f));
    }

    /// <summary>
    /// 박람회 배너오브젝트 실시간 갱신 - 전체
    /// </summary>
    /// <param name="bannerId"></param>
    protected override void C_WILDCARD(int bannerId)
    {
        base.C_WILDCARD(bannerId);
        C_WILDCARD packet = new C_WILDCARD();
        packet.Code = (int)WILDCARD_TYPE.EXPOSITION_BANNER;
        packet.Data = bannerId.ToString();
        packet.Type = 1;
        Single.RealTime.Send(packet);
    }
    
    /// <summary>
    /// 저장버튼 인터렉션 변경
    /// </summary>
    private void SeButtontInteractable()
    {
        go_Btn.GetComponent<CanvasGroup>().interactable = isChangedImage || isChangedSiteUrl;
    }

    #endregion

    #region 공통함수
    /// <summary>
    /// 데이터 적용
    /// </summary>
    /// <param name="targetName"></param>
    /// <param name="tempTex"></param>
    protected override async void OnData_Texture()
    {
        isChangedImage = true;
        SeButtontInteractable();

        input_ImageURL.text = string.Empty;

        if (tempTex == null)
        {
            txtmp_Target.text = string.Empty;

            btn_Delete.gameObject.SetActive(false);
            //img_Frame.sprite = null;
            SetSprite(null);
        }
        else
        {
            if ((FRAMEIMAGEAPPEND_TYPE)bannerInfo.uploadType == FRAMEIMAGEAPPEND_TYPE.로컬이미지)
            {
                go_LoadError.SetActive(false);
                
                // 가로나 세로 길이가 4200이 넘는지 or 15메가 넘는지 체크
                if (Util.CheckTexWidthHeight(maxWidthHeight, tempTex)
                    || await Util.Co_GetFileSize(bannerInfo.uploadValue, eDisk.MB) > maxMB)
                {
                    isChangedImage = false;
                    go_LoadError.SetActive(true);
                    Util.SetMasterLocalizing(txtmp_LoadError, new MasterLocalData("myroom_info_appendimage", maxWidthHeight, maxMB));
                    return;
                }

                txtmp_Target.text = Path.GetFileName(bannerInfo.uploadValue);
                //이름 서버에서 타임스탬프 넣은것으로 수정
                string temp = txtmp_Target.text.Split('_').First();
                if (temp.Length == 14) //타임적용된것 서버에서 받은것
                {
                    txtmp_Target.text = string.Join("_", temp.Skip(1));
                }
            }

            btn_Delete.gameObject.SetActive(true);
            //img_FrameImage.sprite = Util.Tex2Sprite(tempTex);
            SetSprite(Util.Tex2Sprite(tempTex));
        }

    }

 

    #endregion

    /// <summary>
    /// 뒤로가기
    /// </summary>
    protected override void OnClick_Back()
    {
        base.OnClick_Back();
        if (go_Btn.GetComponent<CanvasGroup>().interactable)
        {
            PushPopup<Popup_Basic>()
                .ChainPopupData(new PopupData(POPUPICON.CHECK, BTN_TYPE.ConfirmCancel, null, new MasterLocalData("myroom_confirm_exit_nosave")))
                .ChainPopupAction(new PopupAction(
                    () =>
                    {
                        SceneLogic.instance.isUILock = false;
                        SceneLogic.instance.PopPopup();
                    },
                    () => BackAction_Custom = OnClick_Back));
        }
        else
        {
            BackAction_Custom = null;
            Back();
        }
    }

    #endregion
}
