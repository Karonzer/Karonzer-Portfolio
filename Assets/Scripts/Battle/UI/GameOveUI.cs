using UnityEngine;
using UnityEngine.UI;

public class GameOveUI : MonoBehaviour, IUIHandler
{
	public UIType Type => UIType.GameOver;
	public bool IsOpen => popUp.gameObject.activeSelf;

	[SerializeField] private Transform popUp;

	private void Awake()
	{
		popUp = transform.GetChild(0);
		Initialize_Btn();
	}
	private void OnEnable()
	{
		Setting_PopUpGameObjectSetActive(false);
	}

	public void Hide()
	{
		Setting_PopUpGameObjectSetActive(true);
	}

	public void Show()
	{
		Setting_PopUpGameObjectSetActive(true);
	}

	public void Setting_PopUpGameObjectSetActive(bool _bool)
	{
		popUp.gameObject.SetActive(_bool);
	}

	public void ShowAndInitialie(GameObject _obj = null)
	{
		popUp.gameObject.SetActive(true);
	}

	private void Initialize_Btn()
	{
		Button reStart = popUp.GetChild(0).GetChild(1).GetChild(0).GetComponent<Button>();
		reStart.AddEvent(()=>Click_ReStart());

		Button toTitle = popUp.GetChild(0).GetChild(1).GetChild(1).GetComponent<Button>();
		toTitle.AddEvent(() => Click_ToTitle());
	}	

	public void Click_ReStart()
	{
		GlobalGSC.Instance.sceneManager.RestartBattle();
	}

	public void Click_ToTitle()
	{
		GlobalGSC.Instance.sceneManager.ReturnToTitle();
	}

}
