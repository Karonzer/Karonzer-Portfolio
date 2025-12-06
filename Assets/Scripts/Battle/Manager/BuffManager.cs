using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using NUnit.Framework.Constraints;
public class BuffManager : MonoBehaviour
{
	private Dictionary<BuffType, Coroutine> activeBuffs;

	private void Awake()
	{
		activeBuffs = new Dictionary<BuffType, Coroutine>();
	}

	//private void OnEnable()
	//{
	//	foreach (var b in activeBuffs.Values)
	//	{
	//		if(b != null)
	//		{
	//			StopCoroutine(b);
	//			b = null;
	//		}
	//	}
	//}

	//public void ApplyBuff(BuffData data)
	//{
	//	if (activeBuffs.ContainsKey(data.type))
	//	{
	//		StopCoroutine(activeBuffs[data.type]);
	//	}

	//	activeBuffs[data.type] = StartCoroutine(BuffRoutine(data));
	//}

	//private IEnumerator BuffRoutine(BuffData data)
	//{
	//	// 버프 적용
	//	BattleGSC.Instance.statManager.AddBuff(data);

	//	yield return new WaitForSeconds(data.duration);

	//	// 버프 제거
	//	BattleGSC.Instance.statManager.RemoveBuff(data);

	//	activeBuffs.Remove(data.type);
	//}
}
