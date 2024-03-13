using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FrameWork.UI;
using MEC;

public class Panel_Telescope : PanelBase
{
	Button btn_back;
	GameObject block;

	public bool initialized;

	protected override void SetMemberUI()
	{
		base.SetMemberUI();

		this.GetComponent<CanvasGroup>().alpha = 0f;
		this.GetComponent<CanvasGroup>().blocksRaycasts = false;
	}

	private void OnDestroy()
	{
		btn_back.onClick.RemoveAllListeners();
	}

	private void OnEnable()
	{
		if (initialized) return;

		Util.RunCoroutine(Init());
	}

	private void OnDisable()
	{
		if(block != null) block.SetActive(false);

		OpenPanel<Panel_HUD>().canvasGroup.interactable = true;
	}

	IEnumerator<float> Init()
	{
		yield return Timing.WaitUntilTrue(() => ArzMetaManager.Instance);
		yield return Timing.WaitUntilTrue(() => btn_back = Util.Search<Button>(this.gameObject, "Btn_Back"));
		yield return Timing.WaitUntilTrue(() => block = Util.Search(this.gameObject, "Img_Block").gameObject);
		
		initialized = true;
	}

	public void AddEvent(Telescope _telescope)
	{
		Util.RunCoroutine(AddBackButtonEvent(_telescope), "BackButtonEvent" + this.GetHashCode());
	}

	IEnumerator<float> AddBackButtonEvent(Telescope _telescope)
	{
		yield return Timing.WaitUntilTrue(() => btn_back);
		yield return Timing.WaitUntilTrue(() => block);
		
		btn_back.onClick.AddListener(() => _telescope.ExitTelescope());
		block.SetActive(true);
	}

	public void Show(bool _show)
	{
		OpenPanel<Panel_HUD>().canvasGroup.interactable = !_show;

		this.GetComponent<CanvasGroup>().alpha = _show ? 1f : 0f;
		this.GetComponent<CanvasGroup>().blocksRaycasts = _show;

		block.SetActive(!_show);
	}
}
