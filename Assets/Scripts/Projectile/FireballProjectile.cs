using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class FireballProjectile : Projectile
{
	private Coroutine moveRoutine;
	private Coroutine projectileSurvivalTimeCoroutine;
	private Coroutine hitRoutine;

	private SphereCollider sphereCollider;

	[SerializeField] private GameObject visualRoot;
	[SerializeField] private ParticleSystem hitParticle;

	private bool isHit;

	private void Awake()
	{
		sphereCollider = GetComponent<SphereCollider>();
		sphereCollider.isTrigger = true;

		visualRoot = transform.GetChild(0).gameObject;
		hitParticle = transform.GetChild(1).GetComponent<ParticleSystem>() ;
	}

	private void OnEnable()
	{
		isHit = false;
	}

	public override void Set_ProjectileInfo(string _projectileName, int _projectileDemage, float _projectileRange, Vector3 _dir, float _projectileSpeed, int _projectileSurvivalTime, Vector3 spawnPos)
	{
		projectileName = _projectileName;
		projectileDemage = _projectileDemage;
		projectileRange = _projectileRange;
		projectileDir = _dir;
		projectileSpeed = _projectileSpeed;
		projectileSurvivalTime = _projectileSurvivalTime;
		transform.position = spawnPos;

		Setting_CurrentProjectile();
	}

	private void Setting_CurrentProjectile()
	{
		isHit = false;
		sphereCollider.enabled = true;

		if (visualRoot != null)
			visualRoot.SetActive(true);

		if (hitParticle != null)
		{
			hitParticle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
		}
	}

	public override void Launch_Projectile()
	{
		if(moveRoutine != null)
		{
			StopCoroutine(moveRoutine);
			moveRoutine = null;
		}

		if(projectileSurvivalTimeCoroutine != null)
		{
			StopCoroutine(projectileSurvivalTimeCoroutine);
			projectileSurvivalTimeCoroutine = null;
		}

		if (hitRoutine != null)
		{
			StopCoroutine(hitRoutine);
			hitRoutine = null;
		}

		moveRoutine = StartCoroutine(Start_MoveFireballProjectile());
		projectileSurvivalTimeCoroutine = StartCoroutine(Start_ProjectileSurvivalTimeCoroutine());
	}

	private IEnumerator Start_MoveFireballProjectile()
	{
		while (true)
		{
			transform.Translate(projectileDir * projectileSpeed * Time.deltaTime);
			yield return null;
		}
	}

	private IEnumerator Start_ProjectileSurvivalTimeCoroutine()
	{
		yield return new WaitForSeconds(projectileSurvivalTime);
		transform.gameObject.SetActive(false);
		GSC.Instance.spawn.DeSpawn_Projectile(projectileName, transform.gameObject);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (isHit) return;
		if (other.CompareTag("Player"))
			return;

		isHit = true;

		if (moveRoutine != null)
		{
			StopCoroutine(moveRoutine);
			moveRoutine = null;
		}
		if (projectileSurvivalTimeCoroutine != null)
		{
			StopCoroutine(projectileSurvivalTimeCoroutine);
			projectileSurvivalTimeCoroutine = null;
		}

		sphereCollider.enabled = false;

		if (other.CompareTag("Enemy"))
		{
			Collider[] hits = Physics.OverlapSphere(transform.position, projectileRange);
			List<Transform> enemies = hits.Get_Enemies();
			foreach (var e in enemies)
			{
				if (e.TryGetComponent<IDamageable>(out IDamageable _damageable))
				{
					_damageable.Take_Damage(projectileDemage);
				}
			}

		}

		if (visualRoot != null)
			visualRoot.SetActive(false);

		if (hitParticle != null)
		{
			hitRoutine = StartCoroutine(Wait_HitParticle());
		}
		else
		{
			DespawnImmediately();
		}
	}

	private IEnumerator Wait_HitParticle()
	{
		hitParticle.Play();

		// 파티클이 완전히 끝날 때까지 대기
		while (hitParticle != null && hitParticle.IsAlive(true))
		{
			yield return null;
		}

		DespawnImmediately();
	}

	private void DespawnImmediately()
	{
		gameObject.SetActive(false);
		GSC.Instance.spawn.DeSpawn_Projectile(projectileName, gameObject);
	}
}
