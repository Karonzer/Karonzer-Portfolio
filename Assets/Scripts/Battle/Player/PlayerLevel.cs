using System;
using UnityEngine;

/// <summary>
/// PlayerLevel
/// 
/// 플레이어의 경험치(XP)와 레벨을 관리하는 클래스.
/// - XP 획득
/// - 레벨업 처리
/// - 레벨업 시 업그레이드 UI 호출 흐름 제어
/// 
/// IXPTable 인터페이스를 구현하여
/// UI(XP바 등)가 현재 레벨/XP 상태를 구독할 수 있다.
/// </summary>
public class PlayerLevel : MonoBehaviour, IXPTable
{
	[SerializeField] private int pendingLevelUp;
	[SerializeField] private int currentLevel;
	[SerializeField] private int maxLevel;
	[SerializeField] private int currentXP;
	[SerializeField] private int maxXP;

	public event Action<int> OnLevelChanged;
	public event Action<int, int> OnXPChanged;
	public event Action<int> OnLevelUp;

	[SerializeField] private AnimationCurve xpCurve;

	public int CurrentLevel => currentLevel;

	public int CurrentXP => currentXP;

	public int MaxXP => maxXP;

	private void OnEnable()
	{
		currentLevel = 1;
		maxLevel = 100;
		currentXP = 0;
		pendingLevelUp = 0;
		maxXP = Mathf.RoundToInt(xpCurve.Evaluate(currentLevel));
	}

	/// <summary>
	/// 경험치 추가
	/// - XP 증가
	/// - UI 갱신 이벤트 호출
	/// - 필요 XP를 넘으면 레벨업 처리
	/// </summary>
	public void AddXP(int amount)
	{
		currentXP += amount;
		OnXPChanged?.Invoke(currentXP, maxXP);

		if (currentXP >= maxXP)
		{
			LevelUp();
		}
	}

	/// <summary>
	/// 레벨업 처리
	/// 흐름:
	/// 1) 남은 XP 이월
	/// 2) 레벨 증가(최대 레벨 제한)
	/// 3) 레벨업 이벤트 호출
	/// 4) 다음 레벨 필요 XP 계산
	/// 5) 업그레이드 UI 처리 시도
	/// </summary>

	private void LevelUp()
	{
		currentXP -= maxXP;
		if (currentLevel >= maxLevel)
		{
			currentLevel = maxLevel;
		}
		else
		{
			currentLevel += 1;
		}

		OnLevelUp?.Invoke(currentXP);
		maxXP = CalculateNextLevelXP(currentLevel);
		OnLevelChanged?.Invoke(currentLevel);
		OnXPChanged?.Invoke(CurrentXP, MaxXP);
		pendingLevelUp++;
		TryProcessLevelUpUI();
	}

	/// <summary>
	/// 업그레이드 UI를 띄울 수 있는지 확인 후 처리
	/// - 이미 업그레이드 UI가 열려 있으면 대기
	/// - 열려 있지 않으면 업그레이드 처리 시작
	/// </summary>
	private void TryProcessLevelUpUI()
	{
		if (!BattleGSC.Instance.uIManger.IsUIOpen(UIType.UpgradePopUp))
		{
			BattleGSC.Instance.gameManager.Update_ToPlayerAttackObj();
			pendingLevelUp--;
		}
	}

	/// <summary>
	/// 다음 레벨에 필요한 XP 계산
	/// - AnimationCurve를 사용해 레벨별 XP 곡선을 만든다
	/// </summary>
	int CalculateNextLevelXP(int level)
	{
		return Mathf.RoundToInt(xpCurve.Evaluate(level));
	}

	/// <summary>
	/// 업그레이드 UI가 닫혔을 때 호출
	/// - 대기 중인 레벨업이 있으면 다시 처리 시도
	/// </summary>
	public void Handle_CloseUpgradePopup()
	{
		if (pendingLevelUp > 0)
		{
			TryProcessLevelUpUI();
		}
	}
}
