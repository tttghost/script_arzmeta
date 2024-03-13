using FrameWork.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Panel_MyRoomMain : PanelBase
{
    private Image       img_Title_Input;
    private TMP_Text    txtmp_Title_Input;
    private Button      btn_ItemDetail;

    private Transform cameraTr;
    private int floorLayer;
    private RaycastHit hit;
    private Vector3 pivotPos; //피벗포지션
    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        //img
        img_Title_Input = GetUI_Img(nameof(img_Title_Input));

        //txtmp
        txtmp_Title_Input = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Title_Input));

        //btn
        btn_ItemDetail = GetUI_Button(nameof(btn_ItemDetail), () => PushPopup<Popup_ItemDetail>());
        GetUI_Button("btn_ResetCamera", OnClick_ResetCamera);


    }
    protected override void Start()
    {
        base.Start();
        OnItemType(null);
        floorLayer = 1 << LayerMask.NameToLayer("Floor");
        cameraTr = Scene.MyRoom.EditorCamera.transform;
    }
    private void Update()
    {
        if (Physics.Raycast(cameraTr.position, cameraTr.forward, out hit, floorLayer))
        {
            pivotPos = hit.point;
        }
    }


    /// <summary>
    /// 카메라 위치 원점으로
    /// </summary>
    private void OnClick_ResetCamera()
    {
        //MyRoomCustomManager.Instance.mainCamera.transform.localEulerAngles = Vector3.right * 45f;
        //MyRoomCustomManager.Instance.mainCamera.transform.localPosition = new Vector3(0f, 27f, -27f);
        cameraTr.localPosition += pivotPos;
    }
    public void OnItemType(db.Item item)
    //public void OnItemType(ItemType itemType)
    {
        img_Title_Input.color = Color.clear;
        img_Title_Input.sprite = null;
        //txtmp_Title_Input.text = "아이템을 선택해 주세요.";
        Util.SetMasterLocalizing(txtmp_Title_Input, new MasterLocalData("common_request_select_item"));
        
        btn_ItemDetail.gameObject.SetActive(false);
        if (item != null)
        {
            img_Title_Input.color = Color.white;
            img_Title_Input.sprite = Util.GetItemIconSprite(item.id);
            //txtmp_Title_Input.text = MasterDataManager.Instance.dataLocalization.GetData(item.name).kor;
            Util.SetMasterLocalizing(txtmp_Title_Input, new MasterLocalData(item.name));
            btn_ItemDetail.gameObject.SetActive(true);
        }
    }
}
