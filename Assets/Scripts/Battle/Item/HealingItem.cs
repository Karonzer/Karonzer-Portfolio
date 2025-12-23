using UnityEngine;

/// <summary>
/// 플레이어 체력을 회복시키는 아이템.
/// </summary>
public class HealingItem : Item
{
	// 회복량
	private int amount = 10;

	protected override void Start()
	{
		base.Start();
	}

	/// <summary>
	/// 플레이어 획득 시 체력 회복
	/// </summary>
	protected override void Fuction_Event(GameObject _obj)
	{
		if (_obj.TryGetComponent<Player>(out var _player))
		{
			_player.Healing_CurrentHP(amount);
		}
	}
}
