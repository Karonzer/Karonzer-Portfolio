using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOveUI : MonoBehaviour, IUIHandler
{
	public UIType Type => UIType.GameOver;
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
		Setting_PopUpGameObjectSetActive(true);
	}

	public void Show()
	{
		Setting_PopUpGameObjectSetActive(true);
	}

	public void Setting_PopUpGameObjectSetActive(bool _bool)
	{
		popUp.gameObject.SetActive(_bool);

		float time = BattleGSC.Instance.gameManager.SurvivalTime;

		int minutes = Mathf.FloorToInt(time / 60f);
		int seconds = Mathf.FloorToInt(time % 60f);

		killCountText.text = $"처리할 몬스터 수 : \n{BattleGSC.Instance.gameManager.CurrentKillCount}";
		survivalTimeText.text = $"생존 시간 : \n{minutes}:{seconds:00}";
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
