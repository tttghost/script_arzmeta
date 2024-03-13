using UnityEngine;
using UnityEngine.InputSystem.UI;

[RequireComponent(typeof(InputSystemUIInputModule))]
public class FrontisInputSystem : MonoBehaviour
{
    public static FrontisInputSystem instance;
    public static FrontisInputs inputs;
    
    private static bool _finishSet;
    
    /// <summary>
    /// 초기화 여부 체크
    /// </summary>
    public static bool finishSet{ get { return _finishSet; } private set { _finishSet = value; } }

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this.gameObject);
        
        if (inputs == null) inputs = new FrontisInputs();// 인스턴스 생성

        _finishSet = true;// 초기화 여부 체크
    }

    // 활성화/비활성화 할때 Input도 활성화/비활성화 해줍니다.
    
    private void OnEnable()
    {
        if (inputs == null) return;
        inputs.Enable();
    }

    private void OnDisable()
    {
        if (inputs == null) return;
        inputs.Disable();
    }
}
