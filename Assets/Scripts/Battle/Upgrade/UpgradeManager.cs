using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 업그레이드 카드(선택지)를 관리하는 매니저.
/// 
/// 책임:
/// - 전체 업그레이드 옵션 보관
/// - 현재 상태에 맞는 업그레이드 후보 필터링
/// - 랜덤 업그레이드 카드 제공
/// - 선택된 업그레이드를 UpgradeApplier로 전달
/// </summary>
public class UpgradeManager : MonoBehaviour
{
	[Header("전체 카드 리스트")]
	// 인스펙터에서 세팅되는 모든 업그레이드 옵션
	public List<UpgradeOptionSO> allOptions = new List<UpgradeOptionSO>();
	private Dictionary<string, UpgradeOptionSO> optionsDict;
	private void Awake()
	{
		// 전역 접근 등록
		BattleGSC.Instance.RegisterUpgradeManager(this);
		// 옵션 딕셔너리 구성
		Build_Dict();
	}

	/// <summary>
	/// 업그레이드 옵션을 numKey 기준으로 딕셔너리화
	/// </summary>
	void Build_Dict()
	{
		optionsDict = new Dictionary<string, UpgradeOptionSO>();
		foreach (var s in allOptions)
		{
			if (s != null && !string.IsNullOrEmpty(s.numKey))
				optionsDict[s.numKey] = s;
		}
	}

	/// <summary>
	/// 현재 상태에 맞는 업그레이드 옵션 중 랜덤 선택
	/// </summary>
	public List<UpgradeOptionSO> Get_RandomOptions(int count)
	{
		var candidates = new List<UpgradeOptionSO>();
		var skillMgr = BattleGSC.Instance.skillManager;

		foreach (var option in optionsDict.Values)
		{
			bool hasSkill = skillMgr.currentAttacks.ContainsKey(option.targetKey);

			switch (option.optionType)
			{
				case UpgradeOptionType.SkillUnlock:
					// 스킬이 없을 때만 Unlock 카드 등장
					if (!hasSkill)
						candidates.Add(option);
					break;

				case UpgradeOptionType.SkillUpgrade:
					// 스킬을 가지고 있을 때만 Upgrade 카드 등장
					if (hasSkill)
						candidates.Add(option);
					break;

				default:
					// 나머지 옵션은 무조건 포함
					candidates.Add(option);
					break;
			}
		}

		// 후보군에서 랜덤으로 count개 추출
		List<UpgradeOptionSO> result = new List<UpgradeOptionSO>();
		for (int i = 0; i < count; i++)
		{
			if (candidates.Count == 0) break;

			int idx = Random.Range(0, candidates.Count);
			result.Add(candidates[idx]);
			candidates.RemoveAt(idx);
		}

		return result;
	}

	/// <summary>
	/// 플레이어가 업그레이드 카드를 선택했을 때 호출
	/// </summary>
	public void Select_Upgrade(UpgradeOptionSO _option)
	{
		if (_option == null) return;

		// 스킬 해금 카드라면, 다시 등장하지 않도록 제거
		if (_option.optionType == UpgradeOptionType.SkillUnlock)
		{
			optionsDict.Remove(_option.numKey);
		}
		// 실제 적용 로직은 Applier에게 위임
		UpgradeApplier.Apply_Upgrade(_option);
	}

}
