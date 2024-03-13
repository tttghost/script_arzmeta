using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// 인풋필드 선택했을 때 전체선택 안되고 맨 마지막 글자에 캐럿 생기게 하는 스크립트
/// </summary>
[RequireComponent(typeof(InputField))]
public class InputfieldCaretAlignment : MonoBehaviour, ISelectHandler
{
    private InputField inputField;

    // Start is called before the first frame update
    void Start()
    {
        inputField = GetComponent<InputField>();
        inputField.transition = Selectable.Transition.None;
        inputField.lineType = InputField.LineType.MultiLineNewline;
    }

    public void OnSelect(BaseEventData eventData)
    {
        StartCoroutine(DisableHighlight());
    }

    private IEnumerator DisableHighlight()
    {
        //Get original selection color
        Color originalTextColor = inputField.selectionColor;
        //Remove alpha
        originalTextColor.a = 0f;

        //Apply new selection color without alpha
        inputField.selectionColor = originalTextColor;

        //Wait one Frame(MUST DO THIS!)
        yield return null;

        //Change the caret pos to the end of the text
        inputField.caretPosition = inputField.text.Length;

        //Return alpha
        originalTextColor.a = 1f;

        //Apply new selection color with alpha
        inputField.selectionColor = originalTextColor;
    }
}