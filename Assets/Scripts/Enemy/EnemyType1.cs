using UnityEngine;

public class EnemyType1 : Enemy, IDamageable
{
	private void Awake()
	{
		enemyName = "EnemyType1";
	}
	protected override void Start()
	{
		base.Start();
	}


	public void Take_Damage(int damageInfo)
	{
		Debug.Log($"damageInfo : {damageInfo}");
		enemyStruct.currentHP -= damageInfo;
		Debug.Log($"name : {transform.name} , enemyData.currentHP : {enemyStruct.currentHP} ");

		if (enemyStruct.currentHP <= 0)
		{
			Debug.Log("EnemyType1 is die");
			transform.gameObject.SetActive(false);
		}
	}
}

