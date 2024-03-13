using System;
using FrameWork.UI;
using MEC;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

public class LocalizingEvent : MonoBehaviour
{
    [SerializeField] private MasterLocalData masterLocalData = null;
    private string? str = null;

    #region 핸들러 등록, 삭제

    private void OnEnable()
    {
        SceneLogic.instance.localizingEventHandler += OnLocalizingText;
        if(masterLocalData == null && str == null)
        {
            return;
        }
        OnLocalizingText();
    }


    private void OnDisable()
    {
        SceneLogic.instance.localizingEventHandler -= OnLocalizingText;
    }
    #endregion



    /// <summary>
    /// MasterLocalData 입력
    /// </summary>
    /// <param name="masterLocalData"></param>
    public void SetMasterLocalizing(MasterLocalData masterLocalData)
    {
        isNormalString = false;
        this.masterLocalData = masterLocalData;
        OnLocalizingText();
    }
    bool isNormalString = false;
    /// <summary>
    /// String 입력
    /// </summary>
    /// <param name="str"></param>
    public void SetString(string str)
    {
        isNormalString = true;
        this.str = str;
        OnLocalizingText(); // String 입력 시 초기화 해야 할 경우 필요
    }

    private void OnLocalizingText()
    {
        if (TryGetComponent(out TMP_Text txtmp))
        {
            if(TryGetComponent(out LocalizeStringEvent localizeStringEvent))
            {
                localizeStringEvent.enabled = false;
            }

            if (isNormalString)
            {
                txtmp.text = str;
            }
            else
            {
                if (masterLocalData != null)
                {
                    txtmp.text = Util.GetMasterLocalizing(masterLocalData);
                }
            }
        }
    }
}
