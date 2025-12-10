using UnityEngine;

public class AudioController : MonoBehaviour , IAudioHandler
{
	[SerializeField] protected AudioSource audioSource;
	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
	}

	public virtual void Play(SoundType _type)
	{

	}

	public virtual void Stop()
	{
		audioSource.Stop();
	}

	public virtual void Play_OneShot(SoundType _type)
	{

	}
}
