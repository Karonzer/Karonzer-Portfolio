using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 경험치 바 UI.
/// 
/// 흐름:
/// - ShowAndInitialie(obj)에서 IXPTable을 받아 이벤트 구독
/// - XP/레벨 변화 시 UI 갱신
/// </summary>
public class XPBarUI : MonoBehaviour, IUIHandler
{
	[SerializeField] private Image fill;
	[SerializeField] private TextMeshProUGUI levelText;

	// XP 시스템 인터페이스 (PlayerLevel 등)
	private IXPTable xP;
	public UIType Type => UIType.XPBar;
	public bool IsOpen => transform.gameObject.activeSelf;

	private void Awake()
	{
		fill = transform.Find("XPBarFill").GetComponent<Image>();
		levelText = transform.Find("LevelText").GetComponent<TextMeshProUGUI>();
	}


	public void Show()
	{
		transform.gameObject.SetActive(true);
	}

	public void ShowAndInitialie(GameObject _obj = null)
	{
		transform.gameObject.SetActive(true);
		if (_obj.TryGetComponent<IXPTable>(out IXPTable _xp))
			Initialize(_xp);
	}

	public void Hide()
	{
		transform.gameObject.SetActive(false);
	}

	/// <summary>
	/// XP 시스템 이벤트 구독 및 초기 UI 반영
	/// </summary>
	public void Initialize(IXPTable _system)
	{
		xP = _system;

		xP.OnXPChanged += HandleXPChanged;
		xP.OnLevelChanged += HandleLevelUp;

		HandleXPChanged(xP.CurrentXP, xP.MaxXP);
		HandleLevelUp(xP.CurrentLevel);
	}

	/// <summary>
	/// 획득한 xp를 UI에 반영
	/// </summary>
	private void HandleXPChanged(int _current, int _max)
	{
		fill.fillAmount = (float)_current / _max;
	}

	/// <summary>
	/// 획득한 레벨을 UI에 반영
	/// </summary>
	private void HandleLevelUp(int _level)
	{
		if (levelText != null)
			levelText.text = $"Lv : {_level}";
	}


}
