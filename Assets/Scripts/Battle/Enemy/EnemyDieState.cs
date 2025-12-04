using UnityEngine;

public class EnemyDieState : IState<Enemy>
{
	public StateID ID => StateID.die;

	public void OnEnter(Enemy enemy)
	{
		if (enemy.Get_NavMeshAgent() != null && enemy.Get_NavMeshAgent().isOnNavMesh)
		{
			enemy.Get_NavMeshAgent().isStopped = true;
			enemy.Get_NavMeshAgent().speed = 0;
		}
	}

	public void OnExit(Enemy enemy)
	{

	}

	public void Tick(Enemy enemy)
	{

	}
}
