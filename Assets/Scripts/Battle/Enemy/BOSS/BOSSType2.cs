using UnityEngine;
using System.Collections;
using UnityEngine.AI;

/// <summary>
/// BOSSType2
/// 
/// 보스 타입 2.
/// - Enemy를 상속받아 공통 기능(피격/사망/상태머신 등)을 그대로 사용한다.
/// - 상태 머신은 기본 적과 동일하게 Tracking / Attack / Die 를 사용.
/// - 공격(Do_EnemyAttack) 시:
///   1) 공격 애니메이션 트리거
///   2) 공격 사운드 재생
///   3) BossAttackIndicator(장판/폭발) 투사체를 여러 번 연속 생성
///
/// 특징:
/// - BossSkillSO에 정의된 projectile 정보(key, keyCount, 폭발 범위, 속도 등)를 사용한다.
/// - 장판 스폰 위치는 "현재 플레이어 위치"를 매번 읽어와서 찍는다.
/// - 사망 시 XP를 여러 개(5개) 뿌리도록 오버라이드되어 있다.
/// </summary>
public class BOSSType2 : Enemy
{
	/// <summary>
	/// 보스 스킬 데이터(ScriptableObject)
	/// - 어떤 투사체를 몇 개 찍을지(keyCount)
	/// - 폭발 범위(baseExplosionRange)
	/// - 투사체 속도(baseProjectileSpeed)
	/// 등을 담고 있음
	/// </summary>
	[SerializeField] private BossSkillSO BossSkillSO;
	[SerializeField] protected BossSkill bossSkill;
	private Coroutine attackTime;

	/// <summary>
	/// - Enemy 베이스 초기화(스탯/상태머신 생성/애니메이터 등)
	/// - NavMeshAgent 확보
	/// - 상태 등록(추적/공격/사망)
	/// - 피격 플래시 머티리얼 준비
	/// </summary>
	protected override void Awake()
	{
		base.Awake();

		navigation = GetComponent<NavMeshAgent>();

		stateMachine.AddState(new EnemyTrackingState());
		stateMachine.AddState(new EnemyAttackState());
		stateMachine.AddState(new EnemyDieState());


		if (meshRenderer != null)
			hitMatInstance = meshRenderer.material;

	}

	/// <summary>
	/// - 추적 대상(플레이어) 지정
	/// - 베이스 Start(일시정지/재개 이벤트 구독 등)
	/// </summary>
	protected override void Start()
	{
		targetNavigation = BattleGSC.Instance.gameManager.Get_PlayerObject();
		base.Start();
	}

	/// <summary>
	/// - 풀에서 재사용될 수 있으므로, 사망 이벤트를 다시 연결
	/// - 공격 코루틴이 남아있으면 중단(풀링 재사용 안전장치)
	/// </summary>
	protected override void OnEnable()
	{
		base.OnEnable();
		OnDead += Die_Enemy;

		if (attackTime != null)
		{
			StopCoroutine(attackTime);
			attackTime = null;
		}
	}

	/// <summary>
	/// - 사망 이벤트 구독 해제(중복 방지)
	/// </summary>
	protected override void OnDisable()
	{
		base.OnDisable();
		OnDead -= Die_Enemy;
	}

	/// <summary>
	/// 스폰 직후(또는 풀에서 꺼낸 직후) 보스 초기화
	/// - 최신 스탯 갱신
	/// - 첫 상태를 Tracking으로 시작
	/// </summary>
	public override void Start_Enemy()
	{
		base.Setting_Info();
		stateMachine.ChangeState(StateID.tracking);
	}

	/// <summary>
	/// 피격 처리
	/// - 베이스에서 HP 감소/피격 이벤트/사망 상태 전환까지 처리한다.
	/// </summary>
	public override void Take_Damage(DamageInfo _damageInfo)
	{
		base.Take_Damage(_damageInfo);
	}

