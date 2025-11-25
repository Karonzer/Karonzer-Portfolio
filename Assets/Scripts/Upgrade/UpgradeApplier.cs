using UnityEngine;

public static class UpgradeApplier
{
	public static void Apply_Upgrade(UpgradeOptionSO option)
	{
		var skillMgr = GSC.Instance.skillManager;

		bool hasSkill = skillMgr.currentAttacks.ContainsKey(option.targetKey);

		// 1) 새로운 스킬 해금
		if (option.optionType == UpgradeOptionType.SkillUnlock)
		{
			if (!hasSkill)
			{

			}
			return;
		}

		// 2) 기존 스킬 강화
		if (option.optionType == UpgradeOptionType.SkillUpgrade)
		{
			AttackStats stats = skillMgr.Get_Stats(option.targetKey);

			switch (option.effectType)
			{
				case UpgradeEffectType.DamagePercent:
					stats.baseDamage = Mathf.RoundToInt(
					stats.baseDamage * (1f + option.value / 100f));
					break;

				case UpgradeEffectType.AttackSpeedPercent:
					stats.baseAttackInterval *= (1f - option.value / 100f);
					stats.baseAttackInterval = Mathf.Max(0.1f, stats.baseAttackInterval);
					break;

				case UpgradeEffectType.RangeFlat:
					stats.baseRange += option.value;
					break;

				case UpgradeEffectType.ProjectileSpeedPercent:
					stats.baseProjectileSpeed *= (1f + option.value / 100f);
					break;

				case UpgradeEffectType.ExplosionRangeFlat:
					stats.baseExplosionRange += option.value;
					break;

				case UpgradeEffectType.ExtraProjectileCount:
					stats.ProjectileCount += 1;
					break;
			}

			skillMgr.Set_Stats(option.targetKey, stats);
			skillMgr.NotifyStatsChanged(option.targetKey);
		}
	}
}
