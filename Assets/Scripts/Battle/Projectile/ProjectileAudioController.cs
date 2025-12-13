using UnityEngine;

public class ProjectileAudioController : AudioController
{
	private void OnEnable()
	{
		audioSource.volume = 0.15f;
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
		Play_SkillSound(_type);
	}

	public void Play_SkillSound(SoundType _type)
	{
		if (GlobalGSC.Instance.audioManager.Get_AudioClip(_type, out AudioClip _clip) && !BattleGSC.Instance.gameManager.isPaused)
		{
			audioSource.PlayOneShot(_clip);
		}
	}

}
