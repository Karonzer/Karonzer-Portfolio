using System;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

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

	[SerializeField] protected bool isDead = true;

	[SerializeField] protected SkinnedMeshRenderer meshRenderer;
	protected Material hitMatInstance;
	protected Coroutine hitFlashRoutine;

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

	protected virtual void OnEnable()
	{
		isDead = false;
		gameObject.layer = LayerMask.NameToLayer("Enemy");
		foreach (var col in GetComponentsInChildren<Collider>())
			col.enabled = true;
	}

	protected virtual void OnDisable()
	{

	}

	protected void HandlePause()
	{
		if (navigation != null && navigation.isOnNavMesh)
		{
			navigation.isStopped = true;
			navigation.speed = 0f;
		}
	}

	protected void HandleResume()
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
		if (enemyStruct.currentHP > 0)
			HitFlash();

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


	private void HitFlash()
	{
		if (hitFlashRoutine != null)
			StopCoroutine(hitFlashRoutine);

		hitFlashRoutine = StartCoroutine(Co_HitFlashEmission());
	}

	private IEnumerator Co_HitFlashEmission()
	{
		if (hitMatInstance == null)
			yield break;

		hitMatInstance.EnableKeyword("_EMISSION");
		hitMatInstance.SetColor("_EmissionColor", Color.red * 2f); // 번쩍

		yield return new WaitForSeconds(0.1f);

		hitMatInstance.SetColor("_EmissionColor", Color.black); // 원래대로
	}

	public abstract void Start_Enemy();


	public EnemyStruct Get_EnemyStruct() { return enemyStruct; }
	public NavMeshAgent Get_NavMeshAgent() { return navigation; }
	public GameObject Get_TargetNavigation() { return targetNavigation; }
	public Animator Get_Animator() { return animator; }


}
