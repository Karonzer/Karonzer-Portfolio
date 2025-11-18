using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour
{
	[SerializeField] protected string enemyName;
	[SerializeField] protected EnemyStruct enemyStruct;


	[SerializeField] protected GameObject targetNavigation;
	[SerializeField] protected NavMeshAgent navigation;

	protected virtual void Start()
	{
		if (GSC.Instance != null && GSC.Instance.statManager != null)
			enemyStruct = GSC.Instance.statManager.Get_EnemyData(enemyName);
	}

	public abstract void Start_Enemy();

}
