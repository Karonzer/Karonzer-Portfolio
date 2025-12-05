using UnityEngine;

public class ChestBox : InteractableActor
{
	[SerializeField] private string key = "ChestBox";
	[SerializeField] private SphereCollider sphereCollider;

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

	public override void Interact(GameObject interactor)
	{
		if (!canInteract) return;
		canInteract = false;
		GiveReward(interactor);

	}

	private void GiveReward(GameObject interactor)
	{
		BattleGSC.Instance.gameManager.Update_ToPlayerAttackObjIsChestBox();
		gameObject.SetActive(false);
		BattleGSC.Instance.spawnManager.DeSpawn(PoolObjectType.Actor, key, transform.gameObject);
	}

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
