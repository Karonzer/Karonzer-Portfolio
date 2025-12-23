using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 일시정지(Pause) 팝업 UI.
/// 
/// 역할:
/// - 닫기(일시정지 해제)
/// - 재시작
/// - 타이틀 이동
/// - 사운드 설정 팝업 열기
/// 
/// IUIHandler:
/// - UIManger에서 UIType으로 Show/Hide 제어 가능
/// - UIManger에서 UIType.Pause로 접근
/// </summary>
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

	/// <summary>
	/// 버튼 이벤트 연결
	/// </summary>
	private void Initialize_Btn()
	{
		closeBtn.AddEvent(Click_CloseBtn);
		reStart.AddEvent(Click_ReStart);
		toTilte.AddEvent(Click_ToTilte);
		settingSound.AddEvent(Click_SettingSound);
	}

	/// <summary>
	/// 일시정지 토글 (닫기 버튼)
	/// </summary>
	private void Click_CloseBtn()
	{
		BattleGSC.Instance.gameManager.Toggle_Pause();
		GlobalGSC.Instance.audioManager.Play_Click();
	}

	/// <summary>
	/// 다시 시작 버툰
	/// </summary>
	private void Click_ReStart()
	{
		GlobalGSC.Instance.sceneManager.RestartBattle();
		GlobalGSC.Instance.audioManager.Play_Click();
	}

	/// <summary>
	/// 타이틀 씬 이동
	/// </summary>
	private void Click_ToTilte()
	{
		GlobalGSC.Instance.sceneManager.ReturnToTitle();
		GlobalGSC.Instance.audioManager.Play_Click();
	}

	/// <summary>
	/// 사운드 설정 메뉴 열기
	/// </summary>
	private void Click_SettingSound()
	{
		GlobalGSC.Instance.settingMenulUI.Show_PopUp();
		GlobalGSC.Instance.audioManager.Play_Click();
		Hide();
	}
}


