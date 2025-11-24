using UnityEngine;

public class GSC : GenericSingletonClass<GSC>
{
	public GameManager gameManager {  get; private set; }
	public StatManager statManager { get; private set; }
	public SkillManager skillManager { get; private set; }
	public SpawnManager spawnManager { get; private set; }
	public DamagePopupManager damagePopupManager { get; private set; }
	public UIManger uIManger { get; private set; }


	public void RegisterGameManager(GameManager _gameManager) => gameManager = _gameManager;

	public void RegisterStatManager(StatManager _stat) => statManager = _stat;
	public void RegisterSkillManager(SkillManager _skill) => skillManager = _skill;
	public void RegisterSpawn(SpawnManager _spawn) => spawnManager = _spawn;

	public void RegisterDamagePopupManager(DamagePopupManager _damagePopupManager) => damagePopupManager = _damagePopupManager;

	public void RegisterUIManger(UIManger _uIManger) => uIManger = _uIManger;
}
