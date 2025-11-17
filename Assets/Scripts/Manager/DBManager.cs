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



public static class DBManager
{
	private static int projectileSurvivalTime = 30;
	public static int ProjectileSurvivalTime => projectileSurvivalTime;
}

