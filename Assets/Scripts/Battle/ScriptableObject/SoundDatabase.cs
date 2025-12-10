using UnityEngine;

[CreateAssetMenu(fileName = "SoundDB", menuName = "Audio/Sound Database")]
public class SoundDatabase : ScriptableObject
{
	public SoundEntry[] entries;

	[System.Serializable]
	public class SoundEntry
	{
		public SoundType type;
		public AudioClip clip;
	}

	public AudioClip GetClip(SoundType type)
	{
		foreach (var e in entries)
			if (e.type == type)
				return e.clip;

		return null;
	}
}
