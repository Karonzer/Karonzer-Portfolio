using UnityEngine;

public abstract class AttackRoot : MonoBehaviour
{
	[SerializeField] protected string attackName;

	protected int attackDamage;
	protected float attackRange;
	protected float attackTime;

	protected AttackStats attackStats;

	protected virtual void Start()
	{
		Debug.Log(attackName);
		attackStats = GSC.Instance.SkillManager.GetStats(attackName);
		ApplyStatsFromAttackStats();

		// 스탯 변경 이벤트 구독
		GSC.Instance.SkillManager.OnAttackStatsChanged += HandleAttackStatsChanged;
	}

	protected virtual void OnDestroy()
	{
		// 씬 전환/오브젝트 삭제 시 이벤트 해제
		if (GSC.Instance != null && GSC.Instance.SkillManager != null)
			GSC.Instance.SkillManager.OnAttackStatsChanged -= HandleAttackStatsChanged;
	}

	// SkillManager에서 스탯이 바뀌었다고 알려줄 때 호출
	private void HandleAttackStatsChanged(string key)
	{
		if (key != attackName)
			return; // 내 스킬이 아니면 무시

		attackStats = GSC.Instance.SkillManager.GetStats(attackName);
		ApplyStatsFromAttackStats();
		// 필요하면 콜라이더 반경 등도 여기서 다시 갱신
	}

	protected virtual void ApplyStatsFromAttackStats()
	{
		if (attackStats == null) return;

		attackDamage = attackStats.currentDamage;
		attackRange = attackStats.currentRange;
		attackTime = attackStats.currentAttackInterval;
	}

	protected abstract void Attack();
}
