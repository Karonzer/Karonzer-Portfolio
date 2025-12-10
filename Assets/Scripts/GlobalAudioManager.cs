using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class GlobalAudioManager : MonoBehaviour
{
	public AudioMixer audioMixer;
	[SerializeField] private AudioSource bgm;
	[SerializeField] private AudioSource sfx;
	[SerializeField] private SoundDatabase db;
	[SerializeField] private AudioClip clickClip;

	[SerializeField] private AudioClip titleBGM;
	[SerializeField] private AudioClip battleBGM;

	[Header("Audio Volume")]
	public float masterVolume = 1f;
	public float bgmVolume = 1f;
	public float sfxVolume = 1f;

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

	public bool Get_AudioClip(SoundType _type, out AudioClip _clip)
	{
		_clip = null;
		_clip = db.GetClip(_type);
		if (_clip != null)
		{ return true; }
		else
		{ return false; }

	}

	private void LoadVolume()
	{
		float dMaster = Mathf.Log10(masterVolume) * 20f;
		float dBBGM = Mathf.Log10(bgmVolume) * 20f;
		float dBSFX = Mathf.Log10(sfxVolume) * 20f;
		audioMixer.SetFloat("Master", dMaster);
		audioMixer.SetFloat("BGM", dBBGM);
		audioMixer.SetFloat("SFX", dBSFX);
	}

	private void Setting_BGM()
	{
		bgm.playOnAwake = false;
		bgm.clip = null;
		bgm.loop = true;
		bgmVolume = 0.5f;
		bgm.volume = bgmVolume;

	}
	private void Setting_Effects()
	{
		sfx.playOnAwake = false;
		sfx.clip = null;
		sfx.loop = false;
		sfxVolume = 0.8f;
		sfx.volume = sfxVolume;
	}

	public void Play_Click()
	{
		sfx.PlayOneShot(clickClip, sfxVolume);
	}

	public void SetVolume(bool isMuted)
	{
		masterVolume = isMuted ? 1 : 0;
	}

	public void Set_MasterVolume(float _value)
	{
		masterVolume = _value;
		float dB = (masterVolume <= 0.0001f) ? -80f : Mathf.Log10(masterVolume) * 20f;
		audioMixer.SetFloat("Master", dB);
	}

	public void Set_BGMVolume(float _value)
	{
		bgmVolume = _value;
		float dB = (bgmVolume <= 0.0001f) ? -80f : Mathf.Log10(bgmVolume) * 20f;
		audioMixer.SetFloat("BGM", dB);
	}

	public void Set_SFXVolume(float _value)
	{
		sfxVolume = _value;
		float dB = (sfxVolume <= 0.0001f) ? -80f : Mathf.Log10(sfxVolume) * 20f;
		audioMixer.SetFloat("SFX", dB);
	}

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

	private void FadeTo(AudioClip newClip, float fadeTime = 1f)
	{
		if (fadeRoutine != null)
			StopCoroutine(fadeRoutine);

		fadeRoutine = StartCoroutine(FadeRoutine(newClip, fadeTime));
	}

	private IEnumerator FadeRoutine(AudioClip newClip, float fadeTime)
	{
		float start = bgm.volume;

		// Fade Out
		for (float t = 0; t < fadeTime; t += Time.deltaTime)
		{
			bgm.volume = Mathf.Lerp(start, 0f, t / fadeTime);
			yield return null;
		}

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
