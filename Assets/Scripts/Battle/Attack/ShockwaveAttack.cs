using UnityEngine;
using System.Collections;

public class ShockwaveAttack : AttackRoot
{
	[SerializeField] private ParticleSystem system;
	[SerializeField] private LayerMask enemyMask;
	[SerializeField] private float attackTimer;
	private Coroutine attackTimeRoutine;

	private void Awake()
	{
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
