
using UnityEngine;


public class EnemyAttackState : IState<Enemy>
{
	public StateID ID => StateID.Attack;

	private float attackTimer;
	public void OnEnter(Enemy enemy)
	{
		if (enemy.Get_NavMeshAgent() != null)
		{
			enemy.Get_NavMeshAgent().isStopped = true;
			enemy.Get_NavMeshAgent().speed = 0;
		}
	}

	public void Tick(Enemy enemy)
	{
		if (enemy.Get_TargetNavigation() == null)
			return;

		Vector3 _direction = (enemy.Get_TargetNavigation().transform.position - enemy.transform.position).normalized;
		_direction.y = 0;
		_direction.Normalize();
		enemy.transform.rotation = Quaternion.LookRotation(_direction);

		float distance = Vector3.Distance(
			enemy.transform.position,
			enemy.Get_TargetNavigation().transform.position);

		if (distance > enemy.Get_EnemyStruct().attackRange)
		{
			enemy.stateMachine.ChangeState(StateID.tracking);
			return;
		}

		attackTimer -= Time.deltaTime;
		if (attackTimer <= 0f)
		{
			enemy.Do_EnemyAttack();

			attackTimer = enemy.Get_EnemyStruct().attackInterval;
		}
	}


	public void OnExit(Enemy enemy)
	{
		enemy.Get_Animator().ResetTrigger("Attack");
		// 필요시 정리
	}
}
