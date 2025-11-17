using UnityEngine;

public enum PoolObjectType
{
	Projectile,
	Enemy,
	Item,
}

public enum AttackStatType
{
	Damage,
	AttackInterval,
	Range,
	ProjectileSpeed,
	ExplosionRange
}

[System.Serializable]
public struct EnemyStruct
{
	public string key;
	public float moveSpeed;
	public float currentHP;
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



public interface IDamageable
{
	public void Take_Damage(int damageInfo);
}

public static class DBManager
{
	private static int projectileSurvivalTime = 30;
	public static int ProjectileSurvivalTime => projectileSurvivalTime;
}

