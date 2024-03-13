using FrameWork.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using Unity.Linq;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

// 스크롤 뷰에 마스크 추가하기
// 토글리그에 캔버스 그룹 추가해서 레이캐스트 블락 해제
// 호리젠탈 스크롤 스냅 => 하드 스냅 사용

public class View_KTMFVote : UIBase
{
    #region 변수
    private List<Item_KTMFPage> pages = new List<Item_KTMFPage>();
    public List<Item_KTMFProfile> profiles = new List<Item_KTMFProfile>();

    private HorizontalScrollSnap scrollSnap;
    private PaginationManager paginationManager;

    private Transform toggleParent;

    private GetKTMFVoteInfoPacketRes data;

    private int VoteItemCount => data.voteItems.Length;
    private float DivNum => VoteItemCount < 26 ? 5f : 10f; // 25개 이하: 5개 분배, 26개 이상: 10개 분배
    #endregion 

    protected override void SetMemberUI()
    {
        #region etc
        scrollSnap = GetChildGObject("go_ScrollView").GetComponent<HorizontalScrollSnap>();
        paginationManager = GetUI<ToggleGroup>("togg_Rig").GetComponent<PaginationManager>();

        toggleParent = paginationManager.transform;
        #endregion
    }

    #region 초기화
    public void SetData(GetKTMFVoteInfoPacketRes _data)
    {
        data = _data;

        if (data == null) return;

        InitData();
        SetUI();
    }

    private void InitData()
    {
        pages.Clear();
        profiles.Clear();
        Util.ClearProcessQueue("KTMFProflie");

        scrollSnap.RemoveAllChildren(out GameObject[] ChildrenRemoved);
        foreach (var item in ChildrenRemoved)
        {
            Destroy(item);
        }

        paginationManager.gameObject.Children().Destroy();
    }

    private async void SetUI()
    {
        if (VoteItemCount == 0) return;

        int pageCount = Mathf.CeilToInt(VoteItemCount / DivNum);
        for (int i = 0; i < pageCount; i++)
        {
            pages.Add(CreatePage());
            CreateToggle();
        }

        List<VoteItem> votes = data.voteItems.OrderBy(x => x.displayNum).ToList();
        for (int i = 0; i < VoteItemCount; i++)
        {
            var profile = CreateProfile();
            profile.SetData(new Item_KTMFProfileData(data.selectVoteInfo.id, IsSelect(votes[i].itemNum), votes[i]));
            pages[(int)(i / DivNum)].SetItemParent(profile.gameObject);

            profiles.Add(profile);
        }

        await UniTask.Delay(200);

        SetUpPagination();
    }

    /// <summary>
    /// 페이지네이션 에셋 적용
    /// </summary>
    private void SetUpPagination()
    {
        scrollSnap.DistributePages();
        paginationManager.ResetPaginationChildren();

        paginationManager.GoToScreen(0);
    }
    #endregion

    #region 
    /// <summary>
    /// 선택한 후보 체크 켜기
    /// 선택한 후보 데이터 변경
    /// </summary>
    public void SelectItemCheck()
    {
        int count = data.myVote.Length;

        if (count == 0) return;

        for (int i = 0; i < count; i++)
        {
            int selectNum = data.myVote[i].itemNum;
            Item_KTMFProfile item = profiles.FirstOrDefault(x => x.ItemNum == selectNum);
            if (item != null)
            {
                item.ChangeSelectState(true);
            }
        }
    }

    public void LikeItemUpdate(LikeInfo likeInfo)
    {
        Item_KTMFProfile item = profiles.FirstOrDefault(x => x.ItemNum == likeInfo.itemNum);
        if (item != null)
        {
            item.ChangeLikeState(likeInfo);
        }
    }

    /// <summary>
    /// 페이지 아이템 생성 및 페이지네이션 에셋에 등록
    /// </summary>
    /// <returns></returns> 
    private Item_KTMFPage CreatePage()
    {
        var page = Instantiate(Single.Resources.Load<GameObject>(Cons.Path_Prefab_Item + Cons.Item_KTMFPage)).GetComponent<Item_KTMFPage>();
        scrollSnap.AddChild(page.gameObject);

        return page;
    }

    /// <summary>
    /// 프로필 아이템 생성
    /// </summary>
    /// <returns></returns>
    private Item_KTMFProfile CreateProfile()
    {
        var profile = Instantiate(Single.Resources.Load<GameObject>(Cons.Path_Prefab_Item + Cons.Item_KTMFProfile));

        return profile.GetComponent<Item_KTMFProfile>();
    }

    /// <summary>
    /// 토글 아이템 생성 및 부모 설정
    /// </summary>
    /// <returns></returns>
    private GameObject CreateToggle()
    {
        var toggle = Instantiate(Single.Resources.Load<GameObject>(Cons.Path_Prefab_UI + Cons.tog_KTMFVote));
        Util.SetParentPosition(toggle, toggleParent);

        return toggle;
    }

    private bool IsSelect(int itemNum)
    {
        MyVote myVote = data.myVote.FirstOrDefault(x => x.itemNum == itemNum);
        return myVote != null;
    }
    #endregion
}