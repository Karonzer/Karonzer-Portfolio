using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PenetratingProjectile : Projectile
{
	private Coroutine moveRoutine;
	private Coroutine projectileSurvivalTimeCoroutine;

	private SphereCollider sphereCollider;
	private void Awake()
	{
		sphereCollider = GetComponent<SphereCollider>();
		sphereCollider.isTrigger = true;
		audioHandler = GetComponent<IAudioHandler>();
	}


	private void OnDisable()
	{
		if (projectileSurvivalTimeCoroutine != null)
		{
			StopCoroutine(projectileSurvivalTimeCoroutine);
			projectileSurvivalTimeCoroutine = null;
		}
	}

	public override void Set_ProjectileInfo(string _projectileName, int _projectileDemage, float _projectileRange, Vector3 _dir, float _projectileSpeed, int _projectileSurvivalTime, Vector3 _spawnPos)
	{
		projectileName = _projectileName;
		projectileDemage = _projectileDemage;
		projectileRange = _projectileRange;
		projectileDir = _dir;
		projectileSpeed = _projectileSpeed;
		projectileSurvivalTime = _projectileSurvivalTime;
		transform.position = _spawnPos;

		Setting_CurrentProjectile();
	}

	private void Setting_CurrentProjectile()
	{
		sphereCollider.enabled = true;
	}

	public override void Launch_Projectile()
	{
		if (moveRoutine != null)
		{
			StopCoroutine(moveRoutine);
			moveRoutine = null;
		}

		if (projectileSurvivalTimeCoroutine != null)
		{
			StopCoroutine(projectileSurvivalTimeCoroutine);
			projectileSurvivalTimeCoroutine = null;
		}

		audioHandler.Play_OneShot(SoundType.Skill_Penetrating);
		moveRoutine = StartCoroutine(Start_MoveFireballProjectile());
		projectileSurvivalTimeCoroutine = StartCoroutine(Start_ProjectileSurvivalTimeCoroutine());
	}

	private IEnumerator Start_MoveFireballProjectile()
	{

		while (true)
		{
			if (BattleGSC.Instance.gameManager != null && !BattleGSC.Instance.gameManager.isPaused)
			{
				transform.rotation = Quaternion.LookRotation(projectileDir);
				transform.Translate(projectileDir * projectileSpeed * Time.deltaTime, Space.World);
				yield return null;
			}
			else
			{
				yield return null;
			}
		}
	}

	private IEnumerator Start_ProjectileSurvivalTimeCoroutine()
	{
		yield return new WaitForSeconds(projectileSurvivalTime - 10);
		transform.gameObject.SetActive(false);
		BattleGSC.Instance.spawnManager.DeSpawn(PoolObjectType.Projectile, projectileName, transform.gameObject);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
			return;

		if (other != null && other.CompareTag("Enemy"))
		{
			if (other.TryGetComponent<IDamageable>(out IDamageable _damageable))
			{
				DamageInfo info = BattleGSC.Instance.gameManager.Get_PlayerDamageInfo(projectileDemage, transform.gameObject, Type.Enemy);
				_damageable.Take_Damage(info);
			}
		}


	}


}
