using FrameWork.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Popup_ItemDetail : PopupBase
{
    private Image       img_Target;
    private Image       img_CategoryType_icon;
    private TMP_Text    txtmp_Title;
    private TMP_Text    txtmp_ItemSize;
    private TMP_Text    txtmp_CategoryType;
    private TMP_Text txtmp_Description;

    private Button btn_PopupExit2;
    protected override void SetMemberUI()
    {
        base.SetMemberUI();
        img_Target = GetUI_Img(nameof(img_Target));
        img_CategoryType_icon = GetUI_Img(nameof(img_CategoryType_icon));
        txtmp_Title = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Title));
        txtmp_ItemSize = GetUI_TxtmpMasterLocalizing(nameof(txtmp_ItemSize));
        txtmp_CategoryType = GetUI_TxtmpMasterLocalizing(nameof(txtmp_CategoryType));
        txtmp_Description = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Description));
        btn_PopupExit2 = GetUI_Button(nameof(btn_PopupExit2), Back);
    }
    
    public void OnItemType(db.Item item)
    {
        img_Target.sprite = null;
        Util.SetMasterLocalizing(txtmp_Title, string.Empty);
        Util.SetMasterLocalizing(txtmp_ItemSize, string.Empty);
        Util.SetMasterLocalizing(txtmp_CategoryType, string.Empty);
        Util.SetMasterLocalizing(txtmp_Description, string.Empty);
        if (item != null)
        {
            string categoryName = MasterDataManager.Instance.dataCategoryType.GetData(item.categoryType).name;

            img_Target.sprite = Util.GetItemIconSprite(item.id);
            img_CategoryType_icon.sprite = Single.Resources.Load<Sprite>(Cons.Path_Image + categoryName.Replace("item", "icon") + "_01");
            Util.SetMasterLocalizing(txtmp_Title, new MasterLocalData(item.name));
            db.InteriorInstallInfo interiorInstallInfo = MasterDataManager.Instance.dataInteriorInstallInfo.GetData(item.id);
            Util.SetMasterLocalizing(txtmp_ItemSize, interiorInstallInfo.xSize + " X " + interiorInstallInfo.ySize);
            Util.SetMasterLocalizing(txtmp_CategoryType, new MasterLocalData(categoryName));
            Util.SetMasterLocalizing(txtmp_Description, new MasterLocalData(item.description));
        }
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        //MyRoomManager.Instance.isGridLock = true;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        //MyRoomManager.Instance.isGridLock = false;
    }
}
