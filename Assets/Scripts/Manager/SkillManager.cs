using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
	public List<AttackStatsSO> attackStatsList = new List<AttackStatsSO>();
	private Dictionary<string, AttackStats> statsDict;

	// 특정 스킬 스탯이 바뀔 때 알려주는 이벤트
	public Dictionary<string, Action<string>> eventTableOnAttackStatsChanged = new Dictionary<string, Action<string>>();
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
			Debug.Log(s);
			Debug.Log(s.attackStats.key);
		}
	}

	public AttackStats GetStats(string _key)
	{
		if (statsDict.TryGetValue(_key, out var stats))
			return stats;

		Debug.LogWarning($"[SkillManager] AttackStats not found for key: {_key}");
		return default;
	}



    // 외부에서 “이 스킬 스탯 바뀌었다”고 알리고 싶을 때 쓰는 헬퍼
    public void NotifyStatsChanged(string key)
    {
		Invoke_Action(key);
    }

	public void AddListener(string key, Action<string> callback)
	{
		if (!eventTableOnAttackStatsChanged.ContainsKey(key))
			eventTableOnAttackStatsChanged[key] = null;

		eventTableOnAttackStatsChanged[key] += callback;
	}

	public void RemoveListener(string key, Action<string> callback)
	{
		if (!eventTableOnAttackStatsChanged.ContainsKey(key))
			return;

		eventTableOnAttackStatsChanged[key] -= callback;
	}

	public void Invoke_Action(string key)
	{
		if (eventTableOnAttackStatsChanged.TryGetValue(key, out var action))
			action?.Invoke(key);
	}




}
