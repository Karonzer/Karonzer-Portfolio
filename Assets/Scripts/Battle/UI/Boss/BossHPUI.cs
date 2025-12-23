using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 보스 한 개의 HP를 표시하는 UI.
/// 
/// 흐름:
/// - Setting_BossHPUI로 보스(IHealthChanged) 바인딩
/// - OnHealthChanged로 게이지/텍스트 갱신
/// - OnDead 수신 시 참조 해제 후 OnDead 이벤트 발행(상위 HUD에 알림)
/// </summary>
public class BossHPUI : MonoBehaviour
{
	[SerializeField] private Image hpFillImage;
	[SerializeField] private IHealthChanged boss;
	[SerializeField] private TextMeshProUGUI textMeshProUGUI;

	// HUD(BossHPHUD)에게 죽었으니 UI 반환해줘 알리기 위한 이벤트
	public event Action OnDead;
	 
	private void Awake()
	{
		hpFillImage = transform.Find("HPBarBG").GetChild(0).GetComponent<Image>();
		textMeshProUGUI = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
	}

	/// <summary>
	/// 보스 참조 연결 및 이벤트 구독
	/// </summary>
	public void Setting_BossHPUI(IHealthChanged _boss)
	{
		boss = _boss;
		boss.OnHealthChanged += HandleHealthChanged;
		boss.OnDead += Dead_Boss;
		HandleHealthChanged(boss.CurrentHPHealth, boss.MaxHPHealth);
	}

	void OnDisable()
	{
		Clear_BossReference();
	}

	/// <summary>
	/// 보스 hp UI에 반영
	/// </summary>
	private void HandleHealthChanged(float current, float max)
	{
		if (max <= 0) return;

		float ratio = (float)current / max;
		hpFillImage.fillAmount = Mathf.Clamp01(ratio);
		textMeshProUGUI.text = $"{boss.CurrentHPHealth} / {boss.MaxHPHealth}";
	}

	/// <summary>
	/// 보스 사망 콜백
	/// </summary>
	private void Dead_Boss(IDamageable _damageable)
	{
		Clear_BossReference();
		OnDead?.Invoke();
	}

	/// <summary>
	/// 보스 이벤트 구독 해제 및 참조 제거
	/// - 풀링/비활성화 재사용 대비
	/// </summary>
	public void Clear_BossReference()
	{
		if (boss != null)
		{
			boss.OnHealthChanged -= HandleHealthChanged;
			boss.OnDead -= Dead_Boss;
		}

		boss = null;
	}
}
