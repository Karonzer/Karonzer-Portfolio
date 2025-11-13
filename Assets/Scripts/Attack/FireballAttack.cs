using UnityEngine;
using System.Collections;

public class FireballAttack : AttackRoot
{
	private SphereCollider sphereCollider;
	private Coroutine attackTimeRoutine;

	private void Awake()
	{
		sphereCollider = gameObject.AddComponent<SphereCollider>();
		sphereCollider.isTrigger = true;
	}
	private void Start()
	{
		attackName = "Fireball";
		attackDamage = 25;
		attackRange = 15f;
		attackTime = 2f;

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

	private IEnumerator Coroutine_FindTargetEnemyAttackTime()
	{
		while (true)
		{
			yield return new WaitForSeconds(attackTime);
			if (Find_TargetEnemyDir(out Vector3 direction))
			{
				Debug.Log("Fireball Attack towards direction: " + direction);
				GameObject projectileObj = GSC.Instance.Spawn.Spawn_ProjectileSpawn("FireballProjectile");
				if (projectileObj.TryGetComponent<Projectile>(out Projectile TryGetComponent))
				{
					projectileObj.gameObject.SetActive(true);
					Vector3 spawnOffset = direction.normalized * 0.5f;
					Vector3 spawnPosition = transform.position + spawnOffset;
					spawnPosition += new Vector3(0, 0.5f, 0);
					TryGetComponent.Set_ProjectileInfo("FireballProjectile", 10, direction, 5, 30, spawnPosition);
					TryGetComponent.fire();
				}
			}
			else
			{
				Debug.Log("No target enemy found within range.");
			}
		}

	}

	private bool Find_TargetEnemyDir(out Vector3 direction)
	{
		direction = Vector3.zero;

		Collider target = null;
		Collider[] results = Physics.OverlapSphere(transform.position, attackRange);
		float minDist = Mathf.Infinity;

		foreach (var c in results)
		{
			if (!c.CompareTag("Enemy"))
				continue;
			float dist = (c.transform.position - transform.position).sqrMagnitude;

			if (dist < minDist)
			{
				minDist = dist;
				target = c;
			}
		}

		if (target != null)
		{
			direction = (target.transform.position - transform.position).normalized;
			return true;
		}

		return false;
	}



}
