using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : Player
{
	[SerializeField] private PlayerMoveController moveController;
	[SerializeField] private PlayerInteractor interactor;
	[SerializeField] private PlayerCinemachineFreeLook playerCinemachineFreeLook;
	[SerializeField] private PlayerAudioController audioController;

	protected override void Awake()
	{
		base.Awake();
		if (moveController == null)
		{
			moveController = transform.AddComponent<PlayerMoveController>();
		}

		if (interactor == null)
		{
			interactor = transform.AddComponent<PlayerInteractor>();
		}
		playerCinemachineFreeLook = transform.GetComponentInChildren<PlayerCinemachineFreeLook>();

		audioController = transform.GetComponent<PlayerAudioController>();

		if (meshRenderer != null)
			hitMatInstance = meshRenderer.material;
	}



	private void OnEnable()
	{
		Setting_Action();
	}

	private void OnDisable()
	{
		DiSetting_InputActionMove();
	}
	protected override void Start()
	{
		base.Start();
		Add_AttackObject(StartAttackKey);
		Setting_InputActionMove();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		DiSetting_InputActionMove();
	}

	protected override void Event_ChildEvnet()
	{
		playerCinemachineFreeLook.Shake(1, 1);
	}

	private void Setting_InputActionMove()
	{
		BattleGSC.Instance.inputManager.InputActions.Player.Move.performed += moveController.OnMove;
		BattleGSC.Instance.inputManager.InputActions.Player.Move.canceled += moveController.OnMoveCanceled;
		BattleGSC.Instance.inputManager.InputActions.Player.Jump.performed += moveController.OnJump;
		BattleGSC.Instance.inputManager.InputActions.Player.Interact.performed += interactor.OnInteract;
	}

	private void DiSetting_InputActionMove()
	{
		BattleGSC.Instance.inputManager.InputActions.Player.Move.performed -= moveController.OnMove;
		BattleGSC.Instance.inputManager.InputActions.Player.Move.canceled -= moveController.OnMoveCanceled;
		BattleGSC.Instance.inputManager.InputActions.Player.Jump.performed -= moveController.OnJump;
		BattleGSC.Instance.inputManager.InputActions.Player.Interact.performed -= interactor.OnInteract;
	}

	private void Setting_Action()
	{
		moveController.onJump += audioController.Play_Jump;
	}

}
