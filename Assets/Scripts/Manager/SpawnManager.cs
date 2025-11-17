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
	Dictionary<string, AsyncOperationHandle<GameObject>> prefabHandles;


	private void Awake()
	{
		GSC.Instance.RegisterSpawn(this);
	}
	private void Start()
	{
		projectile = new Dictionary<string, Queue<GameObject>>();
		prefabHandles = new Dictionary<string, AsyncOperationHandle<GameObject>>();

		EnemySpawnGroup = transform.Find("EnemySpawnGroup");
		projectileSpawn = transform.Find("ProjectileSpawn");
	}

	private void OnEnable()
	{
		prefabKey = "FireballProjectile";
	}

	private void OnDestroy()
	{
		if (projectile != null)
		{
			foreach (var kvp in projectile)
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
			for (int i = 0; i < 3; i++)
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
		for (int i = 0; i < 3; i++)
		{
			GameObject obj = Create_ProjectileObject(_projectileName);
			gameObjects.Enqueue(obj);
		}
		projectile.Add(_projectileName, gameObjects);
		return projectile[_projectileName].Dequeue();
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
		projectile[_projectileName].Enqueue(_gameObject);
	}
}
