using UnityEngine;
using UnityEngine.UI;

public class PausePopUp : MonoBehaviour, IUIHandler
{
	public UIType Type => UIType.Pause;

	public bool IsOpen => popUp.gameObject.activeSelf;

	[SerializeField] private Transform popUp;

	[SerializeField] private Button closeBtn;
	[SerializeField] private Button reStart;
	[SerializeField] private Button toTilte;
	[SerializeField] private Button settingSound;

	private void Awake()
	{
		popUp = transform.GetChild(0);
	}

	private void Start()
	{
		Initialize_Btn();
	}

	private void OnEnable()
	{
		popUp.gameObject.SetActive(false);
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

	private void Initialize_Btn()
	{
		closeBtn.AddEvent(Click_CloseBtn);
		reStart.AddEvent(Click_ReStart);
		toTilte.AddEvent(Click_ToTilte);
		settingSound.AddEvent(Click_SettingSound);
	}

	private void Click_CloseBtn()
	{
		BattleGSC.Instance.gameManager.Toggle_Pause();
		GlobalGSC.Instance.audioManager.Play_Click();
	}

	private void Click_ReStart()
	{
		GlobalGSC.Instance.sceneManager.RestartBattle();
		GlobalGSC.Instance.audioManager.Play_Click();
	}

	private void Click_ToTilte()
	{
		GlobalGSC.Instance.sceneManager.ReturnToTitle();
		GlobalGSC.Instance.audioManager.Play_Click();
	}
	private void Click_SettingSound()
	{
		GlobalGSC.Instance.settingMenulUI.Show_PopUp();
		GlobalGSC.Instance.audioManager.Play_Click();
		Hide();
	}
}


