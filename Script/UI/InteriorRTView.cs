using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameWork.UI;
using db;

public class InteriorRTView : MonoBehaviour
{
    private GameObject go_Root;
    private float rotateY;
    [SerializeField]
    private float rotateSpeed = 0.2f;

    private Dictionary<string, GameObject> interiorDic = new Dictionary<string, GameObject>();
    private Coroutine rotateCoroutine;

    private void Awake()
    {
        go_Root = transform.Find(nameof(go_Root)).gameObject;
    }

    public void Show(Item itemDb)
    {
        Hide();

        if (interiorDic.ContainsKey(itemDb.name))
            interiorDic[itemDb.name].SetActive(true);
        else
        {
            GameObject goInterior = Util.LoadInteriorPrefab(itemDb.id);
            if (goInterior != null)
            {
                interiorDic.Add(itemDb.name, Instantiate(goInterior, go_Root.transform));
            }
        }
    }

    public void Hide()
    {
        StopRotate();

        foreach (KeyValuePair<string, GameObject> entry in interiorDic)
        {
            if (entry.Value.activeSelf)
                entry.Value.SetActive(false);
        }
    }

    public void StartRotate()
    {
        if (rotateCoroutine != null)
            StopCoroutine(rotateCoroutine);

        rotateCoroutine = StartCoroutine(coRotate());
    }

    public void StopRotate()
    {
        go_Root.transform.rotation = Quaternion.identity;
        rotateY = 0;

        if (rotateCoroutine != null)
            StopCoroutine(rotateCoroutine);
    }

    IEnumerator coRotate()
    {
        rotateY = 0;

        while(true)
        {
            go_Root.transform.rotation = Quaternion.Euler(0, rotateY += rotateSpeed, 0);

            yield return new WaitForEndOfFrame();
        }
    }
}