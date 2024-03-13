using FrameWork.UI;
using MEC;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Popup_ItemHistory : PopupBase
{
    public List<ItemHistory> itemHistoryList = new List<ItemHistory>();

    private TMP_Text         txtmp_Title;
    private GameObject       go_Content;
    private Scrollbar        scrollbar_Horizontal;
    private Button           btn_Cancel;
    protected override void SetMemberUI()
    {
        base.SetMemberUI();
        txtmp_Title          = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Title), new MasterLocalData("12002"));
        go_Content           = GetChildGObject(nameof(go_Content));
        scrollbar_Horizontal = GetUI<Scrollbar>(nameof(scrollbar_Horizontal));
        btn_Cancel           = GetUI_Button(nameof(btn_Cancel), Back);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        //MyRoomManager.Instance.isGridLock = true;
        Co_SBarHor().RunCoroutine();
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        //MyRoomManager.Instance.isGridLock = false;
    }

    /// <summary>
    /// 히스토리아이템 데이터 추가
    /// </summary>
    /// <param name="historyItem"></param>
    public ItemHistory AddItemHistoryData(int num, db.Item item, GameObject placementObj)
    {
        Item_ItemHistory item_ItemHistory = Single.Resources.Instantiate<Item_ItemHistory>(Cons.Path_Prefab_Item + nameof(item_ItemHistory), go_Content.transform);
        ItemHistory itemHistory = new ItemHistory(num, item, placementObj, item_ItemHistory.gameObject);
        itemHistoryList.Add(itemHistory);

        item_ItemHistory.Initialize();
        item_ItemHistory.SetData(itemHistory);

        return itemHistory;
    }

    /// <summary>
    /// 설치된아이템 데이터 전체 삭제 : 그리드아이템-, 히스토리아이템 -, 인벤아이템 +
    /// </summary>
    public void ClearAllItemHistoryData()
    {
        for (int i = itemHistoryList.Count - 1; i >= 0; i--)
        {
            ItemHistory selectItemHistory = itemHistoryList[i];
            selectItemHistory.historyObj.GetComponent<Item_ItemHistory>().OnClick_ItemMinus();
        }
    }


    /// <summary>
    /// 아이템히스토리 초기값으로 변경 -> Util기능으로 변경해도..?
    /// </summary>
    /// <returns></returns>
    private IEnumerator<float> Co_SBarHor()
    {
        yield return Timing.WaitForOneFrame;
        while (transform.GetChild(1).localScale.x == 0f)
        {
            yield return Timing.WaitForOneFrame;
        }
        scrollbar_Horizontal.value = 0;
    }
}



/// <summary>
/// 아이템 히스토리팝업 저장용
/// </summary>
public class ItemHistory
{
    public int num;
    public db.Item item;
    public GameObject historyObj;
    public GameObject placementObj;

    public ItemHistory(int num,  db.Item item , GameObject placementObj, GameObject historyObj)
    {
        this.num = num;
        this.item = item;
        this.placementObj = placementObj;
        this.historyObj = historyObj;
    }
}
