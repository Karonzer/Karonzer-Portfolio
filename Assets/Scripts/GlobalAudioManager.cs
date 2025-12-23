using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

/// <summary>
/// 전역 오디오 매니저.
/// 
/// 담당:
/// - AudioMixer 파라미터( Master / BGM / SFX ) 볼륨 제어
/// - BGM / SFX AudioSource 초기 세팅
/// - 효과음(SoundDatabase) 조회 + 재생
/// - 씬별 BGM 전환(Title/Loading/Battle) + 페이드 처리
/// 
/// 특징:
/// - GlobalGSC에 등록되어 씬이 바뀌어도 접근 가능
/// - Battle 씬에서 Pause 상태면 SFX 재생 제한(현재 구현 기준)
/// </summary>
public class GlobalAudioManager : MonoBehaviour
{
	// AudioMixer에 노출된 파라미터를 제어하기 위한 Mixer
	public AudioMixer audioMixer;

	// 배경음 / 효과음 재생용 AudioSource
	[SerializeField] private AudioSource bgm;
	[SerializeField] private AudioSource sfx;

	// SoundType -> AudioClip 매핑 DB
	[SerializeField] private SoundDatabase db;
	// UI 클릭 효과음(전용)
	[SerializeField] private AudioClip clickClip;

	[SerializeField] private AudioClip titleBGM;
	[SerializeField] private AudioClip battleBGM;

	[Header("Audio Volume")]
	public float masterVolume = 1f;
	public float bgmVolume = 1f;
	public float sfxVolume = 1f;

	// BGM 페이드 코루틴 핸들(중복 실행 방지용)
	private Coroutine fadeRoutine;

	private void Awake()
	{
		GlobalGSC.Instance.RegisterGlobalAudioManager(this);
		bgm = transform.Find("BGM").GetComponent<AudioSource>();
		sfx = transform.Find("SFX").GetComponent<AudioSource>();
	}

	private void OnEnable()
	{
		Setting_BGM();
		Setting_Effects();
		LoadVolume();
	}

	private void Reset()
	{
		masterVolume = 1;
		bgmVolume = 1;
		sfxVolume = 1;
	}


	/// <summary>
	/// SoundType에 해당하는 AudioClip 반환
	/// </summary>
	public bool Get_AudioClip(SoundType _type, out AudioClip _clip)
	{
		_clip = null;
		_clip = db.GetClip(_type);
		if (_clip != null)
		{ return true; }
		else
		{ return false; }

	}

	/// <summary>
	/// 현재 master/bgm/sfx 선형 볼륨(0~1)을 dB로 변환해서 AudioMixer에 적용.
	/// - AudioMixer는 dB 단위를 사용
	/// </summary>
	private void LoadVolume()
	{
		float dMaster = Mathf.Log10(masterVolume) * 20f;
		float dBBGM = Mathf.Log10(bgmVolume) * 20f;
		float dBSFX = Mathf.Log10(sfxVolume) * 20f;
		audioMixer.SetFloat("Master", dMaster);
		audioMixer.SetFloat("BGM", dBBGM);
		audioMixer.SetFloat("SFX", dBSFX);
	}

	/// <summary>
	/// BGM AudioSource 기본 세팅
	/// </summary>
	private void Setting_BGM()
	{
		bgm.playOnAwake = false;
		bgm.clip = null;
		bgm.loop = true;
		bgmVolume = 0.5f;
		bgm.volume = bgmVolume;

	}

	/// <summary>
	/// SFX AudioSource 기본 세팅
	/// </summary>
	private void Setting_Effects()
	{
		sfx.playOnAwake = false;
		sfx.clip = null;
		sfx.loop = false;
		sfxVolume = 0.8f;
		sfx.volume = sfxVolume;
	}

	/// <summary>
	/// UI 클릭 사운드 재생
	/// </summary>
	public void Play_Click()
	{
		sfx.PlayOneShot(clickClip, sfxVolume);
	}

