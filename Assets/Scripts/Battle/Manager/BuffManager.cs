using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BuffManager
/// 
/// 게임에서 발생하는 버프를 관리하는 매니저.
/// - 버프 적용
/// - 버프 지속 시간 관리
/// - 시간이 끝나면 자동으로 제거
/// 
/// Player 버프 / Skill 버프를 구분해서
/// 각각 StatManager, SkillManager로 위임한다.
/// </summary>
public class BuffManager : MonoBehaviour
{
	// 현재 적용 중인 버프 목록
	// BuffType 별로 하나의 버프만 적용 가능
	private Dictionary<BuffType, Coroutine> activeBuffs;

	/// <summary>
	/// - BattleGSC에 자신을 등록하여 전역 접근 가능하도록 한다
	/// - 활성 버프 테이블 초기화
	/// </summary>
	private void Awake()
	{
		BattleGSC.Instance.RegisterBuffManager(this);
		activeBuffs = new Dictionary<BuffType, Coroutine>();
	}

	/// <summary>
	/// - 오브젝트가 다시 활성화될 때 남아 있던 버프 코루틴이 있으면 모두 정지
	/// </summary>
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

	/// <summary>
	/// 버프 적용 함수
	/// - 같은 BuffType이 이미 있으면 기존 버프를 중단
	/// - 새 버프 코루틴을 시작
	/// </summary>
	public void ApplyBuff(BuffData _data)
	{
		if (activeBuffs.ContainsKey(_data.type))
		{
			StopCoroutine(activeBuffs[_data.type]);
		}

		activeBuffs[_data.type] = StartCoroutine(BuffRoutine(_data));
	}

	/// <summary>
	/// 버프의 전체 생명주기 관리 코루틴
	/// 1. 버프 적용
	/// 2. 지속 시간 동안 대기
	/// 3. 버프 제거
	/// 4. 활성 목록에서 제거
	/// </summary>
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

	/// <summary>
	/// 실제 버프 효과 적용
	/// - Player 버프 → StatManager
	/// - Skill 버프 → SkillManager
	/// </summary>
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

	/// <summary>
	/// 버프 효과 제거
	/// - 적용 시 사용했던 매니저로 다시 위임
	/// </summary>
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
