using UnityEngine;

/// <summary>
/// 모든 투사체의 공통 베이스 클래스
/// - 공격에서 생성된 투사체가 공통으로 가지는 데이터 정의
/// - 실제 이동 / 충돌 / 데미지 로직은 자식 클래스에서 구현
/// </summary>
public abstract class Projectile : MonoBehaviour
{
	// 풀에서 관리될 투사체 식별 키
	[SerializeField] protected string projectileName;

	// 투사체가 주는 데미지
	[SerializeField] protected int projectileDemage;

	// 폭발 / 판정 범위
	[SerializeField] protected float projectileRange;

	// 이동 방향
	[SerializeField] protected Vector3 projectileDir;

	// 이동 속도
	[SerializeField] protected float projectileSpeed;

	// 생존 시간 (초)
	[SerializeField] protected int projectileSurvivalTime;

	// 사운드 재생 핸들러
	[SerializeField] protected IAudioHandler audioHandler;

	/// <summary>
	/// 투사체 정보 세팅
	/// - Spawn 직후 호출됨
	/// - 풀 재사용을 고려해 반드시 모든 값 초기화
	/// </summary>
	public abstract void Set_ProjectileInfo(string _projectileName,int _projectileDemage, float _projectileRange, Vector3 _dir, float _projectileSpeed, int _projectileSurvivalTime, Vector3 _spawnPos);

	/// <summary>
	/// 투사체 실제 발사 시작
	/// - 이동 / 수명 / 공격 루틴 시작 지점
	/// </summary>
	public abstract void Launch_Projectile();
}
