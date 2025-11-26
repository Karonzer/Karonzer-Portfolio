using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.AI;
public class EnemyType1 : Enemy
{

	protected override void Awake()
	{
		base.Awake();

		navigation = GetComponent<NavMeshAgent>();

		StateMachine.AddState(new EnemyTrackingState());
		StateMachine.AddState(new EnemyAttackState());
	}
	protected override void Start()
	{
		base.Start();
		targetNavigation = GSC.Instance.gameManager.Get_PlayerObject();
	}


	private void OnEnable()
	{
		OnDead += Die_Enemy;
	}

	private void OnDisable()
	{
		OnDead -= Die_Enemy;
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
		if(targetNavigation.TryGetComponent<IDamageable>(out IDamageable _player))
		{
			_player.Take_Damage(enemyStruct.damage);
		}
	}

	public override void Die_Enemy()
	{
		base.Die_Enemy();
		GameObject obj = GSC.Instance.spawnManager.Spawn(PoolObjectType.Item, GSC.Instance.gameManager.Get_ItemDataSO().xPItem);
		if (obj.TryGetComponent<Item>(out Item _item))
		{
			Vector3 spawnPosition = transform.position += new Vector3(0, 0.2f, 0);
			_item.Setting_SpwnPos(spawnPosition);
		}

		if (obj.TryGetComponent<XPitem>(out XPitem xpItem))
		{
			xpItem.SetXP(enemyStruct.xpItmeValue);
		}
	}

}

