using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.AI;
public class EnemyType1 : Enemy, IDamageable
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
		StateMachine.ChangeState(StateID.tracking);
	}

	public void Take_Damage(int damageInfo)
	{
		enemyStruct.currentHP -= damageInfo;
		if (enemyStruct.currentHP <= 0)
		{
			Debug.Log("EnemyType1 is die");
			transform.gameObject.SetActive(false);
		}
	}

	public override void DoAttack()
	{
		// EnemyType1 전용 공격 로직
	}

}

