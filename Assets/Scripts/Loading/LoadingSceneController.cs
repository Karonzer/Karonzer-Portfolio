using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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
