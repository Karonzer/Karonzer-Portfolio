using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 위에서 아래로 이동하면 충돌 시 범위 폭발 데미지를 주는 투사체
/// </summary>
public class MeteoProjectile : Projectile
{
	// 이동 / 수명 / 히트 연출 코루틴
	private Coroutine moveRoutine;
	private Coroutine projectileSurvivalTimeCoroutine;
	private Coroutine hitRoutine;

	// 충돌 판정용 콜라이더
	private SphereCollider sphereCollider;

	// 시각 효과 루트
	[SerializeField] private GameObject visualRoot;
	// 충돌 시 재생되는 파티클
	[SerializeField] private ParticleSystem hitParticle;

	// 중복 히트 방지
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

	/// <summary>
	/// 투사체 데이터 초기화
	/// </summary>
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

	/// <summary>
	/// 재사용 시 상태 초기화
	/// </summary>
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

	/// <summary>
	/// 투사체 발사
	/// </summary>
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

	/// <summary>
	/// 투사체 이동 루프
	/// </summary>
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

	/// <summary>
	/// 생존 시간 만료 시 자동 디스폰
	/// </summary>
	private IEnumerator Start_ProjectileSurvivalTimeCoroutine()
	{
		yield return new WaitForSeconds(projectileSurvivalTime);
		transform.gameObject.SetActive(false);
		BattleGSC.Instance.spawnManager.DeSpawn(PoolObjectType.Projectile, projectileName, transform.gameObject);
	}

	/// <summary>
	/// 충돌 처리 (폭발 판정)
	/// </summary>
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

		// 범위 폭발 데미지
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

	/// <summary>
	/// 범위 내에 있는 몬스터한테 데미지 전달
	/// </summary>
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



	/// <summary>
	/// 히트 파티클 종료 대기
	/// </summary>
	private IEnumerator Wait_HitParticle()
	{
		hitParticle.Play();

		while (hitParticle != null && hitParticle.IsAlive(true))
		{
			yield return null;
		}

		Despawn_Immediately();
	}

	/// <summary>
	/// 즉시 풀 반환
	/// </summary>
	private void Despawn_Immediately()
	{
		gameObject.SetActive(false);
		BattleGSC.Instance.spawnManager.DeSpawn(PoolObjectType.Projectile, projectileName, gameObject);
	}
}
