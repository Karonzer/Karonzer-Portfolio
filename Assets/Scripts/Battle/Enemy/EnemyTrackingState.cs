using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// EnemyTrackingState
/// 
/// 적 AI 상태 중 추적(Tracking) 상태.
/// - 플레이어(타겟)를 향해 이동한다.
/// - 공격 사거리 안으로 들어오면 Attack 상태로 전환한다.
/// 
/// StateMachine<Enemy>에서 관리되는 상태 중 하나.
/// </summary>
public class EnemyTrackingState : IState<Enemy>
{
	public StateID ID => StateID.tracking;

	/// <summary>
	/// 추적 상태에 진입했을 때 호출됨
	/// - NavMeshAgent 이동 활성화
	/// - 이동 속도 설정
	/// - 추적 애니메이션 ON
	/// </summary>
	public void OnEnter(Enemy enemy)
	{
		if (enemy.Get_NavMeshAgent() != null)
		{
			enemy.Get_NavMeshAgent().isStopped = false;
			enemy.Get_NavMeshAgent().speed = enemy.Get_EnemyStruct().moveSpeed;
			enemy.Get_Animator().SetBool("Tracking", true);
		}
	}

	/// <summary>
	/// 매 프레임 호출되는 로직
	/// - 타겟을 향해 이동
	/// - 공격 사거리 안에 들어오면 Attack 상태로 전환
	/// </summary>
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
			enemy.stateMachine.ChangeState(StateID.Attack);
		}
	}

	/// <summary>
	/// 추적 상태에서 빠져나갈 때 호출됨
	/// - 추적 애니메이션 OFF
	/// </summary>
	public void OnExit(Enemy enemy)
	{
		enemy.Get_Animator().SetBool("Tracking", false);
	}
}
