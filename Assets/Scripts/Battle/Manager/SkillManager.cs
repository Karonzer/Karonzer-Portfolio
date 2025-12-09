using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
	public List<AttackStatsSO> attackStatsList = new List<AttackStatsSO>();
	private Dictionary<string, AttackStats> statsDict;
	private Dictionary<string, AttackStats> currentPlayerBuffstatsDict;

	[SerializeField] private float globalSkillDamageBuff;
	[SerializeField] private float globalSkillAttackSpeedBuff;

	// 특정 스킬 스탯이 바뀔 때 알려주는 이벤트
	public Dictionary<string, Action<string>> eventTableOnAttackStatsChanged = new Dictionary<string, Action<string>>();
	//현재 보유하고 있는 공격 방식
	public Dictionary<string, AttackRoot> currentAttacks = new Dictionary<string, AttackRoot>();
	private void Awake()
	{
		BattleGSC.Instance.RegisterSkillManager(this);
		Build_Dict();

	}


	private void Reset()
	{
		globalSkillDamageBuff = 0;
		globalSkillAttackSpeedBuff = 0;
	}

	void Build_Dict()
	{
		statsDict = new Dictionary<string, AttackStats>();
		currentPlayerBuffstatsDict = new Dictionary<string, AttackStats>();
		foreach (var s in attackStatsList)
		{
			if (s != null && !string.IsNullOrEmpty(s.attackStats.key))
			{
				statsDict[s.attackStats.key] = s.attackStats;
				currentPlayerBuffstatsDict[s.attackStats.key] = new AttackStats();
			}

		}
	}

	//public AttackStats Get_Stats(string _key)
	//{
	//	if (statsDict.TryGetValue(_key, out AttackStats stats))
	//		return stats;

	//	Debug.LogWarning($"[SkillManager] AttackStats not found for key: {_key}");
	//	return default;
	//}


	public AttackStats Get_Stats(string _key)
	{
		if (!statsDict.TryGetValue(_key, out AttackStats baseStats))
			return default;

		if (!currentPlayerBuffstatsDict.TryGetValue(_key, out AttackStats buff))
			return default;

		AttackStats result = baseStats;

		result.rawDamage = result.rawDamage * (1f + globalSkillDamageBuff);
		result.baseAttackInterval = result.baseAttackInterval * (1f + globalSkillAttackSpeedBuff);
		return result;
	}

	public void Set_Stats(string _key, AttackStats _value)
	{
		if (statsDict.ContainsKey(_key))
		{
			statsDict[_key] = _value;
		}
	}

	public void Add_CurrentAttacks(string _key, AttackRoot _attackRoot)
	{
		if(statsDict.ContainsKey(_key))
		{
			currentAttacks[_attackRoot.AttackKey] = _attackRoot;
		}
	}


	// 외부에서 “이 스킬 스탯 바뀌었다”고 알리고 싶을 때 쓰는 헬퍼
	public void NotifyStatsChanged(string _key)
    {
		Invoke_Action(_key);
    }

	public void AddListener(string _key, Action<string> callback)
	{
		if (!eventTableOnAttackStatsChanged.ContainsKey(_key))
			eventTableOnAttackStatsChanged[_key] = null;

		eventTableOnAttackStatsChanged[_key] += callback;
	}

	public void RemoveListener(string _key, Action<string> _callback)
	{
		if (!eventTableOnAttackStatsChanged.ContainsKey(_key))
			return;

		eventTableOnAttackStatsChanged[_key] -= _callback;
	}

	public void Invoke_Action(string _key)
	{
		if (eventTableOnAttackStatsChanged.TryGetValue(_key, out var _action))
		{
			_action?.Invoke(_key);
		}

	}

	public void AddBuff(BuffData data)
	{
		switch (data.effectType)
		{
			case BuffEffectType.DamagePercent:
					globalSkillDamageBuff += (data.value / 100f);
					foreach (var key in statsDict.Keys)
						NotifyStatsChanged(key);
					break;
				
			case BuffEffectType.AttackSpeedPercent:
				globalSkillAttackSpeedBuff += (data.value / 100f);
				foreach (var key in statsDict.Keys)
					NotifyStatsChanged(key);
				break;

		}
	}


	public void RemoveBuff(BuffData data)
	{
		switch (data.effectType)
		{
			case BuffEffectType.DamagePercent:
				globalSkillDamageBuff = 0;
				foreach (var key in statsDict.Keys)
					NotifyStatsChanged(key);
				break;

			case BuffEffectType.AttackSpeedPercent:
				globalSkillAttackSpeedBuff = 0;
				foreach (var key in statsDict.Keys)
					NotifyStatsChanged(key);
				break;

		}
	}

}
