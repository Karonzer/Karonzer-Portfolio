using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using System.Collections;

/// <summary>
/// 전역 씬 전환 매니저.
/// 
/// 담당:
/// - 로딩 씬 진입
/// - Addressables 기반 Battle 씬 로드(비활성 로드 후 Activate)
/// - Battle 씬 언로드 후 타이틀 복귀
/// - Battle 재시작(언로드 후 LoadingScene으로)
/// 
/// 특징:
/// - Battle 씬을 Addressables로 로드하므로, 핸들(battleHandle) 관리가 중요
/// - 언로드 후 GC.Collect 호출로 메모리 회수 시도
/// </summary>

public class GSCSceneManager : MonoBehaviour
{
	public string loadingSceneName = "LoadingScene";
	public string battleSceneKey = "Battle";


	// Addressables로 로드한 Battle 씬 핸들(언로드에 필요)
	private AsyncOperationHandle<SceneInstance> battleHandle;
	private void Awake()
	{
		// 전역 등록
		GlobalGSC.Instance.RegisterGSCSceneManager(this);

		// 기본 씬/키 세팅
		loadingSceneName = "LoadingScene";
		battleSceneKey = "Battle";
	}

	/// <summary>
	/// 로딩 씬으로 먼저 이동 (로딩 씬에서 실제 LoadBattleSceneCoroutine 실행)
	/// </summary>
	public void LoadBattle_WithLoading()
	{
		SceneManager.LoadScene(loadingSceneName);
	}

	/// <summary>
	/// Battle 씬을 Addressables로 비활성 로드 후, 완료되면 ActivateAsync로 활성화.
	/// - onProgress: 로딩 진행률 UI 업데이트용 콜백
	/// </summary>
	public IEnumerator LoadBattleSceneCoroutine(System.Action<float> onProgress)
	{
		// activateOnLoad = false 로 로드만 진행
		var handle = Addressables.LoadSceneAsync(battleSceneKey, LoadSceneMode.Single, false);
		battleHandle = handle;

		while (!handle.IsDone)
		{
			onProgress?.Invoke(handle.PercentComplete);
			yield return null;
		}

		yield return handle.Result.ActivateAsync();

	}

	/// <summary>
	/// 타이틀로 복귀 요청
	/// </summary>
	public void ReturnToTitle()
	{
		StartCoroutine(ReturnToTitleRoutine());
	}

	/// <summary>
	/// Battle 씬이 로드되어 있다면 언로드 후 타이틀 씬으로 이동
	/// </summary>
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

	/// <summary>
	/// Battle 재시작 요청
	/// </summary>
	public void RestartBattle()
	{
		StartCoroutine(RestartBattleRoutine());
	}

	/// <summary>
	/// Battle 씬 언로드 후 LoadingScene으로 이동 (로딩 씬에서 다시 Battle 로드)
	/// </summary>
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
