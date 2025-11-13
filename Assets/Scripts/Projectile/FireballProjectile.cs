using UnityEngine;
using System.Collections;
public class FireballProjectile : Projectile
{
	private Coroutine moveRoutine;
	private Coroutine projectileSurvivalTimeCoroutine;

	private SphereCollider sphereCollider;

	private void Awake()
	{
		sphereCollider = GetComponent<SphereCollider>();
		sphereCollider.isTrigger = true;
	}

	public override void Set_ProjectileInfo(string _projectileName, int _projectileDemage, float _projectileRange, Vector3 _dir, float _projectileSpeed, int _projectileSurvivalTime, Vector3 spawnPos)
	{
		projectileName = _projectileName;
		projectileDemage = _projectileDemage;
		projectileRange = _projectileRange;
		projectileDir = _dir;
		projectileSpeed = _projectileSpeed;
		projectileSurvivalTime = _projectileSurvivalTime;
		transform.position = spawnPos;
	}

	public override void fire()
	{
		if(moveRoutine != null)
		{
			StopCoroutine(moveRoutine);
			moveRoutine = null;
		}

		if(projectileSurvivalTimeCoroutine != null)
		{
			StopCoroutine(projectileSurvivalTimeCoroutine);
			projectileSurvivalTimeCoroutine = null;
		}
		moveRoutine = StartCoroutine(Start_MoveFireballProjectile());
		projectileSurvivalTimeCoroutine = StartCoroutine(Start_ProjectileSurvivalTimeCoroutine());
	}

	private IEnumerator Start_MoveFireballProjectile()
	{
		while (true)
		{
			transform.Translate(projectileDir * projectileSpeed * Time.deltaTime);
			yield return null;
		}
	}

	private IEnumerator Start_ProjectileSurvivalTimeCoroutine()
	{
		yield return new WaitForSeconds(projectileSurvivalTime);
		transform.gameObject.SetActive(false);
		GSC.Instance.Spawn.DeSpawn_Projectile(projectileName, transform.gameObject);
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player"))
			return;

		if(other.CompareTag("Enemy"))
		{

		}
		GSC.Instance.Spawn.DeSpawn_Projectile(projectileName, transform.gameObject);
	}
}
