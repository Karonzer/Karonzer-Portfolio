using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


public abstract class Enemy : MonoBehaviour, IDamageable, IHealthChanged
{
	[SerializeField] private EnemyDataSO dataSO;
	public string EnemyKey => dataSO.enemyStruct.key;
	public StateMachine<Enemy> StateMachine { get; private set; }

	[SerializeField] protected EnemyStruct enemyStruct;
	[SerializeField] protected Type enemyType = Type.Enemy;


	[SerializeField] protected GameObject targetNavigation;
	[SerializeField] protected NavMeshAgent navigation;

	public event Action<float, float> OnHealthChanged;
	public event Action OnDead;
	public event Action<int, Vector3, Type> OnDamaged;

	public float CurrentHPHealth => enemyStruct.currentHP;
	public float MaxHPHealth => enemyStruct.maxHP;
	public float CurrentHPDamege => enemyStruct.currentHP;
	public float MaxHPDamege => enemyStruct.maxHP;

	protected virtual void Awake()
	{
		if (GSC.Instance != null && GSC.Instance.statManager != null)
			enemyStruct = GSC.Instance.statManager.Get_EnemyData(EnemyKey);

		StateMachine = new StateMachine<Enemy>(this);
	}


	protected virtual void Update()
	{
		if (GSC.Instance.gameManager != null && GSC.Instance.gameManager.isPaused)
		{
			if (navigation != null)
			{
				navigation.isStopped = true;
				navigation.speed = 0f;
			}
			return;
		}

		if (navigation != null)
		{
			navigation.isStopped = false;
			navigation.speed = enemyStruct.moveSpeed;
		}

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

	protected void InvokeDamaged(int damage, Vector3 hitPos, Type _type)
	{
		OnDamaged?.Invoke(damage, hitPos, _type);
	}

	public virtual void Setting_Info()
	{
		if (GSC.Instance != null && GSC.Instance.statManager != null)
			enemyStruct = GSC.Instance.statManager.Get_EnemyData(EnemyKey);
		InvokeHealthChanged();
	}

	public virtual void Take_Damage(int damageInfo)
	{
		enemyStruct.currentHP -= damageInfo;

		if (enemyStruct.currentHP <= 0)
		{
			InvokeDead();
		}
	}

	public virtual void Die_Enemy()
	{
		enemyStruct.currentHP = 0;
		transform.gameObject.SetActive(false);
		GSC.Instance.spawnManager.DeSpawn(PoolObjectType.Enemy, EnemyKey, transform.gameObject);
	}

	public abstract void Start_Enemy();


	public EnemyStruct Get_EnemyStruct() { return enemyStruct; }
	public NavMeshAgent Get_NavMeshAgent() { return navigation; }
	public GameObject Get_TargetNavigation() { return targetNavigation; }


}
