using UnityEngine;

public class EnemyTrackingState : IState<Enemy>
{
	public StateID ID => StateID.tracking;

	public void OnEnter(Enemy enemy)
	{
		if (enemy.Get_NavMeshAgent() != null)
		{
			enemy.Get_NavMeshAgent().isStopped = false;
			enemy.Get_NavMeshAgent().speed = enemy.Get_EnemyStruct().moveSpeed;
		}
	}

	public void Tick(Enemy enemy)
	{
		if (enemy.Get_TargetNavigation() == null || enemy.Get_NavMeshAgent() == null)
			return;

		enemy.Get_NavMeshAgent().destination = enemy.Get_TargetNavigation().transform.position;

		float distance = Vector3.Distance(
			enemy.transform.position,
			enemy.Get_TargetNavigation().transform.position);

		if (distance <= enemy.Get_EnemyStruct().attackRange)
		{
			enemy.StateMachine.ChangeState(StateID.Attack);
		}
	}

	public void OnExit(Enemy enemy)
	{
		// 필요하면 정리 로직
	}
}
