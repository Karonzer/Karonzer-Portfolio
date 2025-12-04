using UnityEngine;

public class HealingItem : Item
{
	private int amount = 10;

	protected override void Start()
	{
		base.Start();
	}
	protected override void Fuction_Event(GameObject _obj)
	{
		if (_obj.TryGetComponent<Player>(out var _player))
		{
			_player.Healing_CurrentHP(amount);
		}
	}
}
