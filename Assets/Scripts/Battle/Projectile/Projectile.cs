using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
	[SerializeField] protected string projectileName;
	[SerializeField] protected int projectileDemage;
	[SerializeField] protected float projectileRange;
	[SerializeField] protected Vector3 projectileDir;
	[SerializeField] protected float projectileSpeed;
	[SerializeField] protected int projectileSurvivalTime;
	public abstract void Set_ProjectileInfo(string _projectileName,int _projectileDemage, float _projectileRange, Vector3 _dir, float _projectileSpeed, int _projectileSurvivalTime, Vector3 _spawnPos);
	public abstract void Launch_Projectile();
}
