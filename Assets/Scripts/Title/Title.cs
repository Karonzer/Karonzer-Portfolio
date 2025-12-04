using UnityEngine;

public class Title : MonoBehaviour
{
	[SerializeField] private GameObject gscPrefab;

	private void Awake()
	{
		if (GlobalGSC.Instance == null)
			Instantiate(gscPrefab);
	}

	public void Click_StartGame()
	{
		GlobalGSC.Instance.sceneManager.LoadBattle_WithLoading();
	}	


}
