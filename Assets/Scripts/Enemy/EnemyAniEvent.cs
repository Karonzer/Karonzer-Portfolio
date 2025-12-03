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
		if (GSC.Instance.gameManager != null && !GSC.Instance.gameManager.isPaused)
		{
			if (enemyDoAttack != null)
			{
				enemyDoAttack.Do_EnemyAttackEvent();
			}
		}

	}
}
