using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using System.Collections;

public class GSCSceneManager : MonoBehaviour
{
	public string loadingSceneName = "LoadingScene";
	public string battleSceneKey = "Battle";

	private AsyncOperationHandle<SceneInstance> battleHandle;
	private void Awake()
	{
		GlobalGSC.Instance.RegisterGSCSceneManager(this);
		loadingSceneName = "LoadingScene";
		battleSceneKey = "Battle";
	}

	public void LoadBattle_WithLoading()
	{
		SceneManager.LoadScene(loadingSceneName);
	}

	public IEnumerator LoadBattleSceneCoroutine(System.Action<float> onProgress)
	{
		var handle = Addressables.LoadSceneAsync(battleSceneKey, LoadSceneMode.Single, false);
		battleHandle = handle;

		while (!handle.IsDone)
		{
			onProgress?.Invoke(handle.PercentComplete);
			yield return null;
		}

		yield return handle.Result.ActivateAsync();

	}

	public void ReturnToTitle()
	{
		StartCoroutine(ReturnToTitleRoutine());
	}

	private IEnumerator ReturnToTitleRoutine()
	{
		if (battleHandle.IsValid())
		{
			yield return Addressables.UnloadSceneAsync(battleHandle);
			yield return null;
			System.GC.Collect();
		}
		SceneManager.LoadScene("TitleScene");
	}

	public void RestartBattle()
	{
		StartCoroutine(RestartBattleRoutine());
	}

	private IEnumerator RestartBattleRoutine()
	{
		if (battleHandle.IsValid())
		{
			yield return Addressables.UnloadSceneAsync(battleHandle);
			yield return null;
			System.GC.Collect();
		}


		SceneManager.LoadScene("LoadingScene");
	}

}
