/*
 * 
 *       엑셀 -> Json 으로 추출된 5데이터 로딩을 위한 기본 클래스
 * 
 */
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

public class TableBaseData<T>
{
    #region 변수
    private Dictionary<int, T> dicData_int = null;
    private Dictionary<string, T> dicData_string = null;
    private Dictionary<(int, int), T> dicData_intint = null;
    private Dictionary<(int, string), T> dicData_intstring = null;
    private List<T> listData = null;
    #endregion

    #region List
    /// <summary>
    /// List 로드 (json 파싱)
    /// </summary>
    public List<T> LoadTable(string json) => listData = JsonConvert.DeserializeObject<T[]>(json).ToList();

    /// <summary>
    /// List 로드 (List 전달)
    /// </summary>
    public List<T> LoadTable(List<T> list) => listData = list.ToList();

    /// <summary>
    /// List 가져오기
    /// </summary>
    public List<T> GetList() => listData;
    #endregion

    #region 키값 int
    public void SetDictionary(Dictionary<int, T> dic) => dicData_int = new Dictionary<int, T>(dic);

    public Dictionary<int, T> GetDictionary_int() => dicData_int;

    public T GetData(int key)
    {
        return dicData_int.TryGetValue(key, out T value) ? (T)Convert.ChangeType(value, typeof(T)) : default;
    }
    #endregion

    #region 키값 string
    public void SetDictionary(Dictionary<string, T> dic) => dicData_string = new Dictionary<string, T>(dic);

    public Dictionary<string, T> GetDictionary_string() => dicData_string;

    public T GetData(string key)
    {
        return dicData_string.TryGetValue(key, out T value) ? (T)Convert.ChangeType(value, typeof(T)) : default;
    }
    #endregion

    #region 키값 int, int
    public void SetDictionary(Dictionary<(int, int), T> dic) => dicData_intint = new Dictionary<(int, int), T>(dic);

    public Dictionary<(int, int), T> GetDictionary_intint() => dicData_intint;

    public T GetData(int item1, int item2)
    {
        return dicData_intint.TryGetValue((item1, item2), out T value) ? (T)Convert.ChangeType(value, typeof(T)) : default;
    }
    #endregion

    #region 키값 int, string
    public void SetDictionary(Dictionary<(int, string), T> dic) => dicData_intstring = new Dictionary<(int, string), T>(dic);

    public Dictionary<(int, string), T> GetDictionary_intstring() => dicData_intstring;

    public T GetData(int item1, string item2)
    {
        return dicData_intstring.TryGetValue((item1, item2), out T value) ? (T)Convert.ChangeType(value, typeof(T)) : default;
    }
    #endregion
}