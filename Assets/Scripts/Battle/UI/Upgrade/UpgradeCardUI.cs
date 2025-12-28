using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 업그레이드 선택 카드 UI 1장의 동작을 담당.
/// 
/// 책임:
/// - UpgradeOptionSO(데이터)를 받아 화면에 표시
/// - 클릭 시 선택 처리 (UpgradeManager에 전달)
/// - 팝업 닫기, 게임 재개, 커서 처리, 레벨업 큐 처리 등 후처리 실행
/// </summary>
public class UpgradeCardUI : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI titleText;
	[SerializeField] private TextMeshProUGUI descText;
	[SerializeField] private Image iconImage;

	[SerializeField] private UpgradeOptionSO option;
	private void Awake()
	{
		Button button = GetComponent<Button>();
		button.AddEvent(OnClick_Select);
	}

	/// <summary>
	/// 카드에 표시할 업그레이드 옵션 세팅
	/// </summary>

	public void Setup(UpgradeOptionSO _option)
	{
		this.option = _option;
		titleText.text = _option.title;
		descText.text = _option.description.Replace("\n", "\n");
		if (iconImage != null && _option.icon != null)
			iconImage.sprite = _option.icon;
	}

	/// <summary>
	/// 카드 클릭 시 호출
	/// </summary>
	public void OnClick_Select()
	{
		// 실제 업그레이드 적용 요청
		BattleGSC.Instance.upgradeManager.Select_Upgrade(option);
		// UI 닫기
		BattleGSC.Instance.uIManger.Hide(UIType.UpgradePopUp);
		// 게임 재개 및 커서 숨김
		BattleGSC.Instance.gameManager.ResumeGame();
		BattleGSC.Instance.gameManager.Set_ShowAndHideCursor(false);
		// 레벨업이 여러 번 쌓인 경우 다음 팝업 처리
		BattleGSC.Instance.gameManager.Check_PendingLevelUp();
		// 클릭 사운드
		GlobalGSC.Instance.audioManager.Play_Click();
	}
}
