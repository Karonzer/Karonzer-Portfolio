using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Player
{
	[SerializeField] private InputSystem_Actions inputActions;

	[SerializeField] private PlayerMoveController moveController;
	[SerializeField] private PlayerInteractor interactor;
	[SerializeField] private PlayerCinemachineFreeLook playerCinemachineFreeLook;

	protected override void Awake()
	{
		base.Awake();
		inputActions = new InputSystem_Actions();
		if (moveController == null)
		{
			moveController = transform.AddComponent<PlayerMoveController>();
		}

		if (interactor == null)
		{
			interactor = transform.AddComponent<PlayerInteractor>();
		}
		playerCinemachineFreeLook = transform.GetComponentInChildren<PlayerCinemachineFreeLook>();
		if (meshRenderer != null)
			hitMatInstance = meshRenderer.material;
	}


	private void OnEnable()
	{
		inputActions.Enable();
		Setting_InputActionMove();
	}

	private void OnDisable()
	{
		inputActions.Disable();
		DiSetting_InputActionMove();
	}
	protected override void Start()
	{
		base.Start();
		Add_AttackObject(StartAttackKey);
	}

	protected override void Event_ChildEvnet()
	{
		playerCinemachineFreeLook.Shake(1, 1);
	}

	private void Setting_InputActionMove()
	{
		inputActions.Player.Move.performed += moveController.OnMove;
		inputActions.Player.Move.canceled += moveController.OnMoveCanceled;
		inputActions.Player.Jump.performed += moveController.OnJump;
		inputActions.Player.Interact.performed += interactor.OnInteract;
	}

	private void DiSetting_InputActionMove()
	{
		inputActions.Player.Move.performed -= moveController.OnMove;
		inputActions.Player.Move.canceled -= moveController.OnMoveCanceled;
		inputActions.Player.Jump.performed -= moveController.OnJump;
		inputActions.Player.Interact.performed -= interactor.OnInteract;
	}
}
