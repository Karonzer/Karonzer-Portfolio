using UnityEngine;
using System.Collections;
using UnityEngine.AI;

/// <summary>
/// EnemyType3
/// 
/// 원거리(투사체) 공격 타입.
/// - AttackState에서 Do_EnemyAttack() 호출 시:
///   Attack 애니메이션만 트리거.
/// - 실제 투사체 발사는 애니메이션 이벤트(EnemyAniEvent)에서
///   Do_EnemyAttackEvent() → Launch_Projectile()로 실행된다.
///
/// 사망 처리:
/// - 즉시 풀 반환 + XP 드랍 (Die 애니메이션 대기 없음)
/// </summary>
public class EnemyType3 : Enemy
{
	private readonly Collider[] playerBuffer = new Collider[1];
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
		targetNavigation = BattleGSC.Instance.gameManager.Get_PlayerObject();
		base.Start();
	}


	protected override void OnEnable()
	{
		base.OnEnable();
		OnDead += Die_Enemy;
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		OnDead -= Die_Enemy;
	}

	public override void Start_Enemy()
	{
		base.Setting_Info();
		stateMachine.ChangeState(StateID.tracking);
	}

	public override void Take_Damage(DamageInfo _damageInfo)
	{
		base.Take_Damage(_damageInfo);
	}

	/// <summary>
	/// 공격 시도
	/// - 공격 애니메이션만 실행
	/// - 실제 발사는 애니메이션 이벤트에서 처리
	/// </summary>
	public override void Do_EnemyAttack()
	{
		animator.SetTrigger("Attack");
	}

	/// <summary>
	/// 애니메이션 이벤트에서 호출되는 실제 공격 실행 지점.
	/// EnemyAniEvent.Do_EnemyDoAttack() → 여기로 연결됨.
	/// </summary>
	public override void Do_EnemyAttackEvent()
	{
		Launch_Projectile();
	}

	/// <summary>
	/// 투사체 발사
	/// - 일시정지면 발사하지 않음
	/// - 공격 범위 안에서 플레이어 방향을 찾으면
	///   Projectile 풀에서 꺼내 세팅 후 발사
	/// </summary>
	private void Launch_Projectile()
	{
		if (BattleGSC.Instance.gameManager != null && !BattleGSC.Instance.gameManager.isPaused)
		{
			if (Find_TargetEnemyDir(out Vector3 _direction))
			{
				GameObject projectileObj = BattleGSC.Instance.spawnManager.Spawn(PoolObjectType.Projectile, "EnemyProjectile");
				if (projectileObj.TryGetComponent<Projectile>(out Projectile _Component))
				{
					projectileObj.gameObject.SetActive(true);
					Vector3 spawnOffset = _direction.normalized * 0.5f;
					Vector3 spawnPosition = transform.position + spawnOffset;
					spawnPosition += new Vector3(0, 0.2f, 0);
					_Component.Set_ProjectileInfo("EnemyProjectile", enemyStruct.damage, 1, _direction, 10, 10, spawnPosition);
					_Component.Launch_Projectile();
					audioHandler.Play_OneShot(SoundType.Enmey_Projectile);
				}
			}


		}

	}

	/// <summary>
	/// 공격 범위 안에 플레이어가 있으면 방향 벡터를 반환
	/// </summary>
	private bool Find_TargetEnemyDir(out Vector3 _direction)
	{
		_direction = Vector3.zero;
		GameObject target = null;
		int playerLayerMask = LayerMask.GetMask("Player");

		int count = Physics.OverlapSphereNonAlloc(transform.position, enemyStruct.attackRange, playerBuffer, playerLayerMask);

		if (count == 0)
			return false;

		for (int i = 0; i < count; i++)
		{
			var col = playerBuffer[i];
			if (col != null && col.CompareTag("Player"))
			{
				target = playerBuffer[i].gameObject;
				break;
			}
		}

		if (target != null)
		{
			_direction = (target.transform.position - transform.position).normalized;
			return true;
		}



		return false;
	}

	/// <summary>
	/// 사망 처리(타입3)
	/// - Dead 레이어로 변경 + 콜라이더 비활성화
	/// - 이동 정지
	/// - 즉시 풀 반환 + XP 드랍 (애니메이션 대기 없음)
	/// </summary>
	public override void Die_Enemy(IDamageable _damageable)
	{
		gameObject.layer = LayerMask.NameToLayer("Dead");
		foreach (var col in GetComponentsInChildren<Collider>())
			col.enabled = false;

		navigation.isStopped = true;
		navigation.speed = 0f;

		base.Die_Enemy(this);
		Spawn_XPItem();

	}



}
