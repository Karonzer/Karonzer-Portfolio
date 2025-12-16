using System;
using System.Collections;
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

	public static event Action<GameObject> OnDeadGlobal;

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

	[SerializeField] protected IAudioHandler audioHandler;

	protected virtual void Awake()
	{
		if (BattleGSC.Instance != null && BattleGSC.Instance.statManager != null)
			enemyStruct = BattleGSC.Instance.statManager.Get_EnemyData(EnemyKey);

		stateMachine = new StateMachine<Enemy>(this);
		animator = transform.GetComponent<Animator>();
		audioHandler = GetComponent<IAudioHandler>();
	}

	protected virtual void Start()
	{
		BattleGSC.Instance.gameManager.OnPause += HandlePause;
		BattleGSC.Instance.gameManager.OnResume += HandleResume;
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
	protected void OnDestroy()
	{
		BattleGSC.Instance.gameManager.OnPause -= HandlePause;
		BattleGSC.Instance.gameManager.OnResume -= HandleResume;
	}

	protected void HandlePause()
	{
		if (navigation != null && navigation.isOnNavMesh)
		{
			navigation.isStopped = true;
		}
	}

	protected void HandleResume()
	{
		if (navigation != null && navigation.isOnNavMesh)
		{
			navigation.isStopped = false;
		}
	}


	protected virtual void Update()
	{
		if (BattleGSC.Instance.gameManager != null && BattleGSC.Instance.gameManager.isPaused)
		{
			return;
		}
		stateMachine.Tick();
	}

	public virtual void Do_EnemyAttack()
	{

	}

	public virtual void Do_EnemyAttackEvent()
	{
	}

	protected void InvokeHealthChanged()
	{
		OnHealthChanged?.Invoke(enemyStruct.currentHP, enemyStruct.maxHP);
	}
	protected void InvokeDead(IDamageable _enemy)
	{
		OnDead?.Invoke(_enemy);
	}

	protected void Invoke_Damaged(DamageInfo _damageInfo)
	{
		OnDamaged?.Invoke(_damageInfo);
	}

	public virtual void Setting_Info()
	{
		if (BattleGSC.Instance != null && BattleGSC.Instance.statManager != null)
			enemyStruct = BattleGSC.Instance.statManager.Get_EnemyData(EnemyKey);
		InvokeHealthChanged();
	}

	public virtual void Take_Damage(DamageInfo _damageInfo)
	{
		if (enemyStruct.currentHP > 0)
			HitFlash();

		audioHandler.Play_OneShot(SoundType.Enemy_Hit);
		Invoke_Damaged(_damageInfo);
		enemyStruct.currentHP -= _damageInfo.damage;
		InvokeHealthChanged();
		if (enemyStruct.currentHP <= 0)
		{
			stateMachine.ChangeState(StateID.die);
			InvokeDead(this);
			OnDeadGlobal?.Invoke(this.gameObject);
		}
	}

	public virtual void Die_Enemy(IDamageable _damageable)
	{
		enemyStruct.currentHP = 0;
		transform.gameObject.SetActive(false);
		BattleGSC.Instance.spawnManager.DeSpawn(PoolObjectType.Enemy, EnemyKey, transform.gameObject);

		float chance = 0.01f;
		if (UnityEngine.Random.value <= chance)
		{
			GameObject obj = BattleGSC.Instance.spawnManager.Spawn(PoolObjectType.Actor, "ChestBox");
			if (obj != null)
			{
				obj.gameObject.SetActive(true);
				obj.transform.position = transform.position;
			}
		}
	}

	public virtual void Spawn_XPItem()
	{

		GameObject obj = BattleGSC.Instance.spawnManager.Spawn(PoolObjectType.Item, BattleGSC.Instance.gameManager.Get_ItemDataSO().xPItem);

		if (obj.TryGetComponent<Item>(out Item _item))
		{
			Vector3 spawnPosition = transform.position + new Vector3(0, 0.2f, 0);
			_item.Setting_SpwnPos(spawnPosition);
		}

		if (obj.TryGetComponent<XPitem>(out XPitem xpItem))
		{
			xpItem.SetXP(enemyStruct.xpItmeValue);
		}
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

	public void ResetForSpawn(Vector3 spawnPos)
	{
		if (navigation == null) navigation = GetComponent<NavMeshAgent>();

		if (navigation != null)
		{
			navigation.isStopped = true;
			navigation.ResetPath();

			navigation.enabled = false;
			transform.position = spawnPos;
			navigation.enabled = true;
			navigation.Warp(spawnPos);

			navigation.isStopped = false;
		}
		else
		{
			transform.position = spawnPos;
		}
	}

	public abstract void Start_Enemy();


	public EnemyStruct Get_EnemyStruct() { return enemyStruct; }
	public NavMeshAgent Get_NavMeshAgent() { return navigation; }
	public GameObject Get_TargetNavigation() { return targetNavigation; }
	public Animator Get_Animator() { return animator; }


}
