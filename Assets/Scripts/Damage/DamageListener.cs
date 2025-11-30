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
		if (GSC.Instance.damagePopupManager == null)
			return;

		GSC.Instance.damagePopupManager.Show_Damage(_damageInfo.damage, _damageInfo.hitPoint, _damageInfo.attacker, _damageInfo.isCritical);
	}
}
