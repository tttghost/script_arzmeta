using UnityEngine;
using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using db;

public static partial class Util
{
    private static StringBuilder sb = new StringBuilder();

    public static Sprite GetItemIconSprite(int id)
    {
        return Single.ItemData.GetItemIconSprite(id);
    }

    public static string GetRemainTime(string startTime, int period)
    {
        DateTime startAt = DateTime.Parse(startTime);
        DateTime endAt = startAt.AddSeconds(period);
        TimeSpan convertTime = (endAt - DateTime.Now).Duration();

        sb.Clear();
        if (convertTime.Days > 0)
        {
            sb.Append(convertTime.Days);
            sb.Append("Day ");
        }
        sb.Append(convertTime.Hours.ToString("0,0"));
        sb.Append(":");
        sb.Append(convertTime.Minutes.ToString("0,0"));
        sb.Append(":");
        sb.Append(convertTime.Seconds.ToString("0,0"));

        return sb.ToString();
    }

    /// <summary>
    /// null 체크 후 로그 및 결과값 반환. 인자값은 object, GetType(), nameof(object) 사용
    /// </summary>
    public static bool NullCheck<T>(T obj, Type classType, string param)
    {
        if (obj == null)
        {
            DEBUG.LOGERROR($"{classType.Name}.{param} is NULL!!!");
            return true;
        }

        return false;
    }
}