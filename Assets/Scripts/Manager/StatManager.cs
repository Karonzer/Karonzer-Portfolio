using System.Collections.Generic;
using UnityEngine;

public class StatManager : MonoBehaviour
{
	public List<EnemyData> enemyStatList = new List<EnemyData>();
	private Dictionary<string, EnemyData> enemyStatDict;

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
		enemyStatDict = new Dictionary<string, EnemyData>();
		foreach (var s in enemyStatList)
		{
			if (s != null && !string.IsNullOrEmpty(s.key))
			enemyStatDict[s.key] = s;
			Debug.Log(s);
			Debug.Log(s.key);
		}
	}

	public EnemyData Get_EnemyData(string _key)
	{
		if (enemyStatDict.TryGetValue(_key, out var stats))
			return stats;

		Debug.LogWarning($"[SkillManager] AttackStats not found for key: {_key}");
		return null;
	}

}
