using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class EnemyType4 : Enemy
{
	private readonly Collider[] playerBuffer = new Collider[1];
	protected override void Awake()
	{
		base.Awake();

		navigation = GetComponent<NavMeshAgent>();

		stateMachine.AddState(new EnemyTrackingState());
		stateMachine.AddState(new EnemyAttackState());
		stateMachine.AddState(new EnemyDieState());


		if (meshRenderer != null)
			hitMatInstance = meshRenderer.material;

	}
	protected override void Start()
	{
		targetNavigation = BattleGSC.Instance.gameManager.Get_PlayerObject();
		base.Start();
	}


	protected override void OnEnable()
	{
		base.OnEnable();
		OnDead += Die_Enemy;
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		OnDead -= Die_Enemy;
	}

	public override void Start_Enemy()
	{
		base.Setting_Info();
		stateMachine.ChangeState(StateID.tracking);
	}

	public override void Take_Damage(DamageInfo _damageInfo)
	{
		base.Take_Damage(_damageInfo);
	}

	public override void Do_EnemyAttack()
	{
		animator.SetTrigger("Attack");
	}

	public override void Do_EnemyAttackEvent()
	{
		if (Check_WhetherThereIsPlayerInTheRange())
		{
			if (targetNavigation.TryGetComponent<IDamageable>(out IDamageable _player))
			{

				DamageInfo info = BattleGSC.Instance.gameManager.Get_EnemyDamageInfo(enemyStruct.damage, enemyStruct.key, _player.CurrentObj, Type.Player);
				_player.Take_Damage(info);
			}
		}
	}

	private bool Check_WhetherThereIsPlayerInTheRange()
	{
		int playerLayerMask = LayerMask.GetMask("Player");
		int count = Physics.OverlapSphereNonAlloc(transform.position, enemyStruct.attackRange, playerBuffer, playerLayerMask);
		for (int i = 0; i < count; i++)
		{
			if (playerBuffer[i].gameObject.CompareTag("Player"))
			{
				float dist = Vector3.Distance(playerBuffer[i].transform.position, transform.transform.position);
				if (dist <= enemyStruct.attackRange)
				{
					return true;
				}
			}
		}
		return false;
	}



	public override void Die_Enemy(IDamageable _damageable)
	{
		gameObject.layer = LayerMask.NameToLayer("Dead");
		foreach (var col in GetComponentsInChildren<Collider>())
			col.enabled = false;

		navigation.isStopped = true;
		navigation.speed = 0f;

		base.Die_Enemy(this);
		Spawn_XPItem();

	}


}
