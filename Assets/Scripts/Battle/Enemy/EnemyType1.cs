using UnityEngine;
using System.Collections;
using UnityEngine.AI;

/// <summary>
/// EnemyType1
/// 
/// 근접 공격 타입(즉시 데미지).
/// - AttackState에서 Do_EnemyAttack()이 호출되면
///   공격 트리거를 켠 뒤, "현재 범위 안에 플레이어가 있으면" 즉시 데미지를 준다.
/// 
/// 사망 처리:
/// - Die 애니메이션을 재생하고,
/// - 애니메이션이 끝나면 풀로 반환 + XP 아이템 드랍.
/// </summary>
public class EnemyType1 : Enemy
{
	// OverlapSphereNonAlloc 결과를 담는 버퍼(1개만 확인하면 충분하다는 설계)
	private readonly Collider[] enemyBuffer = new Collider[1];
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
	protected override void Start()
	{
		// 추적 대상(플레이어) 지정
		targetNavigation = BattleGSC.Instance.gameManager.Get_PlayerObject();
		base.Start();
	}


	protected override void OnEnable()
	{
		base.OnEnable();
		// 적이 죽었을 때 Die_Enemy를 실행하도록 연결(풀링 재사용 시에도 안전)
		OnDead += Die_Enemy;
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		// 이벤트 구독 해제(중복 방지)
		OnDead -= Die_Enemy;
	}
	/// <summary>
	/// 스폰 직후(또는 풀에서 꺼낸 직후) 호출되는 초기화 함수.
	/// - 최신 스탯 갱신
	/// - 첫 상태를 Tracking으로 시작
	/// </summary>
	public override void Start_Enemy()
	{
		base.Setting_Info();
		stateMachine.ChangeState(StateID.tracking);
	}

	public override void Take_Damage(DamageInfo _damageInfo)
	{
		// 베이스: HP 감소/피격 이벤트/사망 상태 전환 처리
		base.Take_Damage(_damageInfo);
	}

	/// <summary>
	/// 공격 시도(근접 즉시 공격)
	/// - Attack 애니메이션 트리거
	/// - 실제 데미지는 여기서 바로 준다(애니메이션 이벤트를 사용하지 않는 구조)
	/// </summary>
	public override void Do_EnemyAttack()
	{
		animator.SetTrigger("Attack");
		// EnemyType1 전용 공격 로직
		if (Check_WhetherThereIsPlayerInTheRange())
		{
			if (targetNavigation.TryGetComponent<IDamageable>(out IDamageable _player))
			{

				DamageInfo info = BattleGSC.Instance.gameManager.Get_EnemyDamageInfo(enemyStruct.damage, enemyStruct.key, _player.CurrentObj, Type.Player);
				_player.Take_Damage(info);
			}
		}
	}

	/// <summary>
	/// 현재 위치 기준 attackRange 안에 플레이어가 있는지 검사.
	/// - OverlapSphereNonAlloc으로 GC 없이 검사
	/// - 버퍼가 1개라 "가장 가까운 1명만 확인"하는 구조
	/// </summary>
	private bool Check_WhetherThereIsPlayerInTheRange()
	{
		int playerLayerMask = LayerMask.GetMask("Player");
		int count = Physics.OverlapSphereNonAlloc(transform.position, enemyStruct.attackRange, enemyBuffer, playerLayerMask);
		for (int i = 0; i < count; i++)
		{
			if (enemyBuffer[i].gameObject.CompareTag("Player"))
			{
				float dist = Vector3.Distance(enemyBuffer[i].transform.position, transform.transform.position);
				if (dist <= enemyStruct.attackRange)
				{
					return true;
				}
			}
		}
		return false;
	}

	/// <summary>
	/// 사망 처리(타입1)
	/// - Dead 레이어로 변경(충돌/타겟팅 등에서 제외하기 위함)
	/// - 콜라이더 비활성화(추가 피격 방지)
	/// - NavMesh 이동 정지
	/// - Die 애니메이션 재생
	/// - 애니메이션이 끝나면 풀로 반환 + XP 아이템 드랍
	/// </summary>
	public override void Die_Enemy(IDamageable _damageable)
	{
		gameObject.layer = LayerMask.NameToLayer("Dead");
		foreach (var col in GetComponentsInChildren<Collider>())
			col.enabled = false;

		navigation.isStopped = true;
		navigation.speed = 0f;
		animator.SetTrigger("Die");

		StartCoroutine(Handle_DieSequence());
	}

	/// <summary>
	/// Die 애니메이션이 끝날 때까지 기다렸다가
	/// - 베이스 Die_Enemy(풀 반환/상자 확률 드랍)
	/// - XP 아이템 스폰
	/// 을 실행한다.
	/// </summary>
	private IEnumerator Handle_DieSequence()
	{
		AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
		yield return new WaitForSeconds(info.length);

		base.Die_Enemy(this);
		Spawn_XPItem();
	}




}

