using UnityEngine;


/// <summary>
/// BattleGSC : Battle Game Singleton Class
///	전투 씬에 사용되는 모든 핵심 매니저을 싱글톤으로 관리하는 클래스
///	
/// 각 매니저들은 자신의 Awake() 메서드에서 BattleGSC.Instance.RegisterXXX(this) 메서드를 호출하여 자신을 등록합니다
/// 다른 스크립트들은 BattleGSC.Instance.xx름 을 통해 해당 매니저에 접근할 수 있습니다
/// 
/// </summary>
public class BattleGSC : GenericSingletonClass<BattleGSC>
{
	// 전투 전체 흐름관 상태를 관리하는 매니저
	public GameManager gameManager {  get; private set; }
	
	// 플레이어와 적의 스탯 계산과 버프 반영을 담당하는 매니저
	public StatManager statManager { get; private set; }

	// 플레이어 스킬 스탯과 강화 관리를 담당하는 매니저
	public SkillManager skillManager { get; private set; }

	// 몬스터, 아이템, 이펙트 등의 스폰을 담당하는 매니저
	public SpawnManager spawnManager { get; private set; }

	// 데미지 숫자 팝업을 관리하는 매니저
	public DamagePopupManager damagePopupManager { get; private set; }

	// 레업실 업드레이드 선택 및 적용을 담당하는 매니저
	public UpgradeManager upgradeManager { get; private set; }

	// 버프 적용과 관리하는 매니저
	public BuffManager BuffManager { get; private set; }

	//전투 UI(HP바, 보스바, 미니맵, 팝업등)를 관리하는 매니저
	public UIManger uIManger { get; private set; }

	//플레이어 입력을 관리하는 매니저
	public InputManager inputManager { get; private set; }


	// 각 매니저들은 자신의 Awake() 메서드에서 BattleGSC.Instance.RegisterXXX(this) 메서드를 호출하여 자신을 등록합니다
	public void RegisterGameManager(GameManager _gameManager) => gameManager = _gameManager;
	public void RegisterStatManager(StatManager _stat) => statManager = _stat;
	public void RegisterSkillManager(SkillManager _skill) => skillManager = _skill;
	public void RegisterSpawn(SpawnManager _spawn) => spawnManager = _spawn;
	public void RegisterDamagePopupManager(DamagePopupManager _damagePopupManager) => damagePopupManager = _damagePopupManager;
	public void RegisterUpgradeManager(UpgradeManager _upgradeManager) => upgradeManager = _upgradeManager;

	public void RegisterBuffManager(BuffManager _buffManager)=> BuffManager = _buffManager;

	public void RegisterUIManger(UIManger _uIManger) => uIManger = _uIManger;
	public void RegisterInputManager(InputManager _inputManager) => inputManager = _inputManager;
}
