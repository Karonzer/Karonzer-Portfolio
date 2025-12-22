using UnityEngine;

public class Title : MonoBehaviour
{
	[SerializeField] private GameObject gscPrefab;

	private void Awake()
	{
		if (GlobalGSC.Instance == null)
			Instantiate(gscPrefab);
	}

	private void Start()
	{
		GlobalGSC.Instance.audioManager.ChangeBGM(SceneBGMType.Title);
	}

	public void Click_StartGame()
	{
		GlobalGSC.Instance.sceneManager.LoadBattle_WithLoading();
		GlobalGSC.Instance.audioManager.Play_Click();
	}	

	public void Click_SoundSetting()
	{
		GlobalGSC.Instance.settingMenulUI.Show_PopUp();
		GlobalGSC.Instance.audioManager.Play_Click();
	}	

	public void Click_GameEnd()
	{
		GlobalGSC.Instance.audioManager.Play_Click();
		Application.Quit();
	}


}
