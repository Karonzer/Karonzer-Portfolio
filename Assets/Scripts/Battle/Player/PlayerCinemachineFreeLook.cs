using Unity.Cinemachine;
using UnityEngine;
using System.Collections;

/// <summary>
/// PlayerCinemachineFreeLook
/// 
/// 플레이어 카메라 연출을 담당하는 컴포넌트.
/// - Cinemachine FreeLook 카메라 입력 활성/비활성 관리
/// - 피격/이벤트 시 카메라 쉐이크(Impulse) 실행
/// - 게임 일시정지 상태에 따라 카메라 입력 차단
/// </summary>
public class PlayerCinemachineFreeLook : MonoBehaviour
{
	// Cinemachine 입력을 제어하는 컴포넌트
	[SerializeField] private CinemachineInputAxisController inputProvider;

	//카메라 흔들림 생성기
	private CinemachineImpulseSource impulseSource;
	private float timer = 1;
	private Coroutine shakeCinemachine;
	private void Awake()
	{
		inputProvider = GetComponent<CinemachineInputAxisController>();
		impulseSource = GetComponent<CinemachineImpulseSource>();
	}

	private void OnEnable()
	{
		if(shakeCinemachine != null)
		{
			StopCoroutine(shakeCinemachine);
			shakeCinemachine = null;
		}
	}

	/// <summary>
	/// 카메라 쉐이크 실행
	/// - intensity : 흔들림 강도
	/// - time      : 지속 시간(현재는 변수만 세팅)
	/// </summary>
	public void Shake(float intensity, float time)
	{
		impulseSource.GenerateImpulse(intensity);
		timer = time;
	}

	private void Update()
	{
		// 게임이 일시정지 상태면 카메라 입력 비활성화
		if (BattleGSC.Instance.gameManager.isPaused)
		{
			inputProvider.enabled = false;
		}
		else
		{
			inputProvider.enabled = true;
		}	
	}
}
