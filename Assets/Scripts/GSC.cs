using UnityEngine;

public class GSC : GenericSingletonClass<GSC>
{
	public StatManager StatManager { get; private set; }
	public SkillManager SkillManager { get; private set; }
	public AttackShardSystemManager attackShardSystemManager { get; private set; }
	public SpawnManager Spawn { get; private set; }

	public void RegisterStatManager(StatManager _stat) => StatManager = _stat;
	public void RegisterSkillManager(SkillManager _skill) => SkillManager = _skill;
	public void RegisterAttackShardSystemManager(AttackShardSystemManager _attack) => attackShardSystemManager = _attack;
	public void RegisterSpawn(SpawnManager _spawn) => Spawn = _spawn;
}
