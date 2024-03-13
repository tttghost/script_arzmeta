using FrameWork.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class Item_NumberInput : UIBase
{
    private TMP_InputField input_Desc;
    private Button btn_Plus;
    private Button btn_Minus;

    private int num;
    private int minNum;
    private int maxNum;
    private Action<int> act;
    private string format;
    
    private int prevNum;
    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        btn_Plus = GetUI_Button(nameof(btn_Plus));
        btn_Minus = GetUI_Button(nameof(btn_Minus));

        InitInput();
    }

    public async void SetData(MinNumMax minNumMax)
    {
        num = minNumMax.num;
        minNum = minNumMax.minNum;
        maxNum = minNumMax.maxNum;
        format = minNumMax.format;
        act = minNumMax.act;

        await UniTask.WaitUntil( () => input_Desc != null );

        SetValue(num);
    }
    private void InitInput()
    {
        input_Desc = GetUI_TMPInputField(nameof(input_Desc),
        (str) =>
        {
            if (str == string.Empty)
            {
                return;
            }

            if (input_Desc.isFocused) //인풋필드 눌린상태
            {
                if (int.Parse(str) > maxNum) //case 더 큰값이면 직전숫자 유지
                {
                    input_Desc.text = prevNum.ToString(format);
                }
                else
                {
                    prevNum = int.Parse(str); //직전숫자 저장
                    if (IsBetween(prevNum)) //직전숫자가 사잇값이면 저장
                    {
                        num = prevNum;
                    }
                }
            }
            else //인풋필드 눌리지 않은 상태 : 버튼용
            {
                if (prevNum != num)
                {
                    prevNum = num;
                    SetValue(GetClamp(prevNum));
                }
            }
        });

        //인풋필드 에딧 끝났을때 행동
        input_Desc.onEndEdit.AddListener((str) =>
        {
            prevNum = str != string.Empty ? (IsBetween(prevNum) ? prevNum : num) : num;
            SetValue(prevNum);
        });

        btn_Plus.onClick.AddListener(() => SetValue(GetClamp(num + 1)));
        btn_Minus.onClick.AddListener(() => SetValue(GetClamp(num - 1)));
    }

    /// <summary>
    /// 사잇값 가져오기 : 버튼용
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    private int GetClamp(int num)
    {
        return this.num = minNum + (int)Mathf.Repeat(num - minNum, maxNum - minNum + 1);
    }

    /// <summary>
    /// 사이값인지 확인
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    private bool IsBetween(int i)
    {
        if (minNum <= i && i <= maxNum)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 벨류 셋업
    /// </summary>
    /// <param name="num"></param>
    public void SetValue(int num)
    {
        if (IsBetween(num))
        {
            this.num = num;
        }

        if (input_Desc != null)
            input_Desc.text = this.num.ToString(format);

        act?.Invoke(this.num);
    }
}













//using FrameWork.UI;
//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;

//public class Item_NumberInput : UIBase
//{
//    public TMP_InputField input_Desc;
//    public Button btn_Plus;
//    public Button btn_Minus;
//    public int minNum;
//    public int maxNum;
//    private int num;
//    public int prevNum; 

//    protected override void SetMemberUI()
//    {
//        base.SetMemberUI();
//        btn_Plus = GetUI_Button(btn_Plus, () => SetValue(num + 1));
//        btn_Minus = GetUI_Button(btn_Minus, () => SetValue(num - 1));
//        InitInput();
//    }

//    public void SetData(MinNumMax minNumMax)
//    {
//        minNum = minNumMax.maxNum; 
//        num = minNumMax.num; 
//        maxNum = minNumMax.maxNum;
//        SetValue(num);
//    }
//    private void InitInput()
//    {
//        input_Desc = GetUI_TMPInputField(nameof(input_Desc));
//        input_Desc.onValueChanged.AddListener((str) =>
//        {
//            if (str == string.Empty)
//            {
//                return;
//            }

//            if (input_Desc.isFocused) //인풋필드 눌린상태
//            {
//                if (int.Parse(str) > maxNum) //case 더 큰값이면 직전숫자 유지
//                {
//                    input_Desc.text = prevNum.ToString();
//                }
//                else
//                {
//                    prevNum = int.Parse(str); //직전숫자 저장
//                    if (IsBetween(prevNum)) //직전숫자가 사잇값이면 저장
//                    {
//                        num = prevNum;
//                    }
//                }
//            }
//            else //인풋필드 눌리지 않은 상태 : 버튼용
//            {
//                if (prevNum != num)
//                {
//                    prevNum = num;
//                    SetValue(GetClamp(prevNum));
//                }
//            }
//        });

//        //인풋필드 에딧 끝났을때 행동
//        input_Desc.onEndEdit.AddListener((str) =>
//        {
//            prevNum = str != string.Empty ? (IsBetween(prevNum) ? prevNum : num) : num;
//            SetValue(prevNum);
//        });
//        btn_Plus.onClick.AddListener(() => SetValue((GetClamp(num + 1))));
//        btn_Minus.onClick.AddListener(() => SetValue((GetClamp(num - 1))));
//    }

//    /// <summary>
//    /// 사잇값 가져오기 : 버튼용
//    /// </summary>
//    /// <param name="num"></param>
//    /// <returns></returns>
//    private int GetClamp(int num)
//    {
//        //return Mathf.Clamp(num, minNum, maxNum);
//        return Mathf.Repeat(num, minNum, maxNum);
//    }

//    /// <summary>
//    /// 사이값인지 확인
//    /// </summary>
//    /// <param name="i"></param>
//    /// <returns></returns>
//    private bool IsBetween(int i)
//    {
//        if (minNum <= i && i <= maxNum)
//        {
//            return true;
//        }
//        return false;
//    }

//    /// <summary>
//    /// 벨류 셋업
//    /// </summary>
//    /// <param name="num"></param>
//    public void SetValue(int num)
//    {
//        if (IsBetween(num))
//        {
//            this.num = num;
//        }
//        input_Desc.text = this.num.ToString();
//    }
//}
