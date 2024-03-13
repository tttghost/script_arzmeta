using FrameWork.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BizCardBase : UIBase
{
    #region 변수
    protected BizCardData data;
    #endregion

    #region 데이터 세팅
    /// <summary>
    /// 데이터 세팅
    /// </summary>
    /// <param name="data"></param>
    public void SetData(BizCardData data)
    {
        this.data = data;

        SetContent();
    }

    protected virtual void SetContent() { }
    #endregion

    #region 데이터 가져오기 및 갱신
    /// <summary>
    /// 데이터 가져오기
    /// </summary>
    /// <returns></returns>
    public BizCardData GetData()
    {
        return data;
    }

    /// <summary>
    /// 명함 데이터만 갱신
    /// </summary>
    /// <param name="info"></param>
    public void UpdateBizData(BizCardInfo info)
    {
        data.bizCard = info;
    }

    /// <summary>
    /// 대표 명함 토글 상태 변경
    /// </summary>
    /// <param name="b"></param>
    public virtual void SetActiveDefault(bool b) { }

    /// <summary>
    /// 데이터에 오류가 있는지 체크
    /// </summary>
    /// <returns></returns>
    public virtual bool CheckData()
    {
        return false;
    }

    /// <summary>
    /// 대표 명함인지 체크
    /// </summary>
    /// <returns></returns>
    public virtual (bool isOn, int index) CheckDefault()
    {
        return (false, -1);
    }
    #endregion
}

#region BizCardData
public class BizCardData
{
    public int index;

    public BizCardInfo bizCard;
    public string memberCode;
    public Dictionary<string, int> avatarInfos;

    public BizCardData(BizCardInfo bizCard, string memberCode = null, Dictionary<string, int> avatarInfos = null, int index = -1)
    {
        this.bizCard = bizCard;
        this.memberCode = memberCode;
        this.avatarInfos = avatarInfos;
        this.index = index;
    }

    /// <summary>
    /// 현재 저장된 대표 명함 기준
    /// 내 데이터가 대표 명함인지
    /// </summary>
    /// <returns></returns>
    public bool IsDefault()
    {
        if (bizCard != null)
        {
            return LocalPlayerData.Method.IsBizDefault(bizCard.num);
        }

        return false;
    }
#endregion
}