	/// <summary>
	/// 보스 공격 시도(AttackState에서 attackInterval마다 호출될 수 있음)
	/// 흐름:
	/// 1) 기존 공격 코루틴 중단(중복 방지)
	/// 2) 공격 애니메이션 트리거
	/// 3) 공격 사운드 재생
	/// 4) 장판(BossAttackIndicator) 연속 생성 코루틴 시작
	/// </summary>
	public override void Do_EnemyAttack()
	{
		if (attackTime != null)
		{
			StopCoroutine(attackTime);
			attackTime = null;
		}

		animator.SetTrigger("Attack");
		audioHandler.Play_OneShot(SoundType.Enemy_attack);
		attackTime = StartCoroutine(Create_BossAttackIndicator());
	}

	/// <summary>
	/// 보스 장판 공격 생성 코루틴
	/// - BossSkillSO에 정의된 keyCount만큼 반복 생성
	/// - 0.75초 간격으로 연속해서 장판을 찍는다
	/// - 스폰 위치는 "현재 플레이어 위치" (Get_TargetNavigation().transform.position)
	/// 
	/// 생성되는 투사체:
	/// - SpawnManager에서 Projectile 풀에서 꺼낸다
	/// - Set_ProjectileInfo로 데미지/범위/속도/생존시간/스폰 위치 세팅
	/// - 실제 장판 동작(경고→충전→폭발 데미지)은 BossAttackIndicator가 처리한다. :contentReference[oaicite:1]{index=1}
	/// </summary>
	private IEnumerator Create_BossAttackIndicator()
	{
		for (int i = 0; i < BossSkillSO.bossSkill.projectile.keyCount; i++)
		{
			GameObject projectileObj = BattleGSC.Instance.spawnManager.Spawn(PoolObjectType.Projectile, BossSkillSO.bossSkill.projectile.key);
			if (projectileObj.TryGetComponent<Projectile>(out Projectile _Component))
			{
				projectileObj.gameObject.SetActive(true);
				Vector3 spawnPosition = Get_TargetNavigation().transform.position;
				_Component.Set_ProjectileInfo(BossSkillSO.bossSkill.projectile.key, enemyStruct.damage, BossSkillSO.bossSkill.projectile.baseExplosionRange,
					Vector3.zero, BossSkillSO.bossSkill.projectile.baseProjectileSpeed, DBManager.ProjectileSurvivalTime, spawnPosition);
			}
			yield return new WaitForSeconds(0.75f);
		}
		attackTime = null;
	}


	/// <summary>
	/// 보스 사망 처리
	/// - Dead 레이어로 변경(추가 충돌/타겟팅 제외)
	/// - 콜라이더 비활성화(추가 피격 방지)
	/// - 이동 정지
	/// - XP를 여러 개 드랍(Spawn_XPItem 오버라이드)
	/// - base.Die_Enemy로 풀 반환/상자 확률 드랍 처리
	/// </summary>
	public override void Die_Enemy(IDamageable _damageable)
	{
		gameObject.layer = LayerMask.NameToLayer("Dead");
		foreach (var col in GetComponentsInChildren<Collider>())
			col.enabled = false;

		navigation.isStopped = true;
		navigation.speed = 0f;
		Spawn_XPItem();
		base.Die_Enemy(this);
	}


	/// <summary>
	/// 보스 전용 XP 드랍
	/// - 일반 적은 1개 드랍이었지만,
	///   보스는 5개를 주변에 랜덤으로 흩뿌린다.
	/// </summary>
	public override void Spawn_XPItem()
	{
		for (int i = 0; i < 5; i++)
		{
			GameObject obj = BattleGSC.Instance.spawnManager.Spawn(PoolObjectType.Item, BattleGSC.Instance.gameManager.Get_ItemDataSO().xPItem);

			if (obj.TryGetComponent<Item>(out Item _item))
			{
				float x = Random.Range(-1f, 1.1f);
				float z = Random.Range(-1f, 1.1f);
				Vector3 spawnPosition = transform.position + new Vector3(x, 0.2f, z);
				_item.Setting_SpwnPos(spawnPosition);
			}

			if (obj.TryGetComponent<XPitem>(out XPitem xpItem))
			{
				xpItem.SetXP(enemyStruct.xpItmeValue);
			}
		}

	}
}
