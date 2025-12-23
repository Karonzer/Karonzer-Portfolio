using UnityEngine;
using System.Collections;

/// <summary>
/// 일정 주기로 플레이어 주변 몬스터에게 충격파 피해를 주는 공격
/// </summary>
public class ShockwaveAttack : AttackRoot
{
	[SerializeField] private ParticleSystem system;
	[SerializeField] private LayerMask enemyMask;
	[SerializeField] private float attackTimer;
	private Coroutine attackTimeRoutine;

	private void Awake()
	{
		// 파티클 및 레이어 초기화
		system = transform.GetChild(0).GetComponent<ParticleSystem>();
		enemyMask = LayerMask.GetMask("Enemy");
	}
	protected override void Start()
	{
		base.Start();
		Attack();
	}

	private void OnEnable()
	{
		Attack();
	}

	protected override void Attack()
	{
		system.Play();

		if (attackTimeRoutine != null)
		{
			StopCoroutine(attackTimeRoutine);
			attackTimeRoutine = null;
		}
		StartCoroutine(Coroutine_FindTargetEnemyAttackTime());

	}

	protected override void Apply_StatsFromAttackStats()
	{
		base.Apply_StatsFromAttackStats();
	}

	/// <summary>
	/// 공격 간격마다 실행
	/// </summary>
	private IEnumerator Coroutine_FindTargetEnemyAttackTime()
	{
		while (true)
		{
			yield return new WaitForSeconds(attackIntervalTime);
			if (BattleGSC.Instance.gameManager != null && !BattleGSC.Instance.gameManager.isPaused)
			{
				ApplyShockwaveDamage();
			}

		}

	}

	/// <summary>
	/// 범위 내 모든 몬스터에게 즉시 피해 적용
	/// </summary>
	private void ApplyShockwaveDamage()
	{
		Collider[] hits = Physics.OverlapSphere(transform.position, attackRange, enemyMask);

		foreach (var hit in hits)
		{
			if (hit.TryGetComponent<IDamageable>(out var _enemy))
			{
				DamageInfo info = BattleGSC.Instance.gameManager.Get_PlayerDamageInfo(attackDamage, _enemy.CurrentObj, Type.Enemy);
				_enemy.Take_Damage(info);
			}
		}
	}
}
