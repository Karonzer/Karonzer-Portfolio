using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 관통형 발사체
/// </summary>
public class PenetratingProjectile : Projectile
{
	// 이동 / 수명 
	private Coroutine moveRoutine;
	private Coroutine projectileSurvivalTimeCoroutine;

	// 충돌 판정용 콜라이더
	private SphereCollider sphereCollider;
	private void Awake()
	{
		sphereCollider = GetComponent<SphereCollider>();
		sphereCollider.isTrigger = true;
		audioHandler = GetComponent<IAudioHandler>();
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
		sphereCollider.enabled = true;
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

		audioHandler.Play_OneShot(SoundType.Skill_Penetrating);
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
		yield return new WaitForSeconds(projectileSurvivalTime - 10);
		transform.gameObject.SetActive(false);
		BattleGSC.Instance.spawnManager.DeSpawn(PoolObjectType.Projectile, projectileName, transform.gameObject);
	}

	/// <summary>
	/// 충돌 하는 몬스터한테 데미지 전달
	/// </summary>
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
			return;

		if (other != null && other.CompareTag("Enemy") || other.CompareTag("Boss"))
		{
			if (other.TryGetComponent<IDamageable>(out IDamageable _damageable))
			{
				DamageInfo info = BattleGSC.Instance.gameManager.Get_PlayerDamageInfo(projectileDemage, transform.gameObject, Type.Enemy);
				_damageable.Take_Damage(info);
			}
		}


	}


}
