using System;
using UnityEngine;

public enum Type
{
	Player,
	Enemy,
}

public enum PoolObjectType
{
	player,
	Projectile,
	Enemy,
	Item,
}


public enum StateID
{
	tracking,
	Attack,
}

public interface IState<T>
{
	StateID ID { get; }
	void OnEnter(T owner);
	void OnExit(T owner);
	void Tick(T owner);
}

public interface IXPTable
{
	public int CurrentLevel { get; }
	public int CurrentXP { get; }
	public int MaxXP { get; }

	public event Action<int> OnLevelChanged;
	public event Action<int, int> OnXPChanged;
}

[System.Serializable]
public struct PlayerStruct
{
	public string key;
	public float moveSpeed;
	public float maxHP;
	public float currentHP;
	public int criticalDamage;
	public int criticalChance;
}

[System.Serializable]
public struct EnemyStruct
{
	public string key;
	public float moveSpeed;
	public float maxHP;
	public float currentHP;
	public float attackInterval;
	public float damage;
	public float attackRange;
	public int xpItmeValue;
}
[System.Serializable]
public struct AttackStats
{
	[Header("Key (스킬 이름)")]
	public string key;

	[Header("Base")]
	public int baseDamage;
	public float baseAttackInterval;
	public float baseRange;
	public float baseProjectileSpeed;
	public float baseExplosionRange;
}

public enum UpgradeEffectType
{
	DamagePercent,          // 데미지 % 증가
	AttackSpeedPercent,     // 공격 간격 % 감소
	RangeFlat,              // 사거리 고정 증가
	ProjectileSpeedPercent, // 투사체 속도 % 증가
	ExplosionRangeFlat,     // 폭발 범위 고정 증가
	ExtraProjectileCount,   // 추가 발사 수 등
}


public interface IDamageable
{
	event Action<int, Vector3,Type> OnDamaged;
	float CurrentHP { get; }
	float MaxHP { get; }
	public void Take_Damage(int damageInfo);
}

public static class DBManager
{
	private static int projectileSurvivalTime = 30;
	public static int ProjectileSurvivalTime => projectileSurvivalTime;

	public const string playerName = "Player";

	//공격 오브젝트
	public const string fireballAttack = "FireballAttack";

	// 발사체
	public const string fireballProjectile = "FireballProjectile";

	// 몬스터
	public const string enemyType1 = "EnemyType1";

	//데이지 팝업
	public const string popupPrefabKey = "DamagePopupPrefab";

	//아이템
	public const string xPItem = "XPitem";

}

