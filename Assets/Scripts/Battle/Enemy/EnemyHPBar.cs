using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// EnemyHPBar
/// 
/// 적 머리 위에 표시되는 HP 바(UI)를 제어하는 스크립트.
/// - 적의 체력 변경 이벤트를 구독해서 HP 바를 갱신한다.
/// - 월드 공간 UI이므로 항상 카메라를 바라보게 한다(Billboard).
/// 
/// 전제:
/// - 부모 오브젝트에 IHealthChanged(Enemy)가 존재
/// - 자식에 HPBarFill이라는 Image가 존재
/// </summary>
public class EnemyHPBar : MonoBehaviour
{
	[SerializeField] private Image hpFillImage;
	[SerializeField] private IHealthChanged enemy;

	private Camera mainCam;

	void Awake()
	{
		// enemy가 직접 할당되지 않았다면,
		// 부모 오브젝트에서 IHealthChanged 구현체(Enemy)를 찾는다.
		if (enemy == null)
			enemy = GetComponentInParent<IHealthChanged>();

		mainCam = Camera.main;
		hpFillImage = transform.Find("HPBarFill").GetComponent<Image>();
	}

	void OnEnable()
	{
		if (enemy != null)
		{
			enemy.OnHealthChanged += HandleHealthChanged;
			HandleHealthChanged(enemy.CurrentHPHealth, enemy.MaxHPHealth);
		}
	}

	void OnDisable()
	{
		if (enemy != null)
			enemy.OnHealthChanged -= HandleHealthChanged;
	}

	/// <summary>
	/// 체력 변경 시 호출되는 콜백
	/// - current / max 비율로 HP 바 fillAmount 계산
	/// </summary>
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
