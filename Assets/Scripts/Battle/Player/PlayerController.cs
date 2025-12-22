using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// PlayerController
/// 
/// Player(베이스 클래스)를 상속받아, 조작 가능한 플레이어를 만드는 클래스.
/// - 이동/점프/상호작용 입력을 각 컴포넌트에 연결
/// - 시작 스킬(AttackRoot)을 장착
/// - 점프 사운드, 피격 시 카메라 쉐이크 같은 연출 연결
/// </summary>
public class PlayerController : Player
{
	// 이동 처리 담당 컴포넌트
	[SerializeField] private PlayerMoveController moveController;
	// 상호작용(E 키 등) 처리 담당 컴포넌트
	[SerializeField] private PlayerInteractor interactor;
	// 카메라 연출 담당(쉐이크 등)
	[SerializeField] private PlayerCinemachineFreeLook playerCinemachineFreeLook;
	// 플레이어 사운드 담당(점프, 이동 등)
	[SerializeField] private PlayerAudioController audioController;

	/// <summary>
	/// - Player 베이스 초기화(base.Awake)
	/// - 필요한 컴포넌트가 없으면 AddComponent로 자동 추가
	/// - 카메라/오디오 참조를 찾아서 세팅
	/// - 피격 번쩍임(hit flash)에 사용할 머티리얼 준비
	/// </summary>
	protected override void Awake()
	{
		base.Awake();
		// 이동 컴포넌트가 없으면 자동 추가
		if (moveController == null)
		{
			moveController = transform.AddComponent<PlayerMoveController>();
		}

		// 상호작용 컴포넌트가 없으면 자동 추가
		if (interactor == null)
		{
			interactor = transform.AddComponent<PlayerInteractor>();
		}

		// 카메라 컨트롤러는 자식 오브젝트에서 찾음
		playerCinemachineFreeLook = transform.GetComponentInChildren<PlayerCinemachineFreeLook>();

		// 오디오 컨트롤러는 같은 오브젝트에서 찾음
		audioController = transform.GetComponent<PlayerAudioController>();

		// 피격 번쩍임에 사용할 머티리얼 인스턴스 확보
		if (meshRenderer != null)
			hitMatInstance = meshRenderer.material;
	}


	/// <summary>
	/// -컴포넌트 간 이벤트 연결 같은 것들을 여기서 수행
	/// </summary>
	private void OnEnable()
	{
		Setting_Action();
	}

	/// <summary>
	/// - 입력 이벤트를 반드시 해제(중복 구독 방지)
	/// </summary>
	private void OnDisable()
	{
		DiSetting_InputActionMove();
	}

	/// <summary>
	/// - Player 베이스 Start 실행(스탯 변경 구독 등)
	/// - 시작 스킬(AttackRoot)을 장착
	/// - 입력 액션(Player.Move/Jump/Interact)을 각 컴포넌트 함수에 연결
	/// </summary>
	protected override void Start()
	{
		base.Start();
		// 시작 스킬(기본 공격) 추가
		Add_AttackObject(StartAttackKey);
		// 입력 연결
		Setting_InputActionMove();
	}

	/// <summary>
	/// - 베이스 정리(base.OnDestroy)
	/// - 입력 이벤트 해제
	/// </summary>
	protected override void OnDestroy()
	{
		base.OnDestroy();
		DiSetting_InputActionMove();
	}

	/// <summary>
	/// Player(베이스)에서 피격 번쩍임 발생 시 호출되는 자식 확장 이벤트
	/// - 여기서는 피격 연출로 카메라 쉐이크를 실행
	/// </summary>
	protected override void Event_ChildEvnet()
	{
		playerCinemachineFreeLook.Shake(1, 1);
	}

	/// <summary>
	/// 입력 액션을 실제 이동/점프/상호작용 처리 함수에 연결
	/// - New Input System 이벤트(performed/canceled)를 구독한다
	/// </summary>
	private void Setting_InputActionMove()
	{
		BattleGSC.Instance.inputManager.InputActions.Player.Move.performed += moveController.OnMove;
		BattleGSC.Instance.inputManager.InputActions.Player.Move.canceled += moveController.OnMoveCanceled;
		BattleGSC.Instance.inputManager.InputActions.Player.Jump.performed += moveController.OnJump;
		BattleGSC.Instance.inputManager.InputActions.Player.Interact.performed += interactor.OnInteract;
	}

	/// <summary>
	/// 입력 액션 구독 해제
	/// - OnDisable / OnDestroy에서 호출해서 중복 구독을 막는다
	/// </summary>
	private void DiSetting_InputActionMove()
	{
		BattleGSC.Instance.inputManager.InputActions.Player.Move.performed -= moveController.OnMove;
		BattleGSC.Instance.inputManager.InputActions.Player.Move.canceled -= moveController.OnMoveCanceled;
		BattleGSC.Instance.inputManager.InputActions.Player.Jump.performed -= moveController.OnJump;
		BattleGSC.Instance.inputManager.InputActions.Player.Interact.performed -= interactor.OnInteract;
	}

	/// <summary>
	/// 컴포넌트 간 이벤트 연결
	/// - 점프가 발생했을 때 사운드 재생
	/// </summary>
	private void Setting_Action()
	{
		moveController.onJump += audioController.Play_Jump;
	}

}
