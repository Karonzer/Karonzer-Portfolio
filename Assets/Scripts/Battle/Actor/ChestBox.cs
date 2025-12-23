using UnityEngine;

/// <summary>
/// 플레이어가 상호작용할 수 있는 상자 오브젝트.
/// 
/// 기능:
/// - 플레이어 접근 시 상호작용 UI 표시
/// - 상호작용 1회 제한
/// - 보상 처리 후 풀로 반환
/// </summary>
public class ChestBox : InteractableActor
{
	// 풀링 시스템에서 사용할 Actor 키
	[SerializeField] private string key = "ChestBox";
	// 플레이어 감지용 트리거 콜라이더
	[SerializeField] private SphereCollider sphereCollider;

	// 월드 상호작용 UI (예: "E 키로 열기")
	[SerializeField] private Transform worldCan;

	private void Awake()
	{
		sphereCollider = GetComponent<SphereCollider>();
		sphereCollider.isTrigger = true;
		worldCan = transform.GetChild(0);
	}

	private void OnEnable()
	{
		canInteract = true;
		if (worldCan != null)
		{
			worldCan.gameObject.SetActive(false);
		}
	}

	/// <summary>
	/// 플레이어가 상호작용 키를 눌렀을 때 호출
	/// </summary>
	public override void Interact(GameObject interactor)
	{
		if (!canInteract) return;
		canInteract = false;
		GiveReward(interactor);

	}

	/// <summary>
	/// 상자 보상 처리
	/// </summary>
	private void GiveReward(GameObject interactor)
	{
		// 게임 매니저에 상자 상호작용 발생 알림
		BattleGSC.Instance.gameManager.Update_ToPlayerAttackObjIsChestBox();

		// 비활성화 후 풀로 반환
		gameObject.SetActive(false);
		BattleGSC.Instance.spawnManager.DeSpawn(PoolObjectType.Actor, key, transform.gameObject);
	}

	/// <summary>
	/// 플레이어 진입/이탈에 따라 상호작용 UI 표시 제어
	/// </summary>
	private void Check_ColliderOther(Collider _other, bool _bool)
	{
		if (!_other.CompareTag("Player"))
			return;

		worldCan.gameObject.SetActive(_bool);
	}

	private void OnTriggerEnter(Collider other)
	{
		Check_ColliderOther(other, true);
	}

	private void OnTriggerStay(Collider other)
	{
		Check_ColliderOther(other, true);
	}

	private void OnTriggerExit(Collider other)
	{
		Check_ColliderOther(other, false);
	}


}
