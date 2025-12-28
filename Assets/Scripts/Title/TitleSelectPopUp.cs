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
		string name = "MO_Player_" + _index;
		curentSelectIndex = _index;
		Debug.Log(curentSelectIndex);
		uiUIManager.titleHandler.Value.SpawnAsync(name);
	}

	public void Click_StartGame()
	{
		GlobalGSC.Instance.audioManager.Play_Click();
		string name = "Player_" + curentSelectIndex;
		GlobalGSC.Instance.currentPlayeName = name;
		GlobalGSC.Instance.sceneManager.LoadBattle_WithLoading();
	}	

	public void GoToMain()
	{
		uiUIManager.titleHandler.Value.GoToMainCamera();
		GlobalGSC.Instance.audioManager.Play_Click();
	}

}
