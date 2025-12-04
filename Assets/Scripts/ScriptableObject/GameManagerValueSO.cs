using UnityEngine;

[CreateAssetMenu(fileName = "GameManagerValue", menuName = "Game/GameManagerValue")]
public class GameManagerValueSO : ScriptableObject
{
	public int minSpawnCount;
	public int maxSpawnCount;
	public int spawnCount;
	public float enemySpawnInterval;
	public float bossSpawnInterval;
}
