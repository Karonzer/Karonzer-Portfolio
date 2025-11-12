using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
	[SerializeField] private InputSystem_Actions inputActions;


	private void Awake()
	{
		inputActions = new InputSystem_Actions();
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
		Vector2 inputVector = context.ReadValue<Vector2>();
		Debug.Log("Move Input: " + inputVector);
	}
	private void OnMoveCanceled(InputAction.CallbackContext context)
	{
		Debug.Log("Move Input Canceled");
	}



}
