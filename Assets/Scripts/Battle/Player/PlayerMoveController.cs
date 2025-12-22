using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

/// <summary>
/// PlayerMoveController
/// 
/// 플레이어 이동/회전/점프 입력을 처리하는 컴포넌트.
/// - 입력(Vector2)을 카메라 기준 월드 방향으로 변환
/// - CharacterController.Move로 이동 적용
/// - GravityController로 중력/점프 처리
/// - Animator 파라미터 갱신(이동/접지/점프)
/// </summary>
public class PlayerMoveController : MonoBehaviour
{

	[Header("기본 설정")]
	// 캐릭터 이동에 사용할 CharacterController
	[SerializeField] private CharacterController characterController;
	// 중력/점프를 담당하는 컨트롤러(별도 컴포넌트)
	[SerializeField] private GravityController playerGravityController;


	[Header("기본 변수 값")]
	// 입력으로 받은 이동 방향(좌/우, 앞/뒤)
	[SerializeField] private Vector2 moveDirection;

	// 회전 스무딩(부드럽게 돌아가게 함)
	[SerializeField] private float turnSmoothTime = 0.1f;
	[SerializeField] private float turnSmoothVelocity;

	// 카메라 방향 기준으로 이동하기 위해 참조하는 카메라
	[SerializeField] private Camera cameraPos;

	// 중력 이동량(프레임마다 계산)
	[SerializeField] private Vector3 gravityDelta;
	// 수평 이동량(프레임마다 계산)
	[SerializeField] private Vector3 horizontalDelta;

	// 이동 애니메이션 제어용 Animator
	[SerializeField] private Animator animator;

	// 이동 계산을 반복해서 하는 코루틴(입력이 들어오는 동안 계속)
	private Coroutine moveRoutine;

	// 점프 이벤트(사운드 등 연결)
	public event Action onJump;

	private void Awake()
	{
		// 필요한 컴포넌트들 세팅
		Initialize_PlayerMoveController();

		// 모델(자식 0번)에 Animator가 있다고 가정하고 가져옴
		animator = transform.GetChild(0).GetComponent<Animator>();
	}

	private void OnEnable()
	{
		// 활성화될 때 이동 입력/상태 초기화
		Setting_PlayerMoveController();
	}

	private void OnDisable()
	{
	}

	void Update()
	{
		// 게임이 Pause면 이동 업데이트 중단
		if (BattleGSC.Instance.gameManager != null && BattleGSC.Instance.gameManager.isPaused)
			return;

		// 중력 이동량 계산
		gravityDelta = Vector3.zero;
		if (playerGravityController != null)
			// 수평 이동(horizontalDelta) + 중력(gravityDelta)을 합쳐 이동 적용
			gravityDelta = playerGravityController.GetGravityDelta(characterController);

		characterController.Move(horizontalDelta + gravityDelta);


		animator.SetFloat("HSpeed", moveDirection.x);
		animator.SetFloat("VSpeed", moveDirection.y);
		animator.SetBool("IsGrounded", playerGravityController.IsGrounded);
	}

	private void FixedUpdate()
	{
		
	}

	/// <summary>
	/// 필수 컴포넌트 초기 세팅
	/// - CharacterController 확보
	/// - GravityController 추가
	/// - 메인 카메라 참조
	/// </summary>
	private void Initialize_PlayerMoveController()
	{
		characterController = GetComponent<CharacterController>();
		playerGravityController = transform.AddComponent<GravityController>();
		cameraPos = Camera.main;
	}


	/// <summary>
	/// 이동 컨트롤러 초기화
	/// - 입력 방향 초기화
	/// - 기존 이동 코루틴이 있다면 종료
	/// </summary>
	private void Setting_PlayerMoveController()
	{
		moveDirection = Vector2.zero;
		if (moveRoutine != null)
		{
			StopCoroutine(moveRoutine);
			moveRoutine = null;
		}
	}

	/// <summary>
	/// 이동 입력이 들어왔을 때 호출됨(performed)
	/// - moveDirection 업데이트
	/// - MoveCoroutine이 없으면 시작
	/// </summary>
	public void OnMove(InputAction.CallbackContext context)
	{
		moveDirection = context.ReadValue<Vector2>();

		if (moveRoutine != null)
		{
			StopCoroutine(moveRoutine);
			moveRoutine = null;
		}

		if (moveRoutine == null)
		{
			moveRoutine = StartCoroutine(MoveCoroutine());
		}

	}

	/// <summary>
	/// 이동 입력이 끊겼을 때 호출됨(canceled)
	/// - 이동 입력 제거
	/// - 수평 이동량 초기화
	/// - 이동 코루틴 종료
	/// </summary>
	public void OnMoveCanceled(InputAction.CallbackContext context)
	{
		moveDirection = Vector2.zero;
		horizontalDelta = Vector3.zero;
		if (moveRoutine != null)
		{
			StopCoroutine(moveRoutine);
			moveRoutine = null;
		}
	}

	/// <summary>
	/// 점프 입력 처리
	/// - Jump()가 성공하면:
	///   - onJump 이벤트 호출(점프 사운드 등)
	///   - 애니메이터 Jump 트리거 실행
	/// </summary>
	public void OnJump(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			if(playerGravityController.Jump())
			{
				onJump?.Invoke();

				animator.SetTrigger("Jump");
			}
		}
	}


	/// <summary>
	/// 이동 계산 코루틴
	/// - 카메라의 forward/right를 이용해서 "카메라 기준 이동"을 만든다
	/// - 플레이어 moveSpeed를 읽어서 이동 속도를 적용한다
	/// - 이동 방향이 있으면 그 방향으로 캐릭터 회전도 수행한다
	/// 
	/// - horizontalDelta 값이 매 프레임 갱신되고,
	/// - Update()에서 CharacterController.Move(horizontalDelta + gravityDelta)로 사용된다
	/// </summary>
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
			horizontalDelta = worldMove * BattleGSC.Instance.statManager.Get_PlayerData(BattleGSC.Instance.gameManager.CurrentPlayerKey).moveSpeed * Time.deltaTime;


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
