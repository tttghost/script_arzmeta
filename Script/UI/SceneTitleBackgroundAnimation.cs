using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SceneTitleBackgroundAnimation : MonoBehaviour
{

    public List<Sprite> sprites = new List<Sprite>();

    private Image image;

    private int currentIndex = 0;

    public float gapSecond = 0.5f;

    private bool delayed = false;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private Coroutine coroutine;
    void OnEnable()
    {
        coroutine = StartCoroutine(Loop());
    }
    private void OnDisable()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
        
    }

    private IEnumerator Loop()
    {
        while (true)
        {
            if (sprites.Count > 0)
            {
                yield return SetSprite();
            }
        }
    }

    private IEnumerator SetSprite()
    {
        if (currentIndex >= sprites.Count) currentIndex = 0;

        image.sprite = sprites[currentIndex];
        
        currentIndex++;

        yield return new WaitForSeconds(gapSecond);
    }
}
