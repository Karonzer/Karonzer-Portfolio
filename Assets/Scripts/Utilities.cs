using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


/// <summary>
/// 프로젝트 전반에서 공통으로 사용하는 유틸리티 함수 모음.
/// 
/// 특징:
/// - static 클래스
/// - 확장 메서드 중심 (Button, Transform, Collider[] 등)
/// - 전투 로직 / UI / 타겟팅에서 자주 사용
/// </summary>
public static class Utilities
{
	/// <summary>
	/// Button에 클릭 이벤트를 안전하게 등록하는 확장 메서드.
	/// - 기존 리스너 제거 후 단일 이벤트 등록
	/// - 중복 클릭/중복 등록 방지
	/// </summary>
	public static void AddEvent(this Button button, UnityAction action)
	{
		button.onClick.RemoveAllListeners();
		button.onClick.AddListener(action);
	}


	/// <summary>
	/// Collider 배열에서 가장 가까운 Enemy Transform 반환
	/// </summary>
	public static Transform Get_CloseEnemy(this Collider[] _results, Transform _from)
	{
		if(_results == null)
			return null;

		float minDist = float.MaxValue;
		Transform target = null;

		foreach (var c in _results)
		{
			if (!c.gameObject.CompareTag("Enemy"))
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

	/// <summary>
	/// Collider 배열 중 count 개까지만 검사하여
	/// - Boss가 있으면 즉시 반환
	/// - 없으면 가장 가까운 Enemy 반환
	/// </summary>
	public static Transform Get_CloseEnemy(this Collider[] _results, Transform _from, int count)
	{
		if (_results == null || count <= 0)
			return null;

		float minDist = float.MaxValue;
		Transform target = null;

		for (int i = 0; i < count; i++)
		{
			Collider c = _results[i];

			if (c.CompareTag("Boss"))
			{
				target = c.transform;
				return target;
			}

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

	/// <summary>
	/// 가장 가까운 Boss Transform 반환
	/// </summary>
	public static Transform Get_CloseBoss(this Collider[] _results, Transform _from, int count)
	{
		if (_results == null || count <= 0)
			return null;

		float minDist = float.MaxValue;
		Transform target = null;

		for (int i = 0; i < count; i++)
		{
			Collider c = _results[i];
			if (!c.CompareTag("Boss"))
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

	/// <summary>
	/// Collider 배열 중 가장 가까운 GameObject 반환
	/// </summary>
	public static GameObject Get_CloseObj(this Collider[] _results, Transform _from, int count)
	{
		if (_results == null || count <= 0)
			return null;

		float minDist = float.MaxValue;
		GameObject target = null;

		for (int i = 0; i < count; i++)
		{
			Collider c = _results[i];

			float dist = (c.transform.position - _from.position).sqrMagnitude;

			if (dist < minDist)
			{
				minDist = dist;
				target = c.gameObject;
			}
		}
		return target;
	}

	/// <summary>
	/// Collider 배열에서 Enemy 태그를 가진 모든 Transform 반환
	/// </summary>
	public static List<Transform> Get_Enemies(this Collider[] _results)
	{
		List<Transform> enemies = new List<Transform>();

		foreach (var c in _results)
		{
			if (c.CompareTag("Enemy"))
				enemies.Add(c.transform);
		}

		return enemies;
	}

	/// <summary>
	/// 타겟을 향한 정규화된 방향 벡터 반환 (Transform 기준)
	/// </summary>
	public static Vector3 Get_TargetDir(this Transform _target, Transform _pos)
	{
		return (_target.position - _pos.position).normalized;
	}

	/// <summary>
	/// 타겟을 향한 정규화된 방향 벡터 반환 (GameObject 기준)
	/// </summary>
	public static Vector3 Get_TargetDir(this GameObject _target, Transform _pos)
	{
		return (_target.transform.position - _pos.position).normalized;
	}
}
