using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Enemy (추상 클래스)
/// 
/// 모든 적이 공통으로 가지는 기본 기능을 모아둔 베이스 클래스.
/// - 스탯(EnemyStruct) 보관
/// - 피격/체력 이벤트 발행(OnDamaged / OnHealthChanged / OnDead)
/// - 상태 머신(StateMachine) 구동(Update에서 Tick)
/// - 사망 처리(상태 전환, 글로벌 이벤트, 풀에 반환, 드랍)
/// - NavMeshAgent 일시정지 대응(OnPause/OnResume)
/// </summary>
public abstract class Enemy : MonoBehaviour, IDamageable, IHealthChanged, IEnemyDoAttack
{
	[SerializeField] private EnemyDataSO dataSO;

	// StatManager에서 몬스터 스탯을 찾기 위한 key
	public string EnemyKey => dataSO.enemyStruct.key;

	// AI 상태 머신(Tracking/Attack/Die 등)
	public StateMachine<Enemy> stateMachine { get; private set; }

	// 현재 몬스터 스탯(HP, 이동속도, 공격 간격 등)
	[SerializeField] protected EnemyStruct enemyStruct;
	// 데미지 정보에서 공격자 타입 구분용
	[SerializeField] protected Type enemyType = Type.Enemy;

	// 적 애니메이터
	[SerializeField] protected Animator animator;
	// 추적할 대상(보통 플레이어)
	[SerializeField] protected GameObject targetNavigation;
	// NavMesh 이동 담당
	[SerializeField] protected NavMeshAgent navigation;

	// 체력 변경(UI 갱신용)
	public event Action<float, float> OnHealthChanged;
	// 피격 이벤트(이펙트/사운드 등)
	public event Action<DamageInfo> OnDamaged;
	// 사망 이벤트(개별 적 기준)
	public event Action<IDamageable> OnDead;
	// 모든 몬스터가 죽을 때 전역으로 알리고 싶을 때 사용(킬카운트, 경험치/드랍 트리거 등)
	public static event Action<GameObject> OnDeadGlobal;

	// 현재 사망 상태인지 여부(상태 플래그)
	[SerializeField] protected bool isDead = true;

	// ===== 피격 연출(히트 플래시) =====
	[SerializeField] protected SkinnedMeshRenderer meshRenderer;
	protected Material hitMatInstance;
	protected Coroutine hitFlashRoutine;

	// ===== 인터페이스 프로퍼티 =====
	public float CurrentHPHealth => enemyStruct.currentHP;
	public float MaxHPHealth => enemyStruct.maxHP;
	public GameObject CurrentObj => this.gameObject;
	public float CurrentHPDamege => enemyStruct.currentHP;
	public float MaxHPDamege => enemyStruct.maxHP;
	public EnemyStruct EnemyStruct => enemyStruct;

	[SerializeField] protected IAudioHandler audioHandler;

	/// <summary>
	/// - StatManager에서 몬스터 스탯을 가져와 초기화
	/// - 상태 머신 생성
	/// - Animator / AudioHandler 참조 확보
	/// </summary>
	protected virtual void Awake()
	{
		if (BattleGSC.Instance != null && BattleGSC.Instance.statManager != null)
			enemyStruct = BattleGSC.Instance.statManager.Get_EnemyData(EnemyKey);

		stateMachine = new StateMachine<Enemy>(this);
		animator = transform.GetComponent<Animator>();
		audioHandler = GetComponent<IAudioHandler>();
	}

	/// <summary>
	/// - 게임 일시정지/재개 이벤트를 구독해서 NavMeshAgent를 멈추고 다시 움직이게 한다
	/// </summary>
	protected virtual void Start()
	{
		BattleGSC.Instance.gameManager.OnPause += HandlePause;
		BattleGSC.Instance.gameManager.OnResume += HandleResume;
	}

	/// <summary>
	/// - 풀에서 다시 꺼내졌을 때(재사용) 호출될 수 있다
	/// - 기본 상태 초기화(죽지 않은 상태)
	/// - 레이어를 Enemy로 설정
	/// - 자식 Collider들을 켠다(피격 판정 다시 활성화)
	/// </summary>
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

