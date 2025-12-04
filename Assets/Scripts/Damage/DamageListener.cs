using UnityEngine;

public class DamageListener : MonoBehaviour
{
	private IDamageable target;

	void Awake()
	{
		target = GetComponent<IDamageable>();
	}

	void OnEnable()
	{
		target.OnDamaged += HandleDamaged;
	}

	void OnDisable()
	{
		target.OnDamaged -= HandleDamaged;
	}

	private void HandleDamaged(DamageInfo _damageInfo)
	{
		if (BattleGSC.Instance.damagePopupManager == null)
			return;

		BattleGSC.Instance.damagePopupManager.Show_Damage(_damageInfo.damage, _damageInfo.hitPoint, _damageInfo.attacker, _damageInfo.isCritical);
	}
}
