using UnityEngine;

/// <summary>
/// EnemyAniEvent
/// 
/// 적 애니메이션 이벤트 전용 스크립트.
/// - Animation Clip에서 특정 프레임에 함수를 호출할 수 있는데,
///   그 호출을 실제 공격 로직(Do_EnemyAttackEvent)으로 연결해준다.
///
/// 핵심 목적:
/// - 공격이 나가는 타이밍을 코드가 아니라 애니메이션 프레임으로 맞추기 위함.
/// - EnemyAttackState에서 공격을 시도(쿨타임 체크)하고,
///   실제 공격 발사/히트 판정은 애니메이션 이벤트에서 실행하는 구조에 잘 맞는다.
/// </summary>
public class EnemyAniEvent : MonoBehaviour
{
	/// <summary>
	/// 실제 공격 실행 함수를 가진 대상(IEnemyDoAttack)
	/// </summary>
	private IEnemyDoAttack enemyDoAttack;

	private void Awake()
	{
		enemyDoAttack = transform.GetComponent<IEnemyDoAttack>();
	}

	/// <summary>
	/// 애니메이션 이벤트에서 호출되는 함수.
	/// - 게임이 일시정지 상태면 공격이 나가면 안 되므로 차단
	/// - enemyDoAttack이 존재하면 Do_EnemyAttackEvent()를 호출해서 실제 공격 수행
	///
	/// 예:
	/// - 공격 애니메이션의 휘두르는 순간 프레임에서 이 함수를 호출
	/// </summary>
	public void Do_EnemyDoAttack()
	{
		if (BattleGSC.Instance.gameManager != null && !BattleGSC.Instance.gameManager.isPaused)
		{
			if (enemyDoAttack != null)
			{
				enemyDoAttack.Do_EnemyAttackEvent();
			}
		}

	}
}
