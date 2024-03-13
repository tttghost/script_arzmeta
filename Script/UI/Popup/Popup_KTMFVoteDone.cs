using FrameWork.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Popup_KTMFVoteDone : PopupBase
{
    #region 변수
    private string countKey = "KTMFVoteDone";

    private Image img_Profile;
    private TMP_Text txtmp_ItemName;

    private Sprite sprite_ori;

    private Item_KTMFProfileData data;
    #endregion

    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        #region TMP_Text
        txtmp_ItemName = GetUI_TxtmpMasterLocalizing(nameof(txtmp_ItemName));

        GetUI_TxtmpMasterLocalizing("txtmp_VoteCompleted", new MasterLocalData("vote_reception_complete"));
        #endregion

        #region Image
        img_Profile = GetUI_Img(nameof(img_Profile));
        if (img_Profile != null)
        {
            sprite_ori = img_Profile.sprite;
        }
        #endregion
    }

    #region 초기화
    public void SetData(Item_KTMFProfileData _data)
    {
        data = _data;

        SetCountDown();
        SetUI();
    }

    /// <summary>
    /// 자동 팝업 종료
    /// </summary>
    private void SetCountDown()
    {
        SetOpenEndCallback(() =>
        {
            CountDownManager.Instance.SetCountDown(countKey, 3);
            CountDownManager.Instance.AddEndAction(countKey, () => SceneLogic.instance.PopPopup());
        });
    }

    private void SetUI()
    {
        if (txtmp_ItemName != null)
        {
            Util.SetMasterLocalizing(txtmp_ItemName, data.voteItem.name);
        }

        if (img_Profile != null)
        {
            img_Profile.sprite = sprite_ori;

            LocalPlayerData.Method.Load_KTMFProflie(data.voteId, data.voteItem.itemNum, data.voteItem.imageName, (sprite) => img_Profile.sprite = sprite);
        }
    }
    #endregion
}
