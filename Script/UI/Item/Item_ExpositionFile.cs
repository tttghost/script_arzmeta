using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using FrameWork.UI;

class Item_ExpositionFile : FancyGridViewCell_Custom
{
    #region 변수
    private Item_ExpositionFileData data;

    private TMP_Text txtmp_FileName;
    private Button btn_Menu;
    private Image img_FileIcon;
    private GameObject go_SubMenu;
    private Camera stackCamera;

    private List<Sprite> iconSpriteList = new List<Sprite>();
    #endregion

    protected override void SetMemberUI()
    {
        #region TMP_Text
        txtmp_FileName = uIBase.GetUI_TxtmpMasterLocalizing(nameof(txtmp_FileName));

        uIBase.GetUI_TxtmpMasterLocalizing("txtmp_Edit", new MasterLocalData("office_filebox_modify"));
        uIBase.GetUI_TxtmpMasterLocalizing("txtmp_Delete", new MasterLocalData("office_filebox_delete"));
        #endregion

        #region Button
        btn_Menu = uIBase.GetUI_Button(nameof(btn_Menu), OnClick_Menu);
        if (btn_Menu != null)
        {
            btn_Menu.gameObject.SetActive(false);
        }

        uIBase.GetUI_Button("btn_ExpositionFile", OnClick_OpenLink);
        uIBase.GetUI_Button("btn_Edit", OnClick_Edit);
        uIBase.GetUI_Button("btn_Delete", OnClick_Delete);
        #endregion

        #region Image
        img_FileIcon = uIBase.GetUI_Img(nameof(img_FileIcon));
        #endregion

        #region etc
        go_SubMenu = uIBase.GetChildGObject(nameof(go_SubMenu));
        if (go_SubMenu != null)
        {
            var eventTrigger = go_SubMenu.GetComponent<EventTrigger>();
            stackCamera = SceneLogic.instance.stackCamera;
            //stackCamera = GameObject.Find(Define.STACKCAMERA).GetComponent<Camera>();
            Util.SetEventTrigger(eventTrigger, EventTriggerType.Deselect, () =>
            {
                if (ScreenPoint((RectTransform)go_SubMenu.transform)
                || ScreenPoint((RectTransform)btn_Menu.transform))
                {
                    if (EventSystem.current.currentSelectedGameObject != go_SubMenu)
                    {
                        EventSystem.current.SetSelectedGameObject(go_SubMenu);
                    }
                    return;
                }
                SetActiveSubMenu(false);
            });
        }
        #endregion

        LoadIcon();
    }

    #region 
    private bool ScreenPoint(RectTransform rect)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(rect, Input.mousePosition, stackCamera);
    }

    /// <summary>
    /// 파일 타입 아이콘 로드
    /// </summary>
    private void LoadIcon()
    {
        var icons = new string[] { "icon_file_cloud_01", "icon_file_document_01", "icon_file_vid_01" }; // 스프라이트 이름 넣기

        foreach (var item in icons)
        {
            var sprite = Single.Resources.Load<Sprite>(Cons.Path_Image + item);
            if (sprite == null)
            {
                sprite = Util.GetDummySprite();
            }
            iconSpriteList.Add(sprite);
        }
    }

    public override void UpdateContent(Item_Data itemData)
    {
        if (itemData is Item_ExpositionFileData _data)
        {
            data = _data;

            base.UpdateContent(itemData);
        }
    }

    protected override void SetContent()
    {
        if (txtmp_FileName != null)
        {
            Util.SetMasterLocalizing(txtmp_FileName, data.fileName);
        }

        if (img_FileIcon != null)
        {
            var index = data.fileBoxType - 1;
            if (iconSpriteList.Count <= 0) return;

            img_FileIcon.sprite = iconSpriteList[index];
        }

        if (btn_Menu != null)
        {
            btn_Menu.gameObject.SetActive(data.IsAdmin);
            SetActiveSubMenu(false);
        }
    }
    #endregion

    #region 
    /// <summary>
    /// 파일 링크 열기
    /// </summary>
    private void OnClick_OpenLink()
    {
        if (string.IsNullOrEmpty(data.link))
        {
            uIBase.PushPopup<Popup_Basic>()
                .ChainPopupData(new PopupData(POPUPICON.WARNING, BTN_TYPE.Confirm, null, new MasterLocalData("office_notice_filebox_empty")));
            return;
        }

        Application.OpenURL(data.link);
    }

    /// <summary>
    /// (관리자일 시) 파일 수정/삭제 메뉴 열기
    /// </summary>
    private void OnClick_Menu()
    {
        SetActiveSubMenu(!go_SubMenu.activeInHierarchy);
    }

    /// <summary>
    /// 게시 파일 데이터 수정
    /// </summary>
    private void OnClick_Edit()
    {
        SetActiveSubMenu(false);
        uIBase.PushPopup<Popup_ExpositionFileUpload>().SetData(data);
    }

    /// <summary>
    /// 게시 파일 삭제
    /// </summary>
    private void OnClick_Delete()
    {
        SetActiveSubMenu(false);
        SceneLogic.instance.PushPopup<Popup_Basic>()
            .ChainPopupData(new PopupData(POPUPICON.WARNING, BTN_TYPE.ConfirmCancel, null, new MasterLocalData("office_request_filebox_delete"))) // 해당 파일을 삭제하시겠습니까?
            .ChainPopupAction(new PopupAction(() =>
            {
                Single.Web.CSAF.DeleteCSAFFilebox(data.boothId, data.id, (res) => SceneLogic.instance.GetPanel<Panel_ExpositionFileBox>().LoadData());
            }));
    }

    private void SetActiveSubMenu(bool b)
    {
        if (go_SubMenu != null)
        {
            if (b) EventSystem.current.SetSelectedGameObject(go_SubMenu);
            go_SubMenu.SetActive(b);
        }
    }
    #endregion
}
