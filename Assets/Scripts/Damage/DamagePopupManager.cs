using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class DamagePopupManager : MonoBehaviour
{

	[SerializeField] private GameObject loadedPrefab;
	[SerializeField] private int initialPoolSize = 20;

	private Queue<GameObject> pool = new Queue<GameObject>();
	private Camera mainCam;
	private bool isLoaded = false;

	void Awake()
	{
		GSC.Instance.RegisterDamagePopupManager(this);
		mainCam = Camera.main;
		LoadPopupPrefab();
	}


	private void OnDestroy()
	{
		if (loadedPrefab != null)
		{
			Addressables.Release(loadedPrefab);
		}

	}

	private void LoadPopupPrefab()
	{
		Addressables.LoadAssetAsync<GameObject>(DBManager.popupPrefabKey).Completed += OnPrefabLoaded;
	}

	private void OnPrefabLoaded(AsyncOperationHandle<GameObject> op)
	{
		if (op.Status == AsyncOperationStatus.Succeeded)
		{
			loadedPrefab = op.Result;
			isLoaded = true;

			Create_DamagePopUpPool();
		}
		else
		{
			Debug.LogError("Failed to load DamagePopup prefab through Addressables.");
		}
	}

	void Create_DamagePopUpPool()
	{
		for (int i = 0; i < initialPoolSize; i++)
		{
			var popup = Instantiate(loadedPrefab, transform);
			popup.gameObject.SetActive(false);
			pool.Enqueue(popup);
		}
	}

	GameObject Get_FromPool()
	{
		if (!isLoaded)
			return null;

		if (pool.Count > 0)
		{
			var popup = pool.Dequeue();
			popup.gameObject.SetActive(true);
			return popup;
		}
		else
		{
			var popup = Instantiate(loadedPrefab, transform);
			return popup;
		}
	}

	public void Return_ToDamagePopUpPool(GameObject popup)
	{
		popup.gameObject.SetActive(false);
		pool.Enqueue(popup);
	}

	public void Show_Damage(int damage, Vector3 worldPos)
	{
		var popup = Get_FromPool();

		popup.transform.position = worldPos;

		if (mainCam == null) mainCam = Camera.main;
		if (mainCam != null)
			popup.transform.forward = mainCam.transform.forward;

		if(popup.TryGetComponent<DamagePopup>(out DamagePopup component))
		{
			component.Init(damage);
		}
	}
}
