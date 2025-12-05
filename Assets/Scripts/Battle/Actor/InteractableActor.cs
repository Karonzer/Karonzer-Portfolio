using UnityEngine;

public abstract class InteractableActor : Actor, IInteractable
{
	[SerializeField] protected bool canInteract;
	public bool CanInteract => canInteract;

	public GameObject currentObj => gameObject;

	public virtual void Interact(GameObject interactor)
	{

	}
}
