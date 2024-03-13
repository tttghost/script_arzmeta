using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class TMPColorTween : MonoBehaviour
{
    private TMP_Text text;

    public Color A = Color.white;
    public Color B = Color.green;
    public float speed = 1.5f;
    private void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        StartCoroutine(ColorTween());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public IEnumerator ColorTween()
    {
        while (true)
        {
            var pingpong = Mathf.PingPong(Time.time, 1.0f);
            var lerped = Color.Lerp(A, B, speed * pingpong);
            text.color = lerped;
            yield return null;
        }
    }
}
