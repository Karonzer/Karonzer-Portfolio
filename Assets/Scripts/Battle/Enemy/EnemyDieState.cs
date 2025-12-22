using UnityEngine;
/// <summary>
/// EnemyDieState
/// 
/// 적 AI 상태 중 사망(Die) 상태.
/// - 사망했을 때 이동을 완전히 멈추기 위해 NavMeshAgent를 정지시킨다.
/// - 이 상태는 보통 더 이상 행동(Tick)이 없고,
///   사망 애니메이션/드랍/디스폰 처리는 Enemy 본체나 애니메이션 이벤트에서 진행된다.
/// </summary>
public class EnemyDieState : IState<Enemy>
{
	public StateID ID => StateID.die;

	/// <summary>
	/// 사망 상태에 진입할 때 호출됨
	/// - NavMeshAgent가 존재하고 NavMesh 위에 있을 때만 안전하게 정지
	/// - isOnNavMesh 체크는 NavMeshAgent가 아직 배치되지 않았거나,
	///   비정상 위치에서 에러가 나는 상황을 방지하기 위한 안전장치
	/// </summary>
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
