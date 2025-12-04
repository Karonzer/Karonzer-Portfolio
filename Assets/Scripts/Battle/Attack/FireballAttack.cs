using UnityEngine;
using System.Collections;

public class FireballAttack : AttackRoot
{
	private Coroutine attackTimeRoutine;
	private readonly Collider[] enemyBuffer = new Collider[20];
	private void Awake()
	{

	}
	protected override void Start()
	{
		base.Start();
	}

	private void OnEnable()
	{
		Attack();
	}
	protected override void Attack()
	{
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
				for (int i = 0; i < attackStats.ProjectileCount; i++)
				{
					if (Find_TargetEnemyDir(out Vector3 _direction))
					{
						GameObject projectileObj = BattleGSC.Instance.spawnManager.Spawn(PoolObjectType.Projectile, ProjectileKey);
						if (projectileObj.TryGetComponent<Projectile>(out Projectile _Component))
						{
							projectileObj.gameObject.SetActive(true);
							Vector3 spawnOffset = _direction.normalized * 0.5f;
							Vector3 spawnPosition = transform.position + spawnOffset;
							spawnPosition += new Vector3(0, 0.2f, 0);
							_Component.Set_ProjectileInfo(ProjectileKey, attackDamage, attackStats.baseExplosionRange, _direction, attackStats.baseProjectileSpeed, DBManager.ProjectileSurvivalTime, spawnPosition);
							_Component.Launch_Projectile();
						}
						yield return new WaitForSeconds(0.1f);

					}
				}

	
			}

		}

	}

	private bool Find_TargetEnemyDir(out Vector3 _direction)
	{
		_direction = Vector3.zero;
		int enemyLayerMask = LayerMask.GetMask("Enemy");

		int count = Physics.OverlapSphereNonAlloc(transform.position, attackRange, enemyBuffer, enemyLayerMask);
		Transform target = enemyBuffer.Get_CloseEnemy(transform, count);

		if (target != null)
		{
			_direction = (target.position - transform.position).normalized;
			return true;
		}

		return false;
	}



}
