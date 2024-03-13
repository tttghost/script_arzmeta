using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using FrameWork.UI;
using MEC;

public class Panel_ConveyorBelt : PanelBase
{
	#region Members

	CanvasGroup canvasgroup;

	Button btn_conveyor_prev;
	Button btn_conveyor_next;
	Button btn_exit;

	TMP_Text txtmp_main;
	//TextMeshProUGUI tmp_coin;

	GameObject group_Content;
	GameObject img_Dimed;

	StoreCostumeContainer[] container;
	Dictionary<Define.ProductType, StoreCostumeContainer> containers = new Dictionary<Define.ProductType, StoreCostumeContainer>();

	string[] label = new string[3] { Define.KR_CostumeSize, Define.KR_CostumeColor, Define.KR_CostumeQuantity };
	public int labelIndex;

	#endregion


	#region Properties

	public GameObject Block { get => img_Dimed; }

	#endregion


	#region Initialize

	protected override void SetMemberUI()
	{
		base.SetMemberUI();

		canvasgroup = GetComponent<CanvasGroup>();

		img_Dimed = this.transform.Search(nameof(img_Dimed)).gameObject;
		img_Dimed.SetActive(false);

		group_Content = this.transform.Search(nameof(group_Content)).gameObject;
		FadeUtils.SetCanvasGroup(group_Content.GetComponent<CanvasGroup>(), 0f, false);
		
		btn_conveyor_prev = GetUI_Button(nameof(btn_conveyor_prev), StoreManager.Instance.GetComponent<StoreConveyorBeltController>().MovePrev);
		btn_conveyor_next = GetUI_Button(nameof(btn_conveyor_next), StoreManager.Instance.GetComponent<StoreConveyorBeltController>().MoveNext);

		btn_exit = GetUI_Button(nameof(btn_exit), StoreManager.Instance.GetComponent<StoreConveyorBeltController>().Cut);
		btn_exit.onClick.AddListener(StoreManager.Instance.GetComponent<StoreAvatarChangeController>().InitOutfit);
		btn_exit.onClick.AddListener(StoreManager.Instance.GetComponent<StorePurchaseController>().ResetBasket);
		btn_exit.onClick.AddListener(CloseUI);

		txtmp_main = this.transform.Search(nameof(txtmp_main)).GetComponent<TMP_Text>();


		container = new StoreCostumeContainer[group_Content.transform.childCount];

		for (int i = 0; i < group_Content.transform.childCount; i++)
		{
			container[i] = group_Content.transform.GetChild(i).GetComponent<StoreCostumeContainer>();

			containers.Add(container[i].containerType, container[i]);
		}


		BackAction_Custom += () => {
			StoreManager.Instance.GetComponent<StoreConveyorBeltController>().Cut();
			StoreManager.Instance.GetComponent<StoreAvatarChangeController>().WearSelectParts(MyPlayer.instance.Childrens[Cons.AVATARPARTS].transform);
			StoreManager.Instance.GetComponent<StorePurchaseController>().ResetBasket();
			StoreManager.Instance.GetComponent<StoreAvatarChangeController>().InitOutfit();
			CloseUI();
		};
	}

	protected override IEnumerator<float> Co_OpenStartAct()
	{
		if (StoreManager.Instance.GetComponent<StorePurchaseController>().basket.Count <= 0) yield break;

		Show(true);

		FadeUtils.FadeCanvasGroup(group_Content.GetComponent<CanvasGroup>(), 1f);

		for (int i = 0; i < StoreManager.Instance.GetComponent<StorePurchaseController>().basket.Count; i++)
		{
			Define.ProductType type = StoreManager.Instance.GetComponent<StorePurchaseController>().basket[i].type;

			containers[type].Show(true);
			containers[type].Clear();
			containers[type].SetInfo(StoreManager.Instance.GetComponent<StorePurchaseController>().basket[i]);
			containers[type].ShowInfo();
		}

		for (int i = 0; i < container.Length; i++)
		{
			container[i].SetLabel(label[labelIndex]);
		}

		btn_conveyor_prev.interactable = false;
		btn_conveyor_next.interactable = true;

		txtmp_main = GetUI_TxtmpMasterLocalizing(nameof(txtmp_main), new MasterLocalData(label[labelIndex]));
	}

