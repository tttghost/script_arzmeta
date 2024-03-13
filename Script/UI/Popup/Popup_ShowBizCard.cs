using FrameWork.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Popup_ShowBizCard : PopupBase
{
    #region 변수
    private Dictionary<string, GameObject> dicBizCards = new Dictionary<string, GameObject>();

    private BizCardInfo bizCardInfo = null;

    private Transform go_CardRig;
    #endregion

    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        #region Button
        GetUI_Button("btn_PopupExit", SceneLogic.instance.PopPopup);
        GetUI_Button("btn_Exit", SceneLogic.instance.PopPopup);
        #endregion

        #region etc
        go_CardRig = GetChildGObject(nameof(go_CardRig)).transform;
        #endregion
    }

    #region 초기화
    public void SetData(BizCardData data)
    {
        if (go_CardRig != null)
        {
            InitCardRig();
            SetBizCard(data);
        }
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
    /// 명함 프리팹 로드및 세팅
    /// </summary>
    /// <param name="data"></param>
    private void SetBizCard(BizCardData data)
    {
        if (data == null) return;

        bizCardInfo = data.bizCard;
        GameObject obj = null;

        if (bizCardInfo != null)
        {
            obj = CreateOrGetObject(Util.GetBizPrefabName(bizCardInfo.templateId, BIZTEMPLATE_TYPE.NOMAL));
            if (obj != null)
            {
                BizCardBase bizCard = obj.GetComponent<BizCardBase>();
                bizCard.SetData(new BizCardData(bizCardInfo, data.memberCode, data.avatarInfos));
            }
        }

        if (obj != null)
        {
            Util.SetParentPosition(obj, go_CardRig);
            obj.SetActive(true);
        }
        else
        {
            SceneLogic.instance.PopPopup();
            SceneLogic.instance.isUILock = false;
            PushPopup<Popup_Basic>()
                .ChainPopupData(new PopupData(POPUPICON.WARNING, BTN_TYPE.Confirm, null, new MasterLocalData("businesscard_state_nonexistent")));
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
}
