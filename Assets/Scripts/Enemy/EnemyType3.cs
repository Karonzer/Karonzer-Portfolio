using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class EnemyType3 : Enemy
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
		targetNavigation = GSC.Instance.gameManager.Get_PlayerObject();
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
		Launch_Projectile();
	}

	private void Launch_Projectile()
	{
		if (GSC.Instance.gameManager != null && !GSC.Instance.gameManager.isPaused)
		{
			if (Find_TargetEnemyDir(out Vector3 _direction))
			{
				GameObject projectileObj = GSC.Instance.spawnManager.Spawn(PoolObjectType.Projectile, "EnemyProjectile");
				if (projectileObj.TryGetComponent<Projectile>(out Projectile _Component))
				{
					projectileObj.gameObject.SetActive(true);
					Vector3 spawnOffset = _direction.normalized * 0.5f;
					Vector3 spawnPosition = transform.position + spawnOffset;
					spawnPosition += new Vector3(0, 0.2f, 0);
					_Component.Set_ProjectileInfo("EnemyProjectile", enemyStruct.damage, 1, _direction, 10, 10, spawnPosition);
					_Component.Launch_Projectile();
				}
			}


		}

	}

	private bool Find_TargetEnemyDir(out Vector3 _direction)
	{
		_direction = Vector3.zero;
		GameObject target = null;
		int playerLayerMask = LayerMask.GetMask("Player");

		int count = Physics.OverlapSphereNonAlloc(transform.position, enemyStruct.attackRange, playerBuffer, playerLayerMask);

		if (count == 0)
			return false;

		for (int i = 0; i < count; i++)
		{
			var col = playerBuffer[i];
			if (col != null && col.CompareTag("Player"))
			{
				target = playerBuffer[i].gameObject;
				break;
			}
		}

		if (target != null)
		{
			_direction = (target.transform.position - transform.position).normalized;
			return true;
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
