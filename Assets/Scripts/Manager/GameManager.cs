using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
	public GameObject player;

	private Coroutine spwanTimeRoutine;



	public bool IsPaused;// { get; private set; }

	private void Awake()
	{
		GSC.Instance.RegisterGameManager(this);
	}

	private void OnEnable()
	{
		IsPaused = false;
		if (spwanTimeRoutine != null)
		{
			StopCoroutine(spwanTimeRoutine);
			spwanTimeRoutine = null;
		}

		spwanTimeRoutine = StartCoroutine(TEST_Spwn());
	}

	public GameObject Get_PlayerObject()
	{
		return player;
	}	

	IEnumerator TEST_Spwn()
	{
		while(true)
		{
			yield return new WaitForSeconds(2.5f);
			GameObject obj = GSC.Instance.spawnManager.Spawn(PoolObjectType.Enemy,"EnemyType1");
			if (obj != null && obj.TryGetComponent<Enemy>(out var _component))
			{
				obj.gameObject.SetActive(true);
				_component.Start_Enemy();
			}
			yield return new WaitForSeconds(2.5f);
		}
	}



	public void PauseGame()
	{
		IsPaused = true;
	}

	public void ResumeGame()
	{
		IsPaused = false;
	}

}
