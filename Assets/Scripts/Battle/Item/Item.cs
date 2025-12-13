using UnityEngine;

public abstract class Item : MonoBehaviour
{
	protected SphereCollider sphereCollider;
	protected virtual void Start()
	{
		sphereCollider = GetComponent<SphereCollider>();
		sphereCollider.isTrigger = true;
	}
	protected abstract void Fuction_Event(GameObject _obj);

	public void Setting_SpwnPos(Vector3 _pos)
	{
		transform.position = _pos;
	}
	protected void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			Fuction_Event(other.gameObject);
			GlobalGSC.Instance.audioManager.Play_Sound(SoundType.Item);
			gameObject.SetActive(false);
		}
	}
}
