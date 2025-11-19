using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


public abstract class Enemy : MonoBehaviour, IDamageable
{
	public StateMachine<Enemy> StateMachine { get; private set; }

	[SerializeField] protected string enemyName;
	[SerializeField] protected EnemyStruct enemyStruct;


	[SerializeField] protected GameObject targetNavigation;
	[SerializeField] protected NavMeshAgent navigation;

	public event Action<float, float> OnHealthChanged;
	public event Action OnDead;
	public event Action<int, Vector3> OnDamaged;
	public float CurrentHP => enemyStruct.currentHP;
	public float MaxHP => enemyStruct.maxHP;

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

	protected void InvokeHealthChanged()
	{
		OnHealthChanged?.Invoke(enemyStruct.currentHP, enemyStruct.maxHP);
	}
	protected void InvokeDead()
	{
		OnDead?.Invoke();
	}

	protected void InvokeDamaged(int damage, Vector3 hitPos)
	{
		OnDamaged?.Invoke(damage, hitPos);
	}

	public virtual void Setting_Info()
	{
		if (GSC.Instance != null && GSC.Instance.statManager != null)
			enemyStruct = GSC.Instance.statManager.Get_EnemyData(enemyName);
		InvokeHealthChanged();
	}

	public virtual void Take_Damage(int damageInfo)
	{
		enemyStruct.currentHP -= damageInfo;

		if (enemyStruct.currentHP <= 0)
		{
			enemyStruct.currentHP = 0;
			transform.gameObject.SetActive(false);
			GSC.Instance.spawnManager.DeSpawn_Enemy(enemyName, transform.gameObject);
		}
	}

	public abstract void Start_Enemy();


	public EnemyStruct Get_EnemyStruct() { return enemyStruct; }
	public NavMeshAgent Get_NavMeshAgent() { return navigation; }
	public GameObject Get_TargetNavigation() { return targetNavigation; }


}
