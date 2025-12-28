using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class TitleSpawnManager : MonoBehaviour
{

	[SerializeField] private GameObject currentObj;

	// Addressables로 로드한 프리팹 핸들을 저장
	private Dictionary<string, AsyncOperationHandle<GameObject>> prefabHandles;
	private Dictionary<string, GameObject> pools;

	[SerializeField] private Transform spawnPos;

	private void Awake()
	{
		// 프리팹 핸들 캐시 테이블 생성
		prefabHandles = new Dictionary<string, AsyncOperationHandle<GameObject>>();
		pools = new Dictionary<string, GameObject>();

		currentObj = null;
	}


	private void OnDestroy()
	{
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
	/// 실제 오브젝트를 새로 1개 생성(비동기)
	/// - 프리팹 로드만 비동기, Instantiate는 메인스레드에서 실행
	/// </summary>
	private async Awaitable<GameObject> CreateObjectAsync(string name)
	{
		GameObject prefab = await LoadPrefabAsync(name);

		GameObject obj = Instantiate(prefab, spawnPos.position, Quaternion.Euler(0,0,0), spawnPos);
		obj.SetActive(false);

		return obj;
	}


	/// <summary>
	/// 풀에서 오브젝트를 꺼내서 반환(비동기 버전)
	/// - 프리팹 로딩/생성을 await로 처리 가능
	/// </summary>
	public async void SpawnAsync(string name)
	{
		if (pools.ContainsKey(name))
		{
			currentObj.SetActive(false);
			currentObj = pools[name];
			currentObj.SetActive(true);
		}
		else
		{
			if(currentObj != null)
			{
				currentObj.SetActive(false);
			}
			var obj = await CreateObjectAsync(name);
			pools.Add(name, obj);
			currentObj = obj;
			currentObj.SetActive(true);
		}
	}

}
