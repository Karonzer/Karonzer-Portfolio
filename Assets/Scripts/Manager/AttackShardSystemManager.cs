using System.Collections.Generic;
using UnityEngine;

public class AttackShardSystemManager : MonoBehaviour
{
	private Dictionary<string, Dictionary<AttackStatType, int>> shardStacks;

	private void Awake()
	{
		GSC.Instance.RegisterAttackShardSystemManager(this);
		shardStacks= new Dictionary<string, Dictionary<AttackStatType, int>>();
	}


	public void AddShard(string _attackKey, AttackStatType _statType, int _count = 1)
	{
		if (!shardStacks.TryGetValue(_attackKey, out var statDict))
		{
			statDict = new Dictionary<AttackStatType, int>();
			shardStacks[_attackKey] = statDict;
		}

		if (!statDict.ContainsKey(_statType))
			statDict[_statType] = 0;

		statDict[_statType] += _count;
	}

	public int GetShardCount(string _attackKey, AttackStatType _statType)
	{
		if (!shardStacks.TryGetValue(_attackKey, out var _statDict))
			return 0;
		if (!_statDict.TryGetValue(_statType, out int _value))
			return 0;
		return _value;
	}

	// AttackStats 한 개에 대해 파편 효과 적용
	public void ApplyToStats(AttackStats _stats)
	{
		if (_stats == null || string.IsNullOrEmpty(_stats.key))
			return;

		// 먼저 base로 리셋해 둔 상태에서 호출된다고 가정
		string key = _stats.key;

		int dmgStack = GetShardCount(key, AttackStatType.Damage);
		int atkSpdStack = GetShardCount(key, AttackStatType.AttackInterval);
		int rangeStack = GetShardCount(key, AttackStatType.Range);
		int projSpdStack = GetShardCount(key, AttackStatType.ProjectileSpeed);
		int explRangeStack = GetShardCount(key, AttackStatType.ExplosionRange);

		// 예시 공식 (나중에 조정 가능)
		_stats.currentDamage += dmgStack * 5;
		_stats.currentAttackInterval = Mathf.Max(0.2f,
			_stats.currentAttackInterval - atkSpdStack * 0.1f);
		_stats.currentRange += rangeStack * 0.5f;
		_stats.currentProjectileSpeed += projSpdStack * 0.5f;
		_stats.currentExplosionRange += explRangeStack * 0.3f;
	}
}
