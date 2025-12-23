using UnityEngine;

/// <summary>
/// 상속 받을 수 있는 싱글톤 클래스
/// </summary>
public class GenericSingletonClass<T> : MonoBehaviour where T : Component
{
	[SerializeField] private static T instance = null;
	public static T Instance
	{
		get
		{
			if (instance == null)
			{
				instance = FindAnyObjectByType<T>();
			}
			return instance;
		}
	}
}
