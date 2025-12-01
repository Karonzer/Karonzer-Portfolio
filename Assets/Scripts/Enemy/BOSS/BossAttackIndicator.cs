using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BossAttackIndicator : Projectile
{
	[SerializeField] private Transform warningSphere;
	[SerializeField] private Transform warningSphereCharging;
	[SerializeField] private ParticleSystem hitParticle;

	private Coroutine hitRoutine;
	private Coroutine hitParticleCoroutine;

	private void Awake()
	{
		warningSphere = transform.GetChild(0);
		warningSphereCharging = transform.GetChild(1);
		hitParticle = transform.GetChild(2).GetComponent<ParticleSystem>();
	}

	public override void Launch_Projectile()
	{

	}

	private void OnEnable()
	{
		if (hitRoutine != null)
		{
			StopCoroutine(hitRoutine);
			hitRoutine = null;
		}

		if(hitParticleCoroutine != null)
		{
			StopCoroutine(hitParticleCoroutine);
			hitParticleCoroutine = null;
		}
		TEST();
	}

	public override void Set_ProjectileInfo(string _projectileName, int _projectileDemage, float _projectileRange, Vector3 _dir, float _projectileSpeed, int _projectileSurvivalTime, Vector3 _spawnPos)
	{
		projectileDemage = _projectileDemage;
		projectileName = _projectileName;
		projectileDemage = _projectileDemage;
		projectileRange = _projectileRange;
		projectileDir = _dir;
		projectileSpeed = _projectileSpeed;
		projectileSurvivalTime = _projectileSurvivalTime;
		transform.position = _spawnPos;

		Setting_CurrentProjectile();
	}


	private void TEST()
	{
		Setting_CurrentProjectile();
	}

	private void Setting_CurrentProjectile()
	{
		warningSphere.gameObject.SetActive(true);
		warningSphereCharging.gameObject.SetActive(true);

		warningSphere.localScale = Vector3.one * projectileRange * 2f;
		warningSphereCharging.localScale = Vector3.zero;

		if (hitRoutine != null)
		{
			StopCoroutine(hitRoutine);
			hitRoutine = null;
		}

		if (hitParticle != null)
		{
			hitParticle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
		}

		hitRoutine = StartCoroutine(Start_WarningSphereCharging());
	}

	private IEnumerator Start_WarningSphereCharging()
	{
		float chargeTime = 1.2f;
		float timer = 0f;
		Vector3 targetScale = warningSphere.localScale;
		while (timer < chargeTime)
		{
			if (GSC.Instance.gameManager != null && !GSC.Instance.gameManager.isPaused)
			{
				timer += Time.deltaTime;
				float t = timer / chargeTime;

				// 점점 확대
				warningSphereCharging.localScale = Vector3.Lerp(Vector3.zero, targetScale, t);
			}

			yield return null;
		}

		if(hitParticleCoroutine != null)
		{
			StopCoroutine(hitParticleCoroutine);
			hitParticleCoroutine = null;
		}

		warningSphere.gameObject.SetActive(false);
		warningSphereCharging.gameObject.SetActive(false);
		hitParticleCoroutine = StartCoroutine(Wait_HitParticle());
		DoHitDamage();
	}

	private void DoHitDamage()
	{
		int playerLayerMask = LayerMask.GetMask("Player");

		Collider[] cols = Physics.OverlapSphere(transform.position, projectileRange, playerLayerMask);

		foreach (var col in cols)
		{
			if (col.TryGetComponent<Player>(out Player _player))
			{
				DamageInfo _dmg = GSC.Instance.gameManager.Get_EnemyDamageInfo(projectileDemage, projectileName, _player.CurrentObj, Type.Player);
				_player.Take_Damage(_dmg);
			}
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

		Despawn_Immediately();
	}

	private void Despawn_Immediately()
	{
		gameObject.SetActive(false);
		GSC.Instance.spawnManager.DeSpawn(PoolObjectType.Projectile, projectileName, gameObject);
	}

}
