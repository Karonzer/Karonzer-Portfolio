using UnityEngine;
using UnityEngine.UI;

public class HpBarFollower : MonoBehaviour, IUIHandler
{
	private IHealthChanged healthChanged;
	[SerializeField] private Transform target;  
	[SerializeField] private Camera mainCam;

	[SerializeField] private RectTransform rect;
	[SerializeField] private Image hpFillImage;
	public GameObject UIObject => transform.gameObject;

	public UIType Type => UIType.PlayerHPFollow;

	private void Awake()
	{
		if (mainCam == null) mainCam = Camera.main;
		rect = GetComponent<RectTransform>();
		hpFillImage = rect.Find("HPBarFill").GetComponent<Image>();
	}


	public void Show()
	{
		transform.gameObject.SetActive(true);
	}

	public void ShowAndInitialie(GameObject _obj = null)
	{
		transform.gameObject.SetActive(true);
		if (_obj.TryGetComponent<IHealthChanged>(out IHealthChanged _healthChanged))
			Initialize(_healthChanged, _obj.transform);
	}

	public void Hide()
	{
		transform.gameObject.SetActive(false);
	}


	public void Initialize(IHealthChanged _healthChanged, Transform _target)
	{
		healthChanged = _healthChanged;
		target = _target;

		healthChanged.OnHealthChanged += HandleHealthChanged;
		HandleHealthChanged(healthChanged.CurrentHPHealth, healthChanged.MaxHPHealth);
	}

	private void OnDisable()
	{
		if (healthChanged != null)
			healthChanged.OnHealthChanged -= HandleHealthChanged;
	}

	void LateUpdate()
	{
		Vector3 worldPos = target.position + new Vector3(0,-1.5f,0);
		Vector3 screenPos = mainCam.WorldToScreenPoint(worldPos);
		rect.position = screenPos;
	}

	private void HandleHealthChanged(float current, float max)
	{
		if (max <= 0) return;

		float ratio = (float)current / max;
		hpFillImage.fillAmount = Mathf.Clamp01(ratio);
	}

}
