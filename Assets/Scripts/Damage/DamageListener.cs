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

	private void HandleDamaged(int damage, Vector3 hitPos, Type _type)
	{
		if (GSC.Instance.damagePopupManager == null)
			return;

		GSC.Instance.damagePopupManager.Show_Damage(damage, hitPos, _type);
	}
}
