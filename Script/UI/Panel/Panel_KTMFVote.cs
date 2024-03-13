using FrameWork.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MEC;
using System;
using System.Linq;

public class Panel_KTMFVote : PanelBase
{
    #region 변수
    private TMP_Text txtmp_VoteTitle;
    private TMP_Text txtmp_VoteEndTime;
    private TMP_Text txtmp_VoteCount;
    private TogglePlus togplus_Vote;
    private TogglePlus togplus_VoteResult;

    public GetKTMFVoteInfoPacketRes data { get; private set; }
    #endregion

    protected override void SetMemberUI()
    {
        #region TMP_Text
        txtmp_VoteTitle = GetUI_TxtmpMasterLocalizing(nameof(txtmp_VoteTitle));
        txtmp_VoteEndTime = GetUI_TxtmpMasterLocalizing(nameof(txtmp_VoteEndTime));
        txtmp_VoteCount = GetUI_TxtmpMasterLocalizing(nameof(txtmp_VoteCount));

        GetUI_TxtmpMasterLocalizing("txtmp_VoteOn", new MasterLocalData("vote_title_vote"));
        GetUI_TxtmpMasterLocalizing("txtmp_VoteOff", new MasterLocalData("vote_title_vote"));
        GetUI_TxtmpMasterLocalizing("txtmp_VoteResultOn", new MasterLocalData("vote_title_result"));
        GetUI_TxtmpMasterLocalizing("txtmp_VoteResultOff", new MasterLocalData("vote_title_result"));
        #endregion

        #region TogglePlus
        togplus_Vote = GetUI<TogglePlus>(nameof(togplus_Vote));
        if (togplus_Vote != null)
        {
            togplus_Vote.SetToggleOnAction(() => ChangeView<View_KTMFVote>());
        }
        togplus_VoteResult = GetUI<TogglePlus>(nameof(togplus_VoteResult));
        if (togplus_VoteResult != null)
        {
            togplus_VoteResult.SetToggleOnAction(() => ChangeView<View_KTMFVoteResult>());
        }
        #endregion

        #region Button
        GetUI_Button("btn_Back", Back);
        #endregion
    }

    protected override IEnumerator<float> Co_OpenEndAct()
    {
        yield return Timing.WaitForOneFrame;

        MyPlayer.instance.SetOtherPlayersVisible(false);
        MyPlayer.instance.SetMyPlayerVisible(false);
    }

    protected override IEnumerator<float> Co_SetCloseStartAct()
    {
        yield return Timing.WaitForOneFrame;

        MyPlayer.instance.SetOtherPlayersVisible(true);
        MyPlayer.instance.SetMyPlayerVisible(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log(data);
        }
    }

    #region 초기화
    /// <summary>
    /// KTMF 투표 데이터 가져오기
    /// </summary>
    /// <returns></returns>
    public void SetData(GetKTMFVoteInfoPacketRes data)
    {
        this.data = data;

        // 토글 플러스 Init 이슈로 인한 지연 필요
        SetOpenStartCallback(() =>
        {
            GetView<View_KTMFVote>().SetData(data);
            GetView<View_KTMFVoteResult>().SetData(data);

            AddActionCountDown();
            SetUI();
        });
    }

    /// <summary>
    /// 카운트 다운 액션 등록
    /// </summary>
    private void AddActionCountDown()
    {
        CountDownManager.Instance.AddSecondAction(Util.Enum2String(KTMF_REMAINTIME.VoteEnd), VoteCloseCount);
        CountDownManager.Instance.AddEndAction(Util.Enum2String(KTMF_REMAINTIME.VoteEnd), VoteCloseEnd);
        CountDownManager.Instance.AddEndAction(Util.Enum2String(KTMF_REMAINTIME.ResultStart), ResultOpenEnd);
        CountDownManager.Instance.AddEndAction(Util.Enum2String(KTMF_REMAINTIME.ResultEnd), ResultCloseEnd);
    }

    protected override void OnDisable()
    {
        CountDownManager.Instance.RemoveSecondAction(Util.Enum2String(KTMF_REMAINTIME.VoteEnd));
        CountDownManager.Instance.RemoveEndAction(Util.Enum2String(KTMF_REMAINTIME.VoteEnd));
        CountDownManager.Instance.RemoveEndAction(Util.Enum2String(KTMF_REMAINTIME.ResultStart));
        CountDownManager.Instance.RemoveEndAction(Util.Enum2String(KTMF_REMAINTIME.ResultEnd));
    }

    /// <summary>
    /// UI 데이터 세팅
    /// </summary>
    private void SetUI()
    {
        if (txtmp_VoteTitle != null)
        {
            Util.SetMasterLocalizing(txtmp_VoteTitle, data.selectVoteInfo.name);
        }

        SetRemainVotes();
        SetViewToggles();

        if (!data.GetRemainState(KTMF_REMAINTIME.VoteEnd))
        {
            SetVoteEndTime();
        }
    }
    #endregion

    #region CountDown 등록 Action
    /// <summary>
    /// 투표 종료까지 카운트 다운
    /// </summary>
    /// <param name="i"></param>
    private void VoteCloseCount(int i)
    {
        TimeSpan timespan = TimeSpan.FromSeconds(i);
        SetVoteEndTime(new MasterLocalData("vote_state_duedate", $"{timespan.Days} {timespan.Hours:00}:{timespan.Minutes:00}:{timespan.Seconds:00}"));
    }

