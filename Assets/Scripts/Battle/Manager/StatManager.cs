using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// StatManager
/// 
/// 플레이어와 몬스터,보스의 스탯를 관리하는 매니저
/// - 기본 스탯OS 목록을 받아서 각 Dictionary로 변환하여 관리
/// - 버프를 더한 최종 플레이어 스탯 제공
/// - 몬스터 전체 난이도 상승 처리
/// </summary>
public class StatManager : MonoBehaviour
{
	// 플레이어 기본 스텟 목록
	public List<PlayerDataSO> playerStatList = new List<PlayerDataSO>();
	private Dictionary<string, PlayerStruct> playerStatDict;

	// 플레이어에게 현재 적용 중인 버프 스탯
	[SerializeField] private PlayerStruct currentPlayerBuffStat = new PlayerStruct();

	// 몬스터 기본 스텟 목록
	public List<EnemyDataSO> enemyStatList = new List<EnemyDataSO>();
	private Dictionary<string, EnemyStruct> enemyStatDict;

	//플레이어 스탯이 바뀌었을때 알리는 이벤트
	public event Action onChangePlayerStruct;

	/// <summary>
	/// - BattleGSC에 자신을 등록하여 전역 접근 가능하도록 한다
	/// - 기본 스탯OS 목록을 받아서 각 Dictionary로 변환하여 관리
	/// </summary>
	private void Awake()
	{
		BattleGSC.Instance.RegisterStatManager(this);
		Build_Dict();

	}
	/// <summary>
	/// - 기본 스탯OS 목록을 받아서 각 Dictionary로 변환하여 관리
	/// </summary>
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

	/// <summary>
	/// 플레이어의 최종 스탯을 반환하는 함수
	/// - 기본 스탯에 현재 적용 중인 버프 스탯을 더해서 반환
	/// </summary>
	public PlayerStruct Get_PlayerData(string _key)
	{
		if (!playerStatDict.TryGetValue(_key, out var baseStats))
			return default;

		PlayerStruct result = baseStats;

		// 버프가 들어가는 구조
		result.moveSpeed += currentPlayerBuffStat.moveSpeed;
		result.maxHP += currentPlayerBuffStat.maxHP;
		result.criticalChance += currentPlayerBuffStat.criticalChance;
		result.criticalChance += currentPlayerBuffStat.criticalChance;
		return result;
	}

	/// <summary>
	/// 플레이어의 기본 스탯을 변경하는 함수
	/// - 업그레이드 등으로 기본 스탯을 변경할때 사용
	/// </summary>
	public void Set_PlayerData(string _key, PlayerStruct _value)
	{
		if (playerStatDict.ContainsKey(_key))
		{
			playerStatDict[_key] = _value;
		}
	}

	/// <summary>
	/// 몬스터의 기본 스탯을 반환하는 함수
	/// - key에 해당하는 몬스터의 기본 스탯을 반환
	/// </summary>
	public EnemyStruct Get_EnemyData(string _key)
	{
		if (enemyStatDict.TryGetValue(_key, out var stats))
			return stats;

		Debug.LogWarning($"[SkillManager] AttackStats not found for key: {_key}");
		return default;
	}

	/// <summary>
	/// 플레이어 스탯이 바뀌었음을 알림
	/// </summary>
	public void InvokeAction_ChangePlayerStruct()
	{
		onChangePlayerStruct?.Invoke();
	}

	/// <summary>
	/// 모든 몬스터의 스탯을 비율만큼 증가시키는 함수
	/// - 난이도 상승 등에 사용
	/// </summary>
	public void IncreaseAllEnemyStats(float percent)
	{
		var keys = new List<string>(enemyStatDict.Keys);

		foreach (var key in keys)
		{
			EnemyStruct s = enemyStatDict[key];

			s.maxHP *= (1f + percent);
			s.currentHP = s.maxHP;
			s.rawDamage = Mathf.RoundToInt(s.damage * (1f + percent));

			enemyStatDict[key] = s;
		}
	}


	/// <summary>
	/// 플레이어 버프 적용
	/// (현재는 이동속도 % 버프만 처리)
	/// </summary>
	public void AddBuff(BuffData _data)
	{
		switch(_data.effectType)
		{
			case BuffEffectType.MoveSpeedPercent:
				{
					var baseStats = playerStatDict[BattleGSC.Instance.gameManager.CurrentPlayerKey];
					float addValue = baseStats.moveSpeed * (_data.value / 100f);
					currentPlayerBuffStat.moveSpeed += addValue;
					break;
				}
		}
		InvokeAction_ChangePlayerStruct();
	}

	/// <summary>
	/// 플레이어 버프 제거
	/// </summary>
	public void RemoveBuff(BuffData _data)
	{
		switch (_data.effectType)
		{
			case BuffEffectType.MoveSpeedPercent:
				currentPlayerBuffStat.moveSpeed = 0;
				break;
		}
		InvokeAction_ChangePlayerStruct();
	}


}
