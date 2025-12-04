using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class ItemFollowToPlayer : MonoBehaviour
{
	private GameObject target;
	private SphereCollider sphereCollider;
	private Coroutine followRoutine;

	private void Awake()
	{
		sphereCollider = transform.GetComponent<SphereCollider>();
		sphereCollider.isTrigger = true;
	}

	private void OnEnable()
	{
		if (followRoutine != null) 
		{
			StopCoroutine(followRoutine);
			followRoutine = null;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player"))
		{
			if (followRoutine == null)
			{
				target = other.gameObject;
				followRoutine = StartCoroutine(Follow_ToPlayer(target));
			}
		}
	}

	private IEnumerator Follow_ToPlayer(GameObject _obj)
	{
		while (true)
		{
			Vector3 projectileDir = _obj.Get_TargetDir(transform);
			transform.parent.Translate(projectileDir * 5 * Time.deltaTime);
			yield return null;
		}
	}


}
