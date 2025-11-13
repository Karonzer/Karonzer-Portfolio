using UnityEngine;

public class GSC : GenericSingletonClass<GSC>
{
	public SpawnManager Spawn { get; private set; }

	public void RegisterSpawn(SpawnManager spawn) => Spawn = spawn;
}