	/// <summary>
	/// - Pause/Resume 이벤트 구독 해제
	/// </summary>
	protected void OnDestroy()
	{
		BattleGSC.Instance.gameManager.OnPause -= HandlePause;
		BattleGSC.Instance.gameManager.OnResume -= HandleResume;
	}

	/// <summary>
	/// 게임 일시정지 시: NavMesh 이동 정지
	/// </summary>
	protected void HandlePause()
	{
		if (navigation != null && navigation.isOnNavMesh)
		{
			navigation.isStopped = true;
		}
	}

	/// <summary>
	/// 게임 재개 시: NavMesh 이동 재개
	/// </summary>
	protected void HandleResume()
	{
		if (navigation != null && navigation.isOnNavMesh)
		{
			navigation.isStopped = false;
		}
	}

	/// <summary>
	/// - 게임이 Pause면 AI 업데이트 중지
	/// - 상태 머신 Tick 실행(현재 상태 로직 실행)
	/// </summary>
	protected virtual void Update()
	{
		if (BattleGSC.Instance.gameManager != null && BattleGSC.Instance.gameManager.isPaused)
		{
			return;
		}
		stateMachine.Tick();
	}

	/// <summary>
	/// 실제 공격 실행(자식 클래스에서 구현)
	/// - AttackState에서 attackInterval마다 호출될 수 있음
	/// </summary>
	public virtual void Do_EnemyAttack()
	{

	}

	/// <summary>
	/// 애니메이션 이벤트 등에서 호출되는 공격 실행 포인트(자식 클래스에서 구현)
	/// </summary>
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

	/// <summary>
	/// 스탯/HP 정보를 다시 세팅하는 함수(풀링 재사용 시 호출될 가능성)
	/// - StatManager에서 최신 적 스탯을 다시 가져온다
	/// - UI 갱신 이벤트 호출
	/// </summary>
	public virtual void Setting_Info()
	{
		if (BattleGSC.Instance != null && BattleGSC.Instance.statManager != null)
			enemyStruct = BattleGSC.Instance.statManager.Get_EnemyData(EnemyKey);
		InvokeHealthChanged();
	}

	/// <summary>
	/// 데미지를 받는다.
	/// 흐름:
	/// 1) 피격 플래시(번쩍임)
	/// 2) 피격 사운드
	/// 3) 피격 이벤트(OnDamaged) 호출
	/// 4) HP 감소
	/// 5) 체력 이벤트(OnHealthChanged) 호출
	/// 6) HP 0 이하이면:
	///    - 상태를 Die로 전환
	///    - OnDead / OnDeadGlobal 호출
	/// </summary>
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

	/// <summary>
	/// 실제 사망 처리(풀 반환/드랍 등)
	/// - 보통 OnDead를 구독한 곳(GameManager 등)에서 호출해줄 가능성이 높다.
	/// 
	/// 동작:
	/// - HP를 0으로 고정
	/// - 오브젝트 비활성화
	/// - SpawnManager 풀에 반환(DeSpawn)
	/// - 일정 확률로 ChestBox 스폰
	/// </summary>
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

	/// <summary>
	/// 경험치 아이템(XP Item) 스폰
	/// - SpawnManager에서 Item을 꺼내고
	/// - Item/XPitem 컴포넌트가 있으면 위치/XP값 세팅
	/// </summary>
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

	// ===== 피격 번쩍임(히트 플래시) =====
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

	/// <summary>
	/// 풀에서 꺼내서 다시 스폰할 때 위치/내비게이션을 안정적으로 초기화한다.
	/// - NavMeshAgent가 켜진 상태에서 position을 바꾸면 꼬일 수 있어서
	///   enabled를 껐다 켰다 하거나 Warp로 보정한다.
	/// </summary>
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

	/// <summary>
	/// 적 시작 초기화(자식 클래스에서 구현)
	/// - 상태 머신에 상태를 등록(AddState)
	/// - 첫 상태로 전환(ChangeState)
	/// - 타겟 설정 등
	/// </summary>
	public abstract void Start_Enemy();


	public EnemyStruct Get_EnemyStruct() { return enemyStruct; }
	public NavMeshAgent Get_NavMeshAgent() { return navigation; }
	public GameObject Get_TargetNavigation() { return targetNavigation; }
	public Animator Get_Animator() { return animator; }


}
