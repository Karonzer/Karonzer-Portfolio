using System.Collections;
using UnityEngine;

/// <summary>
/// XP 아이템 근처에 붙는 보조 트리거.
/// 
/// 역할:
/// - 플레이어가 일정 거리 안으로 들어오면
///   XPitem의 추적을 시작시키는 감지용 컴포넌트.
/// </summary>
public class ItemFollowToPlayer : MonoBehaviour
{
	private XPitem xPitem;
	private SphereCollider sphereCollider;

	private void Awake()
	{
		// 감지용 트리거 콜라이더
		sphereCollider = transform.GetComponent<SphereCollider>();
		sphereCollider.isTrigger = true;


		// 부모 오브젝트에 있는 XPitem 참조
		xPitem = transform.parent.GetComponent<XPitem>();
	}

	private void OnEnable()
	{

	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player"))
		{
			// 아직 추적 중이 아닐 때만 시작
			if (!xPitem.Get_isFollowToPlayer())
			{
				xPitem.Start_FollowToPlayer(other.gameObject);
			}
		}
	}


}
