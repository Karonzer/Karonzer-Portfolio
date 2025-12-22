using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// PlayerInteractor
/// 
/// 플레이어의 상호작용 기능 담당.
/// - 입력(Interact)이 들어오면
/// - 주변 범위(interactRange) 안에서 상호작용 가능한 오브젝트를 찾고
/// - 가장 가까운 대상의 IInteractable.Interact()를 호출한다.
/// 
/// 특징:
/// - Physics.OverlapSphereNonAlloc을 사용해서 GC(메모리 할당)를 줄인다.
/// - 찾는 대상은 "InteractableActor" 레이어에 있는 Collider들만.
/// </summary>
public class PlayerInteractor : MonoBehaviour
{
	/// <summary>
	/// 상호작용 가능한 거리(반경).
	/// 플레이어 기준으로 이 거리 안에 있는 대상만 찾는다.
	/// </summary>
	public float interactRange;

	/// <summary>
	/// OverlapSphereNonAlloc 결과를 담는 버퍼 배열.
	/// - 최대 5개까지만 담는다.
	/// - 5개를 넘어가면 나머지는 무시된다(의도적으로 제한).
	/// </summary>
	private readonly Collider[] tagetObjBuffer = new Collider[5];

	private void Awake()
	{
		interactRange = 1.5f;
	}

	private void OnEnable()
	{

	}

	private void OnDisable()
	{

	}


	/// <summary>
	/// 상호작용 입력 콜백.
	/// - 입력이 performed일 때만 실행
	/// - 주변에서 가장 가까운 상호작용 대상 찾기
	/// - 대상이 IInteractable을 구현했다면 Interact(gameObject) 호출
	/// </summary>
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

	/// <summary>
	/// 상호작용 대상 찾기.
	/// 흐름:
	/// 1) "InteractableActor" 레이어만 대상으로 한다.
	/// 2) 플레이어 주변 반경(interactRange) 안의 Collider들을 버퍼에 담는다.
	/// 3) 결과가 0이면 null 반환
	/// 4) 결과 중 가장 가까운 오브젝트를 선택해서 반환
	/// </summary>
	private GameObject Find_TargetObj()
	{
		int targetObjLayerMask = LayerMask.GetMask("InteractableActor");
		GameObject target = null;
		int count = Physics.OverlapSphereNonAlloc(transform.position, interactRange, tagetObjBuffer, targetObjLayerMask);

		if (count == 0)
			return null;

		target = tagetObjBuffer.Get_CloseObj(transform, count).gameObject;

		if (target != null)
		{
			return target;
		}

		return null;
	}
}
