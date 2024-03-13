using CryptoWebRequestSample;
using Cysharp.Threading.Tasks;
using FrameWork.Network;
using FrameWork.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class WebMember
{
    private string AccountServerUrl => Single.Web.AccountServerUrl;

    #region CheckLive

    /// <summary>
    /// 유효한 사용자인지 계속 체크
    /// </summary>
    public void CheckHeartBeat()
    {
        t = Time.time;

        StopHeartBeat();
        Co_HeartBeat();
    }

    /// <summary>
    /// 사용자 중복 체크 중단
    /// </summary>
    public void StopHeartBeat()
    {
        if (cts != null)
        {
            cts.Cancel();
        }
    }

    private CancellationTokenSource cts;
    double t;
    uint c = 0;
    private async void Co_HeartBeat()
    {
        cts = new CancellationTokenSource();

        DEBUG.LOG($"Time: {Time.time - t} / Count: {c++}", eColorManager.WEB);
        t = Time.time;

        try
        {
            await UniTask.Delay(60000, cancellationToken: cts.Token).ContinueWith(() =>
            {
                CheckLive((res) =>
                {
                    if (res == null)
                    {
                        CheckHeartBeat();
                        return;
                    }
                },
                (res) =>
                {
                    SceneLogic.instance.PushPopup<Popup_Basic>()
                    .ChainPopupData(new PopupData(POPUPICON.WARNING, BTN_TYPE.Confirm, masterLocalDesc: new MasterLocalData("common_error_concurrent_access")))
                    .ChainPopupAction(new PopupAction(() =>
                    {
                        LocalPlayerData.ResetData();
#if UNITY_EDITOR
                        //UnityEditor.EditorApplication.isPlaying = false;
                        UnityEditor.EditorApplication.ExitPlaymode();
#elif UNITY_ANDROID
                                            Application.Quit(); // 강종
#elif UNITY_IOS
                                            Application.Quit(); // 강종
#endif
                    }));

                    StopHeartBeat();
                });
            });
        }
        catch
        {
            StopHeartBeat();
        }

        if (!cts.IsCancellationRequested)
        {
            Co_HeartBeat();
        }
    }

    /// <summary>
    /// 중복 로그인 체크
    /// </summary>
    /// <param name="_res"></param>
    public void CheckLive(Action<DefaultPacketRes> _res, Action<DefaultPacketRes> _error = null)
    {
        Single.Web.SendRequest(new SendRequestData(eHttpVerb.POST, AccountServerUrl, WebPacket.CheckLive, dim: false), _res, _error);
    }
    #endregion

    public void UpdateMyProfile(string nickname, string stateMessage, Action<UpdateProfilePacketRes> _res = null, Action<DefaultPacketRes> _error = null)
    {
        var packet = new
        {
            nickname,
            stateMessage
        };

        Single.Web.SendRequest(new SendRequestData(eHttpVerb.PUT, AccountServerUrl, WebPacket.UpdateMyProfile, packet), _res, _error);
    }

    /// <summary>
    /// 닉네임 중복 확인
    /// </summary>
    public void CheckNickName(string nickname, Action<DefaultPacketRes> _res, Action<DefaultPacketRes> _error = null)
    {
        var packet = new
        {
            nickname
        };

        Single.Web.SendRequest(new SendRequestData(eHttpVerb.POST, AccountServerUrl, WebPacket.CheckNickName, packet), _res, _error);
    }

    /// <summary>
    /// 명함 업데이트
    /// </summary>
    public void UpdateMyCard(BizCardInfo[] editCardInfos, Action<UpdateCardPacketRes> _res = null, Action<DefaultPacketRes> _error = null)
    {
        BizCardInfo[] originCardInfos = LocalPlayerData.BusinessCardInfos;

        List<BizCardInfo> createCardInfos = new List<BizCardInfo>();
        List<BizCardInfo> updateCardInfos = new List<BizCardInfo>();
        List<DefaultCardInfo> deleteCardInfos = new List<DefaultCardInfo>();

        if (editCardInfos.Length > 0)
        {
            foreach (var editCard in editCardInfos)
            {
                int num = editCard.num;
                BizCardInfo oriCard = originCardInfos.FirstOrDefault(x => x.num == num);
                if (oriCard == null)
                {
                    createCardInfos.Add(editCard);
                }
                else
                {
                    if (!Util.EqualMyRoomList(oriCard, editCard))
                    {
                        updateCardInfos.Add(editCard);
                    }
                }
            }
        }

        foreach (var originCard in originCardInfos)
        {
            int num = originCard.num;
            BizCardInfo editCard = editCardInfos.FirstOrDefault(x => x.num == num);
            if (editCard == null)
            {
                deleteCardInfos.Add(new DefaultCardInfo { templateId = originCard.templateId, num = originCard.num });
            }
        }

        var packet = new
        {
            updateCardInfos,
            createCardInfos,
            deleteCardInfos
        };

        Single.Web.SendRequest(new SendRequestData(eHttpVerb.PUT, AccountServerUrl, WebPacket.UpdateMyCard, packet), _res, _error);
    }

    /// <summary>
    /// 탈퇴 진행 여부 확인
    /// </summary>
    public void CheckWithdrawalProgress(int providerType, string accountToken, string password = null, Action<CheckWithdrawalProgressPacketRes> _res = null, Action<DefaultPacketRes> _error = null)
    {
        var packet = new
        {
            providerType,
            accountToken = ClsCrypto.EncryptByAES(accountToken),
            password = ClsCrypto.EncryptByAES(password)
        };

        Single.Web.SendRequest(new SendRequestData(eHttpVerb.POST, AccountServerUrl, WebPacket.CheckWithdrawalProgress, packet), _res, _error);
    }

    /// <summary>
    /// 아바타 설정
    /// </summary>
    public void Avatar(Dictionary<string, int> avatarInfos, Action<CreateAvatarPacketRes> _res, Action<DefaultPacketRes> _error = null)
    {
        var packet = new
        {
            avatarInfos
        };

        Single.Web.SendRequest(new SendRequestData(eHttpVerb.POST, AccountServerUrl, WebPacket.Avatar, packet), _res, _error);
    }

    /// <summary>
    /// 이메일 변경 // 사용 안 함
    /// </summary>
    public void UpdateEmail(string email, Action<DefaultPacketRes> _res, Action<DefaultPacketRes> _error = null)
    {
        CheckEmailPacketReq packet = new CheckEmailPacketReq()
        {
            email = email
        };

        Single.Web.SendRequest(new SendRequestData(eHttpVerb.PUT, AccountServerUrl, WebPacket.UpdateEmail, packet), _res, _error);
    }

    /// <summary>
    /// 회원 탈퇴 // 사용 안 함
    /// </summary>
    public void Withdrawal(Action<DefaultPacketRes> _res, Action<DefaultPacketRes> _error = null)
    {
        Single.Web.SendRequest(new SendRequestData(eHttpVerb.DELETE, AccountServerUrl, WebPacket.Withdrawal), _res, _error);
    }

    /// <summary>
    /// 아바타 프리셋 및 닉네임, 상태 메시지 설정
    /// </summary>
    public void SetAvatarPreset(int presetType, string nickname, string stateMessage, Action<SetAvatarPresetPacketRes> _res, Action<DefaultPacketRes> _error = null)
    {
        var packet = new
        {
            presetType,
            nickname,
            stateMessage
        };

        Single.Web.SendRequest(new SendRequestData(eHttpVerb.POST, AccountServerUrl, WebPacket.SetAvatarPreset, packet), _res, _error);
    }

    /// <summary>
    /// 회원 정보 가져오기
    /// </summary>
    public void GetMemberInfo(Action<GetMemberInfoPacketRes> _res, Action<DefaultPacketRes> _error = null)
    {
        Single.Web.SendRequest(new SendRequestData(eHttpVerb.GET, AccountServerUrl, WebPacket.GetMemberInfo, dim: false), _res, _error);
    }

    /// <summary>
    /// 앱 정보 가져오기
    /// </summary>
    public void GetAppInfo(Action<string> _res, Action<DefaultPacketRes> _error = null)
    {
        Single.Web.SendRequest(new SendRequestData(eHttpVerb.GET, AccountServerUrl, WebPacket.GetAppInfo, dim: false), _res, _error);
    }

    /// <summary>
    /// 비밀번호 변경하기
    /// </summary>
    public void ChangePassword(string password, string newPassword, Action<ChangePasswordPacketRes> _res, Action<DefaultPacketRes> _error = null)
    {
        var packet = new
        {
            password = ClsCrypto.EncryptByAES(password),
            newPassword = ClsCrypto.EncryptByAES(newPassword)
        };

        Single.Web.SendRequest(new SendRequestData(eHttpVerb.PUT, AccountServerUrl, WebPacket.ChangePassword, packet), _res, _error);
    }

    /// <summary>
    /// 기본 명함 설정
    /// </summary>
    public void SetDefaultCardInfo(int templateId, int num, Action<DefaultCardPacketRes> _res, Action<DefaultPacketRes> _error = null)
    {
        var packet = new
        {
            templateId,
            num
        };

        Single.Web.SendRequest(new SendRequestData(eHttpVerb.POST, AccountServerUrl, WebPacket.SetDefaultCardInfo, packet), _res, _error);
    }

    /// <summary>
    /// 기본 명함 삭제
    /// </summary>
    public void DeleteDefaultCardInfo(Action<DefaultPacketRes> _res, Action<DefaultPacketRes> _error = null)
    {
        Single.Web.SendRequest(new SendRequestData(eHttpVerb.DELETE, AccountServerUrl, WebPacket.DeleteDefaultCardInfo), _res, _error);
    }
}