	/// <summary>
	/// 전체 음소거 토글(현재 구현 기준)
	/// </summary>
	public void SetVolume(bool isMuted)
	{
		masterVolume = isMuted ? 1 : 0;
	}

	/// <summary>
	/// 마스터 볼륨을 Mixer에 반영 (0~1 -> dB)
	/// </summary>
	public void Set_MasterVolume(float _value)
	{
		masterVolume = _value;
		float dB = (masterVolume <= 0.0001f) ? -80f : Mathf.Log10(masterVolume) * 20f;
		audioMixer.SetFloat("Master", dB);
	}

	/// <summary>
	/// BGM 볼륨을 Mixer에 반영
	/// </summary>
	public void Set_BGMVolume(float _value)
	{
		bgmVolume = _value;
		float dB = (bgmVolume <= 0.0001f) ? -80f : Mathf.Log10(bgmVolume) * 20f;
		audioMixer.SetFloat("BGM", dB);
	}

	/// <summary>
	/// SFX 볼륨을 Mixer에 반영
	/// </summary>
	public void Set_SFXVolume(float _value)
	{
		sfxVolume = _value;
		float dB = (sfxVolume <= 0.0001f) ? -80f : Mathf.Log10(sfxVolume) * 20f;
		audioMixer.SetFloat("SFX", dB);
	}

	/// <summary>
	/// 효과음 재생 (SoundDatabase 기반)
	/// - Battle 씬 Pause 상태면 재생하지 않도록 제한
	/// </summary>
	public void Play_Sound(SoundType _type)
	{
		if (GlobalGSC.Instance.audioManager.Get_AudioClip(_type, out AudioClip _clip) && !BattleGSC.Instance.gameManager.isPaused)
		{
			sfx.PlayOneShot(_clip);
		}
	}

	/// <summary>
	/// 씬에 따라 BGM 변경 요청
	/// - None: BGM 끔(페이드 아웃)
	/// - Loading: 무음 처리(clip null)
	/// - Title/Battle: 각 클립으로 전환
	/// </summary>
	public void ChangeBGM(SceneBGMType type)
	{
		if (type == SceneBGMType.None)
		{
			FadeTo(null); 
			return;
		}

		AudioClip clip = type switch
		{
			SceneBGMType.Title => titleBGM,
			SceneBGMType.Loading => null, 
			SceneBGMType.Battle => battleBGM,
			_ => null
		};

		FadeTo(clip);
	}

	/// <summary>
	/// 페이드 전환 진입점
	/// - 기존 페이드 코루틴이 있으면 중단 후 새로 시작
	/// </summary>
	private void FadeTo(AudioClip newClip, float fadeTime = 1f)
	{
		if (fadeRoutine != null)
			StopCoroutine(fadeRoutine);

		fadeRoutine = StartCoroutine(FadeRoutine(newClip, fadeTime));
	}

	/// <summary>
	/// BGM 페이드 처리 코루틴
	/// 1) 현재 볼륨에서 0까지 Fade Out
	/// 2) 클립 교체 후 Play
	/// 3) 0에서 bgmVolume까지 Fade In
	/// </summary>
	private IEnumerator FadeRoutine(AudioClip newClip, float fadeTime)
	{
		float start = bgm.volume;

		// Fade Out
		for (float t = 0; t < fadeTime; t += Time.deltaTime)
		{
			bgm.volume = Mathf.Lerp(start, 0f, t / fadeTime);
			yield return null;
		}

		// 새 클립이 없으면 BGM 정지하고 종료(무음 처리)
		if (newClip == null)
		{
			bgm.Stop();
			yield break;
		}

		bgm.clip = newClip;
		bgm.Play();

		// Fade In
		for (float t = 0; t < fadeTime; t += Time.deltaTime)
		{
			bgm.volume = Mathf.Lerp(0f, bgmVolume, t / fadeTime);
			yield return null;
		}

		bgm.volume = bgmVolume;
	}


}
