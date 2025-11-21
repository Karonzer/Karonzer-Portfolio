using UnityEngine;
using System.Collections;

public class FireballAttack : AttackRoot
{
	private SphereCollider sphereCollider;
	private Coroutine attackTimeRoutine;

	private void Awake()
	{
		attackName = DBManager.fireballProjectile;
		sphereCollider = gameObject.GetComponent<SphereCollider>();
		sphereCollider.isTrigger = true;
	}
	protected override void Start()
	{
		base.Start();
		sphereCollider.radius = attackRange;
		
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

		if (sphereCollider != null)
		{
			sphereCollider.radius = attackStats.baseExplosionRange;
		}
	}

	private IEnumerator Coroutine_FindTargetEnemyAttackTime()
	{
		while (true)
		{
			yield return new WaitForSeconds(attackIntervalTime);
			if (GSC.Instance.gameManager != null && !GSC.Instance.gameManager.IsPaused)
			{
				if (Find_TargetEnemyDir(out Vector3 _direction))
				{
					GameObject projectileObj = GSC.Instance.spawnManager.Spawn(PoolObjectType.Projectile, attackName);
					if (projectileObj.TryGetComponent<Projectile>(out Projectile _Component))
					{
						projectileObj.gameObject.SetActive(true);
						Vector3 spawnOffset = _direction.normalized * 0.5f;
						Vector3 spawnPosition = transform.position + spawnOffset;
						spawnPosition += new Vector3(0, 0.2f, 0);
						_Component.Set_ProjectileInfo(attackName, attackDamage, attackStats.baseExplosionRange, _direction, attackStats.baseExplosionRange, DBManager.ProjectileSurvivalTime, spawnPosition);
						_Component.Launch_Projectile();
					}
				}
			}

		}

	}

	private bool Find_TargetEnemyDir(out Vector3 _direction)
	{
		_direction = Vector3.zero;

		Transform target = null;
		Collider[] results = Physics.OverlapSphere(transform.position, attackRange);
		target = results.Get_CloseEnemy(transform);

		if (target != null)
		{
			_direction = target.Get_TargetDir(transform);
			return true;
		}

		return false;
	}



}
