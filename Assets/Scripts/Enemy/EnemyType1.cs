using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.AI;
public class EnemyType1 : Enemy
{

	protected override void Awake()
	{
		enemyName = DBManager.enemyType1;
		base.Awake();

		targetNavigation = GSC.Instance.gameManager.Get_PlayerObject();
		navigation = GetComponent<NavMeshAgent>();

		StateMachine.AddState(new EnemyTrackingState());
		StateMachine.AddState(new EnemyAttackState());
	}


	public override void Start_Enemy()
	{
		base.Setting_Info();
		StateMachine.ChangeState(StateID.tracking);
	}

	public override void Take_Damage(int damageInfo)
	{
		Vector3 hitPos = transform.position + Vector3.up * 1.8f;
		InvokeDamaged(damageInfo, hitPos,enemyType);
		InvokeHealthChanged();
		base.Take_Damage(damageInfo);
	}

	public override void DoAttack()
	{
		// EnemyType1 전용 공격 로직
	}

}

