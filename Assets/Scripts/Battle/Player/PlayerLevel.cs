using System;
using UnityEngine;

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

	public void AddXP(int amount)
	{
		currentXP += amount;
		OnXPChanged?.Invoke(currentXP, maxXP);

		if (currentXP >= maxXP)
		{
			LevelUp();
		}
	}

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


	private void TryProcessLevelUpUI()
	{
		if (!BattleGSC.Instance.uIManger.IsUIOpen(UIType.UpgradePopUp))
		{
			BattleGSC.Instance.gameManager.Update_ToPlayerAttackObj();
			pendingLevelUp--;
		}
	}

	int CalculateNextLevelXP(int level)
	{
		return Mathf.RoundToInt(xpCurve.Evaluate(level));
	}

	public void Handle_CloseUpgradePopup()
	{
		if (pendingLevelUp > 0)
		{
			TryProcessLevelUpUI();
		}
	}
}
