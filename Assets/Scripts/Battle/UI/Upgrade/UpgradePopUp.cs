
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePopUp : MonoBehaviour, IUIHandler
{
	[SerializeField] private Transform popUp;
	[SerializeField] private List<UpgradeCardUI> cardUIList;
	[SerializeField] private Button button;

	public UIType Type => UIType.UpgradePopUp;
	public bool IsOpen => popUp.gameObject.activeSelf;
	private void Awake()
	{
		popUp = transform.GetChild(0);
		button = popUp.GetChild(0).GetChild(2).GetChild(0).GetComponent<Button>();
		button.AddEvent(Click_Btn);
	}

	public void Hide()
	{
		popUp.gameObject.SetActive(false);
	}

	public void Show()
	{
		popUp.gameObject.SetActive(true);
		Open_UpgradeCard();
		button.interactable = true;

	}

	public void ShowAndInitialie(GameObject _obj = null)
	{
		popUp.gameObject.SetActive(true);
		Open_UpgradeCard();
	}

	private void OnEnable()
	{
		popUp.gameObject.SetActive(false);
	}


	private void Open_UpgradeCard()
	{
		if (BattleGSC.Instance != null && BattleGSC.Instance.upgradeManager != null)
		{
			List<UpgradeOptionSO> list = BattleGSC.Instance.upgradeManager.Get_RandomOptions(3);
			for (int i = 0; i < list.Count; i++)
			{
				cardUIList[i].gameObject.SetActive(true);
				cardUIList[i].Setup(list[i]);
			}
		}
	}

	private void Click_Btn()
	{
		button.interactable = false;
		Open_UpgradeCard();
	}
}