    /// <summary>
    /// 투표 종료 시 호출
    /// </summary>
    private void VoteCloseEnd()
    {
        data.ResetRemain(KTMF_REMAINTIME.VoteEnd);

        SetVoteEndTime();
        SetViewToggles();
    }

    /// <summary>
    /// 결과 노출 시작 시 호출
    /// </summary>
    private void ResultOpenEnd()
    {
        data.ResetRemain(KTMF_REMAINTIME.ResultStart);

        SetViewToggles();
    }

    /// <summary>
    /// 결과 노출 종료 시 호출
    /// </summary>
    private void ResultCloseEnd()
    {
        PushPopup<Popup_Basic>()
            .ChainPopupData(new PopupData(POPUPICON.WARNING, BTN_TYPE.Confirm, null, new MasterLocalData("vote_notice_termination", StartDate, EndDate)))
            .ChainPopupAction(new PopupAction(() =>
            {
                SceneLogic.instance.isUILock = false;
                SceneLogic.instance.PopPanel();
            }));
    }
    #endregion

    #region 
    /// <summary>
    /// 투표 했을 때 투표 가능 수 변경
    /// </summary>
    /// <param name="vote"></param>
    public void GetVoted(MyVote[] vote)
    {
        data.myVote = vote;

        SetRemainVotes();
        GetView<View_KTMFVote>().SelectItemCheck();

        GetView<View_KTMFVoteResult>().SetData(data);
        GetView<View_KTMFVoteResult>().LoadData();
    }

    /// <summary>
    /// 좋아요 눌렀을 때 수 변경
    /// </summary>
    /// <param name="likeInfo"></param>
    public void GetLiked(LikeInfo likeInfo)
    {
        GetView<View_KTMFVote>().LikeItemUpdate(likeInfo);
        GetView<View_KTMFVoteResult>().LikeItemUpdate(likeInfo);
    }

    /// <summary>
    /// 투표 종료 알림 문구 표시
    /// 투표 진행 중 : "투표 종료까지  D- {남은 시간}"
    /// 투표 종료 후 : "투표 종료 {투표 시작 일자} ~ {투표 마감 일자}"
    /// </summary>
    /// <param name="localData"></param>
    private void SetVoteEndTime(MasterLocalData localData = null)
    {
        localData ??= new MasterLocalData("vote_state_votingperiod", StartDate, EndDate);
        Util.SetMasterLocalizing(txtmp_VoteEndTime, localData);
    }

    /// <summary>
    /// 토글 상태 변경
    /// </summary>
    private void SetViewToggles()
    {
        togplus_Vote.tog.interactable = !IsVoteEnd;
        togplus_VoteResult.tog.interactable = ResultType != KTMF_RSULTTYPE.Voted ? IsResultStart : IsResultStart && (IsVoteEnd || data.myVote.Length > 0);

        // 미노출 시 오브젝트 끔
        togplus_VoteResult.gameObject.SetActive(ResultType != KTMF_RSULTTYPE.Unexposed);

        SetActiveView(IsVoteEnd);
    }

    /// <summary>
    /// 투표 진행 상황에 따른 뷰 변경
    /// </summary>
    private void SetActiveView(bool isVoteEnd)
    {
        TogglePlus toggle = isVoteEnd ? togplus_VoteResult : togplus_Vote;
        toggle.SetToggleIsOn(true);
    }

    /// <summary>
    /// 투표 가능 수 세팅
    /// </summary>
    private void SetRemainVotes() => Util.SetMasterLocalizing(txtmp_VoteCount, new MasterLocalData("vote_state_point", CurRemainVoteCount));
    #endregion

    #region 기능 메소드
    /// <summary>
    /// 남은 투표권 수 계산
    /// </summary>
    /// <returns></returns>
    public int CurRemainVoteCount => data.selectVoteInfo.voteCount - data.myVote.Length;

    /// <summary>
    /// 투표 시작 일자 포맷팅 반환
    /// </summary>
    private string StartDate => DateTime.Parse(data.selectVoteInfo.startedAt).ToString("yyyy/MM/dd");

    /// <summary>
    /// 투표 마감 일자 포맷팅 반환
    /// </summary>
    private string EndDate => DateTime.Parse(data.selectVoteInfo.endedAt).ToString("yyyy/MM/dd");

    /// <summary>
    /// 투표 진행 중 여부
    /// false: 투표 진행 중, true: 투표 종료
    /// </summary>
    private bool IsVoteEnd => !data.GetRemainState(KTMF_REMAINTIME.VoteEnd);

    /// <summary>
    /// 결과 노출 시작 여부
    /// false: 결과 노출 기간 전, true: 결과 노출 시작
    /// </summary>
    private bool IsResultStart => !data.GetRemainState(KTMF_REMAINTIME.ResultStart);

    /// <summary>
    /// 결과 노출 방식 Enum 변환
    /// </summary>
    private KTMF_RSULTTYPE ResultType => (KTMF_RSULTTYPE)data.selectVoteInfo.resultType;
    #endregion
}