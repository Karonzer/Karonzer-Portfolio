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

	protected override void ApplyStatsFromAttackStats()
	{
		base.ApplyStatsFromAttackStats();

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
			if (Find_TargetEnemyDir(out Vector3 _direction))
			{
				Debug.Log("Fireball Attack towards direction: " + _direction);
				GameObject projectileObj = GSC.Instance.spawnManager.Spawn_ProjectileSpawn(attackName);
				if (projectileObj.TryGetComponent<Projectile>(out Projectile TryGetComponent))
				{
					projectileObj.gameObject.SetActive(true);
					Vector3 spawnOffset = _direction.normalized * 0.5f;
					Vector3 spawnPosition = transform.position + spawnOffset;
					spawnPosition += new Vector3(0, 0.5f, 0);
					TryGetComponent.Set_ProjectileInfo(attackName, attackDamage, attackStats.baseExplosionRange, _direction, attackStats.baseExplosionRange, DBManager.ProjectileSurvivalTime, spawnPosition);
					TryGetComponent.Launch_Projectile();
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
