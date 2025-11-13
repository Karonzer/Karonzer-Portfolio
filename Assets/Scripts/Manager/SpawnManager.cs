using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;
public class SpawnManager : MonoBehaviour
{
	private string prefabKey;
	private GameObject spawnedObject;

	[SerializeField] private Transform EnemySpawnGroup;
	[SerializeField] private Transform projectileSpawn;

	Dictionary<string, Queue<GameObject>> projectile;
	private void Awake()
	{
		GSC.Instance.RegisterSpawn(this);
		projectile = new Dictionary<string, Queue<GameObject>>();

		EnemySpawnGroup= transform.Find("EnemySpawnGroup");
		projectileSpawn = transform.Find("ProjectileSpawn");
	}
	private void OnEnable()
	{
		prefabKey = "FireballProjectile";
	}
	public void Spawn()
	{
		Addressables.LoadAssetAsync<GameObject>(prefabKey).Completed += handle =>
		{
			var prefab = handle.Result;
			spawnedObject = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
		};
	}

	public GameObject Spawn_ProjectileSpawn(string _projectileName)
	{
		if (projectile.ContainsKey(_projectileName))
		{
			return Check_ProjectileSpawnCount(_projectileName);
		}
		else
		{
			return Create_Projectile(_projectileName);
		}
	}

	private GameObject Check_ProjectileSpawnCount(string _projectileName)
	{
		if(projectile[_projectileName].Count == 0)
		{
			for (int i = 0; i < 5; i++)
			{
				GameObject obj = Create_ProjectileObject(_projectileName);
				projectile[_projectileName].Enqueue(obj);
			}
			return projectile[_projectileName].Dequeue();
		}
		else
		{
			return projectile[_projectileName].Dequeue();
		}


	}	

	private GameObject Create_Projectile(string _projectileName)
	{
		Queue<GameObject> gameObjects = new Queue<GameObject>();
		for (int i = 0; i < 10; i++)
		{
			GameObject obj = Create_ProjectileObject(_projectileName);
			gameObjects.Enqueue(obj);
		}
		projectile.Add(_projectileName, gameObjects);
		return projectile[_projectileName].Dequeue();
	}

	private GameObject Create_ProjectileObject(string _projectileName)
	{
		GameObject obj = null;
		AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(_projectileName);
		handle.WaitForCompletion();
		var prefab = handle.Result;
		obj = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity, projectileSpawn);
		obj.SetActive(false);
		return obj;
	}

	public void DeSpawn_Projectile(string _projectileName,GameObject _gameObject)
	{
		_gameObject.SetActive(false);
		projectile[_projectileName].Enqueue(_gameObject);
	}
}
