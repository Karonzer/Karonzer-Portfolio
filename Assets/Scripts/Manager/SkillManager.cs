using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
	public List<AttackStatsSO> attackStatsList = new List<AttackStatsSO>();
	private Dictionary<string, AttackStats> statsDict;

	// 특정 스킬 스탯이 바뀔 때 알려주는 이벤트
	public Dictionary<string, Action<string>> eventTableOnAttackStatsChanged = new Dictionary<string, Action<string>>();
	//현재 보유하고 있는 공격 방식
	public Dictionary<string, AttackRoot> currentAttacks = new Dictionary<string, AttackRoot>();
	private void Awake()
	{
		GSC.Instance.RegisterSkillManager(this);
		Build_Dict();

	}

	void Build_Dict()
	{
		statsDict = new Dictionary<string, AttackStats>();
		foreach (var s in attackStatsList)
		{
			if (s != null && !string.IsNullOrEmpty(s.attackStats.key))
				statsDict[s.attackStats.key] = s.attackStats;
		}
	}

	public AttackStats Get_Stats(string _key)
	{
		if (statsDict.TryGetValue(_key, out AttackStats stats))
			return stats;

		Debug.LogWarning($"[SkillManager] AttackStats not found for key: {_key}");
		return default;
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
		if(currentAttacks.ContainsKey(_key))
		{
			currentAttacks[_key] = _attackRoot;
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




}
