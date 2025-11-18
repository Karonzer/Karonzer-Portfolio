using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


public abstract class Enemy : MonoBehaviour
{
	public StateMachine<Enemy> StateMachine { get; private set; }

	[SerializeField] protected string enemyName;
	[SerializeField] protected EnemyStruct enemyStruct;


	[SerializeField] protected GameObject targetNavigation;
	[SerializeField] protected NavMeshAgent navigation;

	protected virtual void Awake()
	{
		if (GSC.Instance != null && GSC.Instance.statManager != null)
			enemyStruct = GSC.Instance.statManager.Get_EnemyData(enemyName);

		StateMachine = new StateMachine<Enemy>(this);
	}


	protected virtual void Update()
	{
		StateMachine.Tick();
	}

	public virtual void DoAttack()
	{
	}


	public abstract void Start_Enemy();


	public EnemyStruct Get_EnemyStruct() { return enemyStruct; }
	public NavMeshAgent Get_NavMeshAgent() { return navigation; }
	public GameObject Get_TargetNavigation() { return targetNavigation; }


}
