using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary>
/// GameManager
///	전투 씬의 전체 흐름을 관리하는 매니저 스크립트입니다.
///	 - 게임 시작/ 종료
///	 - 플레이어 생성
///	 - 적 / 보스 / 웨이브 스폰 / 주기적 아이템 스폰
///	 - 생존 시간 관리
///	 - 일시정지 / 재개 처리
///	 - 난이도 상승 로직
///	 
/// </summary>
public class GameManager : MonoBehaviour
{
	// 일반 몬스터 소픈 리스트
	public EnemySpawnListSO enemySpawnListSO;
	// 보스 몬스터 스폰 리스트
	public EnemySpawnListSO enemyBossSpawnListSO;
	// 게임 전반 수치(스폰 수,간격, 제한)등 게임 진행이 필요한 값을 담고 있는 데이터
	public GameManagerValueSO gameManagerValueSO;

	// 플레이어 생성과 값을 및 키를 관리하는 변수들
	[SerializeField] private Transform playerPos;
	[SerializeField] private GameObject player;
	[SerializeField] private string currentPlayerKey;
	public string CurrentPlayerKey => currentPlayerKey;

	// 몬스터 스폰 관련 변수들
	[SerializeField] private int minSpawnCount;
	[SerializeField] private int maxSpawnCount;
	[SerializeField] private int spawnCount;

	// 아이템 스폰 리스트
	[SerializeField] private ItemDataSO itemData;
	// 월드 UI 팝업 리스트
	[SerializeField] private PopUpDataSO popUpData;

	// 몬스터 스폰에 관련 변수들
	[SerializeField] private int maxEnemyAlive;
	private int currentEnemyAlive = 0;
	private Coroutine spwanTimeRoutine;
	[SerializeField] private float enemySpawnInterval;
	public float EnemySpawnInterval => enemySpawnInterval;

	// 생존 시간 관련 변수들
	private float survivalTime;
	public float SurvivalTime => survivalTime;
	public float lastWaveTime;
	private Coroutine timerRoutine;

	// 아이템 스폰 변수
	private Coroutine itempSpawnRoutine;

	// 보스 스폰 관련 변수
	private float lastBossSpawnTime;
	[SerializeField] private float bossSpawnInterval;

	// 일시정지 관련 이벤트 및 변수
	public event Action OnPause;
	public event Action OnResume;
	public event Action<float> TimerAction;
	public bool isPaused { get; private set; }

	// 현재 처치한 몬스터 수
	[SerializeField] private int currentKillCount;
	public int CurrentKillCount => currentKillCount;

	/// <summary>
	/// - BattleGSC에 자신을 등록하여 전역 접근 가능하도록 한다
	/// </summary>
	private void Awake()
	{
		// BattleGSC에 자신을 등록
		BattleGSC.Instance.RegisterGameManager(this);
		currentPlayerKey = "Player";// 추후 타이틀 씬에서 선택한 캐릭터의 키로 해당 캐릭터를 생성할 수 있음 

	}

	/// <summary>
	/// Start
	///	전투 씬에 필요한 초기 세팅을 순서대로 수행
	///	 - 플레이어 생성
	///	 - 커서 처리
	///	 - 기본 UI 표시
	///	 - 플레이어 / 몬스트 이벤트 연결
	///	 - 코루틴 시작(타이머, 몬스터 스폰 주기, 아이템 스폰 주기)
	///	 - BGM 전환
	///	 - UI 입력(ESC등) 연결
	/// </summary>
	private void Start()
	{
		Initialize_Player();
		Setting_Cursor();
		Setting_StartUI();
		Settting_PlayerEvnet();
		Setting_EnemyEvent();
		Start_Game();
		GlobalGSC.Instance.audioManager.ChangeBGM(SceneBGMType.Battle);
		Setting_UIInputAction();
	}

	//GameManager 초기 수치 세팅
	private void OnEnable()
	{
		Setting_GameManagerValue();

	}

	/// <summary>
	/// OnDestroy
	///	- Addressables로 생성된 플레이어 해채 및 이벤트 해제
	///	- 이벤트에 등록했던 함수들 해제
	/// </summary>
	private void OnDestroy()
	{ 
		Addressables.ReleaseInstance(player);
		player.GetComponent<Player>().OnDead -= Game_Over;
		ReSetting_EnemyEvent();
		DiSetting_UIInputAction();
	}

