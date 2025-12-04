using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadingSceneController : MonoBehaviour
{
	[SerializeField] private Slider progressBar;

	private IEnumerator Start()
	{
		yield return GlobalGSC.Instance.sceneManager
			.LoadBattleSceneCoroutine(
				(p) => progressBar.value = p
			);
	}
}
