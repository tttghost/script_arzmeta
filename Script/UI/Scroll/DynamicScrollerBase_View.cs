using FrameWork.UI;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// 설명 : 유한 오브젝트 재활용 스크롤뷰 Base Class 및 접었다 폈다 할 수 있는 다이나믹 아이템 기능 (View 용)
/// 사용 아이템 스크립트 : DynamicScrollItem_Custom
/// 사용 방법 : View 스크립트에서 해당 스크립트 상속 받기
///            하위의 go_ScrollView 오브젝트에 DynamicScroll_Custom 컴포넌트 추가
/// 원본 예제(프로젝트) : Assets\_DEV\ThirdParty\GPM\UI\Sample\InfiniteScroll\Scene\DynamicSampleScene
/// 원본 예제(웹사이트) : https://github.com/nhn/gpm.unity
/// </summary>
public class DynamicScrollerBase_View : UIBase
{
    #region 변수
    [SerializeField] protected DynamicScrollItem_Custom itemPrefab;

    [SerializeField] protected float margin = 0; // 콘텐츠 여유분 높이
    protected float curHeight; // 콘텐츠 높이

    // 오브젝트 높이 측정을 위한 더미
    protected TMP_Text txtmp_DummyContents;
    protected RectTransform go_Dummy;

    protected DynamicScroll_Custom scroll;
    #endregion

    protected override void SetMemberUI()
    {
        #region etc
        scroll = GetChildGObject("go_ScrollView").GetComponent<DynamicScroll_Custom>();
        if (scroll != null)
        {
            curHeight = itemPrefab.GetComponent<RectTransform>().sizeDelta.y;
            scroll.itemPrefab = itemPrefab;
            if (scroll.dynamicItemSize)
            {
                CreateDummy();
            }
        }
        #endregion
    }

    #region 초기화
    void CreateDummy()
    {
        GameObject obj = Instantiate(Single.Resources.Load<GameObject>(Cons.Path_Prefab_UI + "DynamicItemDummy"));
        if (obj != null)
        {
            Util.SetParentPosition(obj, scroll.transform);

            go_Dummy = obj.GetComponent<RectTransform>();
            txtmp_DummyContents = obj.GetComponentInChildren<TMP_Text>();
        }
    }

    /// <summary>
    /// 서버에서 데이터 받아오기 및 높이 설정
    /// </summary>
    protected virtual void LoadData(int lastId) { }

    /// <summary>
    /// 높이 설정
    /// </summary>
    /// <param name="contents"></param>
    /// <returns></returns>
    protected virtual float CalcHeight(string contents)
    {
        if (txtmp_DummyContents == null) return 0;

        txtmp_DummyContents.text = contents;
        Canvas.ForceUpdateCanvases();

        float height = go_Dummy.rect.height;
        float expandedSize = curHeight + height + margin;
        return expandedSize;
    }

    /// <summary>
    /// 전체 데이터 만들기
    /// </summary>
    protected virtual void InsertAllData<T>(List<T> datas)
    {
        if (datas.Count == 0) return;

        int count = datas.Count;
        for (int i = 0; i < count; i++)
        {
            InsertData(datas[i] as DynamicScrollData);
        }
    }

    /// <summary>
    /// 개별 데이터 만들기
    /// </summary>
    /// <param name="data"></param>
    protected void InsertData(DynamicScrollData data)
    {
        scroll.InsertData(data);
    }
    #endregion

    #region 기타
    /// <summary>
    /// 데이터 삭제
    /// </summary>
    public virtual void Clear() { }
    #endregion
}
