using UnityEngine;

/// <summary>
/// 개별 오브젝트에 붙는 오디오 컨트롤러의 베이스 클래스.
/// 
/// 목적:
/// - 투사체, 스킬, 이펙트 등
///   자기 자신이 소리를 내야 하는 오브젝트용 공통 인터페이스 제공
/// 
/// 구조적 위치:
/// - GlobalAudioManager : 전역 오디오 정책/믹서/볼륨 담당
/// - AudioController   : 개별 오브젝트 단위의 재생 제어 담당
/// 
/// IAudioHandler:
/// - Play / Stop / Play_OneShot 인터페이스 통일
/// - 실제 재생 로직은 자식 클래스에서 구현
/// </summary>
public class AudioController : MonoBehaviour , IAudioHandler
{
	[SerializeField] protected AudioSource audioSource;
	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
	}

	/// <summary>
	/// 루프성 또는 상태 기반 사운드 재생
	/// - 예: 지속 이펙트, 채널링 스킬
	/// - 기본 구현은 비어 있으며, 자식 클래스에서 override
	/// </summary>
	public virtual void Play(SoundType _type)
	{

	}

	/// <summary>
	/// 현재 재생 중인 사운드 정지
	/// </summary>
	public virtual void Stop()
	{
		audioSource.Stop();
	}

	/// <summary>
	/// 단발성 사운드 재생
	/// - 예: 피격, 폭발, 투사체 발사음
	/// - 기본 구현은 비어 있으며, 자식 클래스에서 override
	/// </summary>
	public virtual void Play_OneShot(SoundType _type)
	{

	}
}
