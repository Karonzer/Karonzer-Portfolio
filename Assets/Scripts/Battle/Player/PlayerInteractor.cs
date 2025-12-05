using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
	public float interactRange;
	private GameObject tagetObj;
	private readonly Collider[] tagetObjBuffer = new Collider[5];

	private void Awake()
	{
		interactRange = 1.5f;
	}

	private void OnEnable()
	{

		tagetObj = null;
	}

	private void OnDisable()
	{

	}



	public void OnInteract(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			GameObject target = Find_TargetObj();
			if (target != null && target.TryGetComponent<IInteractable>(out IInteractable _component))
			{
				_component.Interact(gameObject);
			}
			else
			{
				Debug.Log("not find");
			}
		}
	}

	private GameObject Find_TargetObj()
	{
		int targetObjLayerMask = LayerMask.GetMask("InteractableActor");
		GameObject target = null;
		int count = Physics.OverlapSphereNonAlloc(transform.position, interactRange, tagetObjBuffer, targetObjLayerMask);

		if (count == 0)
			return null;

		Debug.Log("test");
		target = tagetObjBuffer.Get_CloseObj(transform, count).gameObject;

		if (target != null)
		{
			return target;
		}

		return null;
	}
}
