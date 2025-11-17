using System.Collections.Generic;
using UnityEngine;

public class StatManager : MonoBehaviour
{
	public List<EnemyDataSO> enemyStatList = new List<EnemyDataSO>();
	private Dictionary<string, EnemyStruct> enemyStatDict;

	private void Awake()
	{
		GSC.Instance.RegisterStatManager(this);
		Build_Dict();

	}

	private void Start()
	{
		
	}

	void Build_Dict()
	{
		enemyStatDict = new Dictionary<string, EnemyStruct>();
		foreach (var s in enemyStatList)
		{
			if (s != null && !string.IsNullOrEmpty(s.enemyStruct.key))
			enemyStatDict[s.enemyStruct.key] = s.enemyStruct;
			Debug.Log(s);
			Debug.Log(s.enemyStruct.key);
		}
	}

	public EnemyStruct Get_EnemyData(string _key)
	{
		if (enemyStatDict.TryGetValue(_key, out var stats))
			return stats;

		Debug.LogWarning($"[SkillManager] AttackStats not found for key: {_key}");
		return null;
	}

}
