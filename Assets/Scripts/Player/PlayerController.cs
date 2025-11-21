using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : Player
{
	PlayerMoveController moveController;
	protected override void Start()
	{
		playerName = DBManager.playerName;
		base.Start();
		if(moveController == null)
		{
			moveController = transform.AddComponent<PlayerMoveController>();
		}

		Add_AttackObject(DBManager.fireballAttack);
	}
}
