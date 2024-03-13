using FrameWork.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_KTMFPage : UIBase
{
    #region 변수
    private Transform go_KTMFProfile;
    #endregion

    protected override void SetMemberUI()
    {
        #region etc
        go_KTMFProfile = GetChildGObject(nameof(go_KTMFProfile)).transform;
        #endregion
    }

    #region 
    /// <summary>
    /// 하위 아이템 부모 지정
    /// </summary>
    /// <param name="target"></param>
    public void SetItemParent(GameObject target)
    {
        if(go_KTMFProfile != null)
        {
            Util.SetParentPosition(target, go_KTMFProfile);
        }
    }
    #endregion
}
