using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
	[SerializeField] protected string enemyName;
	[SerializeField] protected EnemyStruct enemyStruct;

	protected virtual void Start()
	{
		if (GSC.Instance != null && GSC.Instance.statManager != null)
			enemyStruct = GSC.Instance.statManager.Get_EnemyData(enemyName);

	}

}
