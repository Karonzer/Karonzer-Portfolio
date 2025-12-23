using UnityEngine;
using System.Collections;

/// <summary>
/// 가장 가까운 적을 향해 화염구를 발사하는 공격
/// - 방향성 투사체
/// - 비동기 SpawnAsync 사용
/// </summary>
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
		// 기존 공격 루틴 정리
		if (attackTimeRoutine != null)
		{
			StopCoroutine(attackTimeRoutine);
			attackTimeRoutine = null;
		}

		// 풀 워밍업용 투사체 생성
		Initialize_ProjectileObj();

		// 공격 루프 시작
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

	/// <summary>
	/// 공격 간격마다 몬스터을 탐색하고 발사
	/// </summary>
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

	/// <summary>
	/// 풀에 미리 투사체를 생성해두기 위한 초기화
	/// </summary>
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

	/// <summary>
	/// 가장 가까운 몬스터 방향 탐색
	/// </summary>
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
