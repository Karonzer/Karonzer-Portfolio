using System;
using System.Collections;
using UnityEngine.AI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class GameManager : MonoBehaviour
{
	[SerializeField] private Transform playerPos;
	[SerializeField] private GameObject player;
	[SerializeField] private string currentPlayerKey;
	public string CurrentPlayerKey => currentPlayerKey;


	[SerializeField] private ItemDataSO itemData;
	[SerializeField] private PopUpDataSO popUpData;

	private Coroutine spwanTimeRoutine;
	[SerializeField] private float enemySpawnInterval = 5f; // 초기 5초
	public float EnemySpawnInterval => enemySpawnInterval;


	public event Action OnPause;
	public event Action OnResume;
	public bool isPaused { get; private set; }

	private void Awake()
	{
		GSC.Instance.RegisterGameManager(this);
		currentPlayerKey = "Player";// 추후 타이틀 씬에서 선택한 이름을 가지고 올 예정
		Initialize_Player();
	}
	private void Start()
	{
		GSC.Instance.uIManger.Initialize_UI(player);
		Setting_Cursor();

		player.GetComponent<PlayerLevel>().OnLevelUp += Handle_PlayerLevelUp;
	}

	private void OnEnable()
	{
		isPaused = false;
		if (spwanTimeRoutine != null)
		{
			StopCoroutine(spwanTimeRoutine);
			spwanTimeRoutine = null;
		}

		spwanTimeRoutine = StartCoroutine(Enemy_SpawnRoutine());
	}

	private void OnDestroy()
	{
		Addressables.ReleaseInstance(player);
	}

	private void Initialize_Player()
	{
		Spawn_Player(currentPlayerKey, Vector3.zero,Quaternion.identity);
	}

	public void Spawn_Player(string address, Vector3 pos, Quaternion rot)
	{
		AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(address, pos, rot, playerPos);
		handle.WaitForCompletion();
		player = handle.Result;

	}

	private void Setting_Cursor()
	{
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Confined;
	}

	public void Set_ShowAndHideCursor(bool _show)
	{
		Cursor.visible = _show;
	}

	public GameObject Get_PlayerObject()
	{
		return player;
	}

	IEnumerator Enemy_SpawnRoutine()
	{
		yield return new WaitForSeconds(1f);

		while (true)
		{
			if(!isPaused)
			{
				int count = UnityEngine.Random.Range(3, 6); // 3~5마리
				for (int i = 0; i < count; i++)
				{
					Vector3 spawnPos = GetRandomSpawnPosition();
					if (spawnPos != Vector3.zero)
						SpawnEnemyAt(spawnPos, "EnemyType1");  // 스폰할 적 key
				}

				yield return new WaitForSeconds(EnemySpawnInterval); // 주기
			}
			else
			{
				yield return null;
			}

		}
	}

	private void Handle_PlayerLevelUp()
	{
		// 감소값: 레벨업마다 0.2초씩 감소
		if (enemySpawnInterval < 0.5f)
		{
			enemySpawnInterval = 0.5f;
		}
		else
		{
			enemySpawnInterval -= 0.2f;
		}

		// 최소값 제한 (너무 빨라지는 것 방지)
		enemySpawnInterval = Mathf.Clamp(enemySpawnInterval, 1.0f, 10f);

		Debug.Log("Spawn Interval Changed: " + enemySpawnInterval);
	}

	Vector3 GetRandomSpawnPosition()
	{
		Vector3 playerPos = GSC.Instance.gameManager.Get_PlayerObject().transform.position;

		for (int i = 0; i < 10; i++) // 10번까지 시도
		{
			Vector2 circle = UnityEngine.Random.insideUnitCircle.normalized;
			float distance = UnityEngine.Random.Range(15f, 30f);

			Vector3 randomPos =
				playerPos +
				new Vector3(circle.x, 0f, circle.y) * distance;

			if (NavMesh.SamplePosition(randomPos, out var hit, 3f, NavMesh.AllAreas))
			{
				return hit.position;
			}
		}

		return Vector3.zero; // 실패
	}

	void SpawnEnemyAt(Vector3 pos, string key)
	{
		GameObject obj = GSC.Instance.spawnManager.Spawn(PoolObjectType.Enemy, key);

		if (obj != null && obj.TryGetComponent<Enemy>(out var enemy))
		{
			obj.transform.position = pos;
			obj.gameObject.SetActive(true);
			enemy.Start_Enemy();
		}
	}


	public void Update_ToPlayerAttackObj()
	{
		PauseGame();
		Set_ShowAndHideCursor(true);
		GSC.Instance.uIManger.Show_UI("UpgradePopUp");
	}

	public ItemDataSO Get_ItemDataSO()
	{ return itemData; }

	public PopUpDataSO Get_PopUpData()
	{ return popUpData; }


	public void PauseGame()
	{
		OnPause?.Invoke();
		isPaused = true;
	}

	public void ResumeGame()
	{
		OnResume?.Invoke();
		isPaused = false;
	}

	public DamageInfo Get_PlayerDamageInfo(int damage, GameObject hitPoint, Type attacker)
	{
		Vector3 hitPos = hitPoint.transform.position + Vector3.up * 1.8f;
		int damageInfo = DBManager.CalculateCriticalDamage(GSC.Instance.statManager.Get_PlayerData(GSC.Instance.gameManager.CurrentPlayerKey), damage, out bool _isCritical);
		DamageInfo info = new DamageInfo(damageInfo, hitPos, attacker, _isCritical);
		return info;
	}

	public DamageInfo Get_EnemyDamageInfo(int damage, string _key ,GameObject hitPoint, Type attacker)
	{
		Vector3 hitPos = hitPoint.transform.position + Vector3.up * 1.8f;
		DamageInfo info = new DamageInfo(damage, hitPos, attacker, false);
		return info;
	}

}
