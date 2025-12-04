using UnityEngine;

public class EnemyAniEvent : MonoBehaviour
{
	private IEnemyDoAttack enemyDoAttack;

	private void Awake()
	{
		enemyDoAttack = transform.GetComponent<IEnemyDoAttack>();
	}

	public void Do_EnemyDoAttack()
	{
		if (BattleGSC.Instance.gameManager != null && !BattleGSC.Instance.gameManager.isPaused)
		{
			if (enemyDoAttack != null)
			{
				enemyDoAttack.Do_EnemyAttackEvent();
			}
		}

	}
}
