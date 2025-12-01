using System.Collections.Generic;
using UnityEngine;
using System;

public class StatManager : MonoBehaviour
{
	public List<PlayerDataSO> playerStatList = new List<PlayerDataSO>();
	private Dictionary<string, PlayerStruct> playerStatDict;

	public List<EnemyDataSO> enemyStatList = new List<EnemyDataSO>();
	private Dictionary<string, EnemyStruct> enemyStatDict;

	public event Action onChangePlayerStruct;

	private void Awake()
	{
		GSC.Instance.RegisterStatManager(this);
		Build_Dict();

	}

	void Build_Dict()
	{
		playerStatDict = new Dictionary<string, PlayerStruct>();
		enemyStatDict = new Dictionary<string, EnemyStruct>();

		foreach (var s in playerStatList)
		{
			if (s != null && !string.IsNullOrEmpty(s.playerStruct.key))
				playerStatDict[s.playerStruct.key] = s.playerStruct;
		}

		foreach (var s in enemyStatList)
		{
			if (s != null && !string.IsNullOrEmpty(s.enemyStruct.key))
			enemyStatDict[s.enemyStruct.key] = s.enemyStruct;
		}
	}

	public PlayerStruct Get_PlayerData(string _key)
	{
		if (playerStatDict.TryGetValue(_key, out var stats))
			return stats;

		Debug.LogWarning($"[SkillManager] AttackStats not found for key: {_key}");
		return default;
	}

	public void Set_PlayerData(string _key, PlayerStruct _value)
	{
		if (playerStatDict.ContainsKey(_key))
		{
			playerStatDict[_key] = _value;
		}
	}

	public EnemyStruct Get_EnemyData(string _key)
	{
		if (enemyStatDict.TryGetValue(_key, out var stats))
			return stats;

		Debug.LogWarning($"[SkillManager] AttackStats not found for key: {_key}");
		return default;
	}

	public void InvokeAction_ChangePlayerStruct()
	{
		onChangePlayerStruct?.Invoke();
	}

	public void IncreaseAllEnemyStats(float percent)
	{
		var keys = new List<string>(enemyStatDict.Keys);

		foreach (var key in keys)
		{
			EnemyStruct s = enemyStatDict[key];

			s.maxHP *= (1f + percent);
			s.currentHP = s.maxHP;
			s.damage = Mathf.RoundToInt(s.damage * (1f + percent));

			enemyStatDict[key] = s;
		}
	}


}
