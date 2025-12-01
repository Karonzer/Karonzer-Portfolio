using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossHPHUD : MonoBehaviour
{
	public GameObject BossPrefab;
	[SerializeField] private Transform area;
	private List<GameObject> BossUIList; 
	private Dictionary<IHealthChanged, BossHPUI> bossUIMap;

	private void Awake()
	{
		area = transform.GetChild(0);
		BossUIList = new List<GameObject>();
		bossUIMap = new Dictionary<IHealthChanged, BossHPUI>();
	}


	public void Setting_BossUIBar(IHealthChanged boss)
	{
		if (bossUIMap.TryGetValue(boss, out var existingUI))
		{
			existingUI.gameObject.SetActive(true);
			return;
		}

		GameObject uiObj = GetOrCreateUIObject();
		uiObj.SetActive(true);

		if (uiObj.TryGetComponent<BossHPUI>(out var hpUI))
		{
			hpUI.Setting_BossHPUI(boss);
			hpUI.OnDead += () => ReturnUI(uiObj);
			bossUIMap[boss] = hpUI;
		}
	}

	// ✔ 풀에서 UI 가져오기
	private GameObject GetOrCreateUIObject()
	{
		foreach (var ui in BossUIList)
		{
			if (!ui.activeSelf)
				return ui;
		}

		GameObject newUI = Instantiate(BossPrefab, area);
		newUI.SetActive(false);
		BossUIList.Add(newUI);
		return newUI;
	}

	private void ReturnUI(GameObject uiObj)
	{
		uiObj.SetActive(false);

		if (uiObj.TryGetComponent<BossHPUI>(out var hpUI))
		{
			hpUI.Clear_BossReference();
		}
	}
}
