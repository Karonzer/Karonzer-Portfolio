using UnityEngine;
using UnityEngine.UI;

public class HpBarFollower : MonoBehaviour
{
	private IHealthChanged healthChanged;
	[SerializeField] private Transform target;  
	[SerializeField] private Camera mainCam;

	[SerializeField] private RectTransform rect;
	[SerializeField] private Image hpFillImage;

	private void Awake()
	{
		if (mainCam == null) mainCam = Camera.main;
	}

	private void Start()
	{
		target = GSC.Instance.gameManager.player.transform;
		healthChanged = GSC.Instance.gameManager.player.GetComponent<IHealthChanged>();
		rect = GetComponent<RectTransform>();
		hpFillImage = rect.Find("HPBarFill").GetComponent<Image>();
	}

	private void OnEnable()
	{
		if(healthChanged != null)
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
