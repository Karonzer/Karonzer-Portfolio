using UnityEngine;

[CreateAssetMenu(fileName = "GameManagerValue", menuName = "Game/GameManagerValue")]
public class GameManagerValueSO : ScriptableObject
{
	public int minSpawnCount;
	public int maxSpawnCount;
	public int spawnCount;
	public int spawnCountDecreaseValue;
	public float enemySpawnInterval;
	public float enemySpawnIntervalMin;
	public float enemySpawnIntervalDecrease;
	public float bossSpawnInterval;
	public int LevetCheck;

}
