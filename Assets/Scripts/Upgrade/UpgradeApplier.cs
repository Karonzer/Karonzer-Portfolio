using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public static class UpgradeApplier
{
	public static void Apply_Upgrade(UpgradeOptionSO option)
	{
		var skillMgr = GSC.Instance.skillManager;

		bool hasSkill = skillMgr.currentAttacks.ContainsKey(option.targetKey);

		if (option.optionType == UpgradeOptionType.Global)
		{
			var stat = GSC.Instance.statManager;
			var data = stat.Get_PlayerData(GSC.Instance.gameManager.CurrentPlayerKey);

			switch (option.effectType)
			{
				case UpgradeEffectType.MoveSpeed:
					data.moveSpeed = Mathf.RoundToInt(data.moveSpeed * (1f + option.value / 100f));
					break;

				case UpgradeEffectType.CurrentHPAndMaxHP:
					data.maxHP = Mathf.RoundToInt(data.maxHP * (1f + option.value / 100f));
					break;

				case UpgradeEffectType.CriticalDamage:
					data.criticalDamage += (int)option.value;
					break;

				case UpgradeEffectType.CriticalChance:
					data.criticalChance += (int)option.value;
					break;


			}

			stat.Set_PlayerData(GSC.Instance.gameManager.CurrentPlayerKey,data);
			stat.InvokeAction_ChangePlayerStruct();

			// 전체 강화라면 여기서 별도 처리 (예: PlayerHP 등)
			// 일단 예시는 스킬 하나만 다룬다고 보고 넘어가자
			return;
		}

		// 1) 새로운 스킬 해금
		if (option.optionType == UpgradeOptionType.SkillUnlock)
		{
			if (!hasSkill)
			{
				if(GSC.Instance.gameManager.Get_PlayerObject().TryGetComponent<Player>(out Player _player))
				{
					_player.Add_AttackObject(option.targetKey);
				}
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
					stats.baseDamage = Mathf.RoundToInt(stats.baseDamage * (1f + option.value / 100f));
					break;

				case UpgradeEffectType.AttackSpeedPercent:
					stats.baseAttackInterval *= (1f - option.value / 100f);
					stats.baseAttackInterval = Mathf.Max(0.1f, stats.baseAttackInterval);
					break;

				case UpgradeEffectType.RangeFlat:
					stats.baseRange *= (1f + option.value / 100f);
					break;

				case UpgradeEffectType.ProjectileSpeedPercent:
					stats.baseProjectileSpeed *= (1f + option.value / 100f);
					break;

				case UpgradeEffectType.ExplosionRangeFlat:
					stats.baseExplosionRange *= (1f + option.value / 100f);
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