	// GameManager 초기 수치 세팅
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

	/// <summary>
	/// 코루틴 기반 게임 흐름 시작
	///	- 혹시 이밎 실행 중인 코루틴이 있으면 중복 실행을 막기 위해 정치 후 null 처리후 코투린 시작
	///	- 생존 시각 타이머, 몬스터 스폰 주기, 아이템 스폰 주기 코루틴 시작
	/// </summary>
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

	/// <summary>
	/// 플레이어를 전투 맵 내 내부메쉬 위 랜덤 위치에 생성
	///	- 바닥에 약간 올려 지면 겹칩/끼임 방지
	/// </summary>
	private void Initialize_Player()
	{
		Vector3 pos = Get_RandomPointOnNavMesh();
		pos.y += 0.5f;
		Spawn_Player(currentPlayerKey, pos, Quaternion.identity);
	}


	/// <summary>
	/// 플레이어 관련 이벤트 연결
	///	- 레벨업 시 난이도 상승 처리
	///	- 사망 시 게임 오버 처리
	/// </summary>
	private void Settting_PlayerEvnet()
	{
		player.GetComponent<PlayerLevel>().OnLevelUp += Handle_PlayerLevelUp;
		player.GetComponent<Player>().OnDead += Game_Over;
	}

	/// <summary>
	/// 몬스터 관련 이벤트 연결
	///	- 몬스터가 죽을 때마다 아이템 스폰 체크
	///	- 몬스터가 죽을 때마다 현지 처치 수 증가	
	/// </summary>
	private void Setting_EnemyEvent()
	{
		Enemy.OnDeadGlobal += Check_ItmeSpawn;
		Enemy.OnDeadGlobal += Add_CurrentKillCount;

	}

	/// <summary>
	/// Setting_EnemyEvent에 등록한 이벤트 해제
	///	- 씬 종료 또는 재시작 시 중복 등록 방지
	/// </summary>
	private void ReSetting_EnemyEvent()
	{
		Enemy.OnDeadGlobal -= Check_ItmeSpawn;
		Enemy.OnDeadGlobal -= Add_CurrentKillCount;
	}

	/// <summary>
	/// 전투 시작 시 기본으로 보여줄 UI 세팅
	///	- 경험치 바
	///	- 플레이어 고정 HP 바	
	///	- 플레이어 따라다니는 HP 바
	///	- 타이머 UI
	/// </summary>
	private void Setting_StartUI()
	{
		BattleGSC.Instance.uIManger.Show(UIType.XPBar, player);
		BattleGSC.Instance.uIManger.Show(UIType.PlayerHP, player);
		BattleGSC.Instance.uIManger.Show(UIType.PlayerHPFollow, player);
		BattleGSC.Instance.uIManger.Show(UIType.Timer, gameObject);
	}

	/// <summary>
	/// Addressables를 통해 플레이어 생성
	///	- parent로 playerPos를 지정해 플레이어 위치 관리
	/// </summary>
	public void Spawn_Player(string address, Vector3 pos, Quaternion rot)
	{
		AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(address, pos, rot, playerPos);
		handle.WaitForCompletion();
		player = handle.Result;
	}

	// 비동기 처리 버전
	//public async Awaitable<GameObject> Spawn_Player(string address, Vector3 pos, Quaternion rot)
	//{
	//	AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(address, pos, rot, playerPos);
	//	await handle.Task;
	//	player = handle.Result;
	//}

	/// <summary>
	/// 전투 중 마우스 커서를 숨기고 화면 안에 고정
	///	- 키보드/ 마우스 조작 전투에서 커서가 화면 밖으로 나가 클릭 이탈되는 것을 방지
	/// </summary>
	private void Setting_Cursor()
	{
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Confined;
	}

	/// <summary>
	/// 상황에 따라 커서를 표시 숨길 처리하는 함수
	///	- 일시정지 / 업그레이드 팝업등에서 커서를 표시할 때 사용
	/// </summary>
	public void Set_ShowAndHideCursor(bool _show)
	{
		Cursor.visible = _show;
	}

	/// <summary>
	/// 현재 플레이어 오브젝트 반환
	///	- 다른 시스템에서 플레이어 참조가 필요할때 사용
	/// </summary>
	public GameObject Get_PlayerObject()
	{
		return player;
	}

