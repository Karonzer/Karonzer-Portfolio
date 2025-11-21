using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMoveController : MonoBehaviour
{

	[Header("기본 설정")]
	[SerializeField] private CharacterController characterController;
	[SerializeField] private InputSystem_Actions inputActions;
	[SerializeField] private GravityController playerGravityController;


	[Header("기본 변수 값")]
	[SerializeField] private Vector2 moveDirection;
	[SerializeField] private float turnSmoothTime = 0.1f;
	[SerializeField] private float turnSmoothVelocity;
	[SerializeField] private Camera cameraPos;
	[SerializeField] private Vector3 gravityDelta;
	[SerializeField] private Vector3 horizontalDelta;

	[SerializeField] private Animator animator;

	private Coroutine moveRoutine;



	private void Awake()
	{
		Initialize_PlayerMoveController();

		animator = transform.GetChild(0).GetComponent<Animator>();
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

	void Update()
	{
		if (GSC.Instance.gameManager != null && GSC.Instance.gameManager.IsPaused)
			return;

		gravityDelta = Vector3.zero;
		if (playerGravityController != null)
			gravityDelta = playerGravityController.GetGravityDelta(characterController);

		characterController.Move(horizontalDelta + gravityDelta);

		animator.SetFloat("HSpeed", moveDirection.x);
		animator.SetFloat("VSpeed", moveDirection.y);
		animator.SetBool("IsGrounded", playerGravityController.IsGrounded);
	}

	private void Initialize_PlayerMoveController()
	{
		characterController = GetComponent<CharacterController>();
		inputActions = new InputSystem_Actions();
		playerGravityController = transform.AddComponent<GravityController>();
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
		inputActions.Player.Jump.performed += OnJump;
	}	

	private void DiSetting_InputActionMove()
	{
		inputActions.Player.Move.performed -= OnMove;
		inputActions.Player.Move.canceled -= OnMoveCanceled;
		inputActions.Player.Jump.performed -= OnJump;
	}

	private void OnMove(InputAction.CallbackContext context)
	{
		moveDirection = context.ReadValue<Vector2>();
		if (moveRoutine == null)
		{
			moveRoutine = StartCoroutine(MoveCoroutine());
		}

	}

	private void OnMoveCanceled(InputAction.CallbackContext context)
	{
		moveDirection = Vector2.zero;
		horizontalDelta = Vector3.zero;
		if (moveRoutine != null)
		{
			StopCoroutine(moveRoutine);
			moveRoutine = null;
		}
	}

	public void OnJump(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			playerGravityController?.Jump();
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
			horizontalDelta = worldMove * GSC.Instance.statManager.Get_PlayerData(DBManager.playerName).moveSpeed * Time.deltaTime;


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
