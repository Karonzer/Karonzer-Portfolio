using UnityEngine;


/// <summary>
/// 모든 공격 스킬의 공통 베이스 클래스
/// - 스탯 로딩
/// - 스탯 변경 이벤트 구독
/// - 실제 공격 로직은 자식 클래스에서 구현
/// </summary>
public abstract class AttackRoot : MonoBehaviour
{
	[SerializeField] AttackStatsSO attackStatsSO;
	public string AttackKey => attackStatsSO.attackStats.key;
	public string ProjectileKey => attackStatsSO.attackStats.projectileKey;

	[SerializeField] protected int attackDamage;
	[SerializeField] protected float attackRange;
	[SerializeField] protected float attackIntervalTime;

	[SerializeField] protected AttackStats attackStats;

	protected virtual void Start()
	{
		// 현재 스킬 스탯 로드
		attackStats = BattleGSC.Instance.skillManager.Get_Stats(AttackKey);
		Apply_StatsFromAttackStats();

		// 스탯 변경 이벤트 구독
		BattleGSC.Instance.skillManager.AddListener(AttackKey, Handle_AttackStatsChanged);
	}

	protected virtual void OnDestroy()
	{
		// 씬 전환/오브젝트 삭제 시 이벤트 해제
		if (BattleGSC.Instance != null && BattleGSC.Instance.skillManager != null)
			BattleGSC.Instance.skillManager.RemoveListener(AttackKey, Handle_AttackStatsChanged);
	}

	/// <summary>
	/// SkillManager에서 해당 스킬 스탯이 변경되었을 때 호출됨
	/// </summary>
	private void Handle_AttackStatsChanged(string _key)
	{
		if (_key != AttackKey)
			return; // 내 스킬이 아니면 무시

		// 최신 스탯 다시 로드
		attackStats = BattleGSC.Instance.skillManager.Get_Stats(AttackKey);
		Apply_StatsFromAttackStats();


	}
	/// <summary>
	/// AttackStats 데이터를 실제 전투 변수에 반영
	/// </summary>
	protected virtual void Apply_StatsFromAttackStats()
	{
		attackDamage = attackStats.baseDamage;
		attackRange = attackStats.baseRange;
		attackIntervalTime = attackStats.baseAttackInterval;
	}

	/// <summary>
	/// 실제 공격 로직 (자식 클래스에서 구현)
	/// </summary>
	protected abstract void Attack();
}
