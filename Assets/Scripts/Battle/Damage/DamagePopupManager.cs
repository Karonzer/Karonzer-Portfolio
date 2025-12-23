using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary>
/// 데미지 팝업(숫자 텍스트) 생성/풀링/표시를 담당하는 매니저.
/// 
/// 핵심 흐름:
/// 1) Addressables로 팝업 프리팹 로드
/// 2) 초기 풀 생성
/// 3) Show_Damage / Show_Text 호출 시 풀에서 꺼내서 위치 설정 후 Init
/// 4) DamagePopup이 비활성화되면 OnDisable에서 다시 풀로 반환
/// </summary>
public class DamagePopupManager : MonoBehaviour
{
	// Addressables로 로드된 팝업 프리팹 원본
	[SerializeField] private GameObject loadedPrefab;

	// 초기 풀 사이즈
	[SerializeField] private int initialPoolSize = 20;

	// 팝업 오브젝트 풀
	private Queue<GameObject> pool = new Queue<GameObject>();

	// 팝업이 바라볼 카메라
	private Camera mainCam;

	// 프리팹 로드 완료 여부
	private bool isLoaded = false;

	void Awake()
	{
		// BattleGSC에 자신 등록 
		BattleGSC.Instance.RegisterDamagePopupManager(this);
		mainCam = Camera.main;
	}

	private void Start()
	{
		// 게임 시작 시 프리팹 로드 요청
		LoadPopupPrefab();
	}


	private void OnDestroy()
	{
		if (loadedPrefab != null)
		{
			Addressables.Release(loadedPrefab);
		}
	}

	/// <summary>
	/// Addressables를 통해 데미지 팝업 프리팹을 비동기로 로드
	/// </summary>
	private void LoadPopupPrefab()
	{
		Addressables.LoadAssetAsync<GameObject>(BattleGSC.Instance.gameManager.Get_PopUpData().DamagePopupPrefab).Completed += OnPrefabLoaded;
	}

	/// <summary>
	/// 프리팹 로드 완료 콜백
	/// </summary>
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

	/// <summary>
	/// 초기 풀 생성
	/// </summary>
	void Create_DamagePopUpPool()
	{
		for (int i = 0; i < initialPoolSize; i++)
		{
			var popup = Instantiate(loadedPrefab, transform);
			popup.gameObject.SetActive(false);
			pool.Enqueue(popup);
		}
	}

	/// <summary>
	/// 풀에서 팝업 하나를 가져옴 (없으면 새로 생성)
	/// </summary>
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
			// 풀 부족 시 즉시 추가 생성
			var popup = Instantiate(loadedPrefab, transform);
			return popup;
		}
	}

	/// <summary>
	/// 팝업을 풀로 반환
	/// </summary>
	public void Return_ToDamagePopUpPool(GameObject popup)
	{
		popup.gameObject.SetActive(false);
		pool.Enqueue(popup);
	}


	/// <summary>
	/// 데미지 숫자 표시
	/// </summary>
	public void Show_Damage(int damage, Vector3 worldPos ,Type _type,bool _critical)
	{
		var popup = Get_FromPool();

		// 월드 위치에 배치
		popup.transform.position = worldPos;

		// 카메라 방향을 향하도록 정렬
		if (mainCam == null) mainCam = Camera.main;
		if (mainCam != null)
			popup.transform.forward = mainCam.transform.forward;

		// DamagePopup 컴포넌트 초기화
		if (popup.TryGetComponent<DamagePopup>(out DamagePopup _component))
		{
			switch(_type)
			{
				case Type.Player:
					_component.Init_Player(damage);
					break;
				case Type.Enemy:
					if(_critical)
					{
						_component.Init_CriticalEnemy(damage);
					}
					else
					{
						_component.Init_Enemy(damage);
					}
					break;
			}

		}
	}

	/// <summary>
	/// 텍스트 팝업 표시 (버프/상태 메시지 등)
	/// </summary>
	public void Show_Text(string _text, Vector3 worldPos, Type _type)
	{
		var popup = Get_FromPool();

		popup.transform.position = worldPos;

		if (mainCam == null) mainCam = Camera.main;
		if (mainCam != null)
			popup.transform.forward = mainCam.transform.forward;

		if (popup.TryGetComponent<DamagePopup>(out DamagePopup _component))
		{
			switch (_type)
			{
				case Type.Player:
					_component.Init_Text(_text);
					break;

			}

		}
	}
}
