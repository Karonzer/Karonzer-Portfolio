
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningProjectile : Projectile
{
	private Coroutine hitRoutine;
	private Coroutine projectileSurvivalTimeCoroutine;

	private BoxCollider boxCollider;

	[SerializeField] private ParticleSystem lightning;
	[SerializeField] private new Light light;

	private readonly HashSet<IDamageable> targets = new HashSet<IDamageable>();
	private float tickInterval = 0.3f;

	private void Awake()
	{
		boxCollider = GetComponent<BoxCollider>();
		boxCollider.isTrigger = true;

		lightning = transform.GetChild(0).GetComponent<ParticleSystem>();
		light = transform.GetChild(1).GetComponent<Light>();
	}


	private void OnDisable()
	{
		if (hitRoutine != null)
			StopCoroutine(hitRoutine);

		targets.Clear();
	}

	public override void Set_ProjectileInfo(string _projectileName, int _projectileDemage, float _projectileRange, Vector3 _dir, float _projectileSpeed, int _projectileSurvivalTime, Vector3 _spawnPos)
	{
		projectileName = _projectileName;
		projectileDemage = _projectileDemage;
		projectileRange = _projectileRange;
		projectileDir = _dir;
		projectileSpeed = _projectileSpeed;
		projectileSurvivalTime = _projectileSurvivalTime;
		transform.position = _spawnPos;

		boxCollider.size = new Vector3(projectileRange, 10, projectileRange);
		lightning.transform.localScale = new Vector3(projectileRange, 1, projectileRange);

		light.intensity = projectileRange;
		light.range = projectileRange;

		if (hitRoutine != null)
			StopCoroutine(hitRoutine);

		hitRoutine = StartCoroutine(DamageTickRoutine());


		if (projectileSurvivalTimeCoroutine != null)
		{
			StopCoroutine(projectileSurvivalTimeCoroutine);
			projectileSurvivalTimeCoroutine = null;
		}

		projectileSurvivalTimeCoroutine = StartCoroutine(Start_ProjectileSurvivalTimeCoroutine());
	}



	public override void Launch_Projectile()
	{

	}


	private void OnTriggerEnter(Collider other)
	{
		if (other.TryGetComponent<IDamageable>(out IDamageable _enemy))
		{
			_enemy.OnDead += HandleEnemyDead;
			targets.Add(_enemy);

		}
	}



	private void OnTriggerExit(Collider other)
	{
		if (other.TryGetComponent<IDamageable>(out IDamageable _enemy))
		{
			RemoveTarget(_enemy);
		}
	}
	private IEnumerator DamageTickRoutine()
	{
		while (true)
		{
			yield return new WaitForSeconds(tickInterval);

			var snapshot = new List<IDamageable>(targets);

			foreach (var t in snapshot)
			{
				if (t == null) continue;

				DamageInfo info = GSC.Instance.gameManager.Get_PlayerDamageInfo(projectileDemage, t.CurrentObj, Type.Enemy);
				t.Take_Damage(info);
			}
		}
	}

	private IEnumerator Start_ProjectileSurvivalTimeCoroutine()
	{
		yield return new WaitForSeconds(projectileSurvivalTime -20);
		transform.gameObject.SetActive(false);
		GSC.Instance.spawnManager.DeSpawn(PoolObjectType.Projectile, projectileName, transform.gameObject);
	}
	private void HandleEnemyDead(IDamageable __target)
	{
		RemoveTarget(__target);
	}
	private void RemoveTarget(IDamageable _target)
	{
		if (targets.Contains(_target))
		{
			_target.OnDead -= HandleEnemyDead;
			targets.Remove(_target);


		}
	}

}
