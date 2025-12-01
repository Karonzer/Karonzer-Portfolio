using System;
using UnityEngine;

public class PlayerLevel : MonoBehaviour, IXPTable
{
	[SerializeField] private int currentLevel;
	[SerializeField] private int currentXP;
	[SerializeField] private int maxXP;

	public event Action<int> OnLevelChanged;
	public event Action<int, int> OnXPChanged;
	public event Action OnLevelUp;

	[SerializeField] private AnimationCurve xpCurve;

	public int CurrentLevel => currentLevel;

	public int CurrentXP => currentXP;

	public int MaxXP => maxXP;

	private void OnEnable()
	{
		currentLevel = 1;
		currentXP = 0;
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
		currentLevel += 1;
		OnLevelUp?.Invoke();

		maxXP = CalculateNextLevelXP(currentLevel);
		OnLevelChanged?.Invoke(currentLevel);
		OnXPChanged?.Invoke(CurrentXP, MaxXP);
		GSC.Instance.gameManager.Update_ToPlayerAttackObj();
		GSC.Instance.statManager.IncreaseAllEnemyStats(0.05f);
	}


	int CalculateNextLevelXP(int level)
	{
		return Mathf.RoundToInt(xpCurve.Evaluate(level));
	}
}
