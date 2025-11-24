using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HpHUD : MonoBehaviour
{
	private IHealthChanged healthChanged;
	[SerializeField] private Image hpFillImage;
	[SerializeField] private TextMeshProUGUI textMeshProUGUI;

	private void Start()
	{
		healthChanged = GSC.Instance.gameManager.player.GetComponent<IHealthChanged>();
		hpFillImage = transform.Find("HPBarFill").GetComponent<Image>();
		textMeshProUGUI = transform.Find("HPText").GetComponent<TextMeshProUGUI>();
	}

	private void OnEnable()
	{
		if (healthChanged != null)
		{
			healthChanged.OnHealthChanged += HandleHealthChanged;
			HandleHealthChanged(healthChanged.CurrentHPHealth, healthChanged.MaxHPHealth);
		}

	}

	private void OnDisable()
	{
		if (healthChanged != null)
			healthChanged.OnHealthChanged -= HandleHealthChanged;
	}

	private void HandleHealthChanged(float current, float max)
	{
		if (max <= 0) return;

		float ratio = (float)current / max;
		textMeshProUGUI.text = $"{healthChanged.MaxHPHealth} / {healthChanged.CurrentHPHealth}";
		hpFillImage.fillAmount = Mathf.Clamp01(ratio);
	}
}
