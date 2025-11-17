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

public interface IDamageable
{
	public void Take_Damage(int damageInfo);
}

public static class DBManager
{
	private static int projectileSurvivalTime = 30;
	public static int ProjectileSurvivalTime => projectileSurvivalTime;
}

