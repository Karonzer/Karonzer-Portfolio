using UnityEngine;

public class MagnetItem : Item
{

	protected override void Fuction_Event(GameObject _obj)
	{

		if(BattleGSC.Instance.spawnManager.Get_ItemParents(out Transform _tra))
		{
			var value = _tra.GetComponentsInChildren<XPitem>();
			foreach (var item in value) 
			{
				item.Start_FollowToPlayer();
			}
		}


	}
}
