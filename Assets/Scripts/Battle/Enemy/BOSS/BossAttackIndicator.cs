using System.Collections;
using UnityEngine;

/// <summary>
/// BossAttackIndicator
/// 
/// 보스의 광역 공격(장판/폭발) 경고 표시 + 타이밍 데미지"를 담당하는 투사체.
/// Projectile을 상속하지만, 일반적인 날아가는 투사체가 아니라
/// 1) 바닥에 경고 원을 보여주고
/// 2) 일정 시간(충전) 후
/// 3) 범위 내 플레이어에게 데미지 적용
/// 4) 히트 파티클 재생이 끝나면 풀로 반환
/// 
/// 사용 흐름:
/// - Set_ProjectileInfo(...)로 범위/데미지/스폰 위치 등을 세팅
/// - Setting_CurrentProjectile()에서 경고 원 초기화 + 충전 코루틴 시작
/// - 충전 완료 시 DoHitDamage()로 실제 데미지 적용
/// - 파티클 종료까지 기다렸다가 DeSpawn 처리
/// </summary>
public class BossAttackIndicator : Projectile
{
	/// 고정 경고 원(최종 범위 표시)
	[SerializeField] private Transform warningSphere;
	/// 충전 효과(0에서 시작해서 최종 범위까지 커지는 원)
	[SerializeField] private Transform warningSphereCharging;
	/// 폭발/히트 파티클(충전 종료 후 재생)
	[SerializeField] private ParticleSystem hitParticle;

	private Coroutine hitRoutine;
	private Coroutine hitParticleCoroutine;

	private void Awake()
	{
		warningSphere = transform.GetChild(0);
		warningSphereCharging = transform.GetChild(1);
		hitParticle = transform.GetChild(2).GetComponent<ParticleSystem>();
	}

	/// <summary>
	/// Projectile의 발사 함수지만,
	/// 이 오브젝트는 날아가는 투사체가 아니라서 비워둔 상태
	/// </summary>
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
	}

	/// <summary>
	/// 투사체 정보 세팅
	/// - 데미지/범위/속도/생존시간/스폰 위치 등 저장
	/// - 세팅이 끝나면 바로 Setting_CurrentProjectile()로 연출 시작
	/// </summary>
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

	/// <summary>
	/// 장판(경고 원) 초기화 및 충전 시작
	/// - 경고 원 ON
	/// - 최종 범위를 projectileRange 기반으로 스케일 설정
	/// - 충전 원은 0에서 시작
	/// - 충전 코루틴 시작
	/// </summary>

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

	/// <summary>
	/// 충전 코루틴
	/// - 일정 시간(chargeTime) 동안 충전 원을 0 → 최종 크기로 Lerp 확대
	/// - 게임 Pause 상태면 타이머를 진행하지 않음
	/// 
	/// 충전 완료되면:
	/// - 경고 원/충전 원 숨김
	/// - 히트 파티클 재생 코루틴 시작
	/// - DoHitDamage()로 실제 데미지 적용
	/// </summary>
	private IEnumerator Start_WarningSphereCharging()
	{
		float chargeTime = 1.2f;
		float timer = 0f;
		Vector3 targetScale = warningSphere.localScale;
		while (timer < chargeTime)
		{
			if (BattleGSC.Instance.gameManager != null && !BattleGSC.Instance.gameManager.isPaused)
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

	/// <summary>
	/// 실제 데미지 적용
	/// - projectileRange 반경 안에 있는 Player 콜라이더를 찾고
	/// - Player 컴포넌트가 있으면 DamageInfo를 만들어 Take_Damage 호출
	/// </summary>
	private void DoHitDamage()
	{
		int playerLayerMask = LayerMask.GetMask("Player");

		Collider[] cols = Physics.OverlapSphere(transform.position, projectileRange, playerLayerMask);

		foreach (var col in cols)
		{
			if (col.TryGetComponent<Player>(out Player _player))
			{
				DamageInfo _dmg = BattleGSC.Instance.gameManager.Get_EnemyDamageInfo(projectileDemage, projectileName, _player.CurrentObj, Type.Player);
				_player.Take_Damage(_dmg);
			}
		}
	}

	/// <summary>
	/// 히트 파티클 재생 후, 완전히 끝날 때까지 대기
	/// - 파티클이 끝나면 즉시 풀로 반환
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
	/// 즉시 디스폰(풀 반환)
	/// - 오브젝트 비활성화 후 SpawnManager에 반환
	/// </summary>
	private void Despawn_Immediately()
	{
		gameObject.SetActive(false);
		BattleGSC.Instance.spawnManager.DeSpawn(PoolObjectType.Projectile, projectileName, gameObject);
	}

}
