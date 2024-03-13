/************************************************************************************************************
 * 
 *          View_HUD_ToCenter class
 * 
 ************************************************************************************************************/
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using FrameWork.UI;

public class View_HUD_TopCenter : UIBase
{
    private Scene_OfficeRoom sceneofficeRoom;

    private GameObject goToolBar = null;
    private bool isOpenToolbar = true;
    private bool isOpenAnimation = false;
    private float distance = 100;
    private float time = 0.3f;

    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        Button btnTemp = null;

        goToolBar = GetChildGObject("go_ToolBar");

        Button btnActiveToolbar = GetUI<Button>("btn_ActiveToolBar");
        if (btnActiveToolbar != null)
        {
            btnActiveToolbar.onClick.AddListener(OnClick_ActiveToolBar);
        }

        btnTemp = GetUI<Button>("btn_ShareWeb");
        if (btnTemp != null)
        {
            btnTemp.onClick.AddListener(Onclick_OpenShareWebView);
        }

        btnTemp = GetUI<Button>("btn_ShareVideo");
        if (btnTemp != null)
        {
            btnTemp.onClick.AddListener(Onclick_OpenVideoSearch);
        }

        btnTemp = GetUI<Button>("btn_LocalFileBrowsing");
        if (btnTemp != null)
        {
            btnTemp.onClick.AddListener(Onclick_OpenLocalFileBrowsing);
        }
    }

    protected override void Start()
    {
        base.Start();

        if (Util.UtilOffice.IsOffice())
		{
            sceneofficeRoom = SceneLogic.instance as Scene_OfficeRoom;

            if (goToolBar != null)
                goToolBar.SetActive(true);
        }
        else
		{
            if (goToolBar != null)
                goToolBar.SetActive(false);
        }
    }

    protected override void OnEnable()
	{
		base.OnEnable();
    }

    public async void OnClick_ActiveToolBar()
    {
        if (goToolBar == null)
            return;

        if (isOpenAnimation)
            return;

        DEBUG.LOG("ActiveToolBar()", eColorManager.UI);

        isOpenAnimation = true;

        isOpenToolbar = !isOpenToolbar;

        float posY = -50.0f;
        if (isOpenToolbar == false)
            posY = 110.0f;

        LeanTween.moveLocalY(goToolBar, posY, 0.3f);

        await UniTask.Delay(300);

        isOpenAnimation = false;
    }

    private void Onclick_OpenShareWebView()
    {
        if (IsHaveNoOfficePermission(eOfficePermissionMaster.SCREEN, null))
            return;

        SceneLogic.instance.PushPanel<Panel_WebviewSharing>(false);
    }

    private void Onclick_OpenVideoSearch()
    {
        if (IsHaveNoOfficePermission(eOfficePermissionMaster.SCREEN, null))
            return;

        SceneLogic.instance.PushPanel<Panel_VideoSharing>(false);
    }

    private void Onclick_OpenLocalFileBrowsing()
    {
        if (sceneofficeRoom == null)
            return;

        // TODO : BLUEKID - 로컬라이징 키 적용 필요
        if (sceneofficeRoom.myPermission.ScreenPermission == false)
        {
            OpenToast<Toast_Basic>()
            .ChainToastData(new ToastData_Basic(TOASTICON.Current, new MasterLocalData("office_reception_permission_unavailable")));
            return;
        }

        // BKK TODO: 상세 기획 나오면 할 것

        FilePicker filePicker = FindObjectOfType<FilePicker>();

        filePicker.OnButtonPdfBrowsing();
    }

    private bool IsHaveNoOfficePermission(eOfficePermissionMaster permissionMaster, Toggle toggle)
    {
        bool isRet = false;

        switch (permissionMaster)
        {
        case eOfficePermissionMaster.CHAT:
            {
                if (sceneofficeRoom.myPermission.ChatPermission == false)
                {
                    if (toggle != null)
                        toggle.isOn = false;

                    isRet = true;
                }
            }
            break;

        case eOfficePermissionMaster.VOICE:
            {
                if (sceneofficeRoom.myPermission.VoicePermission == false)
                {
                    if (toggle != null)
                        toggle.isOn = false;

                    isRet = true;
                }
            }
            break;

        case eOfficePermissionMaster.VIDEO:
            {
                if (sceneofficeRoom.myPermission.VideoPermission == false)
                {
                    if (toggle != null)
                        toggle.isOn = false;

                    isRet = true;
                }
                break;
            }

        case eOfficePermissionMaster.SCREEN:
            {
                if (sceneofficeRoom.myPermission.ScreenPermission == false)
                {
                    if (toggle != null)
                        toggle.isOn = false;

                    isRet = true;
                }
            }
            break;
        }

        if (isRet)
        {
            OpenToast<Toast_Basic>()
            .ChainToastData(new ToastData_Basic(TOASTICON.Current, new MasterLocalData("office_reception_permission_unavailable")));

            if (toggle != null)
                toggle.isOn = false;
            isRet = true;
        }

        return isRet;
    }
}
