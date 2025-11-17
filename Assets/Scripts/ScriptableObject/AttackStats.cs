using UnityEngine;

[CreateAssetMenu(fileName = "AttackStats", menuName = "Skill/AttackStats")]
public class AttackStats : ScriptableObject
{
	[Header("Key (스킬 이름)")]
	public string key;

	[Header("Base")]
	public int baseDamage = 10;
	public float baseAttackInterval = 2f;
	public float baseRange = 5f;
	public float baseProjectileSpeed = 5f;
	public float baseExplosionRange = 1f;

	[Header("Current (런타임)")]
	public int currentDamage;
	public float currentAttackInterval;
	public float currentRange;
	public float currentProjectileSpeed;
	public float currentExplosionRange;

	public void ResetToBase()
	{
		currentDamage = baseDamage;
		currentAttackInterval = baseAttackInterval;
		currentRange = baseRange;
		currentProjectileSpeed = baseProjectileSpeed;
		currentExplosionRange = baseExplosionRange;
	}
}
