using UnityEngine;

public class GSC : GenericSingletonClass<GSC>
{
	public GameManager gameManager {  get; private set; }
	public StatManager statManager { get; private set; }
	public SkillManager skillManager { get; private set; }
	public SpawnManager spawnManager { get; private set; }

	public void RegisterGameManager(GameManager _gameManager) => gameManager = _gameManager;

	public void RegisterStatManager(StatManager _stat) => statManager = _stat;
	public void RegisterSkillManager(SkillManager _skill) => skillManager = _skill;
	public void RegisterSpawn(SpawnManager _spawn) => spawnManager = _spawn;
}
