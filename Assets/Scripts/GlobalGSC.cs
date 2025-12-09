using UnityEngine;

public class GlobalGSC : GenericSingletonClass<GlobalGSC>
{
	public GSCSceneManager sceneManager { get; private set; }
	public GlobalAudioManager audioManager { get; private set; }

	private void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}

	public void RegisterGSCSceneManager(GSCSceneManager _sceneManager) => sceneManager = _sceneManager;

	public void RegisterGlobalAudioManager(GlobalAudioManager _audioManager) => audioManager = _audioManager;
}
