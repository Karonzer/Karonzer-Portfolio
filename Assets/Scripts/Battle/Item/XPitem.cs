using System.Collections;
using UnityEngine;

public class XPitem : Item
{
	private GameObject target;
	private Coroutine followRoutine;

	private int xpAmount;
	private bool isFollowToPlayer;


	protected override void Start()
	{
		base.Start();
	}

	private void OnEnable()
	{
		isFollowToPlayer = false;
		target = null;
		if (followRoutine != null)
		{
			StopCoroutine(followRoutine);
			followRoutine = null;
		}
	}

	public void SetXP(int amount)
	{
		xpAmount = amount;
	}

	public void Start_FollowToPlayer()
	{

		if (followRoutine == null)
		{
			isFollowToPlayer = true;
			target = BattleGSC.Instance.gameManager.Get_PlayerObject();
			followRoutine = StartCoroutine(Follow_ToPlayer(target));
		}
	}


	public void Start_FollowToPlayer(GameObject _obj)
	{

		if (followRoutine == null)
		{
			isFollowToPlayer = true;
			target = _obj;
			followRoutine = StartCoroutine(Follow_ToPlayer(target));
		}
	}

	protected override void Fuction_Event(GameObject _obj)
	{
		if (_obj.TryGetComponent<PlayerLevel>(out var levelSystem))
		{
			levelSystem.AddXP(xpAmount);
		}
	}


	private IEnumerator Follow_ToPlayer(GameObject _obj)
	{
		while (true)
		{
			Vector3 projectileDir = _obj.Get_TargetDir(transform);
			transform.Translate(projectileDir * 5 * Time.deltaTime);
			yield return null;
		}
	}

	public bool Get_isFollowToPlayer()
	{
		return isFollowToPlayer;
	}
}
