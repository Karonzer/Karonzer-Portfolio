using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
public class PlayerMoveController : MonoBehaviour
{
	[SerializeField] private CharacterController characterController;
	[SerializeField] private InputSystem_Actions inputActions;
	[SerializeField] private PlayerData playerData;
	[SerializeField] private Vector2 moveDirection;
	[SerializeField] private float turnSmoothTime = 0.1f;
	[SerializeField] private float turnSmoothVelocity;
	[SerializeField] private Camera cameraPos;
	private Coroutine moveRoutine;

	private void Awake()
	{
		Initialize_PlayerMoveController();
	}

	private void OnEnable()
	{
		inputActions.Enable();
		Setting_InputActionMove();
		Setting_PlayerMoveController();
	}

	private void OnDisable()
	{
		inputActions.Disable();
		DiSetting_InputActionMove();
	}

	private void Initialize_PlayerMoveController()
	{
		characterController = GetComponent<CharacterController>();
		inputActions = new InputSystem_Actions();
		playerData = new PlayerData();

		cameraPos = Camera.main;
	}

	private void Setting_PlayerMoveController()
	{
		moveDirection = Vector2.zero;
		if (moveRoutine != null)
		{
			StopCoroutine(moveRoutine);
			moveRoutine = null;
		}
	}


	private void Setting_InputActionMove()
	{

		inputActions.Player.Move.performed += OnMove;
		inputActions.Player.Move.canceled += OnMoveCanceled;
	}	

	private void DiSetting_InputActionMove()
	{
		inputActions.Player.Move.performed -= OnMove;
		inputActions.Player.Move.canceled -= OnMoveCanceled;
	}
	private void OnMove(InputAction.CallbackContext context)
	{
		moveDirection = context.ReadValue<Vector2>();
		Debug.Log($"Move: {moveDirection}");
		if (moveRoutine == null)
		{
			moveRoutine = StartCoroutine(MoveCoroutine());
		}

	}
	private void OnMoveCanceled(InputAction.CallbackContext context)
	{
		moveDirection = Vector2.zero;

		if (moveRoutine != null)
		{
			StopCoroutine(moveRoutine);
			moveRoutine = null;
		}
	}

	private IEnumerator MoveCoroutine()
	{
		while (true)
		{
			Vector3 camForward =  cameraPos.transform.forward;
			Vector3 camRight =  cameraPos.transform.right;

			camForward.y = 0f;
			camRight.y = 0f;
			camForward.Normalize();
			camRight.Normalize();

			Vector3 worldMove = camRight * moveDirection.x + camForward * moveDirection.y;
			if (worldMove.sqrMagnitude > 1f) worldMove.Normalize();
			characterController.Move(worldMove * playerData.moveSpeed * Time.deltaTime);

			if (worldMove.sqrMagnitude > 0.001f)
			{
				float targetAngle = Mathf.Atan2(worldMove.x, worldMove.z) * Mathf.Rad2Deg;
				float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
				transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);
			}

			yield return null;
		}
	}


}
