using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
	[SerializeField] protected string enemyName;
	[SerializeField] protected EnemyData enemyData;

	protected virtual void Start()
	{
		if (GSC.Instance != null && GSC.Instance.statManager != null)
			enemyData = GSC.Instance.statManager.Get_EnemyData(enemyName);
	}

}
