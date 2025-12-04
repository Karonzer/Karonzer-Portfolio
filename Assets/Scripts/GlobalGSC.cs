using UnityEngine;

public class GlobalGSC : GenericSingletonClass<GlobalGSC>
{
	public GSCSceneManager sceneManager { get; private set; }

	private void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}

	public void RegisterGSCSceneManager(GSCSceneManager _sceneManager) => sceneManager = _sceneManager;
}
