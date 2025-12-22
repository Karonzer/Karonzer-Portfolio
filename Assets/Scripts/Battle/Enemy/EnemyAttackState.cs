
using UnityEngine;

/// <summary>
/// EnemyAttackState
/// 
/// 적 AI 상태 중 "공격(Attack)" 상태.
/// - 이동을 멈춘다.
/// - 플레이어를 바라본다.
/// - 일정한 공격 간격(attackInterval)마다 공격을 실행한다.
/// - 공격 사거리에서 벗어나면 다시 추적 상태로 돌아간다.
/// </summary>
public class EnemyAttackState : IState<Enemy>
{
	public StateID ID => StateID.Attack;

	private float attackTimer;

	/// <summary>
	/// 공격 상태에 진입했을 때 호출됨
	/// - NavMeshAgent 이동을 멈춤
	/// - 속도를 0으로 만들어 완전히 정지
	/// </summary>
	public void OnEnter(Enemy enemy)
	{
		if (enemy.Get_NavMeshAgent() != null)
		{
			enemy.Get_NavMeshAgent().isStopped = true;
			enemy.Get_NavMeshAgent().speed = 0;
		}
	}

	/// <summary>
	/// 매 프레임 호출되는 공격 상태 로직
	/// 흐름:
	/// 1) 플레이어를 바라보게 회전
	/// 2) 공격 사거리 체크
	/// 3) 사거리 밖이면 Tracking 상태로 전환
	/// 4) 공격 쿨타임이 끝났으면 공격 실행
	/// </summary>
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

	/// <summary>
	/// 공격 상태에서 빠져나갈 때 호출됨
	/// - 공격 애니메이션 트리거 초기화
	/// </summary>
	public void OnExit(Enemy enemy)
	{
		enemy.Get_Animator().ResetTrigger("Attack");
		// 필요시 정리
	}
}
