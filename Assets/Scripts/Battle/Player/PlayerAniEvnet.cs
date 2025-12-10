
using UnityEngine;

public class PlayerAniEvnet : MonoBehaviour
{
	[SerializeField] private IAudioHandler audioHandler;

	private void Awake()
	{
		audioHandler = transform.parent.GetComponent<IAudioHandler>();

	}

	public void Play_MoveSound()
	{
		audioHandler.Play_OneShot(SoundType.Player_Move);
	}
}
