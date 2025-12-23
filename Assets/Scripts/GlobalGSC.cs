using UnityEngine;

/// <summary>
/// Global Game Singleton Class
/// 
/// 역할:
/// - 씬이 바뀌어도 유지되어야 하는 "전역 시스템"을 보관하는 컨테이너
/// - 전투/플레이 중에 끊기면 안 되는 매니저들을 한 곳에서 접근 가능하게 함
/// 
/// 포함 대상 예:
/// - 씬 전환 관리(SceneManager)
/// - 글로벌 오디오(AudioManager)
/// - 설정 메뉴 UI
/// 
/// 구조적 의도:
/// - BattleGSC : 전투 씬 전용 서비스 컨테이너
/// - GlobalGSC : 게임 전체에서 항상 유지되는 서비스 컨테이너
/// </summary>
public class GlobalGSC : GenericSingletonClass<GlobalGSC>
{
	/// <summary>
	/// 씬 전환을 담당하는 전역 씬 매니저
	/// (타이틀 ↔ 전투 ↔ 재시작 등)
	/// </summary>
	public GSCSceneManager sceneManager { get; private set; }

	/// <summary>
	/// 전역 오디오 매니저
	/// - BGM
	/// - 공통 UI 사운드
	/// - 아이템/버튼 효과음 등
	/// </summary>
	public GlobalAudioManager audioManager { get; private set; }

	/// <summary>
	/// 설정 메뉴 UI
	/// - 사운드 설정
	/// - 옵션 메뉴
	/// - 일시정지 UI에서 접근
	/// </summary>
	public SettingMenulUI settingMenulUI { get; private set; }

	private void Awake()
	{
		// 씬이 변경되어도 파괴되지 않도록 설정
		DontDestroyOnLoad(gameObject);
	}

	/// <summary>
	/// 각 매니저 등록
	/// - 각 매니저 스크립트 Awake에서 호출됨
	/// </summary>
	public void RegisterGSCSceneManager(GSCSceneManager _sceneManager) => sceneManager = _sceneManager;

	public void RegisterGlobalAudioManager(GlobalAudioManager _audioManager) => audioManager = _audioManager;

	public void RegisterSettingMenulUI(SettingMenulUI _settingMenulUI) => settingMenulUI= _settingMenulUI;
}
