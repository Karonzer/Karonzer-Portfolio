using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class XPBarUI : MonoBehaviour, IUIHandler
{
	[SerializeField] private Image fill;
	[SerializeField] private TextMeshProUGUI levelText;

	private IXPTable xP;
	public GameObject UIObject => transform.gameObject;

	public UIType Type => UIType.XPBar;

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

	public void Initialize(IXPTable _system)
	{
		xP = _system;

		xP.OnXPChanged += HandleXPChanged;
		xP.OnLevelChanged += HandleLevelUp;

		HandleXPChanged(xP.CurrentXP, xP.MaxXP);
		HandleLevelUp(xP.CurrentLevel);
	}

	private void HandleXPChanged(int _current, int _max)
	{
		fill.fillAmount = (float)_current / _max;
	}

	private void HandleLevelUp(int _level)
	{
		if (levelText != null)
			levelText.text = $"Lv : {_level}";
	}


}
