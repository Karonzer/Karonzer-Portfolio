using UnityEngine;

public class GlobalGSC : GenericSingletonClass<GlobalGSC>
{
	private void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}
}
