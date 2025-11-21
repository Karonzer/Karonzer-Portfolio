using System;
using UnityEngine;

public enum Type
{
	Player,
	Enemy,
}

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

}

