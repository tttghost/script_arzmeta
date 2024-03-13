using FrameWork.UI;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using MEC;

public class Panel_ExpositionFileBox : PanelBase
{
    #region 변수 
    private enum SORT_TYPE { Name, Upload }

    private TMP_Text txtmp_BoothTitle;
    private TogglePlus togplus_Sort;
    private GameObject go_Upload;
    private GameObject go_TextRig;
    private GridView_Custom gridView;

    private List<Item_ExpositionFileData> fileDataList;
    private SORT_TYPE curSortType;

    private Scene_Room_Exposition_Booth sceneLogic_Booth;
    private Booth boothInfo;
    private bool isMyBooth;
    #endregion

    protected override void SetMemberUI()
    {
        #region TMP_Text
        txtmp_BoothTitle = GetUI_TxtmpMasterLocalizing(nameof(txtmp_BoothTitle));

        GetUI_TxtmpMasterLocalizing("txtmp_Title", new MasterLocalData("office_filebox_name"));
        GetUI_TxtmpMasterLocalizing("txtmp_Upload", new MasterLocalData("office_filebox_upload"));
        GetUI_TxtmpMasterLocalizing("txtmp_Sort_Name", new MasterLocalData("office_filebox_order_name"));
        GetUI_TxtmpMasterLocalizing("txtmp_Sort_Name_tog", new MasterLocalData("office_filebox_order_name"));
        GetUI_TxtmpMasterLocalizing("txtmp_Sort_Upload", new MasterLocalData("office_filebox_order_upload"));
        GetUI_TxtmpMasterLocalizing("txtmp_Sort_Upload_tog", new MasterLocalData("office_filebox_order_upload"));
        GetUI_TxtmpMasterLocalizing("txtmp_NoItem", new MasterLocalData("office_notice_filebox_empty"));
        #endregion

        #region Button
        GetUI_Button("btn_Upload", () => PushPopup<Popup_ExpositionFileUpload>().SetData());
        GetUI_Button("btn_Back", Back);
        #endregion

        #region TogglePlus
        togplus_Sort = GetUI<TogglePlus>(nameof(togplus_Sort));
        if (togplus_Sort != null)
        {
            togplus_Sort.SetToggleAction((b) =>
            {
                curSortType = (SORT_TYPE)Convert.ToInt32(b);
                GenerateCells();
            });
        }
        #endregion

        #region etc
        go_Upload = GetChildGObject(nameof(go_Upload));
        go_TextRig = GetChildGObject(nameof(go_TextRig));
        gridView = GetChildGObject("go_ScrollView").GetComponent<GridView_Custom>();
        sceneLogic_Booth = SceneLogic.instance as Scene_Room_Exposition_Booth;
        #endregion
    }

    #region 초기화
    protected override void OnEnable()
    {

        if (togplus_Sort != null)
        {
            curSortType = SORT_TYPE.Name;
            togplus_Sort.SetToggleIsOnWithoutNotify(false);
        }

        Util.RunCoroutine(Co_LoadData());
    }

    private IEnumerator<float> Co_LoadData()
    {
        if (sceneLogic_Booth != null)
        {
            if (boothInfo == null)
            {
                yield return Timing.WaitUntilTrue(() => sceneLogic_Booth.GetBooth() != null);

                boothInfo = sceneLogic_Booth.GetBooth();
                isMyBooth = sceneLogic_Booth.IsMyBooth();
            }

            LoadData();
            SetActiveUpload();
        }
    }

    public void LoadData()
    {
        if (boothInfo == null)
        {
            PushPopup<Popup_Basic>().
                ChainPopupData(new PopupData(POPUPICON.WARNING, BTN_TYPE.Confirm, null, new MasterLocalData("office_booth_notice_file")));

            SetActiveTextRig(true);
            return;
        }
        
        SetActiveTextRig(false);

        fileDataList = new List<Item_ExpositionFileData>();

        int boothId = boothInfo.id;
        Single.Web.CSAF.GetCSAFFileBoxList(boothId, (res) =>
       {
           var datas = res.fileboxes;
           int count = datas.Length;
           if (count == 0)
           {
               SetActiveTextRig(true);
           }

           for (int i = 0; i < count; i++)
           {
               var data = new Item_ExpositionFileData
               {
                   boothId = boothId,
                   id = datas[i].id,
                   fileBoxType = datas[i].fileBoxType,
                   fileName = datas[i].fileName,
                   link = datas[i].link,
                   updatedAt = datas[i].updatedAt,
                   IsAdmin = isMyBooth,
               };
               fileDataList.Add(data);
           }

           SetContent();
           GenerateCells();
       });

    }

    private void SetContent()
    {
        if (txtmp_BoothTitle != null)
        {
            Util.SetMasterLocalizing(txtmp_BoothTitle, boothInfo.name);
        }
    }

    /// <summary>
    /// 정렬 후 아이템 생성
    /// </summary>
    private void GenerateCells()
    {
        var datas = new List<Item_ExpositionFileData>();
        switch (curSortType)
        {
            case SORT_TYPE.Name: datas = fileDataList.OrderBy(x => x.fileName).ToList(); break;
            case SORT_TYPE.Upload: datas = fileDataList.OrderBy(x => x.updatedAt).ToList(); break;
        }
        gridView.UpdateContents(datas.ConvertAll(x => x as Item_Data));
    }

    /// <summary>
    /// 데이터 없을 시 텍스트 비/활성화
    /// </summary>
    /// <param name="b"></param>
    private void SetActiveTextRig(bool b)
    {
        if (go_TextRig != null) go_TextRig.SetActive(b);
    }

    /// <summary>
    /// 관리자 여부에 따른 업로드 버튼 비/활성화
    /// </summary>
    /// <param name="b"></param>
    private void SetActiveUpload()
    {
        if (go_Upload != null) go_Upload.SetActive(isMyBooth);
    }
    #endregion
}
