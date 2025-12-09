using UnityEngine;

public class BuffItem : Item
{
	[SerializeField] private BuffDataSO dataSO;

	protected override void Start()
	{
		base.Start();
	}
	protected override void Fuction_Event(GameObject _obj)
	{
		if (_obj.TryGetComponent<Player>(out var _player))
		{
			BattleGSC.Instance.BuffManager.ApplyBuff(dataSO.buffData);
			HandleText();
		}
	}

	private void HandleText()
	{
		if (BattleGSC.Instance.damagePopupManager == null)
			return;
		Vector3 pos = transform.position;
		pos.y += 1;
		BattleGSC.Instance.damagePopupManager.Show_Text(dataSO.buffData.description, pos, Type.Player);
	}
}
