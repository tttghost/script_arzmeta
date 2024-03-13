//using Photon.Pun;
using System.Collections;
using UnityEngine;


/// <summary>
/// 모노비헤이비어 base 클래스
/// </summary>
public class MonoBase : MonoBehaviour
{
    private bool isInit = false;

    /// <summary>
    /// SingletonInit 매니저를 하이라키에 생성하기 위한 초기화
    /// </summary>
    public virtual void SingletonInit() { }
    private void Awake() => AWAKE();
    protected virtual void AWAKE() { }
    private void Start()
    {
        if (!isInit)
        {
            isInit = true;
            START();
        }
    }
    protected virtual void START()
    {

    }


}
