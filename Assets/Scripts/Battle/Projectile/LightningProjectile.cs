
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 범위 내 적에게 지속 피해(Tick Damage)를 주는 번개 장판형 투사체
/// </summary>
public class LightningProjectile : Projectile
{
	private Coroutine hitRoutine;
	private Coroutine projectileSurvivalTimeCoroutine;

	private BoxCollider boxCollider;

	[SerializeField] private ParticleSystem lightning;
	[SerializeField] private new Light light;

	// 현재 타격 중인 적 목록 (중복 방지)
	private readonly HashSet<IDamageable> targets = new HashSet<IDamageable>();

	// 틱 데미지 간격
	private float tickInterval = 0.3f;

	private void Awake()
	{
		boxCollider = GetComponent<BoxCollider>();
		boxCollider.isTrigger = true;

		lightning = transform.GetChild(0).GetComponent<ParticleSystem>();
		light = transform.GetChild(1).GetComponent<Light>();
		audioHandler = GetComponent<IAudioHandler>();
	}


	private void OnDisable()
	{
		if (hitRoutine != null)
			StopCoroutine(hitRoutine);

		targets.Clear();
	}

	/// <summary>
	/// 투사체 세팅 (범위 기반)
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

		// 범위에 따라 판정 크기 조절
		boxCollider.size = new Vector3(projectileRange, 10, projectileRange);
		lightning.transform.localScale = new Vector3(projectileRange, 1, projectileRange);

		light.intensity = projectileRange;
		light.range = projectileRange;

	}


	/// <summary>
	/// 투사체 데미지 시작
	/// - 데미지 전달 코루틴 실행
	/// - 생존 시간 체크 코루틴 실행
	/// </summary>
	public override void Launch_Projectile()
	{
		if (hitRoutine != null)
			StopCoroutine(hitRoutine);

		hitRoutine = StartCoroutine(DamageTickRoutine());


		if (projectileSurvivalTimeCoroutine != null)
		{
			StopCoroutine(projectileSurvivalTimeCoroutine);
			projectileSurvivalTimeCoroutine = null;
		}

		audioHandler.Play_OneShot(SoundType.Skill_Lightning);
		projectileSurvivalTimeCoroutine = StartCoroutine(Start_ProjectileSurvivalTimeCoroutine());
	}

	/// <summary>
	/// 범위 내 몬스터 등록
	/// </summary>
	private void OnTriggerEnter(Collider other)
	{
		if (other.TryGetComponent<IDamageable>(out IDamageable _enemy))
		{
			_enemy.OnDead += HandleEnemyDead;
			targets.Add(_enemy);

		}
	}


	/// <summary>
	/// 범위 이탈 시 제거
	/// </summary>
	private void OnTriggerExit(Collider other)
	{
		if (other.TryGetComponent<IDamageable>(out IDamageable _enemy))
		{
			RemoveTarget(_enemy);
		}
	}

	/// <summary>
	/// 주기적으로 대상에게 데미지 적용
	/// </summary>
	private IEnumerator DamageTickRoutine()
	{
		while (true)
		{
			yield return new WaitForSeconds(tickInterval);

			if (BattleGSC.Instance.gameManager != null && !BattleGSC.Instance.gameManager.isPaused)
			{
				yield return null;
			}

			var snapshot = new List<IDamageable>(targets);

			foreach (var t in snapshot)
			{
				if (t == null) continue;

				DamageInfo info = BattleGSC.Instance.gameManager.Get_PlayerDamageInfo(projectileDemage, t.CurrentObj, Type.Enemy);
				t.Take_Damage(info);
			}
		}
	}

	/// <summary>
	/// 생존 시간 만료 시 자동 디스폰
	/// </summary>
	private IEnumerator Start_ProjectileSurvivalTimeCoroutine()
	{
		yield return new WaitForSeconds(projectileSurvivalTime -20);
		transform.gameObject.SetActive(false);
		BattleGSC.Instance.spawnManager.DeSpawn(PoolObjectType.Projectile, projectileName, transform.gameObject);
	}
	/// <summary>
	/// 범위 내에 몬스터가 처치되었을때 해당 몬스터 목록에서 제거
	/// </summary>
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
