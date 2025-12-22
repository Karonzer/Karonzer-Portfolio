
using UnityEngine;

/// <summary>
/// PlayerAniEvnet
/// 
/// 플레이어 애니메이션 이벤트 전용 스크립트.
/// - Animation Clip 안에서 호출되는 함수들을 모아둔다.
/// - 주로 발소리, 공격 사운드 같은 "타이밍이 중요한 사운드"를 처리한다.
/// 
/// 특징:
/// - 실제 사운드 재생은 IAudioHandler에 위임
/// - 애니메이션과 사운드 시스템을 직접 연결하지 않기 위한 중간 역할
/// </summary>
public class PlayerAniEvnet : MonoBehaviour
{
	/// <summary>
	/// 사운드 재생을 담당하는 인터페이스
	/// - 보통 PlayerAudioController가 구현하고 있음
	/// - 부모 오브젝트(Player)에서 가져온다
	/// </summary>
	[SerializeField] private IAudioHandler audioHandler;

	private void Awake()
	{
		audioHandler = transform.parent.GetComponent<IAudioHandler>();

	}

	/// <summary>
	/// 이동 애니메이션 중 발소리 재생
	/// - Animation Event에서 직접 호출됨
	/// - 예: 걷기/달리기 클립의 특정 프레임
	/// </summary>
	public void Play_MoveSound()
	{
		audioHandler.Play_OneShot(SoundType.Player_Move);
	}
}
