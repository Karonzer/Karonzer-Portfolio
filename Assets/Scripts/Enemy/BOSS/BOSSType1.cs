using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class BOSSType1 : Enemy
{
	[SerializeField] private BossSkillSO BossSkillSO;
	[SerializeField] protected BossSkill bossSkill;
	private Coroutine attackTime;
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

		if(attackTime != null)
		{
			StopCoroutine(attackTime);
			attackTime = null;
		}
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
		if (attackTime != null)
		{
			StopCoroutine(attackTime);
			attackTime = null;
		}
		attackTime = StartCoroutine(Create_BossAttackIndicator());
	}

	private IEnumerator Create_BossAttackIndicator()
	{
		for (int i = 0; i < BossSkillSO.bossSkill.projectile.keyCount; i++)
		{
			GameObject projectileObj = GSC.Instance.spawnManager.Spawn(PoolObjectType.Projectile, BossSkillSO.bossSkill.projectile.key);
			if (projectileObj.TryGetComponent<Projectile>(out Projectile _Component))
			{
				projectileObj.gameObject.SetActive(true);
				Vector3 spawnPosition = Get_TargetNavigation().transform.position;
				_Component.Set_ProjectileInfo(BossSkillSO.bossSkill.projectile.key, enemyStruct.damage, BossSkillSO.bossSkill.projectile.baseExplosionRange, 
					Vector3.zero, BossSkillSO.bossSkill.projectile.baseProjectileSpeed, DBManager.ProjectileSurvivalTime, spawnPosition);
			}
			yield return new WaitForSeconds(0.75f);
		}
		attackTime = null;
	}



	public override void Die_Enemy(IDamageable _damageable)
	{
		gameObject.layer = LayerMask.NameToLayer("Dead");
		foreach (var col in GetComponentsInChildren<Collider>())
			col.enabled = false;

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

		for(int i = 0; i < 5;i++)
		{
			GameObject obj = GSC.Instance.spawnManager.Spawn(PoolObjectType.Item, GSC.Instance.gameManager.Get_ItemDataSO().xPItem);

			if (obj.TryGetComponent<Item>(out Item _item))
			{
				float x = Random.Range(-1f, 1.1f);
				float z = Random.Range(-1f, 1.1f);
				Vector3 spawnPosition = transform.position + new Vector3(x, 0.2f, z);
				_item.Setting_SpwnPos(spawnPosition);
			}

			if (obj.TryGetComponent<XPitem>(out XPitem xpItem))
			{
				xpItem.SetXP(enemyStruct.xpItmeValue);
			}
		}

	}
}
