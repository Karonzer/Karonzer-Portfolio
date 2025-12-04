using UnityEngine;

public class XPitem : Item
{
	private int xpAmount;
	protected override void Start()
	{
		base.Start();
	}

	public void SetXP(int amount)
	{
		xpAmount = amount;
	}

	protected override void Fuction_Event(GameObject _obj)
	{
		if (_obj.TryGetComponent<PlayerLevel>(out var levelSystem))
		{
			levelSystem.AddXP(xpAmount);
		}
	}
}
