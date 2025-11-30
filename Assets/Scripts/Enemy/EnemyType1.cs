using UnityEngine;
using System.Collections;
using UnityEngine.AI;
public class EnemyType1 : Enemy
{
	private readonly Collider[] enemyBuffer = new Collider[1];
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
		targetNavigation = GSC.Instance.gameManager.Get_PlayerObject();
		base.Start();
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
		stateMachine.ChangeState(StateID.tracking);
	}

	public override void Take_Damage(DamageInfo _damageInfo)
	{
		base.Take_Damage(_damageInfo);
	}

	public override void DoAttack()
	{
		animator.SetTrigger("Attack");
		// EnemyType1 전용 공격 로직
		if (Check_WhetherThereIsPlayerInTheRange())
		{
			if (targetNavigation.TryGetComponent<IDamageable>(out IDamageable _player))
			{

				DamageInfo info = GSC.Instance.gameManager.Get_EnemyDamageInfo(enemyStruct.damage, enemyStruct.key, _player.CurrentObj, Type.Player);
				_player.Take_Damage(info);
			}
		}
	}

	private bool Check_WhetherThereIsPlayerInTheRange()
	{
		int playerLayerMask = LayerMask.GetMask("Player");
		int count = Physics.OverlapSphereNonAlloc(transform.position, enemyStruct.attackRange, enemyBuffer, playerLayerMask);
		for (int i = 0; i < count; i++)
		{
			if (enemyBuffer[i].gameObject.CompareTag("Player"))
			{
				float dist = Vector3.Distance(enemyBuffer[i].transform.position, transform.transform.position);
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
		navigation.isStopped = true;
		navigation.speed = 0f;
		animator.SetTrigger("Die");

		StartCoroutine(Handle_DieSequence());
	}

	private IEnumerator Handle_DieSequence()
	{
		AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
		yield return new WaitForSeconds(info.length);

		Spawn_XPItem();
	}

	private void Spawn_XPItem()
	{
		base.Die_Enemy(this);

		GameObject obj = GSC.Instance.spawnManager.Spawn(PoolObjectType.Item, GSC.Instance.gameManager.Get_ItemDataSO().xPItem);

		if (obj.TryGetComponent<Item>(out Item _item))
		{
			Vector3 spawnPosition = transform.position + new Vector3(0, 0.2f, 0);
			_item.Setting_SpwnPos(spawnPosition);
		}

		if (obj.TryGetComponent<XPitem>(out XPitem xpItem))
		{
			xpItem.SetXP(enemyStruct.xpItmeValue);
		}
	}


}

