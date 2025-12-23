
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 레벨업 시 표시되는 업그레이드 팝업 UI.
/// 
/// 책임:
/// - 팝업 열기/닫기
/// - UpgradeManager로부터 랜덤 카드 목록을 받아 카드 UI에 세팅
/// - (버튼 클릭 시) 카드 새로고침 기능 제공
/// 
/// IUIHandler 구현:
/// - UIManager가 공통 방식으로 Show/Hide를 호출할 수 있게 함
/// </summary>
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

	/// <summary>
	/// 팝업 숨기기
	/// </summary>
	public void Hide()
	{
		popUp.gameObject.SetActive(false);
	}

	/// <summary>
	/// 팝업 표시
	/// - 카드 목록을 새로 뽑아 세팅
	/// - 버튼 다시 활성화
	/// </summary>
	public void Show()
	{
		popUp.gameObject.SetActive(true);
		Open_UpgradeCard();
		button.interactable = true;

	}

	/// <summary>
	/// UIManager 패턴에 맞춘 초기화 포함 Show
	/// - 현재는 Show와 동일 동작
	/// </summary>
	public void ShowAndInitialie(GameObject _obj = null)
	{
		popUp.gameObject.SetActive(true);
		Open_UpgradeCard();
	}

	private void OnEnable()
	{
		popUp.gameObject.SetActive(false);
	}


	/// <summary>
	/// UpgradeManager에서 랜덤 카드 3장을 받아 카드 UI에 반영
	/// </summary>
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

	/// <summary>
	/// 리롤/새로고침 버튼 클릭
	/// - 중복 클릭 방지를 위해 interactable false 처리
	/// - 새로운 카드 다시 오픈
	/// </summary>
	private void Click_Btn()
	{
		button.interactable = false;
		Open_UpgradeCard();
	}
}
