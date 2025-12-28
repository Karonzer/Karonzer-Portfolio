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
/// 

public interface ITitleHandler<T>
{
	T Value { get; }
}


public class Title : MonoBehaviour, ITitleHandler<Title>
{
	// GlobalGSC 프리팹
	[SerializeField] private GameObject gscPrefab;
	[SerializeField] public TitleUIManager uIManager;
	[SerializeField] public TitleCameraManager cameraManager;

	public Title Value => this;

	private void Awake()
	{
		// 아직 GlobalGSC가 없다면 생성
		// (타이틀 씬 최초 진입 시)
		if (GlobalGSC.Instance == null)
			Instantiate(gscPrefab);

		uIManager.Setting_TitleHandler(this);
		cameraManager.Setting_TitleHandler(this);
	}

	private void Start()
	{
		// 타이틀 씬 진입 시 타이틀 BGM 재생
		GlobalGSC.Instance.audioManager.ChangeBGM(SceneBGMType.Title);

		uIManager.Show(UIType.titleBtns);
	}


	public void GoToSelectCamera()
	{
		cameraManager.To_SelectCam();
		uIManager.Hide(UIType.titleBtns);
	}

	public void GoToMainCamera()
	{
		cameraManager.To_MainCam();
		uIManager.Show(UIType.titleBtns);
	}


}
