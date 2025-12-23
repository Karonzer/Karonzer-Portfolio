using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// 로딩씬 스크립트
/// 타이틀에서 배틀씬으로 이동할때 진행바를 표시하는 스크립트
/// </summary>
public class LoadingSceneController : MonoBehaviour
{
	[SerializeField] private Slider progressBar;

	private IEnumerator Start()
	{
		GlobalGSC.Instance.audioManager.ChangeBGM(SceneBGMType.None);
		yield return GlobalGSC.Instance.sceneManager
			.LoadBattleSceneCoroutine(
				(p) => progressBar.value = p
			);
	}
}
