/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 *
 *          확장메서드
 *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 확장메서드 모음
/// </summary>
public static class ExtensionMethod
{
    #region LocalText
    private static void SetText(MonoBehaviour mono, string str)
    {
        if (mono.GetType() == typeof(Text))
        {
            Text text = (Text)mono;
            text.text = str;
        }
        if (mono.GetType() == typeof(TextMeshProUGUI))
        {
            TextMeshProUGUI text = (TextMeshProUGUI)mono;
            text.text = str;
        }
    }

    /// <summary>
    /// 로컬라이징 변수 가져오기
    /// </summary>
    /// <param name="localData"></param>
    /// <returns></returns>
    //private static IVariable GetLocalVariable(LocalData localData)
    //{
    //    //Debug.Log("localData.str: " + localData.str);
    //    //Debug.Log("localData.table: " + localData.table);
    //    //Debug.Log("localData.entry: " + localData.entry);
    //    if (localData.table == string.Empty && localData.entry == string.Empty) //데이터 없다면?
    //    {
    //        return new StringVariable() { Value = localData.str };
    //    }
    //    else
    //    {
    //        return new LocalizedString(localData.table, localData.entry);
    //    }
    //}

    /// <summary>
    /// 로컬의 로컬데이터
    /// </summary>
    /// <param name="lse"></param>
    /// <param name="cLocalDatas"></param>
    //public static void CLocalData(this LocalizedString ls, LocalData[] cLocalDatas)
    //{
    //    //로컬라이즈 데이터 판별 시작
    //    for (int i = 0; i < cLocalDatas.Length; i++)
    //    {
    //        LocalData cLocalData = cLocalDatas[i];
    //        IVariable obj = GetLocalVariable(cLocalData); //스트링데이터를 넣으면 스트링이나 로컬스트링 뱉음

    //        string variableName = (i).ToString();
    //        if (obj.GetType() == typeof(StringVariable)) //그냥스트링이라면
    //        {
    //            ls.Add(variableName, (StringVariable)obj);
    //        }
    //        else if (obj.GetType() == typeof(LocalizedString)) //로컬라이제이션스트링이라면
    //        {
    //            ls.Add(variableName, (LocalizedString)obj);
    //            if (cLocalData.localDatas != null) //로컬데이터가 자식으로 있다면??
    //            {
    //                ((LocalizedString)obj).CLocalData(cLocalData.localDatas); //재귀
    //            }
    //        }
    //    }
    //}

    /// <summary>
    /// 로컬라이징 안쓰고 텍스트 넣기
    /// </summary>
    /// <param name="mono"></param>
    /// <param name="str"></param>
    //public static void LocalText(this MonoBehaviour mono, string str)
    //{
    //    mono.LocalText(new LocalData(str));
    //}

    /// <summary>
    /// 기본 로컬라이징, 테이블과 엔트리 구성, 추가 아규먼트 1차원으로 추가 가능
    /// </summary>
    /// <param name="mono"></param>
    /// <param name="table">부모테이블명</param>
    /// <param name="entry">자식테이블명</param>
    /// <param name="cLocalData">자식로컬데이터</param>
    //public static void LocalText(this MonoBehaviour mono, string table, string entry, LocalData cLocalData = null)
    //{
    //    mono.LocalText(new LocalData(table, entry, cLocalData));
    //}

    /// <summary>
    /// 커스텀 로컬라이징 -> 재귀에 재귀
    /// </summary>
    /// <param name="mono"></param>
    /// <param name="pLocalData">부모로컬데이터</param>
    //public static void LocalText(this MonoBehaviour mono, params LocalData[] pLocalData)
    //{        
    //    if(mono.TryGetComponent(out LocalizingEvent localizingEvent))
    //    {
    //        localizingEvent.enabled = false;
    //    }

    //    Util.RunCoroutine(Co_LocalText(mono, pLocalData));
    //}

    //private static IEnumerator<float> Co_LocalText(this MonoBehaviour mono, params LocalData[] localDatas)
    //{
    //    //로컬라이즈스트링이벤트 생성
    //    LocalizeStringEvent lse = default; //본체
    //    if (!mono.TryGetComponent(out lse))
    //    {
    //        lse = mono.gameObject.AddComponent<LocalizeStringEvent>();
    //    }
    //    //로컬라이즈 데이터인지 아닌지 판별 위해 lse 일단 꺼줌
    //    lse.enabled = false;

