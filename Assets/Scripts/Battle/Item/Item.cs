using UnityEngine;

/// <summary>
/// 모든 아이템의 공통 베이스 클래스.
/// 
/// 책임:
/// - 플레이어와의 충돌 감지
/// - 아이템 획득 시 공통 처리 (사운드, 비활성화)
/// - 실제 효과는 자식 클래스에서 구현
/// 
/// 구조:
/// Item
///  ├─ XPitem
///  ├─ HealingItem
///  ├─ BuffItem
///  └─ MagnetItem
/// </summary>
public abstract class Item : MonoBehaviour
{
	// 플레이어 획득 판정용 콜라이더
	protected SphereCollider sphereCollider;
	protected virtual void Start()
	{
		sphereCollider = GetComponent<SphereCollider>();
		sphereCollider.isTrigger = true;
	}
	/// <summary>
	/// 아이템 고유 효과 처리
	/// - 플레이어와 충돌했을 때 호출됨
	/// - 자식 클래스에서 반드시 구현
	/// </summary>
	protected abstract void Fuction_Event(GameObject _obj);

	/// <summary>
	/// 스폰 시 위치 세팅 (드랍 위치 등)
	/// </summary>
	public void Setting_SpwnPos(Vector3 _pos)
	{
		transform.position = _pos;
	}

	/// <summary>
	/// 플레이어와 충돌 시 아이템 획득 처리
	/// </summary>
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
