using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MeteoProjectile : Projectile
{
	private Coroutine moveRoutine;
	private Coroutine projectileSurvivalTimeCoroutine;
	private Coroutine hitRoutine;

	private SphereCollider sphereCollider;

	[SerializeField] private GameObject visualRoot;
	[SerializeField] private ParticleSystem hitParticle;

	private readonly Collider[] enemyBuffer = new Collider[20];

	private bool isHit;

	private void Awake()
	{
		sphereCollider = GetComponent<SphereCollider>();
		sphereCollider.isTrigger = true;

		visualRoot = transform.GetChild(0).gameObject;
		hitParticle = transform.GetChild(1).GetComponent<ParticleSystem>();
		audioHandler = GetComponent<IAudioHandler>();
	}

	private void OnEnable()
	{
		isHit = false;
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
			if (BattleGSC.Instance.gameManager != null && !BattleGSC.Instance.gameManager.isPaused)
			{
				transform.Translate(Vector3.down * projectileSpeed * Time.deltaTime);
				yield return null;
			}
			else
			{
				yield return null;
			}
		}
	}

	private IEnumerator Start_ProjectileSurvivalTimeCoroutine()
	{
		yield return new WaitForSeconds(projectileSurvivalTime);
		transform.gameObject.SetActive(false);
		BattleGSC.Instance.spawnManager.DeSpawn(PoolObjectType.Projectile, projectileName, transform.gameObject);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (isHit) return;
		if (other.CompareTag("Player"))
			return;

		isHit = true;
		audioHandler.Play_OneShot(SoundType.Skill_Meteo);
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

		ApplyShockwaveDamage();


		if (visualRoot != null)
			visualRoot.SetActive(false);

		if (hitParticle != null)
		{
			hitRoutine = StartCoroutine(Wait_HitParticle());
		}
		else
		{
			Despawn_Immediately();
		}
	}

	private void ApplyShockwaveDamage()
	{
		int enemyLayerMask = LayerMask.GetMask("Enemy");
		Collider[] hits = Physics.OverlapSphere(transform.position, projectileRange, enemyLayerMask);

		foreach (var hit in hits)
		{
			if (hit.TryGetComponent<IDamageable>(out var _enemy))
			{
				DamageInfo info = BattleGSC.Instance.gameManager.Get_PlayerDamageInfo(projectileDemage, _enemy.CurrentObj, Type.Enemy);
				_enemy.Take_Damage(info);
			}
		}
	}


	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("Player"))
			return;
	}

	private IEnumerator Wait_HitParticle()
	{
		hitParticle.Play();

		while (hitParticle != null && hitParticle.IsAlive(true))
		{
			yield return null;
		}

		Despawn_Immediately();
	}

	private void Despawn_Immediately()
	{
		gameObject.SetActive(false);
		BattleGSC.Instance.spawnManager.DeSpawn(PoolObjectType.Projectile, projectileName, gameObject);
	}
}