	/// <summary>
	/// 일반 몬스터 스폰 루틴
	///	- 첫 5초 대기 후 시작
	///	- 일시정지 상태에서 스폰하지 않고 다음 프레임까지 대기
	///	- 현재 몬스터 수가 최대 몬스터 수보다 적을 때만 스폰
	///	- 3~5마리 랜덤 수의 몬스터를 플레이어 중심에서 35~45 거리 내 랜덤 위치에 스폰
	/// </summary>
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


	/// <summary>
	/// 보스 몬스트 생성 시간 조건으로 스폰
	///	- boss 리스트에서 랜덤 키 선택
	///	- 일반 스폰 위치 로직를 재 사용
	/// </summary>
	private void SpawnBossByTime()
	{
		int spawnKey = UnityEngine.Random.Range(1, enemyBossSpawnListSO.enemyKey.Length);
		Vector3 pos = GetRandomSpawnPosition();
		if (pos != Vector3.zero)
		{
			Spawn_BossAt(pos, enemyBossSpawnListSO.enemyKey[spawnKey]);
		}
	}

	/// <summary>
	/// 생존 타이머 루틴
	///	-  일시정지가 아닐 때만 시간 증가
	///	- TimerAction 이벤트로 시간 갱신
	/// - 보스 스폰 시간 조건 체크
	///	- 일반 몬스터 웨이브 스폰 시간 조건 체크
	/// </summary>
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

