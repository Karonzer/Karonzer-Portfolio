using UnityEngine;
using System.Collections;
using UnityEngine.AI;

/// <summary>
/// EnemyType2
/// 
/// 근접 공격 타입(즉시 데미지) - Type1과 동일한 공격 방식.
/// - 공격 시: 범위 안에 플레이어가 있으면 즉시 데미지
/// - 사망 시: Die 애니메이션 종료 후 풀 반환 + XP 드랍
/// 
/// (실제 차이는 프리팹/스탯/애니메이션에서 발생하는 타입)
/// </summary>
public class EnemyType2 : Enemy
{
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

	private IEnumerator Handle_DieSequence()
	{
		AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
		yield return new WaitForSeconds(info.length);

		base.Die_Enemy(this);
		Spawn_XPItem();
	}


}
