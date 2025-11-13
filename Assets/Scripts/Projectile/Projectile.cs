using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
	protected PoolObjectType poolObjectType = PoolObjectType.Projectile;
	protected string projectileName;
	protected int projectileDemage;
	protected Vector3 projectileDir;
	protected float projectileSpeed;
	protected int projectileSurvivalTime;
	public abstract void Set_ProjectileInfo(string _projectileName,int _projectileDemage, Vector3 _dir, float _projectileSpeed, int _projectileSurvivalTime, Vector3 spawnPos);
	public abstract void fire();
}
