using System;
using System.Collections;
using UnityEngine.AI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class GameManager : MonoBehaviour
{
	public EnemySpawnListSO enemySpawnListSO;
	public EnemySpawnListSO enemyBossSpawnListSO;
	public GameManagerValueSO gameManagerValueSO;

	[SerializeField] private Transform playerPos;
	[SerializeField] private GameObject player;
	[SerializeField] private string currentPlayerKey;
	public string CurrentPlayerKey => currentPlayerKey;

	[SerializeField] private int minSpawnCount;
	[SerializeField] private int maxSpawnCount;
	[SerializeField] private int spawnCount;

	[SerializeField] private ItemDataSO itemData;
	[SerializeField] private PopUpDataSO popUpData;

	private Coroutine spwanTimeRoutine;
	[SerializeField] private float enemySpawnInterval;
	public float EnemySpawnInterval => enemySpawnInterval;

	private float survivalTime;
	public float SurvivalTime => survivalTime;
	private Coroutine timerRoutine;

	private float lastBossSpawnTime;
	[SerializeField] private float bossSpawnInterval;


	public event Action OnPause;
	public event Action OnResume;
	public event Action<float> TimerAction;
	public bool isPaused { get; private set; }

	private void Awake()
	{
		BattleGSC.Instance.RegisterGameManager(this);
		currentPlayerKey = "Player";// 추후 타이틀 씬에서 선택한 이름을 가지고 올 예정
		Initialize_Player();
	}
	private void Start()
	{
		Setting_Cursor();
		Setting_StartUI();
		Settting_PlayerEvnet();
		Start_Game();
	}

	private void OnEnable()
	{
		Setting_GameManagerValue();

	}

	private void OnDestroy()
	{ 
		Addressables.ReleaseInstance(player);
		player.GetComponent<Player>().OnDead -= Game_Over;
	}

	private void Setting_GameManagerValue()
	{

		survivalTime = 0;
		enemySpawnInterval = gameManagerValueSO.enemySpawnInterval;
		lastBossSpawnTime = 0;

		isPaused = false;

		minSpawnCount = gameManagerValueSO.minSpawnCount;
		maxSpawnCount = gameManagerValueSO.maxSpawnCount;
		spawnCount = gameManagerValueSO.spawnCount;

		bossSpawnInterval = gameManagerValueSO.bossSpawnInterval;
	}

	private void Start_Game()
	{
		if (spwanTimeRoutine != null)
		{
			StopCoroutine(spwanTimeRoutine);
			spwanTimeRoutine = null;
		}

		if (timerRoutine != null)
			StopCoroutine(timerRoutine);
		timerRoutine = StartCoroutine(SurvivalTimerRoutine());

		spwanTimeRoutine = StartCoroutine(Enemy_SpawnRoutine());
	}

	private void Initialize_Player()
	{
		Spawn_Player(currentPlayerKey, Vector3.zero,Quaternion.identity);
	}

	private void Settting_PlayerEvnet()
	{
		player.GetComponent<PlayerLevel>().OnLevelUp += Handle_PlayerLevelUp;
		player.GetComponent<Player>().OnDead += Game_Over;
	}

	private void Setting_StartUI()
	{
		BattleGSC.Instance.uIManger.Show(UIType.XPBar, player);
		BattleGSC.Instance.uIManger.Show(UIType.PlayerHP, player);
		BattleGSC.Instance.uIManger.Show(UIType.PlayerHPFollow, player);
		BattleGSC.Instance.uIManger.Show(UIType.Timer, gameObject);
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
			if (!isPaused)
			{
				int count = UnityEngine.Random.Range(minSpawnCount, maxSpawnCount); // 3~5마리
				int spawnKey = UnityEngine.Random.Range(0, spawnCount);
				for (int i = 0; i < count; i++)
				{
					Vector3 spawnPos = GetRandomSpawnPosition();
					if (spawnPos != Vector3.zero)
						Spawn_EnemyAt(spawnPos, enemySpawnListSO.enemyKey[spawnKey]);  // 스폰할 적 key
				}

				yield return new WaitForSeconds(EnemySpawnInterval); // 주기
			}
			else
			{
				yield return null;
			}

		}
	}

	private void SpawnBossByTime()
	{
		int spawnKey = UnityEngine.Random.Range(1, enemyBossSpawnListSO.enemyKey.Length);
		Vector3 pos = GetRandomSpawnPosition();
		if (pos != Vector3.zero)
		{
			Spawn_BossAt(pos, enemyBossSpawnListSO.enemyKey[spawnKey]);
		}
	}

	IEnumerator SurvivalTimerRoutine()
	{
		survivalTime = 0f;

		while (true)
		{
			if (!isPaused)
			{
				survivalTime += Time.deltaTime;
				TimerAction?.Invoke(survivalTime);


				if (survivalTime - lastBossSpawnTime >= bossSpawnInterval)
				{
					SpawnBossByTime();
					lastBossSpawnTime = survivalTime;
				}
			}
			yield return null;
		}
	}


	private void Handle_PlayerLevelUp(int _currentLevel)
	{

		if (enemySpawnInterval < gameManagerValueSO.enemySpawnIntervalMin)
		{
			enemySpawnInterval = gameManagerValueSO.enemySpawnIntervalMin;
		}
		else
		{
			enemySpawnInterval -= gameManagerValueSO.enemySpawnIntervalDecrease;
		}

		if(_currentLevel / gameManagerValueSO.LevetCheck == 0)
		{
			minSpawnCount++;
			maxSpawnCount++;
		}

		if(spawnCount < enemySpawnListSO.enemyKey.Length && _currentLevel / gameManagerValueSO.spawnCountDecreaseValue == 0)
		{
			spawnCount++;
		}

		enemySpawnInterval = Mathf.Clamp(enemySpawnInterval, 1.0f, 10f);
	}

	Vector3 GetRandomSpawnPosition()
	{
		Vector3 playerPos = BattleGSC.Instance.gameManager.Get_PlayerObject().transform.position;

		for (int i = 0; i < 10; i++)
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

		return Vector3.zero;
	}

	private void Spawn_EnemyAt(Vector3 pos, string key)
	{
		GameObject obj = BattleGSC.Instance.spawnManager.Spawn(PoolObjectType.Enemy, key);

		if (obj != null && obj.TryGetComponent<Enemy>(out var enemy))
		{
			obj.transform.position = pos;
			obj.gameObject.SetActive(true);
			enemy.Start_Enemy();
		}
	}

	private void Spawn_BossAt(Vector3 pos, string key)
	{
		GameObject obj = BattleGSC.Instance.spawnManager.Spawn(PoolObjectType.Enemy, key);

		if (obj != null && obj.TryGetComponent<Enemy>(out var enemy))
		{
			obj.transform.position = pos;
			obj.gameObject.SetActive(true);
			enemy.Start_Enemy();
			BattleGSC.Instance.uIManger.Show(UIType.BossHP, obj);
		}
	}

	public void Update_ToPlayerAttackObj()
	{
		PauseGame();
		Set_ShowAndHideCursor(true);
		BattleGSC.Instance.uIManger.Show(UIType.UpgradePopUp);
		BattleGSC.Instance.statManager.IncreaseAllEnemyStats(0.05f);
	}

	public void Update_ToPlayerAttackObjIsChestBox()
	{
		PauseGame();
		Set_ShowAndHideCursor(true);
		BattleGSC.Instance.uIManger.Show(UIType.UpgradePopUp);
	}

	public void Check_PendingLevelUp()
	{
		player.GetComponent<PlayerLevel>().Handle_CloseUpgradePopup();
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

	public void Game_Over(IDamageable _damageable)
	{
		isPaused = true;
		Set_ShowAndHideCursor(true);
		BattleGSC.Instance.spawnManager.Despawn_All();
		BattleGSC.Instance.uIManger.Show(UIType.GameOver);
	}	

	public DamageInfo Get_PlayerDamageInfo(int damage, GameObject hitPoint, Type attacker)
	{
		Vector3 hitPos = hitPoint.transform.position + Vector3.up * 1.8f;
		int damageInfo = DBManager.CalculateCriticalDamage(BattleGSC.Instance.statManager.Get_PlayerData(BattleGSC.Instance.gameManager.CurrentPlayerKey), damage, out bool _isCritical);
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
