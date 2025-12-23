
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 보스 HP UI를 여러 개 관리하는 HUD.
/// 
/// 기능:
/// - 보스가 등장하면 ShowAndInitialie(bossObj)로 호출되어 UI 생성/재사용
/// - 보스별로 BossHPUI를 매핑하여 중복 생성 방지
/// - 보스가 죽으면 해당 UI를 비활성화하여 풀로 반환
/// </summary>
public class BossHPHUD : MonoBehaviour, IUIHandler
{

    // BossHPUI 프리팹
	public GameObject BossPrefab;
	[SerializeField] private Transform area;

	// 생성된 UI 오브젝트 풀
	private List<GameObject> BossUIList;
	// 보스(체력 인터페이스) -> UI 컴포넌트 매핑
	private Dictionary<IHealthChanged, BossHPUI> bossUIMap;

	public UIType Type => UIType.BossHP;
	public bool IsOpen => area.gameObject.activeSelf;
	private void Awake()
	{
		area = transform.GetChild(0);
		BossUIList = new List<GameObject>();
		bossUIMap = new Dictionary<IHealthChanged, BossHPUI>();
	}

	public void Show()
	{

	}

	public void ShowAndInitialie(GameObject _obj = null)
	{
		if (_obj.TryGetComponent<IHealthChanged>(out IHealthChanged _healthChanged))
			Setting_BossUIBar(_healthChanged);
	}

	public void Hide()
	{

	}

	/// <summary>
	/// 보스 한 개에 대한 HP UI를 세팅
	/// </summary>
	public void Setting_BossUIBar(IHealthChanged boss)
	{
		// 이미 UI가 있으면 재활성화만
		if (bossUIMap.TryGetValue(boss, out var existingUI))
		{
			existingUI.gameObject.SetActive(true);
			return;
		}


		// 풀에서 UI를 가져오거나 새로 생성
		GameObject uiObj = GetOrCreateUIObject();
		uiObj.SetActive(true);

		if (uiObj.TryGetComponent<BossHPUI>(out var hpUI))
		{
			// 보스 참조 연결 및 이벤트 구독
			hpUI.Setting_BossHPUI(boss);
			// 보스 사망 시 UI 반환
			hpUI.OnDead += () => ReturnUI(uiObj);
			bossUIMap[boss] = hpUI;
		}
	}

	/// <summary>
	/// 풀에서 비활성 UI를 찾고 없으면 새로 생성
	/// </summary>
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

	/// <summary>
	/// UI 비활성화 및 보스 참조 해제(이벤트 정리)
	/// </summary>
	private void ReturnUI(GameObject uiObj)
	{
		uiObj.SetActive(false);

		if (uiObj.TryGetComponent<BossHPUI>(out var hpUI))
		{
			hpUI.Clear_BossReference();
		}
	}

}
