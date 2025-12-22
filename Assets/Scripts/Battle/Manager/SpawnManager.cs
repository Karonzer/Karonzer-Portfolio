using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary>
/// SpawnManager
/// 
/// 오브젝트 풀링을 담당하는 매니저.
/// - Enemy / Projectile / Item / Actor 등을 미리 만들어두고 재사용한다.
/// - Addressables로 프리팹을 로드하고 캐싱(prefabHandles)한다.
/// - Spawn()으로 꺼내 쓰고, DeSpawn()으로 다시 풀에 넣는다.
/// 
/// </summary>
public class SpawnManager : MonoBehaviour
{
	// Addressables로 로드한 프리팹 핸들을 저장
	Dictionary<string, AsyncOperationHandle<GameObject>> prefabHandles;

	// 실제 오브젝트 풀
	private Dictionary<PoolObjectType, Dictionary<string, Queue<GameObject>>> pools;

	// 타입별로 생성될 부모 Transform
	private Dictionary<PoolObjectType, Transform> parents;

	/// <summary>
	/// - BattleGSC에 자신을 등록하여 전역 접근 가능하도록 한다
	/// </summary>
	private void Awake()
	{
		BattleGSC.Instance.RegisterSpawn(this);
	}

	private void Start()
	{
		// 프리팹 핸들 캐시 테이블 생성
		prefabHandles = new Dictionary<string, AsyncOperationHandle<GameObject>>();

		// 타입별 풀 테이블 생성
		pools = new Dictionary<PoolObjectType, Dictionary<string, Queue<GameObject>>>()
		{
			{ PoolObjectType.Enemy, new Dictionary<string, Queue<GameObject>>() },
			{ PoolObjectType.Projectile, new Dictionary<string, Queue<GameObject>>() },
			{ PoolObjectType.Item, new Dictionary<string, Queue<GameObject>>() },
			{ PoolObjectType.Actor, new Dictionary<string, Queue<GameObject>>() }
		};

		// 타입별 부모 Transform 지정
		parents = new Dictionary<PoolObjectType, Transform>()
		{
			{ PoolObjectType.Enemy, transform.Find(PoolObjectTypeSpawnParents.EnemySpawnGroup.ToString()) },
			{ PoolObjectType.Projectile, transform.Find(PoolObjectTypeSpawnParents.ProjectileSpawn.ToString()) },
			{ PoolObjectType.Item, transform.Find(PoolObjectTypeSpawnParents.ItemSpawnGroup.ToString()) },
			{ PoolObjectType.Actor, transform.Find(PoolObjectTypeSpawnParents.ActorSpawnGroup.ToString()) }
		};
	}



	private void OnDestroy()
	{
		// 모든 풀 안의 모든 오브젝트 삭제
		// - 씬 종료 시 메모리/오브젝트 잔존 방지
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

		// Addressables 핸들 해제
		// - 로드한 프리팹 리소스를 제대로 해제하기 위함
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

	/// <summary>
	/// 프리팹을 Addressables에서 로드(동기)하고 캐싱한다.
	/// - 같은 address를 여러 번 로드하지 않도록 prefabHandles에 저장
	/// </summary>
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

	/// <summary>
	/// 프리팹을 Addressables에서 로드(비동기)하고 캐싱한다.
	/// - 로드가 끝나면 prefab(GameObject)을 반환
	/// </summary>
	private async Awaitable<GameObject> LoadPrefabAsync(string address)
	{
		if (!prefabHandles.TryGetValue(address, out var handle))
		{
			handle = Addressables.LoadAssetAsync<GameObject>(address);
			prefabHandles[address] = handle;
			await handle.Task;
		}

		return handle.Result;
	}

	/// <summary>
	/// 실제 오브젝트를 새로 1개 생성(동기)
	/// - 프리팹 로드 → Instantiate → 비활성 상태로 보관
	/// </summary>
	private GameObject CreateObject(PoolObjectType type, string name)
	{
		GameObject prefab = LoadPrefab(name);
		Transform parent = parents[type];

		GameObject obj = Instantiate(prefab, Vector3.zero, Quaternion.identity, parent);
		obj.SetActive(false);

		return obj;
	}

	/// <summary>
	/// 실제 오브젝트를 새로 1개 생성(비동기)
	/// - 프리팹 로드만 비동기, Instantiate는 메인스레드에서 실행
	/// </summary>
	private async Awaitable<GameObject> CreateObjectAsync(PoolObjectType type, string name)
	{
		GameObject prefab = await LoadPrefabAsync(name);
		Transform parent = parents[type];

		GameObject obj = Instantiate(prefab, Vector3.zero, Quaternion.identity, parent);
		obj.SetActive(false);

		return obj;
	}

	/// <summary>
	/// 풀에서 오브젝트를 꺼내서 반환(동기)
	/// 흐름:
	/// 1) (타입,이름) 풀 없으면 생성
	/// 2) 풀에 비어있으면 미리 3개 만들어서 채움
	/// 3) 하나 꺼내서 활성화 후 반환
	/// </summary>
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


	/// <summary>
	/// 풀에서 오브젝트를 꺼내서 반환(비동기 버전)
	/// - 프리팹 로딩/생성을 await로 처리 가능
	/// </summary>
	public async Awaitable<GameObject> SpawnAsync(PoolObjectType type, string name)
	{
		var dict = pools[type];

		if (!dict.ContainsKey(name))
		{
			dict[name] = new Queue<GameObject>();
		}

		if (dict[name].Count == 0)
		{
			for (int i = 0; i < 3; i++)
			{
				var obj = await CreateObjectAsync(type, name);
				dict[name].Enqueue(obj);
			}
		}

		GameObject result = dict[name].Dequeue();
		result.SetActive(true);
		return result;
	}

	/// <summary>
	/// 사용이 끝난 오브젝트를 풀에 다시 넣는다.
	/// - 비활성화 후 Queue에 Enqueue
	/// </summary>
	public void DeSpawn(PoolObjectType type, string name, GameObject obj)
	{
		obj.SetActive(false);
		pools[type][name].Enqueue(obj);
	}

	/// <summary>
	/// 현재 씬에 활성화된 오브젝트들을 전부 비활성화한다.
	/// - GameOver 등에서 필드 정리 용도로 사용
	/// </summary>
	public void Despawn_All()
	{
		Despawn_ByType(PoolObjectType.Enemy);
		Despawn_ByType(PoolObjectType.Projectile);
		Despawn_ByType(PoolObjectType.Item);
	}

	/// <summary>
	/// 특정 타입의 부모 밑에 있는 모든 자식 오브젝트를 비활성화한다.
	/// 오브젝트가 비활성화가 되면 DeSpawn되어 다시 플에 넣는다
	/// </summary>
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

	/// <summary>
	/// Item 타입의 부모 Transform을 외부로 전달한다.
	/// - 아이템 그룹에 접근이 필요할 때 사용
	/// </summary>
	public bool Get_ItemParents(out Transform _tra)
	{
		_tra = null;
		if (parents.TryGetValue(PoolObjectType.Item, out Transform _value))
		{
			_tra = _value;
			return true;
		}
		else
		{
			return false;
		}

	}


}
