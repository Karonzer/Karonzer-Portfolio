using UnityEngine;

/// <summary>
/// 맵에 존재하는 모든 XP 아이템을
/// 플레이어 쪽으로 끌어당기는 아이템.
/// </summary>
public class MagnetItem : Item
{

	protected override void Fuction_Event(GameObject _obj)
	{
		// 스폰 매니저로부터 아이템 부모 트랜스폼 획득
		if (BattleGSC.Instance.spawnManager.Get_ItemParents(out Transform _tra))
		{
			// 현재 맵에 존재하는 모든 XP 아이템 탐색
			var value = _tra.GetComponentsInChildren<XPitem>();
			foreach (var item in value) 
			{
				// 강제 추적 시작
				item.Start_FollowToPlayer();
			}
		}


	}
}
