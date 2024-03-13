using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using System;

public class CountDownManager : Singleton<CountDownManager>
{
    #region 변수
    private Dictionary<string, CoroutineHandle> coroutineHandleDic = new Dictionary<string, CoroutineHandle>();
    private Dictionary<string, Action<int>> secondsAct = new Dictionary<string, Action<int>>();
    private Dictionary<string, Action> endAct = new Dictionary<string, Action>();
    #endregion

    #region 중요 메소드
    public void SetCountDown(string key, int time)
    {
        if (!coroutineHandleDic.ContainsKey(key))
        {
            coroutineHandleDic.Add(key, default);
        }
        coroutineHandleDic[key] = Util.RunCoroutine(Co_CountDown(key, time), key);
    }

    private IEnumerator<float> Co_CountDown(string key, int remainTime)
    {
        while (remainTime > 0)
        {
            if (isComeBackApp)
            {
                isComeBackApp = false;
                Debug.Log("앱이 멈췄던 시간(초) : " + calTime);
                remainTime -= calTime;
            }

            if (secondsAct.ContainsKey(key))
            {
                secondsAct[key]?.Invoke(remainTime);
            }

            yield return Timing.WaitForSeconds(1);
            remainTime--;
        }

        if (endAct.ContainsKey(key))
        {
            endAct[key]?.Invoke();
        }
    }
    #endregion

    #region 액션 등록 및 제거
    public void AddSecondAction(string key, Action<int> action)
    {
        if (!secondsAct.ContainsKey(key))
        {
            secondsAct.Add(key, default);
        }
        secondsAct[key] = action;
    }

    public void AddEndAction(string key, Action action)
    {
        if (!endAct.ContainsKey(key))
        {
            endAct.Add(key, default);
        }
        endAct[key] = action;
    }

    public void RemoveSecondAction(string key)
    {
        if (secondsAct.ContainsKey(key))
        {
            secondsAct[key] = null;
        }
    }

    public void RemoveEndAction(string key)
    {
        if (endAct.ContainsKey(key))
        {
            endAct[key] = null;
        }
    }

    public void KillCountDown(string key)
    {
        if (coroutineHandleDic.ContainsKey(key))
        {
            Util.KillCoroutine(key);
        }
    }
    #endregion

    #region 앱 정지 시 처리
    private bool isComeBackApp = false;
    private int calTime = 0;

    private DateTime pauseTime;

    protected override void OnApplicationPause(bool _isPaused)
    {
        base.OnApplicationPause(_isPaused);

        if (_isPaused)
        {
            calTime = 0;
            pauseTime = DateTime.Now;
        }
        else
        {
            isComeBackApp = true;
            calTime = (int)(DateTime.Now - pauseTime).TotalSeconds;
        }
    }
    #endregion
}
