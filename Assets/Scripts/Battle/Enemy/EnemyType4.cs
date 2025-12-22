using UnityEngine;
using System.Collections;
using UnityEngine.AI;

/// <summary>
/// EnemyType4
/// 
/// 근접 공격 타입(애니메이션 이벤트 기반).
/// - Do_EnemyAttack()에서는 공격 애니메이션만 트리거.
/// - 실제 데미지 적용은 애니메이션 이벤트에서
///   Do_EnemyAttackEvent()가 호출될 때 수행.
/// 
/// 장점:
/// - 공격 타이밍을 애니메이션 프레임에 정확히 맞출 수 있다.
/// </summary>
public class EnemyType4 : Enemy
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
	/// - 애니메이션만 트리거
	/// - 실제 데미지는 Do_EnemyAttackEvent에서 적용
	/// </summary>
	public override void Do_EnemyAttack()
	{
		animator.SetTrigger("Attack");
	}

	/// <summary>
	/// 애니메이션 이벤트에서 호출되는 실제 데미지 처리 지점.
	/// - 공격 범위 안이면 플레이어에게 데미지 적용
	/// </summary>
	public override void Do_EnemyAttackEvent()
	{
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
	/// 범위 안에 플레이어가 존재하는지 체크
	/// </summary>
	private bool Check_WhetherThereIsPlayerInTheRange()
	{
		int playerLayerMask = LayerMask.GetMask("Player");
		int count = Physics.OverlapSphereNonAlloc(transform.position, enemyStruct.attackRange, playerBuffer, playerLayerMask);
		for (int i = 0; i < count; i++)
		{
			if (playerBuffer[i].gameObject.CompareTag("Player"))
			{
				float dist = Vector3.Distance(playerBuffer[i].transform.position, transform.transform.position);
				if (dist <= enemyStruct.attackRange)
				{
					return true;
				}
			}
		}
		return false;
	}


	/// <summary>
	/// 사망 처리(타입4)
	/// - Dead 레이어 + 콜라이더 비활성화
	/// - 이동 정지
	/// - 즉시 풀 반환 + XP 드랍
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
