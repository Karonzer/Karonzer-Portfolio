using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SkillManager
/// 
/// 스킬(공격) 스탯 데이트를 관리하는 매니저
/// - AttackStatsSO 목록을 받아서 Dictionary로 변환하여 관리
/// - 특정 스킬 스탯이 변경되었을 해당 스킬 스크립트에 알림 이벤트 제공
/// </summary>
public class SkillManager : MonoBehaviour
{
	/// <summary>
	/// 인스펙터에서 등록하는 스킬/공격 스탯 SO 목록
	/// 각 SO 내부 attackStats.key를 기준으로 Dictionary에 추가
	/// </summary>
	public List<AttackStatsSO> attackStatsList = new List<AttackStatsSO>();
	private Dictionary<string, AttackStats> statsDict;

	// 각 스킬별로 플레이어가 얻은 버프 스탯을 저장하는 딕셔너리
	private Dictionary<string, AttackStats> currentPlayerBuffstatsDict;

	// 공통 버프 값 변수
	[SerializeField] private float globalSkillDamageBuff;
	[SerializeField] private float globalSkillAttackSpeedBuff;

	// 특정 스킬 스탯이 바뀔 때 알려주는 이벤트
	public Dictionary<string, Action<string>> eventTableOnAttackStatsChanged = new Dictionary<string, Action<string>>();
	//현재 보유하고 있는 공격 방식
	public Dictionary<string, AttackRoot> currentAttacks = new Dictionary<string, AttackRoot>();

	/// <summary>
	/// - BattleGSC에 자신을 등록하여 전역 접근 가능하도록 한다
	/// - AttackStatsSO 목록을 받아서 Dictionary로 변환하여 관리
	/// </summary>
	private void Awake()
	{
		BattleGSC.Instance.RegisterSkillManager(this);
		Build_Dict();

	}

	// 버프 값 초기화
	private void OnEnable()
	{
		globalSkillDamageBuff = 0;
		globalSkillAttackSpeedBuff = 0;
	}

	private void Reset()
	{
		globalSkillDamageBuff = 0;
		globalSkillAttackSpeedBuff = 0;
	}

	/// <summary>
	/// - AttackStatsSO 목록을 받아서 Dictionary로 변환하여 관리
	/// </summary>
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

	/// <summary>
	/// 특정 스킬의 최종 스탯을 반환하는 함수
	/// - 기본 스탯에 플레이어가 얻은 버프 스탯을 합산하여 반환
	/// </summary>
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

	/// <summary>
	/// 특정 스킬의 기본 스탯을 설정하는 함수
	/// - 업	그레이드 등으로 스탯이 변경될 때 사용
	/// </summary>
	public void Set_Stats(string _key, AttackStats _value)
	{
		if (statsDict.ContainsKey(_key))
		{
			statsDict[_key] = _value;
		}
	}

	/// <summary>
	/// 현재 보유하고 있는 공격 방식을 추가하는 함수
	/// - 외부에서 공격 방식을 추가할 때 사용
	/// </summary>
	public void Add_CurrentAttacks(string _key, AttackRoot _attackRoot)
	{
		if(statsDict.ContainsKey(_key))
		{
			currentAttacks[_attackRoot.AttackKey] = _attackRoot;
		}
	}

	/// <summary>
	/// 외부에서 “이 스킬 스탯 바뀌었다”고 알리고 싶을 때 쓰는 헬퍼
	/// - 업그레이드 등으로 스탯이 변경될 때 사용
	/// </summary>
	public void NotifyStatsChanged(string _key)
    {
		Invoke_Action(_key);
    }

	/// <summary>
	/// 공격 오브젝트 스크립트에서 스탯 변경 이벤트를 구독할 때 사용하는 함수
	/// </summary>
	public void AddListener(string _key, Action<string> callback)
	{
		if (!eventTableOnAttackStatsChanged.ContainsKey(_key))
			eventTableOnAttackStatsChanged[_key] = null;

		eventTableOnAttackStatsChanged[_key] += callback;
	}

	/// <summary>
	/// 공격 오브젝트 스크립트에서 스탯 변경 이벤트를 해제할때 사용하는 함수
	/// </summary>
	public void RemoveListener(string _key, Action<string> _callback)
	{
		if (!eventTableOnAttackStatsChanged.ContainsKey(_key))
			return;

		eventTableOnAttackStatsChanged[_key] -= _callback;
	}


	/// <summary>
	/// 공격 오브젝트 스크립트에서 스탯 변경 이벤트를 호출할때 사용하는 함수
	/// </summary>
	public void Invoke_Action(string _key)
	{
		if (eventTableOnAttackStatsChanged.TryGetValue(_key, out var _action))
		{
			_action?.Invoke(_key);
		}

	}

	/// <summary>
	/// 스킬 관련 버프를 적용하는 함수
	/// - DamagePercent: globalSkillDamageBuff에 value(%)를 누적
	/// - AttackSpeedPercent: globalSkillAttackSpeedBuff에 value(%)를 누적
	/// </summary>
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

	/// <summary>
	/// 스킬 관련 적용된 버프를 제거하는 함수
	/// </summary>
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
