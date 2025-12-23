using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 특정 타겟(플레이어 등)을 따라다니는 월드→스크린 HP바.
/// 
/// 특징:
/// - IHealthChanged 이벤트로 HP 갱신
/// - LateUpdate에서 타겟 월드 좌표를 스크린 좌표로 변환해 위치 갱신
/// </summary>
public class HpBarFollower : MonoBehaviour, IUIHandler
{
	private IHealthChanged healthChanged;
	[SerializeField] private Transform target;  
	[SerializeField] private Camera mainCam;

	[SerializeField] private RectTransform rect;
	[SerializeField] private Image hpFillImage;
	public UIType Type => UIType.PlayerHPFollow;
	public bool IsOpen => transform.gameObject.activeSelf;
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


	/// <summary>
	/// HP 이벤트 구독 및 초기 값 반영
	/// </summary>
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
		// 타겟 위치 기준 오프셋
		Vector3 worldPos = target.position + new Vector3(0,-1.5f,0);

		// 월드 → 스크린 좌표 변환
		Vector3 screenPos = mainCam.WorldToScreenPoint(worldPos);
		rect.position = screenPos;
	}

	/// <summary>
	/// HP은 UI에 반영
	/// </summary>
	private void HandleHealthChanged(float current, float max)
	{
		if (max <= 0) return;

		float ratio = (float)current / max;
		hpFillImage.fillAmount = Mathf.Clamp01(ratio);
	}

}
