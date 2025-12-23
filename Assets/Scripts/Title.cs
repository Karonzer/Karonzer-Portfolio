using UnityEngine;


/// <summary>
/// 타이틀 씬을 제어하는 스크립트.
/// 
/// 역할:
/// - GlobalGSC가 존재하지 않으면 생성
/// - 타이틀 BGM 재생
/// - 버튼 입력에 따른 씬 전환 / 설정 / 종료 처리
/// 
/// 특징:
/// - 타이틀 씬은 게임 진입점이므로
///   GlobalGSC 생성 책임을 이곳에서 가짐
/// </summary>
public class Title : MonoBehaviour
{
	// GlobalGSC 프리팹
	[SerializeField] private GameObject gscPrefab;

	private void Awake()
	{
		// 아직 GlobalGSC가 없다면 생성
		// (타이틀 씬 최초 진입 시)
		if (GlobalGSC.Instance == null)
			Instantiate(gscPrefab);
	}

	private void Start()
	{
		// 타이틀 씬 진입 시 타이틀 BGM 재생
		GlobalGSC.Instance.audioManager.ChangeBGM(SceneBGMType.Title);
	}

	/// <summary>
	/// 게임 시작 버튼
	/// - 로딩 씬으로 이동 후 Battle 씬 로드
	/// </summary>
	public void Click_StartGame()
	{
		GlobalGSC.Instance.sceneManager.LoadBattle_WithLoading();
		GlobalGSC.Instance.audioManager.Play_Click();
	}

	/// <summary>
	/// 사운드 설정 버튼
	/// - 전역 설정 메뉴 팝업 오픈
	/// </summary>
	public void Click_SoundSetting()
	{
		GlobalGSC.Instance.settingMenulUI.Show_PopUp();
		GlobalGSC.Instance.audioManager.Play_Click();
	}


	/// <summary>
	/// 게임 종료 버튼
	/// </summary>
	public void Click_GameEnd()
	{
		GlobalGSC.Instance.audioManager.Play_Click();
		Application.Quit();
	}


}
