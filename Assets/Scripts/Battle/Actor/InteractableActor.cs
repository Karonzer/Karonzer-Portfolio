using UnityEngine;

/// <summary>
/// 플레이어가 상호작용할 수 있는 Actor의 베이스 클래스.
/// 
/// 구조:
/// Actor
///  └─ InteractableActor
///      └─ ChestBox, Lever, NPC, Portal ...
/// 
/// 핵심 책임:
/// - 상호작용 가능 여부 관리
/// - IInteractable 인터페이스 구현
/// - 실제 상호작용 로직은 자식 클래스에서 구현
/// </summary>
public abstract class InteractableActor : Actor, IInteractable
{
	// 현재 상호작용 가능 여부
	[SerializeField] protected bool canInteract;

	// 외부 접근용 읽기 전용 프로퍼티
	public bool CanInteract => canInteract;

	// 현재 상호작용 대상 오브젝트 반환
	public GameObject currentObj => gameObject;


	/// <summary>
	/// 상호작용 시 호출되는 함수
	/// - 실제 동작은 자식 클래스에서 override
	/// - interactor: 상호작용을 시도한 오브젝트 (보통 Player)
	/// </summary>
	public virtual void Interact(GameObject interactor)
	{

	}
}