    //    for (int i = 0; i < localDatas.Length; i++)
    //    {
    //        LocalData localData = localDatas[i];
    //        IVariable obj = GetLocalVariable(localData);

    //        if (obj.GetType() == typeof(StringVariable)) //그냥스트링이라면
    //        {
    //            SetText(mono, ((StringVariable)obj).Value);
    //        }

    //        else if (obj.GetType() == typeof(LocalizedString)) //로컬라이제이션스트링이라면
    //        {
    //            LocalizedString ls = ((LocalizedString)obj);

    //            lse.StringReference = ls;

    //            if (localData.localDatas != null && localData.localDatas.Length > 0 && localData.localDatas[0] != null) //params 는 null로 주면 length가 1인 null 인자가 들어간다!!!
    //            {
    //                ls.CLocalData(localData.localDatas); //실제 자식로컬데이터 넣어주는 재귀함수
    //            }

    //            yield return Timing.WaitForOneFrame;

    //            lse.enabled = true;

    //            lse.OnUpdateString.RemoveAllListeners();

    //            if (mono.TryGetComponent(out Text text))
    //            {
    //                lse.OnUpdateString.AddListener((str) => text.text = str);
    //            }

    //            else if (mono.TryGetComponent(out TMP_Text tMP_Text))
    //            {
    //                lse.OnUpdateString.AddListener((str) => tMP_Text.text = str);
    //            }

    //            lse.RefreshString();
    //        }
    //    }
    //}



    #endregion

    #region TMP_InputFieldPlaceHolder 인풋필드


    /// <summary>
    /// 인풋필드의 플레이스홀더 변경
    /// </summary>
    /// <param name="input"></param>
    /// <param name="masterLocalData"></param>
    public static void MasterLocalInputFieldPlaceHolder(this TMP_InputField input, MasterLocalData masterLocalData)
    {
        MasterLocalInputFieldPlaceHolder(input, masterLocalData.id, masterLocalData.args);
    }
    public static void MasterLocalInputFieldPlaceHolder(this TMP_InputField input, string id, params object[] args)
    {
        TMP_Text txtmp = input.placeholder.GetComponent<TMP_Text>();
        Util.SetMasterLocalizing(txtmp, new MasterLocalData(id, args));
    }

    /// <summary>
    /// 인풋필드 초기화
    /// </summary>
    /// <param name="input"></param>
    public static void Clear(this TMP_InputField input)
    {
        //input.Select(); // 20230630 한효주 - 모바일 빌드 시 Select로 인해 키보드가 자동으로 켜지는 현상때문에 주석처리함
        input.text = "";
    }

    #endregion

    #region Dropdown 드롭다운



    /// <summary>
    /// 선택된데이터 가져오기
    /// </summary>
    /// <param name="dropdown"></param>
    /// <returns></returns>
    public static string GetText(this TMP_Dropdown dropdown)
    {
        return dropdown.options[dropdown.value].text;
    }

    /// <summary>
    /// 액션 실행 없이 첫번째 값으로 초기화하기
    /// </summary>
    /// <param name="dropdown"></param>
    /// <param name="value"></param>
    public static void Initialize(this TMP_Dropdown dropdown, int value = 0)
    {
        dropdown.SetValueWithoutNotify(value);
    }

    #endregion
}

//public class GLocalData
//{
//    //public LocalData localData;
//    public MasterLocalData masterLocalData;

//    //public GLocalData(LocalData localData)
//    //{
//    //    this.localData = localData;
//    //    this.masterLocalData = null;
//    //}
//    public GLocalData(MasterLocalData masterLocalData)
//    {
//        //this.localData = null;
//        this.masterLocalData = masterLocalData;
//    }
//}
//public class LocalData
//{
//    public string str = "";
//    public string table = "";
//    public string entry = "";
//    public LocalData[] localDatas = null;

//    public LocalData(string table, string entry, params LocalData[] localDatas)
//    {
//        this.table = table;
//        this.entry = entry;
//        this.localDatas = localDatas;
//    }
//    public LocalData(params LocalData[] localDatas)
//    {
//        this.localDatas = localDatas;
//    }
//    public LocalData(string str)
//    {
//        this.str = str;
//    }
//}


[System.Serializable]
public class MasterLocalData
{
    public string id;
    public  object[] args;

    public MasterLocalData() {}

    public MasterLocalData(string id, params object[] args)
    {
        this.id = id;
        this.args = args;
    }
}




