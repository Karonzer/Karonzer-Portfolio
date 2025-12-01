using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HpHUD : MonoBehaviour, IUIInitializable
{
	private IHealthChanged healthChanged;
	[SerializeField] private Image hpFillImage;
	[SerializeField] private TextMeshProUGUI textMeshProUGUI;

	public GameObject UIObject => transform.gameObject;

	private void Awake()
	{
		hpFillImage = transform.Find("HPBarFill").GetComponent<Image>();
		textMeshProUGUI = transform.Find("HPText").GetComponent<TextMeshProUGUI>();
	}

	public void Initialize_UI(GameObject _player)
	{
		if (_player.TryGetComponent<IHealthChanged>(out IHealthChanged _healthChanged))
			Initialize(_healthChanged);
	}

	public void Initialize(IHealthChanged _healthChanged)
	{
		healthChanged = _healthChanged;
		healthChanged.OnHealthChanged += HandleHealthChanged;
		HandleHealthChanged(healthChanged.CurrentHPHealth, healthChanged.MaxHPHealth);
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
		textMeshProUGUI.text = $"{healthChanged.CurrentHPHealth} / {healthChanged.MaxHPHealth}";
		hpFillImage.fillAmount = Mathf.Clamp01(ratio);
	}


}
