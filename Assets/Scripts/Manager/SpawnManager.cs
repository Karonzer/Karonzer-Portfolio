using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;
public class SpawnManager : MonoBehaviour
{
	Dictionary<string, AsyncOperationHandle<GameObject>> prefabHandles;

	private Dictionary<PoolObjectType, Dictionary<string, Queue<GameObject>>> pools;
	private Dictionary<PoolObjectType, Transform> parents;

	private void Start()
	{
		prefabHandles = new Dictionary<string, AsyncOperationHandle<GameObject>>();

		pools = new Dictionary<PoolObjectType, Dictionary<string, Queue<GameObject>>>()
		{
			{ PoolObjectType.Enemy, new Dictionary<string, Queue<GameObject>>() },
			{ PoolObjectType.Projectile, new Dictionary<string, Queue<GameObject>>() },
			{ PoolObjectType.Item, new Dictionary<string, Queue<GameObject>>() }
		};

		parents = new Dictionary<PoolObjectType, Transform>()
		{
			{ PoolObjectType.Enemy, transform.Find("EnemySpawnGroup") },
			{ PoolObjectType.Projectile, transform.Find("ProjectileSpawn") },
			{ PoolObjectType.Item, transform.Find("ItemSpawnGroup") }
		};
	}

	private void Awake()
	{
		GSC.Instance.RegisterSpawn(this);
	}

	private void OnDestroy()
	{
		// 1) 모든 풀 안의 모든 오브젝트 삭제
		if (pools != null)
		{
			foreach (var typePool in pools) // Enemy / Projectile / Item
			{
				foreach (var kvp in typePool.Value) // 이름별 풀
				{
					while (kvp.Value.Count > 0)
					{
						var obj = kvp.Value.Dequeue();
						if (obj != null)
						{
							Destroy(obj);
						}
					}
				}
			}
		}

		// 2) Addressables 핸들 해제
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

	private GameObject LoadPrefab(string address)
	{
		if (!prefabHandles.ContainsKey(address))
		{
			var handle = Addressables.LoadAssetAsync<GameObject>(address);
			handle.WaitForCompletion();
			prefabHandles[address] = handle;
		}

		return prefabHandles[address].Result;
	}

	private GameObject CreateObject(PoolObjectType type, string name)
	{
		GameObject prefab = LoadPrefab(name);
		Transform parent = parents[type];

		GameObject obj = Instantiate(prefab, Vector3.zero, Quaternion.identity, parent);
		obj.SetActive(false);

		return obj;
	}

	public GameObject Spawn(PoolObjectType type, string name)
	{
		var dict = pools[type];

		// 풀에 해당 이름이 없으면 새로 만든다
		if (!dict.ContainsKey(name))
		{
			dict[name] = new Queue<GameObject>();
		}

		// 풀에 비어있으면 1~3개 추가 생성
		if (dict[name].Count == 0)
		{
			for (int i = 0; i < 3; i++)
			{
				dict[name].Enqueue(CreateObject(type, name));
			}
		}

		GameObject obj = dict[name].Dequeue();
		obj.SetActive(true);
		return obj;
	}

	public void DeSpawn(PoolObjectType type, string name, GameObject obj)
	{
		obj.SetActive(false);
		pools[type][name].Enqueue(obj);
	}

	public void Despawn_All()
	{
		Despawn_ByType(PoolObjectType.Enemy);
		Despawn_ByType(PoolObjectType.Projectile);
		Despawn_ByType(PoolObjectType.Item);
	}

	private void Despawn_ByType(PoolObjectType type)
	{
		if (!parents.ContainsKey(type))
			return;

		Transform parent = parents[type];

		foreach (Transform child in parent)
		{
			if (child.gameObject.activeSelf)
			{
				child.gameObject.SetActive(false);
			}
		}
	}

}
