using System.Collections;
using UnityEngine;
public class ItemFollowToPlayer : MonoBehaviour
{
	private XPitem xPitem;
	private SphereCollider sphereCollider;

	private void Awake()
	{
		sphereCollider = transform.GetComponent<SphereCollider>();
		sphereCollider.isTrigger = true;
		xPitem = transform.parent.GetComponent<XPitem>();
	}

	private void OnEnable()
	{

	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player"))
		{
			if (!xPitem.Get_isFollowToPlayer())
			{
				xPitem.Start_FollowToPlayer(other.gameObject);
			}
		}
	}


}
