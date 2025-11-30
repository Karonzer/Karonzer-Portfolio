using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : Player
{
	PlayerMoveController moveController;
	protected override void Awake()
	{
		base.Awake();
		if (meshRenderer != null)
			hitMatInstance = meshRenderer.material;
	}
	protected override void Start()
	{
		base.Start();
		if(moveController == null)
		{
			moveController = transform.AddComponent<PlayerMoveController>();
		}

		Add_AttackObject(StartAttackKey);
	}
}