	#endregion



	#region Methods

	public void Hide()
	{
		FadeUtils.FadeCanvasGroup(canvasgroup, 0f, 1f, 0f, ()=>
		{
			FadeUtils.SetCanvasGroup(group_Content.GetComponent<CanvasGroup>(), 0f, false);
			img_Dimed.SetActive(false);

			for (int i = 0; i < group_Content.transform.childCount; i++)
			{
				group_Content.transform.GetChild(i).GetComponent<StoreCostumeContainer>().Clear();
				group_Content.transform.GetChild(i).GetComponent<StoreCostumeContainer>().Show(false);
			}

			SceneLogic.instance.PopPanel();
		});
	}



	public void RefreshUI(float _delay = 3f, bool _next = true)
	{
		Util.RunCoroutine(RefreshUISequance(_delay, _next), "Animator" + this.GetHashCode());
	}

	IEnumerator<float> RefreshUISequance(float _delay, bool _next)
	{
		FadeUtils.FadeCanvasGroup(canvasgroup, 0f);

		yield return Timing.WaitForSeconds(_delay);

		for (int i = 0; i < container.Length; i++)
		{
			container[i].ChangeOption(_next);
		}

		btn_conveyor_prev.interactable = btn_conveyor_next.interactable = true;

		if (StoreManager.Instance.GetComponent<StoreConveyorBeltController>().status == Define.ConveyorStatus.Cut) btn_conveyor_prev.interactable = false;
		else if (StoreManager.Instance.GetComponent<StoreConveyorBeltController>().status == Define.ConveyorStatus.Package) btn_conveyor_next.interactable = false;

		RefreshLabel(_next);

		FadeUtils.FadeCanvasGroup(canvasgroup, 1f);
	}

	public void RefreshCoinUI(int _target, bool _quick, float _lerpspeed = 1f)
	{
		if (_quick)
		{
			return;
		}

		StartCoroutine(RefreshCoinUISequance(_target, _lerpspeed));
	}

	IEnumerator RefreshCoinUISequance(int _target, float _lerpspeed = 1f)
	{
		float lerpvalue = 0f;

		float current = StoreManager.Instance.GetComponent<StorePurchaseController>().money;
		float target = _target;

		while (Mathf.Abs(current - target) > 0.01f)
		{
			current = Mathf.Lerp(current, target, lerpvalue += _lerpspeed * Time.deltaTime);
			//tmp_coin.text = current.ToString("N0");

			yield return null;
		}
		//tmp_coin.text = target.ToString("N0");

		StoreManager.Instance.GetComponent<StorePurchaseController>().money = Convert.ToInt32(target);
	}

	public void RefreshLabel(bool _next)
	{
		if (_next) labelIndex = Mathf.Clamp(labelIndex + 1, 0, label.Length);
		else labelIndex = Mathf.Clamp(labelIndex - 1, 0, label.Length);

		for (int i = 0; i < container.Length; i++)
		{
			container[i].SetLabel(label[labelIndex]);
		}

		txtmp_main = GetUI_TxtmpMasterLocalizing(nameof(txtmp_main), new MasterLocalData(label[labelIndex]));
	}

	public void CloseUI()
	{
		Single.Sound.PlayEffect(Cons.effect_poshit_2);

		for (int i = 0; i < container.Length; i++)
		{
			container[i].HideInfo();
			container[i].ResetContainerInfo();
		}

		labelIndex = 0;

		StoreManager.Instance.GetComponent<StoreConveyorBeltController>().Switch.InteractEnd();
	}

	public void NextLevel(bool _interactable) => btn_conveyor_next.interactable = _interactable;

	#endregion
}
