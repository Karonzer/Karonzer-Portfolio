using UnityEngine;
using UnityEngine.Audio;

public class GlobalAudioManager : MonoBehaviour
{
	public AudioMixer audioMixer;
	[SerializeField] private AudioSource bgm;
	[SerializeField] private AudioSource effect;
	[SerializeField] private AudioClip clickClip;

	private float masterVolume = 1f;
	private float uiVolume = 1f;

	private void Awake()
	{
		GlobalGSC.Instance.RegisterGlobalAudioManager(this);
		bgm = transform.Find("BGM").GetComponent<AudioSource>();
		effect = transform.Find("Effect").GetComponent<AudioSource>();

	}

	private void OnEnable()
	{
		Setting_BGM();
		Setting_Effects();
	}

	private void Setting_BGM()
	{
		bgm.playOnAwake = false;
		bgm.clip = null;
		bgm.loop = true;
		bgm.volume = 0.25f;
	}
	private void Setting_Effects()
	{
		effect.playOnAwake = false;
		effect.clip = null;
		effect.loop = false;
		effect.volume = 0.8f;
	}

	public void Play_Click()
	{
		effect.PlayOneShot(clickClip, uiVolume * masterVolume);
	}

	public void SetVolume(bool isMuted)
	{
		if (isMuted)
		{
			audioMixer.SetFloat("BGM", -80f); 
		}
		else
		{
			audioMixer.SetFloat("BGM", 0.25f);
		}
	}
}
