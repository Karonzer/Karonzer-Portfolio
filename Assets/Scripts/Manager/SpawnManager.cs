using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;
public class SpawnManager : MonoBehaviour
{


	[SerializeField] private Transform EnemySpawnGroup;
	[SerializeField] private Transform projectileSpawn;

	Dictionary<string, AsyncOperationHandle<GameObject>> prefabHandles;

	Dictionary<string, Queue<GameObject>> enemyPool;

	Dictionary<string, Queue<GameObject>> projectilePool;


	private void Awake()
	{
		GSC.Instance.RegisterSpawn(this);
	}
	private void Start()
	{
		prefabHandles = new Dictionary<string, AsyncOperationHandle<GameObject>>();

		enemyPool = new Dictionary<string, Queue<GameObject>>();
		projectilePool = new Dictionary<string, Queue<GameObject>>();


		EnemySpawnGroup = transform.Find("EnemySpawnGroup");
		projectileSpawn = transform.Find("ProjectileSpawn");
	}


	private void OnDestroy()
	{
		if (projectilePool != null)
		{
			foreach (var kvp in projectilePool)
			{
				while (kvp.Value.Count > 0)
				{
					var obj = kvp.Value.Dequeue();
					if (obj != null)
						Destroy(obj);
				}
			}
		}

		if (prefabHandles != null)
		{
			foreach (var kvp in prefabHandles)
			{
				var handle = kvp.Value;
				if (handle.IsValid())
				{
					Addressables.Release(handle);
				}
			}

			prefabHandles.Clear();
		}
	}

	public void Spawn()
	{
		//Addressables.LoadAssetAsync<GameObject>(prefabKey).Completed += handle =>
		//{
		//	var prefab = handle.Result;
		//	spawnedObject = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
		//};
	}

	public GameObject Spawn_ProjectileSpawn(string _projectileName)
	{
		if (projectilePool.ContainsKey(_projectileName))
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
		if(projectilePool[_projectileName].Count == 0)
		{
			for (int i = 0; i < 3; i++)
			{
				GameObject obj = Create_ProjectileObject(_projectileName);
				projectilePool[_projectileName].Enqueue(obj);
			}
			return projectilePool[_projectileName].Dequeue();
		}
		else
		{
			return projectilePool[_projectileName].Dequeue();
		}


	}	

	private GameObject Create_Projectile(string _projectileName)
	{
		Queue<GameObject> gameObjects = new Queue<GameObject>();
		for (int i = 0; i < 3; i++)
		{
			GameObject obj = Create_ProjectileObject(_projectileName);
			gameObjects.Enqueue(obj);
		}
		projectilePool.Add(_projectileName, gameObjects);
		return projectilePool[_projectileName].Dequeue();
	}

	private GameObject Create_ProjectileObject(string _projectileName)
	{
		if (!prefabHandles.ContainsKey(_projectileName))
		{
			AsyncOperationHandle<GameObject> handle =
				Addressables.LoadAssetAsync<GameObject>(_projectileName);
			handle.WaitForCompletion();

			prefabHandles[_projectileName] = handle;
		}

		var prefab = prefabHandles[_projectileName].Result;

		GameObject obj = Instantiate(
			prefab,
			new Vector3(0, 0, 0),
			Quaternion.identity,
			projectileSpawn
		);

		obj.SetActive(false);
		return obj;
	}

	public void DeSpawn_Projectile(string _projectileName,GameObject _gameObject)
	{
		_gameObject.SetActive(false);
		projectilePool[_projectileName].Enqueue(_gameObject);
	}



	public GameObject Spawn_EnemySpawn(string _enemyName)
	{
		if (enemyPool.ContainsKey(_enemyName))
		{
			return Check_EnemySpawnCount(_enemyName);
		}
		else
		{
			return Create_Enemy(_enemyName);
		}
	}

	private GameObject Check_EnemySpawnCount(string _enemyName)
	{
		if (enemyPool[_enemyName].Count == 0)
		{
			GameObject obj = Create_EnemyObject(_enemyName);
			enemyPool[_enemyName].Enqueue(obj);
			return enemyPool[_enemyName].Dequeue();
		}
		else
		{
			return enemyPool[_enemyName].Dequeue();
		}


	}

	private GameObject Create_Enemy(string _enemyName)
	{
		Queue<GameObject> gameObjects = new Queue<GameObject>();
		GameObject obj = Create_EnemyObject(_enemyName);
		gameObjects.Enqueue(obj);
		enemyPool.Add(_enemyName, gameObjects);
		return enemyPool[_enemyName].Dequeue();
	}

	private GameObject Create_EnemyObject(string _enemyName)
	{
		if (!prefabHandles.ContainsKey(_enemyName))
		{
			AsyncOperationHandle<GameObject> handle =
				Addressables.LoadAssetAsync<GameObject>(_enemyName);
			handle.WaitForCompletion();

			prefabHandles[_enemyName] = handle;
		}

		var prefab = prefabHandles[_enemyName].Result;

		GameObject obj = Instantiate(
			prefab,
			new Vector3(0, 0, 0),
			Quaternion.identity,
			EnemySpawnGroup
		);

		obj.SetActive(false);
		return obj;
	}
	public void DeSpawn_Enemy(string _enemyName, GameObject _gameObject)
	{
		_gameObject.SetActive(false);
		enemyPool[_enemyName].Enqueue(_gameObject);
	}


}
