using UnityEngine;
using UnityEngine.UI;

public class EnemyHPBar : MonoBehaviour
{
	[SerializeField] private Image hpFillImage;
	[SerializeField] private Enemy enemy;

	private Camera mainCam;

	void Awake()
	{
		if (enemy == null)
			enemy = GetComponentInParent<Enemy>();

		mainCam = Camera.main;
		hpFillImage = transform.Find("HPBarFill").GetComponent<Image>();
	}

	void OnEnable()
	{
		if (enemy != null)
		{
			enemy.OnHealthChanged += HandleHealthChanged;
			HandleHealthChanged(enemy.CurrentHP, enemy.MaxHP);
		}
	}

	void OnDisable()
	{
		if (enemy != null)
			enemy.OnHealthChanged -= HandleHealthChanged;
	}

	private void HandleHealthChanged(float current, float max)
	{
		if (max <= 0) return;

		float ratio = (float)current / max;
		hpFillImage.fillAmount = Mathf.Clamp01(ratio);
	}

	// 여기는 여전히 매 프레임 카메라만 바라보게
	void LateUpdate()
	{
		if (mainCam != null)
			transform.forward = mainCam.transform.forward;
	}
}
