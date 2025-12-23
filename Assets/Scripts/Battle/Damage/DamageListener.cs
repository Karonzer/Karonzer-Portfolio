using UnityEngine;

/// <summary>
/// IDamageable(플레이어/적 등)의 OnDamaged 이벤트를 구독해서
/// 데미지 팝업 시스템(DamagePopupManager)으로 전달하는 브릿지 컴포넌트.
/// 
/// - 대상 오브젝트에 붙여두면, 피격 시 자동으로 데미지 숫자를 띄워줌.
/// - 실제 팝업 생성/풀링은 DamagePopupManager가 담당.
/// </summary>
public class DamageListener : MonoBehaviour
{
	// 이 컴포넌트가 붙은 오브젝트의 데미지 인터페이스
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

	/// <summary>
	/// 피격 이벤트 수신 → DamagePopupManager로 전달
	/// </summary>
	private void HandleDamaged(DamageInfo _damageInfo)
	{
		if (BattleGSC.Instance.damagePopupManager == null)
			return;

		// 데미지 값/히트 위치/공격자 타입/크리티컬 여부를 팝업으로 표시
		BattleGSC.Instance.damagePopupManager.Show_Damage(_damageInfo.damage, _damageInfo.hitPoint, _damageInfo.attacker, _damageInfo.isCritical);
	}
}
