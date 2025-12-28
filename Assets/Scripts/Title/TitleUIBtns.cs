using UnityEngine;

public class TitleUIBtns : MonoBehaviour, IUIHandler
{
	[SerializeField] private TitleUIManager uiUIManager;
	[SerializeField] private Transform popUp;
	public UIType Type => UIType.titleBtns;

	public bool IsOpen => popUp.gameObject.activeSelf;
	private void Awake()
	{
		uiUIManager = transform.parent.GetComponent<TitleUIManager>();
		popUp = transform.GetChild(0);
	}
	public void Hide()
	{
		popUp.gameObject.SetActive(false);
	}

	public void Show()
	{
		popUp.gameObject.SetActive(true);
	}

	public void ShowAndInitialie(GameObject _obj = null)
	{
		popUp.gameObject.SetActive(true);
	}


	/// <summary>
	/// 게임 시작 버튼
	/// - 로딩 씬으로 이동 후 Battle 씬 로드
	/// </summary>
	public void Click_SelectMenu()
	{
		uiUIManager.titleHandler.Value.GoToSelectCamera();
		//GlobalGSC.Instance.sceneManager.LoadBattle_WithLoading();
		GlobalGSC.Instance.audioManager.Play_Click();

	}

	/// <summary>
	/// 사운드 설정 버튼
	/// - 전역 설정 메뉴 팝업 오픈
	/// </summary>
	public void Click_SoundSetting()
	{
		GlobalGSC.Instance.settingMenulUI.Show_PopUp();
		GlobalGSC.Instance.audioManager.Play_Click();
	}


	/// <summary>
	/// 게임 종료 버튼
	/// </summary>
	public void Click_GameEnd()
	{
		GlobalGSC.Instance.audioManager.Play_Click();
		Application.Quit();
	}

}
