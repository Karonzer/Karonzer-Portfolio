
using System.Collections.Generic;
using UnityEngine;

public class UpgradePopUp : MonoBehaviour, IUIHandler
{
	[SerializeField] private Transform popUp;
	[SerializeField] private List<UpgradeCardUI> cardUIList;

	public UIType Type => UIType.UpgradePopUp;
	private void Awake()
	{
		popUp = transform.GetChild(0);
	}

	public void Hide()
	{
		popUp.gameObject.SetActive(false);
	}

	public void Show()
	{
		popUp.gameObject.SetActive(true);
		Open_UpgradeCard();

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
		if (GSC.Instance != null && GSC.Instance.upgradeManager != null)
		{
			List<UpgradeOptionSO> list = GSC.Instance.upgradeManager.Get_RandomOptions(3);
			for (int i = 0; i < list.Count; i++)
			{
				cardUIList[i].gameObject.SetActive(true);
				cardUIList[i].Setup(list[i]);
			}
		}
	}
}
