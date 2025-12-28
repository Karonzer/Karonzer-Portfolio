using UnityEngine;

public class TitleSelectPopUp : MonoBehaviour, IUIHandler
{
	[SerializeField] private TitleUIManager uiUIManager;
	[SerializeField] private Transform popUp;

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


}
