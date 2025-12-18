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

		//Spawn_Projectile();
	}
	protected override void Attack()
	{
		if (attackTimeRoutine != null)
		{
			StopCoroutine(attackTimeRoutine);
			attackTimeRoutine = null;
		}
		Initialize_ProjectileObj();
		Start_FindTargetEnemyAttackTime();
	}

	private async void Start_FindTargetEnemyAttackTime()
	{
		await Coroutine_FindTargetEnemyAttackTime();
	}

	protected override void Apply_StatsFromAttackStats()
	{
		base.Apply_StatsFromAttackStats();
	}



	private async Awaitable Coroutine_FindTargetEnemyAttackTime()
	{
		while (true)
		{
			await Awaitable.WaitForSecondsAsync(attackIntervalTime);
			if (BattleGSC.Instance.gameManager != null && !BattleGSC.Instance.gameManager.isPaused)
			{
				for (int i = 0; i < attackStats.ProjectileCount; i++)
				{
					if (Find_TargetEnemyDir(out Vector3 _direction))
					{
						GameObject projectileObj = await BattleGSC.Instance.spawnManager.SpawnAsync(PoolObjectType.Projectile, ProjectileKey);
						if (projectileObj.TryGetComponent<Projectile>(out Projectile _Component))
						{
							projectileObj.gameObject.SetActive(true);
							Vector3 spawnOffset = _direction.normalized * 0.5f;
							Vector3 spawnPosition = transform.position + spawnOffset;
							spawnPosition += new Vector3(0, 0.2f, 0);
							_Component.Set_ProjectileInfo(ProjectileKey, attackDamage, attackStats.baseExplosionRange, _direction, attackStats.baseProjectileSpeed, DBManager.ProjectileSurvivalTime, spawnPosition);
							_Component.Launch_Projectile();
						}
						await Awaitable.WaitForSecondsAsync(0.1f);

					}
				}

	
			}

		}

	}

	private async void Initialize_ProjectileObj()
	{
		await Spawn_Projectile();
	}	

	private async Awaitable Spawn_Projectile()
	{
		GameObject projectileObj = await BattleGSC.Instance.spawnManager.SpawnAsync(PoolObjectType.Projectile, ProjectileKey);
		if (projectileObj.TryGetComponent<Projectile>(out Projectile _Component))
		{
			Vector3 spawnPosition = transform.position;
			_Component.Set_ProjectileInfo(ProjectileKey, attackDamage, attackStats.baseExplosionRange, Vector3.zero, attackStats.baseProjectileSpeed, DBManager.ProjectileSurvivalTime, spawnPosition);
			projectileObj.gameObject.SetActive(false);
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
