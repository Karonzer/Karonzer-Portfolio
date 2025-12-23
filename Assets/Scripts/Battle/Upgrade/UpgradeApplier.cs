using UnityEngine;

/// <summary>
/// 선택된 UpgradeOptionSO를 실제 게임 데이터에 적용하는 책임 전용 클래스.
/// 
/// 특징:
/// - static 클래스 (상태 없음)
/// - UI / 선택 로직과 완전히 분리
/// - 업그레이드를 적용한다는 행위만 담당
/// 
/// 적용 대상:
/// 1) 글로벌 플레이어 스탯
/// 2) 스킬 해금
/// 3) 기존 스킬 강화
/// </summary>
public static class UpgradeApplier
{
	/// <summary>
	/// 업그레이드 옵션 적용 진입점
	/// </summary>
	public static void Apply_Upgrade(UpgradeOptionSO option)
	{
		var skillMgr = BattleGSC.Instance.skillManager;

		// 해당 스킬을 이미 가지고 있는지 여부
		bool hasSkill = skillMgr.currentAttacks.ContainsKey(option.targetKey);

		//글로벌 업그레이드 (플레이어 공통 스탯)
		if (option.optionType == UpgradeOptionType.Global)
		{

			// 현재 플레이어 스탯 복사본 획득
			var stat = BattleGSC.Instance.statManager;
			var data = stat.Get_PlayerData(BattleGSC.Instance.gameManager.CurrentPlayerKey);

			// 업그레이드 효과 타입별 처리
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

			// 수정된 스탯 적용
			stat.Set_PlayerData(BattleGSC.Instance.gameManager.CurrentPlayerKey,data);
			stat.InvokeAction_ChangePlayerStruct();

			return;
		}

		//새로운 스킬 해금
		if (option.optionType == UpgradeOptionType.SkillUnlock)
		{
			// 이미 스킬을 가지고 있다면 무시
			if (!hasSkill)
			{
				if(BattleGSC.Instance.gameManager.Get_PlayerObject().TryGetComponent<Player>(out Player _player))
				{
					// 플레이어에게 공격 스킬 오브젝트 추가
					_player.Add_AttackObject(option.targetKey);
				}
			}
			return;
		}

		//기존 스킬 강화
		if (option.optionType == UpgradeOptionType.SkillUpgrade)
		{
			// 현재 스킬 스탯 획득
			AttackStats stats = skillMgr.Get_Stats(option.targetKey);

			// 강화 타입에 따라 스탯 수정
			switch (option.effectType)
			{
				case UpgradeEffectType.DamagePercent:
					stats.rawDamage *= (1f + option.value / 100f);
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

			// 변경된 스탯 저장
			skillMgr.Set_Stats(option.targetKey, stats);
			// 해당 스킬을 사용하는 AttackRoot에 변경 알림
			skillMgr.NotifyStatsChanged(option.targetKey);
		}
	}
}
