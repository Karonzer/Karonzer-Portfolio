using UnityEngine;

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
		attackStats = GSC.Instance.skillManager.Get_Stats(AttackKey);
		Apply_StatsFromAttackStats();

		// 스탯 변경 이벤트 구독
		GSC.Instance.skillManager.AddListener(AttackKey, Handle_AttackStatsChanged);
	}

	protected virtual void OnDestroy()
	{
		// 씬 전환/오브젝트 삭제 시 이벤트 해제
		if (GSC.Instance != null && GSC.Instance.skillManager != null)
			GSC.Instance.skillManager.RemoveListener(AttackKey, Handle_AttackStatsChanged);
	}

	// SkillManager에서 스탯이 바뀌었다고 알려줄 때 호출
	private void Handle_AttackStatsChanged(string _key)
	{
		if (_key != AttackKey)
			return; // 내 스킬이 아니면 무시

		attackStats = GSC.Instance.skillManager.Get_Stats(AttackKey);
		Apply_StatsFromAttackStats();

	}

	protected virtual void Apply_StatsFromAttackStats()
	{
		attackDamage = attackStats.baseDamage;
		attackRange = attackStats.baseRange;
		attackIntervalTime = attackStats.baseAttackInterval;
	}

	protected abstract void Attack();
}
