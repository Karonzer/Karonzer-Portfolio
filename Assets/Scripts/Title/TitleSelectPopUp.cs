using UnityEngine;

public class TitleSelectPopUp : MonoBehaviour, IUIHandler
{
	[SerializeField] private TitleUIManager uiUIManager;
	[SerializeField] private Transform popUp;

	private int curentSelectIndex;
	public UIType Type => UIType.titleSelect;

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

	public void Click_SelectBtn(int _index)
	{
		uiUIManager.titleHandler.Value.SpawnAsync(_index);
	}

	public void Click_StartGame()
	{
		GlobalGSC.Instance.audioManager.Play_Click();
		uiUIManager.titleHandler.Value.Start_Game();
		GlobalGSC.Instance.sceneManager.LoadBattle_WithLoading();
	}	

	public void GoToMain()
	{
		uiUIManager.titleHandler.Value.GoToMainCamera();
		GlobalGSC.Instance.audioManager.Play_Click();
	}

}
