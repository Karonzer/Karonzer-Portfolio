using System;
using UnityEngine;
using UnityEngine.AI;


public abstract class Enemy : MonoBehaviour, IDamageable, IHealthChanged, IEnemyDoAttack
{
	[SerializeField] private EnemyDataSO dataSO;
	public string EnemyKey => dataSO.enemyStruct.key;
	public StateMachine<Enemy> stateMachine { get; private set; }

	[SerializeField] protected EnemyStruct enemyStruct;
	[SerializeField] protected Type enemyType = Type.Enemy;

	[SerializeField] protected Animator animator;

	[SerializeField] protected GameObject targetNavigation;
	[SerializeField] protected NavMeshAgent navigation;

	public event Action<float, float> OnHealthChanged;
	public event Action<DamageInfo> OnDamaged;
	public event Action<IDamageable> OnDead;

	public float CurrentHPHealth => enemyStruct.currentHP;
	public float MaxHPHealth => enemyStruct.maxHP;
	public GameObject CurrentObj => this.gameObject;
	public float CurrentHPDamege => enemyStruct.currentHP;
	public float MaxHPDamege => enemyStruct.maxHP;
	public EnemyStruct EnemyStruct => enemyStruct;

	protected virtual void Awake()
	{
		if (GSC.Instance != null && GSC.Instance.statManager != null)
			enemyStruct = GSC.Instance.statManager.Get_EnemyData(EnemyKey);

		stateMachine = new StateMachine<Enemy>(this);
		animator = transform.GetComponent<Animator>();
	}

	protected virtual void Start()
	{
		GSC.Instance.gameManager.OnPause += HandlePause;
		GSC.Instance.gameManager.OnResume += HandleResume;
	}

	private void OnDisable()
	{
		GSC.Instance.gameManager.OnPause -= HandlePause;
		GSC.Instance.gameManager.OnResume -= HandleResume;
	}

	private void HandlePause()
	{
		if (navigation != null && navigation.isOnNavMesh)
		{
			navigation.isStopped = true;
			navigation.speed = 0f;
		}
	}

	private void HandleResume()
	{
		if (navigation != null && navigation.isOnNavMesh)
		{
			navigation.isStopped = false;
			navigation.speed = enemyStruct.moveSpeed;
		}
	}


	protected virtual void Update()
	{
		if (GSC.Instance.gameManager != null && GSC.Instance.gameManager.isPaused)
		{
			return;
		}
		stateMachine.Tick();
	}

	public virtual void DoAttack()
	{

	}

	protected void InvokeHealthChanged()
	{
		OnHealthChanged?.Invoke(enemyStruct.currentHP, enemyStruct.maxHP);
	}
	protected void InvokeDead(Enemy _enemy)
	{
		OnDead?.Invoke(_enemy);
	}

	protected void Invoke_Damaged(DamageInfo _damageInfo)
	{
		OnDamaged?.Invoke(_damageInfo);
	}

	public virtual void Setting_Info()
	{
		if (GSC.Instance != null && GSC.Instance.statManager != null)
			enemyStruct = GSC.Instance.statManager.Get_EnemyData(EnemyKey);
		InvokeHealthChanged();
	}

	public virtual void Take_Damage(DamageInfo _damageInfo)
	{
		//Vector3 hitPos = transform.position + Vector3.up * 1.8f;
		//int damageInfo = DBManager.CalculateCriticalDamage(GSC.Instance.statManager.Get_PlayerData(GSC.Instance.gameManager.CurrentPlayerKey), _damageInfo, out bool _isCritical);
		Invoke_Damaged(_damageInfo);
		enemyStruct.currentHP -= _damageInfo.damage;
		InvokeHealthChanged();
		if (enemyStruct.currentHP <= 0)
		{
			stateMachine.ChangeState(StateID.die);
			InvokeDead(this);
		}
	}

	public virtual void Die_Enemy(IDamageable _damageable)
	{
		enemyStruct.currentHP = 0;
		transform.gameObject.SetActive(false);
		GSC.Instance.spawnManager.DeSpawn(PoolObjectType.Enemy, EnemyKey, transform.gameObject);
	}

	public abstract void Start_Enemy();


	public EnemyStruct Get_EnemyStruct() { return enemyStruct; }
	public NavMeshAgent Get_NavMeshAgent() { return navigation; }
	public GameObject Get_TargetNavigation() { return targetNavigation; }
	public Animator Get_Animator() { return animator; }


}
