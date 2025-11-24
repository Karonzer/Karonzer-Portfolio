using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class XPBarUI : MonoBehaviour
{
	[SerializeField] private Image fill;
	[SerializeField] private TextMeshProUGUI levelText;

	private IXPTable xP;

	private void Awake()
	{
		fill = transform.GetChild(0).Find("XPBarBG").Find("XPBarFill").GetComponent<Image>();
		levelText = transform.GetChild(0).Find("XPBarBG").Find("LevelText").GetComponent<TextMeshProUGUI>();
	}

	public void Initialize(IXPTable _system)
	{
		Debug.Log(_system);
		xP = _system;

		xP.OnXPChanged += HandleXPChanged;
		xP.OnLevelChanged += HandleLevelUp;

		HandleXPChanged(xP.CurrentXP, xP.MaxXP);
		HandleLevelUp(xP.CurrentLevel);
	}

	private void HandleXPChanged(int current, int max)
	{
		fill.fillAmount = (float)current / max;
	}

	private void HandleLevelUp(int level)
	{
		if (levelText != null)
			levelText.text = $"Lv : {level}";
	}
}
