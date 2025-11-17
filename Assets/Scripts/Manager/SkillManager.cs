using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
	public List<AttackStats> attackStatsList = new List<AttackStats>();
	private Dictionary<string, AttackStats> statsDict;

	// 특정 스킬 스탯이 바뀔 때 알려주는 이벤트
	public event Action<string> OnAttackStatsChanged;

	private void Awake()
	{
		GSC.Instance.RegisterSkillManager(this);
		BuildDict();
		//RefreshAllStats();
	}

	void BuildDict()
	{
		statsDict = new Dictionary<string, AttackStats>();
		foreach (var s in attackStatsList)
		{
			if (s != null && !string.IsNullOrEmpty(s.key))
				statsDict[s.key] = s;
			Debug.Log(s);
			Debug.Log(s.key);
		}
	}

	public AttackStats GetStats(string _key)
	{
		if (statsDict.TryGetValue(_key, out var stats))
			return stats;

		Debug.LogWarning($"[SkillManager] AttackStats not found for key: {_key}");
		return null;
	}

	public void RefreshAllStats()
	{
		foreach (var s in attackStatsList)
		{
			if (s == null) continue;
			s.ResetToBase();
			GSC.Instance.attackShardSystemManager.ApplyToStats(s);
			OnAttackStatsChanged?.Invoke(s.key);   // 모든 스킬에게 갱신 알림
		}
	}

	// 특정 스킬만 다시 계산하고 알림
	public void RefreshAttackStats(string _key)
	{
		var s = GetStats(_key);
		if (s == null) return;

		s.ResetToBase();
		GSC.Instance.attackShardSystemManager.ApplyToStats(s);
		OnAttackStatsChanged?.Invoke(_key);
	}

	// 파편 추가 예시
	public void AddFireballShard(string _name ,AttackStatType _type, int _count = 1 )
	{
		GSC.Instance.attackShardSystemManager.AddShard(_name, _type, _count);
		RefreshAttackStats(_name);   // 파이어볼만 다시 계산 + 이벤트
	}


}
