using TMPro;
using UnityEngine;


/// <summary>
/// 생존 시간(또는 타이머)을 표시하는 UI.
/// 
/// 흐름:
/// - ShowAndInitialie(obj)로 GameManager를 전달받으면,
///   GameManager.TimerAction 이벤트를 구독하여 갱신.
/// - Disable 시 반드시 구독 해제.
/// 
/// - UIManger에서 UIType으로 Show/Hide 제어 가능
/// </summary>
public class TimerUI : MonoBehaviour, IUIHandler
{
	public UIType Type => UIType.Timer;
	[SerializeField] private TextMeshProUGUI uITextMeshPro;

	public bool IsOpen => uITextMeshPro.gameObject.activeSelf;

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

	/// <summary>
	/// GameManager에서 전달받은 타이머 값으로 텍스트 갱신
	/// </summary>
	public void On_Notify(float _value)
	{
		float time = _value;

		int minutes = Mathf.FloorToInt(time / 60f);
		int seconds = Mathf.FloorToInt(time % 60f);

		uITextMeshPro.text = $"{minutes}:{seconds:00}";
	}
}
