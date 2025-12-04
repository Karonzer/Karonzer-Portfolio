using TMPro;
using UnityEngine;


public class TimerUI : MonoBehaviour, IUIHandler
{
	public UIType Type => UIType.Timer;
	[SerializeField] private TextMeshProUGUI uITextMeshPro;

	private void Awake()
	{
		uITextMeshPro = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
	}
	private void OnEnable()
	{
		uITextMeshPro.text = "0:00";
	}

	private void OnDisable()
	{
		if (BattleGSC.Instance.gameManager.TryGetComponent<GameManager>(out var manager))
		{
			manager.TimerAction -= On_Notify;
		}
	}

	public void Hide()
	{
		uITextMeshPro.gameObject.SetActive(false);
	}

	public void Show()
	{
		uITextMeshPro.gameObject.SetActive(true);
	}


	public void ShowAndInitialie(GameObject _obj = null)
	{
		uITextMeshPro.gameObject.SetActive(true);

		if (_obj.TryGetComponent<GameManager>(out var manager))
		{
			manager.TimerAction += On_Notify;
		}
	}


	public void On_Notify(float _value)
	{
		float time = _value;

		int minutes = Mathf.FloorToInt(time / 60f);
		int seconds = Mathf.FloorToInt(time % 60f);

		uITextMeshPro.text = $"{minutes}:{seconds:00}";
	}
}
