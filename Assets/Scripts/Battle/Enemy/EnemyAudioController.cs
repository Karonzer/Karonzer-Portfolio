using UnityEngine;

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


	public void Play_Sound(SoundType _type)
	{
		if (GlobalGSC.Instance.audioManager.Get_AudioClip(_type, out AudioClip _clip) && !BattleGSC.Instance.gameManager.isPaused)
		{
			audioSource.PlayOneShot(_clip);
		}
	}
}
