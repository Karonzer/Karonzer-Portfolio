using System;
using System.Collections;
using UnityEngine;

public class PenetratingAttack : AttackRoot
{
	private Coroutine attackTimeRoutine;
	private Vector3 cachedCamForward;
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
		StartCoroutine(Coroutine_PenetratingAttackTime());
	}

	protected override void Apply_StatsFromAttackStats()
	{
		base.Apply_StatsFromAttackStats();
	}



	private IEnumerator Coroutine_PenetratingAttackTime()
	{
		while (true)
		{
			yield return new WaitForSeconds(attackIntervalTime);
			if (GSC.Instance.gameManager != null && !GSC.Instance.gameManager.isPaused)
			{
				for (int i = 0; i < attackStats.ProjectileCount; i++)
				{
					if (Find_PenetratingDir(i, attackStats.ProjectileCount, out Vector3 _direction))
					{
						GameObject projectileObj = GSC.Instance.spawnManager.Spawn(PoolObjectType.Projectile, ProjectileKey);
						if (projectileObj.TryGetComponent<Projectile>(out Projectile _Component))
						{
							projectileObj.gameObject.SetActive(true);
							Vector3 spawnOffset = _direction.normalized * 0.5f;
							Vector3 spawnPosition = transform.position + spawnOffset;
							spawnPosition += new Vector3(0, 0.1f, 0);
							_Component.Set_ProjectileInfo(ProjectileKey, attackDamage, attackStats.baseExplosionRange, _direction, attackStats.baseProjectileSpeed, DBManager.ProjectileSurvivalTime, spawnPosition);
							_Component.Launch_Projectile();
						}
						yield return new WaitForSeconds(0.1f);

					}
				}


			}

		}

	}

	private bool Find_PenetratingDir(int index, int totalCount, out Vector3 _direction)
	{
		Vector3 camForward = Camera.main.transform.forward;
		camForward.y = 0f;
		camForward.Normalize();

		// 부채꼴 각도 설정 (예: 60도)
		float angleRange = 60f;
		float halfRange = angleRange / 2f;

		// 발사체가 1개일 경우 정면으로
		if (totalCount == 1)
		{
			_direction = camForward;
			return true;
		}

		// 발사체 수에 따라 각도 분배
		float angleStep = angleRange / (totalCount - 1);
		float angle = -halfRange + angleStep * index;

		// 회전 계산
		Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
		_direction = rotation * camForward;

		return true;
	}
}
