using UnityEngine;
using Unity.Cinemachine;

/// <summary>
/// 타이틀 씬에서 사용되는 카메라 전환 전용 매니저
/// - Cinemachine Camera의 Priority를 이용해 카메라를 전환
/// - 메인 화면 / 캐릭터 선택 화면 등의 시점 제어를 담당
/// </summary>
public class TitleCameraManager : MonoBehaviour
{

	/// <summary>
	/// 타이틀 상태 및 로직을 제어하는 외부 핸들러
	/// (현재 스크립트에서는 직접 사용되지는 않지만
	/// 타이틀 흐름과의 연결을 위한 확장 포인트)
	/// </summary>
	[SerializeField] private ITitleHandler<Title> titleHandler;

	/// <summary>
	/// 기본 타이틀 화면을 담당하는 메인 카메라
	/// </summary>
	[SerializeField] private CinemachineCamera mainCam;

	/// <summary>
	/// 캐릭터 선택 등 특정 연출용 카메라
	/// </summary>
	[SerializeField] private CinemachineCamera SelectCam;

	/// <summary>
	/// 비활성 상태의 카메라 Priority 값
	/// </summary>
	[SerializeField] private int basePriority = 10;

	/// <summary>
	/// 활성 상태의 카메라 Priority 값
	/// </summary>
	[SerializeField] private int activePriority = 20;


	/// <summary>
	/// 외부에서 TitleHandler를 주입받아 설정
	/// (TitleManager, GameManager 등에서 호출)
	/// </summary>
	public void Setting_TitleHandler(ITitleHandler<Title> _titleHandler)
	{
		titleHandler = _titleHandler;
	}

	private void Start()
	{
		// 타이틀 씬 진입 시 기본 카메라는 메인 카메라
		To_MainCam();
	}


	/// <summary>
	/// 선택 화면 카메라로 전환
	/// - SelectCam을 활성 Priority로 설정
	/// - MainCam은 비활성 Priority로 낮춤
	/// </summary>
	public void To_SelectCam()
	{
		mainCam.Priority = basePriority;
		SelectCam.Priority = activePriority;  
	}

	/// <summary>
	/// 메인 타이틀 카메라로 전환
	/// - MainCam을 활성 Priority로 설정
	/// - SelectCam은 비활성 Priority로 낮춤
	/// </summary>
	public void To_MainCam()
	{
		SelectCam.Priority = basePriority;
		mainCam.Priority = activePriority;   
	}
}
