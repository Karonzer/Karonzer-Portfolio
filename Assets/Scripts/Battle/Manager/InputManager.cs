using UnityEngine;

public class InputManager : MonoBehaviour
{
	[SerializeField] private InputSystem_Actions inputActions;
	public InputSystem_Actions InputActions => inputActions;
	private void Awake()
	{
		inputActions = new InputSystem_Actions();
		BattleGSC.Instance.RegisterInputManager(this);
	}

	private void OnEnable()
	{
		inputActions.Enable();

	}

	private void OnDisable()
	{
		inputActions.Disable();
	}
}