	/// <summary>
	/// 몬스터 웨이브 스폰 루틴
	///	- 현재 필드에 몬스터가 많으면 웨이브를 생락하여 컴퓨터 랙 방지 및 플레이어 부담 완화
	///	- 웨이브가 시작할때 텍스트 팝업 표시
	/// </summary>
	IEnumerator SpawnWaveRoutine()
	{
		// 적이 너무 많으면 웨이브 스폰 자체를 하지 않음
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

			// 프레임 분산(한 프레임에 너무 많이 스폰하지 않도록)
			yield return null; 
		}
	}

	/// <summary>
	/// 필드에 아이템을 주기적으로 생성
	///	- 일정 시간마다 아이템 리스트에서 랜덤으로 선택하여 플레이어 주변에서 랜덤 위치에 생성
	/// </summary>
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

	/// <summary>
	/// 플레이어 레벨업 시 난이도 증가
	///	- enemySpawnInterval를 점점 줄여서 스폰을 빠르게 변경
	///	- 최소값 이래로 내려가면 더 이상 줄이지 않고 몬스터 스텟을 증가 시키는 방식
	///	- spawnCount를 증가시켜 스폰 가능한 몬스터 종류를 늘림
	/// </summary>
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

	/// <summary>
	/// 플레이어 주변에서 스폰을 가능한 내부 메쉬 위치를 찾는 함수
	///	- 10회 시도하면 성공하면 해당 위치 반환, 실패 시 (0,0,0) 반환
	/// </summary>
	public Vector3 GetRandomSpawnPosition()
	{
		Vector3 playerPos = BattleGSC.Instance.gameManager.Get_PlayerObject().transform.position;

		for (int i = 0; i < 10; i++)
		{
			Vector2 circle = UnityEngine.Random.insideUnitCircle.normalized;
			float distance = UnityEngine.Random.Range(35f, 45f);

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

	/// <summary>
	/// 웨이브용으로 프손 위치 찾는 함수
	///	- 일반 스폰 보다 더 멀리에서 생성해 몰려오는 압박을 만든다
	/// </summary>
	public Vector3 Get_RandomSpawnWavePosition()
	{
		Vector3 playerPos = BattleGSC.Instance.gameManager.Get_PlayerObject().transform.position;

		for (int i = 0; i < 10; i++)
		{
			Vector2 circle = UnityEngine.Random.insideUnitCircle.normalized;
			float distance = UnityEngine.Random.Range(75f, 100f);

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

	/// <summary>
	/// 아이템 생성 위치 찾는 함수
	/// </summary>
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

	/// <summary>
	/// - 현재 위치를 기준으로 네비메쉬 위의 랜덤 위치 반환
	/// - 초기 플레이어 생성 위치 등에 사용
	/// </summary>
	public Vector3 Get_RandomPointOnNavMesh()
	{
		float range = 10f;
		Vector3 randomPoint = transform.position + UnityEngine.Random.insideUnitSphere * range;
		NavMeshHit hit;

		if (NavMesh.SamplePosition(randomPoint, out hit, range, NavMesh.AllAreas))
		{
			return hit.position;
		}
		else
		{
			return Vector3.zero;
		}
	}

	/// <summary>
	/// - 몬스터 처치 시 아에팀 드랍 확률 체크
	/// - 현재 5% 고정 확룰 
	/// - 조건이 만족했을때 해당 몬스터 위치에 아이템 생성 시도
	/// </summary>
	public void Check_ItmeSpawn(GameObject _obj)
	{
		float value = 0.05f;
		if (UnityEngine.Random.value <= value)
		{
			Spawn_ItemPos(_obj.transform.position);
		}
	}

	/// <summary>
	/// - 특정 위치에서 아이템 생성
	/// - 아이템 리스트에서 랜덤으로 생성
	/// </summary>
	public void Spawn_ItemPos(Vector3 _pos)
	{
		int spawnKey = UnityEngine.Random.Range(0, itemData.list.Count);

		Vector3 spawnPos = _pos;
		if (spawnPos != Vector3.zero)
			Spawn_ItemAt(spawnPos, itemData.list[spawnKey]);
	}

	/// <summary>
	///  오브젝트 풀링을 통해 아이템을 꺼내 배치
	///  - y를 +1 올려서 바닥에 파묻히는 문제 방지
	/// </summary>
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

	/// <summary>
	///  오브젝트 풀링을 통해 몬스터 생성
	/// </summary>
	private void Spawn_EnemyAt(Vector3 pos, string key)
	{
		GameObject obj = BattleGSC.Instance.spawnManager.Spawn(PoolObjectType.Enemy, key);

		if (obj != null && obj.TryGetComponent<Enemy>(out var enemy))
		{
			obj.gameObject.SetActive(true);
			enemy.ResetForSpawn(pos);
			enemy.Start_Enemy();
		}
	}


	/// <summary>
	/// 보스 몬스터 생성
	/// - 보스 몬스터 생성 후 보스 HP 바 UI를 활성화 한다
	/// </summary>
	private void Spawn_BossAt(Vector3 pos, string key)
	{
		GameObject obj = BattleGSC.Instance.spawnManager.Spawn(PoolObjectType.Enemy, key);

		if (obj != null && obj.TryGetComponent<Enemy>(out var enemy))
		{
			obj.gameObject.SetActive(true);
			enemy.ResetForSpawn(pos);
			enemy.Start_Enemy();
			BattleGSC.Instance.uIManger.Show(UIType.BossHP, obj);
		}
	}

	/// <summary>
	/// 몬스터 처치 시 현재 처치 수 증가
	/// </summary>
	public void Add_CurrentKillCount(GameObject _obj)
	{
		currentKillCount++;
	}

	/// <summary>
	/// 레벨업 등 업그레이드 팝업을 띄우는 함수
	/// - 사운드 재생
	/// - 일시정지
	/// - 커서 표시
	/// - 업그레이드 팝업 표시
	/// - 몬스터 스탯 증가로 난이도 상승
	/// </summary>
	public void Update_ToPlayerAttackObj()
	{
		GlobalGSC.Instance.audioManager.Play_Sound(SoundType.LevelUp);
		PauseGame();
		Set_ShowAndHideCursor(true);
		BattleGSC.Instance.uIManger.Show(UIType.UpgradePopUp);
		BattleGSC.Instance.statManager.IncreaseAllEnemyStats(0.05f);
	}

	/// <summary>
	/// 보물상자 오브젝트 상호작용하여 업그레이드 팝업을 띄우는 함수
	/// - 레벨업과 달리 몬스터 스탯 증가는 하지 않도록 분리
	/// </summary>
	public void Update_ToPlayerAttackObjIsChestBox()
	{
		GlobalGSC.Instance.audioManager.Play_Sound(SoundType.LevelUp);
		PauseGame();
		Set_ShowAndHideCursor(true);
		BattleGSC.Instance.uIManger.Show(UIType.UpgradePopUp);
	}

	/// <summary>
	/// 업그레이드 팝업을 닫을 뒤 남아 있는 레벨업 처리를 이어가기 위한 트리거
	/// </summary>
	public void Check_PendingLevelUp()
	{
		player.GetComponent<PlayerLevel>().Handle_CloseUpgradePopup();
	}

	/// <summary>
	/// 아이템 리스트 반환
	/// </summary>
	public ItemDataSO Get_ItemDataSO()
	{ return itemData; }

	/// <summary>
	/// 월드 UI 팝업 리스트 반환
	/// </summary>
	public PopUpDataSO Get_PopUpData()
	{ return popUpData; }


	/// <summary>
	/// 게임 일시정지 처리
	/// - OnPause 이벤트로 다른 스크립트에 일시정지 알림
	/// </summary>
	public void PauseGame()
	{
		OnPause?.Invoke();
		isPaused = true;
	}

	/// <summary>
	/// 게임 재개 처리
	/// - OnPause 이벤트로 다른 스크립트에 일시정지 알림
	/// </summary>
	public void ResumeGame()
	{
		OnResume?.Invoke();
		isPaused = false;
	}

	/// <summary>
	/// 일시정지 토클 처리
	/// - ESC 입력 또는 설정 메뉴 닫기 시 호출
	/// </summary>
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


	/// <summary>
	/// 일시정지 토클 처리
	/// - UI 버튼, 메뉴 닫기 이벤트 등에서 호출하기 위한 오버로드 함수
	/// </summary>
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

	/// <summary>
	/// UI 입력 이벤트 연결
	/// - 설정 메뉴 UI의 닫기 이벤트에 일시정지 토글 함수 연결
	/// </summary>
	private void Setting_UIInputAction()
	{
		GlobalGSC.Instance.settingMenulUI.OnClose += Toggle_Pause;
		BattleGSC.Instance.inputManager.InputActions.UI.ESC.performed += Toggle_Pause;
	}

	/// <summary>
	/// UI 입력 이벤트 해제 
	/// - 해제하여 중복 구독 및 메모리 누수 방지
	/// </summary>
	private void DiSetting_UIInputAction()
	{
		GlobalGSC.Instance.settingMenulUI.OnClose -= Toggle_Pause;
		BattleGSC.Instance.inputManager.InputActions.UI.ESC.performed -= Toggle_Pause;
	}

	/// <summary>
	/// 게임 오버 처리
	/// - 사망 사운드 재생
	/// - 일시정지
	/// - 커서 표시
	/// - 필드 전체 디스폰(몬스터,아이템)
	/// - 게임오버 UI 표시
	/// </summary>
	public void Game_Over(IDamageable _damageable)
	{
		GlobalGSC.Instance.audioManager.Play_Sound(SoundType.Player_Die);
		isPaused = true;
		Set_ShowAndHideCursor(true);
		BattleGSC.Instance.spawnManager.Despawn_All();
		BattleGSC.Instance.uIManger.Show(UIType.GameOver);
	}

	/// <summary>
	/// 플레이어가 공격할때 전달할 데미지 정보 생성
	/// - 크리티컬 데미지 계산
	/// - 히트 위치 보정
	/// </summary>
	public DamageInfo Get_PlayerDamageInfo(int damage, GameObject hitPoint, Type attacker)
	{
		Vector3 hitPos = hitPoint.transform.position + Vector3.up * 1.8f;
		int damageInfo = DBManager.CalculateCriticalDamage(BattleGSC.Instance.statManager.Get_PlayerData(BattleGSC.Instance.gameManager.CurrentPlayerKey), damage, out bool _isCritical);
		DamageInfo info = new DamageInfo(damageInfo, hitPos, attacker, _isCritical);
		return info;
	}

	/// <summary>
	/// 몬스터가 플레이어를 공격할때 전달할 데미지 정보 생성
	/// - 현재는 치명타를 사용하지 않으므로 isCritical을 false로 고정
	/// - _key 파라미터는 이후 적 타입별 데미지 계산/속성 적용 등에 확장 가능
	/// </summary>
	public DamageInfo Get_EnemyDamageInfo(int damage, string _key ,GameObject hitPoint, Type attacker)
	{
		Vector3 hitPos = hitPoint.transform.position + Vector3.up * 1.8f;
		DamageInfo info = new DamageInfo(damage, hitPos, attacker, false);
		return info;
	}

}
