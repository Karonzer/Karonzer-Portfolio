using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class UpgradePopUp : MonoBehaviour
{
	[SerializeField] private List<UpgradeCardUI> cardUIList;
	private void Awake()
	{
		transform.gameObject.SetActive(false);
	}
	private void OnEnable()
	{
		if(GSC.Instance != null && GSC.Instance.upgradeManager != null)
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
