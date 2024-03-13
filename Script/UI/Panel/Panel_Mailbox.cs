using Cysharp.Threading.Tasks;
using FrameWork.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Panel_Mailbox : PanelBase
{
    private TMP_Text txtmp_Title;
    private TogglePlus togplus_Godowon;
    private TogglePlus togplus_Giftbox;
    private TMP_Text txtmp_GodowonOn;
    private TMP_Text txtmp_GodowonOff;
    private TMP_Text txtmp_GiftboxOn;
    private TMP_Text txtmp_GiftboxOff;
    private Image img_newGiftbox;

    WebPostbox webPostbox;  // 우편함 관련 네트워크

    protected override void Awake()
    {
        base.Awake();

        webPostbox = Single.Web.webPostbox;

        // 선물함 목록 조회 요청 후 콜백 등록
        webPostbox.OnResponseGiftMailList += OnResponseGiftMailList;
    }

    private void OnDestroy()
    {
        // 선물함 목록 조회 요청 후 콜백 해제
        if (webPostbox != null)
            webPostbox.OnResponseGiftMailList -= OnResponseGiftMailList;
    }

    protected override void SetMemberUI()
    {
        //이제 우편함부터 열려야함
        base.SetMemberUI();

        txtmp_Title = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Title), new MasterLocalData("10100"));

        togplus_Godowon = GetUI<TogglePlus>(nameof(togplus_Godowon));
        togplus_Godowon.SetToggleOnAction(() => ChangeView<View_Godowon>());
        txtmp_GodowonOn = GetUI_TxtmpMasterLocalizing(nameof(txtmp_GodowonOn), new MasterLocalData("mail_morningmail"));
        txtmp_GodowonOff = GetUI_TxtmpMasterLocalizing(nameof(txtmp_GodowonOff), new MasterLocalData("mail_morningmail"));

        togplus_Giftbox = GetUI<TogglePlus>(nameof(togplus_Giftbox));
        togplus_Giftbox.SetToggleOnAction(() => ChangeView<View_Giftbox>());
        txtmp_GiftboxOn = GetUI_TxtmpMasterLocalizing(nameof(txtmp_GiftboxOn), new MasterLocalData("mail_gift"));
        txtmp_GiftboxOff = GetUI_TxtmpMasterLocalizing(nameof(txtmp_GiftboxOff), new MasterLocalData("mail_gift"));

        img_newGiftbox = GetUI_Img(nameof(img_newGiftbox));
        go_Center = GetChildGObject(nameof(go_Center));
    }

    public GameObject go_Center { get; set; }

    protected async override void OnEnable()
    {
        base.OnEnable();
        await UniTask.Delay(TimeSpan.FromSeconds(0.01f));
        togplus_Giftbox.SetToggleIsOn(true);
        //SetOpenStartCallback(() => ToggleIsOn(tog_Giftbox));
        //Util.ProcessQueue("mailbox", () => tog_Godowon.isOn = true, 0.1f);

        // 뉴 아이콘 체크. 최초 우편함 목록 조회를 OnResponseGiftMailList 콜백 등록 전에 호출 하기에 먼저 뉴아이콘 체크를 한다
        OnResponseGiftMailList();
    }

    public override void Back(int cnt = 1)
    {
        base.Back(cnt);

        // 우편함 열람 정보 갱신
        LocalPlayerData.Method.OpenGiftMailbox();
        Single.Web.webPostbox.OnResponseGiftMailList?.Invoke();
    }

    /// <summary> 서버로부터 우편 목록 정보 획득 후 new 표시 처리용 </summary>
    void OnResponseGiftMailList()
    {
        bool isNew = LocalPlayerData.Method.IsExistNewMail();
        img_newGiftbox.gameObject.SetActive(isNew);
    }
}
