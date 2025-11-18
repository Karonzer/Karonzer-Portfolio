using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.AI;
public class EnemyType1 : Enemy, IDamageable
{

	private Coroutine MoveCoroutine;
	private void Awake()
	{
		enemyName = "EnemyType1";
	}
	protected override void Start()
	{
		base.Start();
		targetNavigation = GSC.Instance.gameManager.Get_PlayerObject();
		navigation = GetComponent<NavMeshAgent>();
	}


	public override void Start_Enemy()
	{
		if (MoveCoroutine != null)
		{
			StopCoroutine(MoveCoroutine);
			MoveCoroutine = null;
		}
		MoveCoroutine = StartCoroutine(Move_EnemyType1());
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
	IEnumerator Move_EnemyType1()
	{
		while (true)
		{
			if (targetNavigation == null)
				yield break;

			navigation.destination = targetNavigation.transform.position;

			float distance = Vector3.Distance(transform.position, targetNavigation.transform.position);

			if (distance > enemyStruct.attackRange)
			{
				navigation.isStopped = false;
				navigation.speed = enemyStruct.moveSpeed;
			}
			else
			{
				navigation.isStopped = true;
			}

			yield return null;
		}
	}
}

