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
		Debug.Log($"enemyData.currentHP : {enemyData.currentHP}, damageInfo : {damageInfo}, enemyData.currentHP -= damageInfo : {enemyData.currentHP -= damageInfo} ");
		enemyData.currentHP -= damageInfo;



		if(enemyData.currentHP <=0)
		{
			Debug.Log("EnemyType1 is die");
		}
	}
}

