using UnityEngine;

/// <summary>
/// 플레이어에게 버프를 적용하는 아이템.
/// 
/// 예:
/// - 이동 속도 증가
/// - 공격력 증가
/// - 쿨타임 감소
/// </summary>
public class BuffItem : Item
{
	// 적용할 버프 데이터
	[SerializeField] private BuffDataSO dataSO;

	protected override void Start()
	{
		base.Start();
	}
	/// <summary>
	/// 플레이어 획득 시 버프 적용
	/// </summary>
	protected override void Fuction_Event(GameObject _obj)
	{
		if (_obj.TryGetComponent<Player>(out var _player))
		{
			// 버프 매니저를 통해 버프 적용
			BattleGSC.Instance.BuffManager.ApplyBuff(dataSO.buffData);
			// 버프 설명 텍스트 출력
			HandleText();
		}
	}

	/// <summary>
	/// 버프 획득 시 설명 텍스트 팝업 표시
	/// </summary>
	private void HandleText()
	{
		if (BattleGSC.Instance.damagePopupManager == null)
			return;
		Vector3 pos = transform.position;
		pos.y += 1;
		BattleGSC.Instance.damagePopupManager.Show_Text(dataSO.buffData.description, pos, Type.Player);
	}
}
