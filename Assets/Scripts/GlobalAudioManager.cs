using UnityEngine;
using UnityEngine.Audio;

public class GlobalAudioManager : MonoBehaviour
{
	public AudioMixer audioMixer;
	[SerializeField] private AudioSource bgm;
	[SerializeField] private AudioSource sfx;
	[SerializeField] private AudioClip clickClip;

	[Header("Audio Volume")]
	public float masterVolume = 1f;
	public float bgmVolume = 1f;
	public float sfxVolume = 1f;

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
		AudioListener.volume = masterVolume;
		bgmVolume = 1;
		sfxVolume = 1;
	}

	private void LoadVolume()
	{
		audioMixer.SetFloat("BGM", bgmVolume);
		audioMixer.SetFloat("Effect", sfxVolume);
	}

	private void Setting_BGM()
	{
		bgm.playOnAwake = false;
		bgm.clip = null;
		bgm.loop = true;
		bgmVolume = 0.25f;
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
		AudioListener.volume = masterVolume;
	}

	public void Set_BGMVolume(float _value)
	{
		bgmVolume = _value;
		audioMixer.SetFloat("BGM", bgmVolume);
	}

	public void Set_SFXVolume(float _value)
	{
		sfxVolume = _value;
		audioMixer.SetFloat("SFX", sfxVolume);
	}


}
