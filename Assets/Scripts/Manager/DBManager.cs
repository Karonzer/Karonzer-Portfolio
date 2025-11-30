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
	die,
}

public enum UpgradeOptionType
{
	SkillUpgrade,   // 기존 스킬 강화
	SkillUnlock,     // 새로운 스킬 획득
	Global
}
public enum UpgradeEffectType
{
	DamagePercent,          // 데미지 % 증가
	AttackSpeedPercent,     // 공격 간격 % 감소
	RangeFlat,              // 사거리 고정 증가
	ProjectileSpeedPercent, // 투사체 속도 % 증가
	ExplosionRangeFlat,     // 폭발 범위 고정 증가
	ExtraProjectileCount,    // 추가 발사 수 등

	MoveSpeed,          // 이동 % 속도
	CurrentHPAndMaxHP,  // 체력 % 증가
	CriticalDamage,     // 치명타 % 증가
	CriticalChance,     // 치명타 확률 % 증가
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
	public string startAttackObj;
}

[System.Serializable]
public struct EnemyStruct
{
	public string key;
	public float moveSpeed;
	public float maxHP;
	public float currentHP;
	public float attackInterval;
	public int damage;
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
	public string projectileKey;
	public int ProjectileCount;
}

public struct DamageInfo
{
	public int damage;
	public Vector3 hitPoint;
	public Type attacker;
	public bool isCritical;

	public DamageInfo(int damage, Vector3 hitPoint, Type attacker, bool isCritical)
	{
		this.damage = damage;
		this.hitPoint = hitPoint;
		this.attacker = attacker;
		this.isCritical = isCritical;
	}
}



public interface IHealthChanged
{
	public event Action<float, float> OnHealthChanged;
	float CurrentHPHealth { get; }
	float MaxHPHealth { get; }
}

public interface IDamageable
{
	float CurrentHPDamege { get; }
	float MaxHPDamege { get; }
	GameObject CurrentObj { get; }
	event Action<DamageInfo> OnDamaged;
	event Action<IDamageable> OnDead;

	void Take_Damage(DamageInfo info);
}

public interface IEnemyDoAttack
{
	public void DoAttack();
	public EnemyStruct EnemyStruct { get; }
}

public interface IUIInitializable
{
	void Initialize_UI(GameObject _player);
}

public static class DBManager
{
	private static int projectileSurvivalTime = 30;
	public static int ProjectileSurvivalTime => projectileSurvivalTime;

	private static System.Random rand = new System.Random();
	public static int CalculateCriticalDamage(this PlayerStruct stats, int baseDamage, out bool isCritical)
	{
		int chance = rand.Next(0, 100);

		isCritical = chance < stats.criticalChance;

		if (isCritical)
		{
			float multiplier = 1f + (stats.criticalDamage / 100f);
			return Mathf.RoundToInt(baseDamage * multiplier);
		}
		else
		{
			return baseDamage;
		}
	}


}

