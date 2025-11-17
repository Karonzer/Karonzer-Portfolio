using UnityEngine;

public class GSC : GenericSingletonClass<GSC>
{
	public StatManager statManager { get; private set; }
	public SkillManager skillManager { get; private set; }
	public SpawnManager spawn { get; private set; }

	public void RegisterStatManager(StatManager _stat) => statManager = _stat;
	public void RegisterSkillManager(SkillManager _skill) => skillManager = _skill;
	public void RegisterSpawn(SpawnManager _spawn) => spawn = _spawn;
}
