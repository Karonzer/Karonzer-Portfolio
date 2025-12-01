using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class BossHPUI : MonoBehaviour
{
	[SerializeField] private Image hpFillImage;
	[SerializeField] private IHealthChanged boss;
	[SerializeField] private TextMeshProUGUI textMeshProUGUI;

	public event Action OnDead;
	 
	void Awake()
	{
		hpFillImage = transform.Find("HPBarBG").GetChild(0).GetComponent<Image>();
		textMeshProUGUI = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
	}

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

	private void HandleHealthChanged(float current, float max)
	{
		if (max <= 0) return;

		float ratio = (float)current / max;
		hpFillImage.fillAmount = Mathf.Clamp01(ratio);
		textMeshProUGUI.text = $"{boss.CurrentHPHealth} / {boss.MaxHPHealth}";
	}

	private void Dead_Boss(IDamageable _damageable)
	{
		Clear_BossReference();
		OnDead?.Invoke();
	}

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
