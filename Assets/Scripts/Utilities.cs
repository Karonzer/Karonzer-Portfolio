using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public static class Utilities
{
	public static void AddEvent(this Button button, UnityAction action)
	{
		button.onClick.RemoveAllListeners();
		button.onClick.AddListener(action);
	}



	public static Transform Get_CloseEnemy(this Collider[] _results, Transform _from)
	{
		float minDist = float.MaxValue;
		Transform target = null;

		foreach (var c in _results)
		{
			if (!c.CompareTag("Enemy"))
				continue;
			float dist = (c.transform.position - _from.position).sqrMagnitude;

			if (dist < minDist)
			{
				minDist = dist;
				target = c.transform;
			}
		}
		return target;
	}

	public static Vector3 Get_TargetDir(this Transform _target, Transform _pos)
	{
		return (_target.position - _pos.position).normalized;
	}
}
