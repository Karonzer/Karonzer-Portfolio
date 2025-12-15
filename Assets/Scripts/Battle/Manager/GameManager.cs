using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AI;
using UnityEngine.InputSystem;
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

	[SerializeField] private int maxEnemyAlive;
	private int currentEnemyAlive = 0;
	private Coroutine spwanTimeRoutine;
	[SerializeField] private float enemySpawnInterval;
	public float EnemySpawnInterval => enemySpawnInterval;

	private float survivalTime;
	public float SurvivalTime => survivalTime;
	public float lastWaveTime;
	private Coroutine timerRoutine;

	private Coroutine itempSpawnRoutine;

	private float lastBossSpawnTime;
	[SerializeField] private float bossSpawnInterval;


	public event Action OnPause;
	public event Action OnResume;
	public event Action<float> TimerAction;
	public bool isPaused { get; private set; }

	[SerializeField] private int currentKillCount;
	public int CurrentKillCount => currentKillCount;

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
		Setting_EnemyEvent();
		Start_Game();
		GlobalGSC.Instance.audioManager.ChangeBGM(SceneBGMType.Battle);
		Setting_UIInputAction();
	}

	private void OnEnable()
	{
		Setting_GameManagerValue();

	}

	private void OnDestroy()
	{ 
		Addressables.ReleaseInstance(player);
		player.GetComponent<Player>().OnDead -= Game_Over;
		ReSetting_EnemyEvent();
		DiSetting_UIInputAction();
	}

	private void Setting_GameManagerValue()
	{
		currentEnemyAlive = 0;
		maxEnemyAlive = gameManagerValueSO.maxEnemyAlive;

		survivalTime = 0;
		lastWaveTime = 0;
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
		{
			StopCoroutine(timerRoutine);
			timerRoutine = null;
		}

		if(itempSpawnRoutine != null)
		{
			StopCoroutine(itempSpawnRoutine);
			itempSpawnRoutine = null;
		}

		timerRoutine = StartCoroutine(SurvivalTimerRoutine());
		spwanTimeRoutine = StartCoroutine(Enemy_SpawnRoutine());
		itempSpawnRoutine= StartCoroutine(Itemp_SpawnRoutine());

	}

	private void Initialize_Player()
	{
		Vector3 pos = Get_RandomPointOnNavMesh();
		pos.y += 1;
		Spawn_Player(currentPlayerKey, pos, Quaternion.identity);
	}

	private void Settting_PlayerEvnet()
	{
		player.GetComponent<PlayerLevel>().OnLevelUp += Handle_PlayerLevelUp;
		player.GetComponent<Player>().OnDead += Game_Over;
	}

	private void Setting_EnemyEvent()
	{
		Enemy.OnDeadGlobal += Check_ItmeSpawn;
		Enemy.OnDeadGlobal += Add_CurrentKillCount;

	}

	private void ReSetting_EnemyEvent()
	{
		Enemy.OnDeadGlobal -= Check_ItmeSpawn;
		Enemy.OnDeadGlobal -= Add_CurrentKillCount;
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
		yield return new WaitForSeconds(5f);
		while (true)
		{
			if (!isPaused)
			{
				if(currentEnemyAlive < maxEnemyAlive)
				{
					int count = UnityEngine.Random.Range(minSpawnCount, maxSpawnCount); // 3~5마리
					int spawnKey = UnityEngine.Random.Range(0, spawnCount);
					for (int i = 0; i < count; i++)
					{
						Vector3 spawnPos = GetRandomSpawnPosition();
						if (spawnPos != Vector3.zero)
							Spawn_EnemyAt(spawnPos, enemySpawnListSO.enemyKey[spawnKey]);  // 스폰할 적 key
					}
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

				if (survivalTime - lastWaveTime >= gameManagerValueSO.waveInterval)
				{
					StartCoroutine(SpawnWaveRoutine());
					lastWaveTime = survivalTime;
				}
			}
			yield return null;
		}
	}

	IEnumerator SpawnWaveRoutine()
	{
		if (currentEnemyAlive > maxEnemyAlive * 0.7f)
		{
			yield break;
		}
		BattleGSC.Instance.uIManger.Show(UIType.TextPopUp);
		yield return new WaitForSeconds(1);
		int count = UnityEngine.Random.Range(gameManagerValueSO.waveMinCount, gameManagerValueSO.waveMaxCount);
		for (int i = 0; i < count; i++)
		{
			Vector3 pos = Get_RandomSpawnWavePosition();
			int spawnKey = UnityEngine.Random.Range(0, spawnCount);
			if (pos != Vector3.zero)
				Spawn_EnemyAt(pos, enemySpawnListSO.enemyKey[spawnKey]);

			yield return null; 
		}
	}

	IEnumerator Itemp_SpawnRoutine()
	{
		yield return new WaitForSeconds(1f);
		while (true)
		{
			if (!isPaused)
			{
				int spawnKey = UnityEngine.Random.Range(0, itemData.list.Count);

				Vector3 spawnPos = Get_RandomPointOnItem();
				if (spawnPos != Vector3.zero)
					Spawn_ItemAt(spawnPos, itemData.list[spawnKey]);

				yield return new WaitForSeconds(50f);
			}
			else
			{
				yield return null;
			}

		}
	}


	private void Handle_PlayerLevelUp(int _currentLevel)
	{

		if (enemySpawnInterval < gameManagerValueSO.enemySpawnIntervalMin)
		{
			enemySpawnInterval = gameManagerValueSO.enemySpawnIntervalMin;
			BattleGSC.Instance.statManager.IncreaseAllEnemyStats(0.05f);
		}
		else
		{
			enemySpawnInterval -= gameManagerValueSO.enemySpawnIntervalDecrease;
		}


		if(spawnCount < enemySpawnListSO.enemyKey.Length && _currentLevel / gameManagerValueSO.spawnCountDecreaseValue == 0)
		{
			spawnCount++;
		}

		enemySpawnInterval = Mathf.Clamp(enemySpawnInterval, 1.0f, 10f);
	}

	public Vector3 GetRandomSpawnPosition()
	{
		Vector3 playerPos = BattleGSC.Instance.gameManager.Get_PlayerObject().transform.position;

		for (int i = 0; i < 10; i++)
		{
			Vector2 circle = UnityEngine.Random.insideUnitCircle.normalized;
			float distance = UnityEngine.Random.Range(30f, 45f);

			Vector3 randomPos =
				playerPos +
				new Vector3(circle.x, 0f, circle.y) * distance;

			if (NavMesh.SamplePosition(randomPos, out var hit, 5f, NavMesh.AllAreas))
			{
				return hit.position;
			}
		}

		return Vector3.zero;
	}

	public Vector3 Get_RandomSpawnWavePosition()
	{
		Vector3 playerPos = BattleGSC.Instance.gameManager.Get_PlayerObject().transform.position;

		for (int i = 0; i < 10; i++)
		{
			Vector2 circle = UnityEngine.Random.insideUnitCircle.normalized;
			float distance = UnityEngine.Random.Range(100f, 125f);

			Vector3 randomPos =
				playerPos +
				new Vector3(circle.x, 0f, circle.y) * distance;

			if (NavMesh.SamplePosition(randomPos, out var hit, 10f, NavMesh.AllAreas))
			{
				return hit.position;
			}
		}

		return Vector3.zero;
	}

	public static Vector3 Get_RandomPointOnItem()
	{
		Vector3 playerPos = BattleGSC.Instance.gameManager.Get_PlayerObject().transform.position;

		for (int i = 0; i < 10; i++)
		{
			Vector2 circle = UnityEngine.Random.insideUnitCircle.normalized;
			float distance = UnityEngine.Random.Range(50f, 75f);

			Vector3 randomPos =
				playerPos +
				new Vector3(circle.x, 0f, circle.y) * distance;

			if (NavMesh.SamplePosition(randomPos, out var hit, 10f, NavMesh.AllAreas))
			{
				return hit.position;
			}
		}
		return Vector3.zero;
	}

	public Vector3 Get_RandomPointOnNavMesh()
	{
		NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();

		int triangleIndex = UnityEngine.Random.Range(0, navMeshData.indices.Length / 3) * 3;

		Vector3 v0 = navMeshData.vertices[navMeshData.indices[triangleIndex]];
		Vector3 v1 = navMeshData.vertices[navMeshData.indices[triangleIndex + 1]];
		Vector3 v2 = navMeshData.vertices[navMeshData.indices[triangleIndex + 2]];

		float r1 = Mathf.Sqrt(UnityEngine.Random.value);
		float r2 = UnityEngine.Random.value;

		Vector3 randomPoint =
			(1 - r1) * v0 +
			(r1 * (1 - r2)) * v1 +
			(r1 * r2) * v2;

		if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 10f, NavMesh.AllAreas))
		{
			return hit.position;
		}

		return Vector3.zero;
	}

	public void Check_ItmeSpawn(GameObject _obj)
	{
		float value = 0.05f;
		if (UnityEngine.Random.value <= value)
		{
			Spawn_ItemPos(_obj.transform.position);
		}
	}

	public void Spawn_ItemPos(Vector3 _pos)
	{
		int spawnKey = UnityEngine.Random.Range(0, itemData.list.Count);

		Vector3 spawnPos = _pos;
		if (spawnPos != Vector3.zero)
			Spawn_ItemAt(spawnPos, itemData.list[spawnKey]);
	}

	public void Spawn_ItemAt(Vector3 pos, string key)
	{
		GameObject obj = BattleGSC.Instance.spawnManager.Spawn(PoolObjectType.Item, key);
		if (obj != null)
		{
			pos.y += 1;
			obj.transform.position = pos;
			obj.gameObject.SetActive(true);
		}
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

	public void Add_CurrentKillCount(GameObject _obj)
	{
		currentKillCount++;
	}

	public void Update_ToPlayerAttackObj()
	{
		GlobalGSC.Instance.audioManager.Play_Sound(SoundType.LevelUp);
		PauseGame();
		Set_ShowAndHideCursor(true);
		BattleGSC.Instance.uIManger.Show(UIType.UpgradePopUp);
		BattleGSC.Instance.statManager.IncreaseAllEnemyStats(0.05f);
	}

	public void Update_ToPlayerAttackObjIsChestBox()
	{
		GlobalGSC.Instance.audioManager.Play_Sound(SoundType.LevelUp);
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

	public void Toggle_Pause(InputAction.CallbackContext context)
	{
		if (isPaused)
		{
			ResumeGame();
			Set_ShowAndHideCursor(false);
			BattleGSC.Instance.uIManger.Hide(UIType.Pause);
		}
		else
		{
			PauseGame();
			Set_ShowAndHideCursor(true);
			BattleGSC.Instance.uIManger.Show(UIType.Pause);
		}
	}

	public void Toggle_Pause()
	{
		if (isPaused)
		{
			ResumeGame();
			Set_ShowAndHideCursor(false);
			BattleGSC.Instance.uIManger.Hide(UIType.Pause);
		}
		else
		{
			PauseGame();
			Set_ShowAndHideCursor(true);
			BattleGSC.Instance.uIManger.Show(UIType.Pause);
		}
	}

	private void Setting_UIInputAction()
	{
		GlobalGSC.Instance.settingMenulUI.OnClose += Toggle_Pause;
		BattleGSC.Instance.inputManager.InputActions.UI.ESC.performed += Toggle_Pause;
	}

	private void DiSetting_UIInputAction()
	{
		GlobalGSC.Instance.settingMenulUI.OnClose -= Toggle_Pause;
		BattleGSC.Instance.inputManager.InputActions.UI.ESC.performed -= Toggle_Pause;
	}

	public void Game_Over(IDamageable _damageable)
	{
		GlobalGSC.Instance.audioManager.Play_Sound(SoundType.Player_Die);
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
