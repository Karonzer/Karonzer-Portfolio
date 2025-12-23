using UnityEngine;

/// <summary>
/// EnemyAudioController
/// 
/// 적(Enemy) 전용 오디오 컨트롤러.
/// - AudioController를 상속받아 기본 오디오 구조를 재사용
/// - 적 사운드(피격, 공격 등)를 재생하는 역할만 담당
/// 
/// 특징:
/// - 전역 AudioManager(GlobalGSC.audioManager)에서 AudioClip을 가져온다
/// - 게임이 Pause 상태일 때는 사운드를 재생하지 않는다
/// - 적 사운드는 플레이어보다 작게 들리도록 볼륨을 낮게 설정
/// </summary>
public class EnemyAudioController : AudioController
{
	private void OnEnable()
	{
		audioSource.volume = 0.1f;
	}

	public override void Play(SoundType _type)
	{

	}

	public override void Stop()
	{
		audioSource.Stop();
	}

	public override void Play_OneShot(SoundType _type)
	{
		Play_Sound(_type);
	}

	/// <summary>
	/// 실제 사운드 재생 처리 함수
	/// 흐름:
	/// 1) Global AudioManager에서 SoundType에 해당하는 AudioClip 요청
	/// 2) 게임이 일시정지 상태가 아니면
	/// 3) AudioSource.PlayOneShot으로 사운드 재생
	/// </summary>
	public void Play_Sound(SoundType _type)
	{
		if (GlobalGSC.Instance.audioManager.Get_AudioClip(_type, out AudioClip _clip) && !BattleGSC.Instance.gameManager.isPaused)
		{
			audioSource.PlayOneShot(_clip);
		}
	}
}
