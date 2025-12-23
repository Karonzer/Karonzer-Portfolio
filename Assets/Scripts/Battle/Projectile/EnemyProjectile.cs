using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 원거리 몬스터가 발사하는 투사체
/// </summary>
public class EnemyProjectile : Projectile
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

	private bool isHit;

	private void Awake()
	{
		sphereCollider = GetComponent<SphereCollider>();
		sphereCollider.isTrigger = true;

		visualRoot = transform.GetChild(0).gameObject;
		hitParticle = transform.GetChild(1).GetComponent<ParticleSystem>();
	}

	private void OnEnable()
	{
		isHit = false;
	}

	private void OnDisable()
	{
		if (projectileSurvivalTimeCoroutine != null)
		{
			StopCoroutine(projectileSurvivalTimeCoroutine);
			projectileSurvivalTimeCoroutine = null;
		}
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
	/// - 투사체 이동 코루틴 실행
	/// - 생존 시간 체크 코루틴 실행
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
				transform.rotation = Quaternion.LookRotation(projectileDir);
				transform.Translate(projectileDir * projectileSpeed * Time.deltaTime, Space.World);
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
	/// 충돌 처리 플레이어와 충돌 했을때 데미지 전달
	/// </summary>
	private void OnTriggerEnter(Collider other)
	{
		if (isHit) return;
		if (other.CompareTag("Enemy"))
			return;

		if (other != null && other.CompareTag("Player"))
		{
			isHit = true;

			if (moveRoutine != null)
			{
				StopCoroutine(moveRoutine);
				moveRoutine = null;
			}

			sphereCollider.enabled = false;

			if (hitParticle != null)
			{
				hitRoutine = StartCoroutine(Wait_HitParticle());
			}
			else
			{
				Despawn_Immediately();
			}

			if (visualRoot != null)
				visualRoot.SetActive(false);

			if (other.TryGetComponent<IDamageable>(out IDamageable _damageable))
			{
				DamageInfo info = BattleGSC.Instance.gameManager.Get_PlayerDamageInfo(projectileDemage, transform.gameObject, Type.Enemy);
				_damageable.Take_Damage(info);
			}
		}
	}



	/// <summary>
	/// 히트 파티클 종료 대기
	/// </summary>
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

	/// <summary>
	/// 즉시 풀 반환
	/// </summary>
	private void Despawn_Immediately()
	{
		gameObject.SetActive(false);
		BattleGSC.Instance.spawnManager.DeSpawn(PoolObjectType.Projectile, projectileName, gameObject);
	}
}
