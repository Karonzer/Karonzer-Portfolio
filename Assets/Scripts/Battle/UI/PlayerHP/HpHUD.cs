using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 화면 고정형 플레이어 HP HUD.
/// 
/// 흐름:
/// - ShowAndInitialie(obj)에서 IHealthChanged를 받아 이벤트 구독
/// - 체력 변화 시 텍스트/게이지 갱신
/// </summary>
public class HpHUD : MonoBehaviour, IUIHandler
{
	private IHealthChanged healthChanged;
	[SerializeField] private Image hpFillImage;
	[SerializeField] private TextMeshProUGUI textMeshProUGUI;
	public UIType Type => UIType.PlayerHP;
	public bool IsOpen => transform.gameObject.activeSelf;
	private void Awake()
	{

		hpFillImage = transform.Find("HPBarFill").GetComponent<Image>();
		textMeshProUGUI = transform.Find("HPText").GetComponent<TextMeshProUGUI>();
	}

	public void Show()
	{
		transform.gameObject.SetActive(true);
	}

	public void ShowAndInitialie(GameObject _obj = null)
	{
		transform.gameObject.SetActive(true);
		if (_obj.TryGetComponent<IHealthChanged>(out IHealthChanged _healthChanged))
			Initialize(_healthChanged);
	}

	public void Hide()
	{
		transform.gameObject.SetActive(false);
	}

	/// <summary>
	/// HP 이벤트 구독 및 초기 값 반영
	/// </summary>
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

	/// <summary>
	/// HP은 UI에 반영
	/// </summary>
	private void HandleHealthChanged(float current, float max)
	{
		if (max <= 0) return;

		float ratio = (float)current / max;
		textMeshProUGUI.text = $"{healthChanged.CurrentHPHealth} / {healthChanged.MaxHPHealth}";
		hpFillImage.fillAmount = Mathf.Clamp01(ratio);
	}


}
