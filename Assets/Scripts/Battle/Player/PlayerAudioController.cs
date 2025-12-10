using UnityEngine;
using UnityEngine.Audio;

public class PlayerAudioController : AudioController
{
	private void OnEnable()
	{
		audioSource.volume = 0.2f;
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

		switch(_type)
		{
			case SoundType.Player_Move:
				Play_MoveSound();
				break;
			case SoundType.Player_Jump:
				Play_Jump();
				break;
		}

	}

	public void Play_MoveSound()
	{
		if (GlobalGSC.Instance.audioManager.Get_AudioClip(SoundType.Player_Move, out AudioClip _clip) && !BattleGSC.Instance.gameManager.isPaused )
		{
			audioSource.PlayOneShot(_clip);
		}
	}

	public void Play_Jump()
	{
		if (GlobalGSC.Instance.audioManager.Get_AudioClip(SoundType.Player_Jump, out AudioClip _clip) && !BattleGSC.Instance.gameManager.isPaused )
		{
			audioSource.PlayOneShot(_clip);
		}
	}

}
