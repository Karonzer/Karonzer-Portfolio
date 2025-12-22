using UnityEngine;

/// <summary>
/// InputManager
/// 
/// Unity New Input System에서 생성된 InputActions(InputSystem_Actions)을
/// 한 곳에서 만들고, Enable/Disable을 관리하는 매니저.
/// 
/// - 전투 씬에서 입력 데이터를 하나의 출처로 관리하기 위함
/// - 다른 스크립트가 BattleGSC.Instance.inputManager.InputActions 로 접근 가능
/// </summary>
public class InputManager : MonoBehaviour
{
	/// <summary>
	/// Input System 자동 생성 클래스 인스턴스
	/// (예: Player/ UI 액션맵의 입력 이벤트에 접근할 때 사용)
	/// </summary>
	[SerializeField] private InputSystem_Actions inputActions;

	// 외부에서 inputActions를 읽기만 할 수 있도록 공개
	public InputSystem_Actions InputActions => inputActions;

	/// <summary>
	/// - BattleGSC에 자신을 등록하여 전역 접근 가능하도록 한다
	/// </summary>
	private void Awake()
	{
		// InputActions 생성 (씬 시작 시 1회)
		inputActions = new InputSystem_Actions();
		// BattleGSC에 자신을 등록
		BattleGSC.Instance.RegisterInputManager(this);
	}

	private void OnEnable()
	{
		// 오브젝트가 활성화되면 입력 활성화
		// (입력 이벤트가 동작하기 시작)
		inputActions.Enable();

	}

	private void OnDisable()
	{
		// 오브젝트가 비활성화되면 입력 비활성화
		// (입력 이벤트가 더 이상 들어오지 않음)
		inputActions.Disable();
	}
}
