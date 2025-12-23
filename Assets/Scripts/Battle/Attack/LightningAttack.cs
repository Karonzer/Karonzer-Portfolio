using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 범위 내 적 중 랜덤 타겟에게  장판형 번개를 떨어뜨리는 공격
/// </summary>
public class LightningAttack : AttackRoot
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
		StartCoroutine(Coroutine_FindRandomTargetEnemyAttackTime());
	}

	protected override void Apply_StatsFromAttackStats()
	{
		base.Apply_StatsFromAttackStats();
	}


	/// <summary>
	/// 공격 간격마다 몬스터을 탐색하고 해당 위치에 생성
	/// </summary>
	private IEnumerator Coroutine_FindRandomTargetEnemyAttackTime()
	{
		while (true)
		{
			yield return new WaitForSeconds(attackIntervalTime);
			if (BattleGSC.Instance.gameManager != null && !BattleGSC.Instance.gameManager.isPaused)
			{
				if (Find_TargetEnemyRandomList(out List<GameObject> _list))
				{
					for (int i = 0; i < attackStats.ProjectileCount; i++)
					{
						GameObject obj = _list[Random.Range(0, _list.Count)];
						GameObject projectileObj = BattleGSC.Instance.spawnManager.Spawn(PoolObjectType.Projectile, ProjectileKey);
						if (projectileObj.TryGetComponent<Projectile>(out Projectile _Component))
						{
							projectileObj.gameObject.SetActive(true);
							Vector3 spawnPosition = obj.transform.position;
							spawnPosition += new Vector3(0, 3.5f, 0);
							_Component.Set_ProjectileInfo(ProjectileKey, attackDamage, attackStats.baseExplosionRange, Vector3.zero, attackStats.baseProjectileSpeed, DBManager.ProjectileSurvivalTime, spawnPosition);
							_Component.Launch_Projectile();
						}
						yield return new WaitForSeconds(0.2f);
					}
				}

			}

		}

	}

	/// <summary>
	/// 범위 내 몬스터 리스트 수집
	/// </summary>
	private bool Find_TargetEnemyRandomList(out List<GameObject> _list)
	{
		_list = new List<GameObject>(); 

		int enemyLayerMask = LayerMask.GetMask("Enemy");

		int count = Physics.OverlapSphereNonAlloc(transform.position, attackRange, enemyBuffer, enemyLayerMask);
		if (count == 0)
			return false;

		for(int i = 0; i < count;i++)
		{
			var col = enemyBuffer[i];
			if (col != null && col.CompareTag("Enemy") || col.CompareTag("Boss"))
			{
				_list.Add(col.gameObject);
			}
		}


		return count > 0;
	}
}
