using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class XPBarUI : MonoBehaviour, IUIInitializable
{
	[SerializeField] private Image fill;
	[SerializeField] private TextMeshProUGUI levelText;

	private IXPTable xP;
	public GameObject UIObject => transform.gameObject;
	private void Awake()
	{
		fill = transform.Find("XPBarFill").GetComponent<Image>();
		levelText = transform.Find("LevelText").GetComponent<TextMeshProUGUI>();
	}

	public void Initialize_UI(GameObject player)
	{
		if (player.TryGetComponent<IXPTable>(out IXPTable _xp))
			Initialize(_xp);
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
