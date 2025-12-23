using UnityEngine;
using UnityEngine.UI;
using TMPro;


/// <summary>
/// 게임 오버 팝업 UI.
/// 
/// 역할:
/// - 게임 종료 시 팝업 표시
/// - 생존 시간 / 처치 수 표시
/// - 재시작 / 타이틀 이동 버튼 처리
/// 
/// IUIHandler:
/// - UIManger에서 UIType으로 Show/Hide 제어 가능
/// </summary>
public class GameOveUI : MonoBehaviour, IUIHandler
{
	public UIType Type => UIType.GameOver;


	// popUp(루트)이 활성화되어 있는지로 열림 여부 판단
	public bool IsOpen => popUp.gameObject.activeSelf;

	[SerializeField] private Transform popUp;
	[SerializeField] private TextMeshProUGUI survivalTimeText;
	[SerializeField] private TextMeshProUGUI killCountText;

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
		Setting_PopUpGameObjectSetActive(false);
	}

	public void Show()
	{
		Setting_PopUpGameObjectSetActive(true);
	}

	/// <summary>
	/// 팝업 활성/비활성 + 텍스트 업데이트
	/// - 생존 시간/킬 카운트는 GameManager에서 가져와 표시
	/// </summary>
	public void Setting_PopUpGameObjectSetActive(bool _bool)
	{
		popUp.gameObject.SetActive(_bool);


		// 누적 생존 시간 가져오기
		float time = BattleGSC.Instance.gameManager.SurvivalTime;

		// mm:ss 변환
		int minutes = Mathf.FloorToInt(time / 60f);
		int seconds = Mathf.FloorToInt(time % 60f);

		killCountText.text = $"처리할 몬스터 수 : \n{BattleGSC.Instance.gameManager.CurrentKillCount}";
		survivalTimeText.text = $"생존 시간 : \n{minutes}:{seconds:00}";
	}

	public void ShowAndInitialie(GameObject _obj = null)
	{
		// UIManager.Show(type, obj) 호출 대응용
		popUp.gameObject.SetActive(true);
	}

	/// <summary>
	/// 버튼 이벤트 연결
	/// </summary>
	private void Initialize_Btn()
	{
		Button reStart = popUp.GetChild(0).GetChild(1).GetChild(0).GetComponent<Button>();
		reStart.AddEvent(()=>Click_ReStart());

		Button toTitle = popUp.GetChild(0).GetChild(1).GetChild(1).GetComponent<Button>();
		toTitle.AddEvent(() => Click_ToTitle());
	}

	/// <summary>
	/// 전투 씬 재시작
	/// </summary>
	public void Click_ReStart()
	{
		GlobalGSC.Instance.sceneManager.RestartBattle();
	}

	/// <summary>
	/// 타이틀 씬으로 이동
	/// </summary>
	public void Click_ToTitle()
	{
		GlobalGSC.Instance.sceneManager.ReturnToTitle();
	}

}
