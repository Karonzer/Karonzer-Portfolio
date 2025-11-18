using UnityEngine;

public class EnemyAttackState : IState<Enemy>
{
	public StateID ID => StateID.Attack;

	private float attackTimer;

	public void OnEnter(Enemy enemy)
	{
		attackTimer = 0f;
		if (enemy.Get_NavMeshAgent() != null)
			enemy.Get_NavMeshAgent().isStopped = true;
	}

	public void Tick(Enemy enemy)
	{
		if (enemy.Get_TargetNavigation() == null)
			return;

		float distance = Vector3.Distance(
			enemy.transform.position,
			enemy.Get_TargetNavigation().transform.position);

		if (distance > enemy.Get_EnemyStruct().attackRange)
		{
			enemy.StateMachine.ChangeState(StateID.tracking);
			return;
		}

		attackTimer -= Time.deltaTime;
		if (attackTimer <= 0f)
		{
			Debug.Log("공격 테스트");
			enemy.DoAttack();

			attackTimer = enemy.Get_EnemyStruct().attackInterval;
		}
	}

	public void OnExit(Enemy enemy)
	{
		// 필요시 정리
	}
}
