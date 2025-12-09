using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BuffManager : MonoBehaviour
{
	private Dictionary<BuffType, Coroutine> activeBuffs;

	private void Awake()
	{
		BattleGSC.Instance.RegisterBuffManager(this);
		activeBuffs = new Dictionary<BuffType, Coroutine>();
	}

	private void OnEnable()
	{
		foreach (var key in activeBuffs.Values)
		{
			if (key != null)
			{
				StopCoroutine(key);
			}
		}
	}

	public void ApplyBuff(BuffData _data)
	{
		if (activeBuffs.ContainsKey(_data.type))
		{
			StopCoroutine(activeBuffs[_data.type]);
		}

		activeBuffs[_data.type] = StartCoroutine(BuffRoutine(_data));
	}

	private IEnumerator BuffRoutine(BuffData data)
	{

		Apply_Buff(data);

		float time = data.duration;

		while (time > 0f)
		{
			if (BattleGSC.Instance.gameManager != null &&
				BattleGSC.Instance.gameManager.isPaused)
			{
				yield return null;
				continue;
			}

			time -= Time.deltaTime;
			yield return null;
		}

		// 3) 버프 제거
		Remove_Buff(data);

		// 4) 리스트에서 제거
		activeBuffs.Remove(data.type);
	}

	private void Apply_Buff(BuffData data)
	{
		switch (data.type)
		{
			case BuffType.Player:
				BattleGSC.Instance.statManager.AddBuff(data);
				break;

			case BuffType.Skill:
				BattleGSC.Instance.skillManager.AddBuff(data);
				break;
		}
	}

	private void Remove_Buff(BuffData data)
	{
		switch (data.type)
		{
			case BuffType.Player:
				BattleGSC.Instance.statManager.RemoveBuff(data);
				break;

			case BuffType.Skill:
				BattleGSC.Instance.skillManager.RemoveBuff(data);
				break;
		}
	}

}
