using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
	[Header("전체 카드 리스트")]
	public List<UpgradeOptionSO> allOptions = new List<UpgradeOptionSO>();
	private Dictionary<string, UpgradeOptionSO> optionsDict;
	private void Awake()
	{
		GSC.Instance.RegisterUpgradeManager(this);
		Build_Dict();
	}

	void Build_Dict()
	{
		optionsDict = new Dictionary<string, UpgradeOptionSO>();
		foreach (var s in allOptions)
		{
			if (s != null && !string.IsNullOrEmpty(s.numKey))
				optionsDict[s.numKey] = s;
		}
	}

	public List<UpgradeOptionSO> Get_RandomOptions(int count)
	{
		List<UpgradeOptionSO> candidates = new List<UpgradeOptionSO>();

		var skillMgr = GSC.Instance.skillManager;

		foreach (var option in optionsDict)
		{
			bool hasSkill = skillMgr.currentAttacks.ContainsKey(option.Value.targetKey);

			if (option.Value.optionType == UpgradeOptionType.SkillUpgrade)
			{
				if (hasSkill)
					candidates.Add(option.Value);
			}
			else if (option.Value.optionType == UpgradeOptionType.SkillUnlock)
			{
				if (!hasSkill)
					candidates.Add(option.Value);
			}

			candidates.Add(option.Value);
		}

		// 랜덤 셔플
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

	public void Select_Upgrade(UpgradeOptionSO _option)
	{
		if (_option == null) return;

		if(_option.optionType == UpgradeOptionType.SkillUnlock)
		{
			optionsDict.Remove(_option.numKey);
		}
		UpgradeApplier.Apply_Upgrade(_option);
	}

}
