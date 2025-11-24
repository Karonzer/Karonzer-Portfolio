using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
	public GameObject player;
	[SerializeField] private string currentPlayerKey;
	public string CurrentPlayerKey => currentPlayerKey;


	[SerializeField] private ItemDataSO itemData;
	[SerializeField] private PopUpDataSO popUpData;

	private Coroutine spwanTimeRoutine;



	public bool isPaused { get; private set; }

	private void Awake()
	{
		GSC.Instance.RegisterGameManager(this);

		currentPlayerKey = "Player";// 추후 타이틀 씬에서 선택한 이름을 가지고 올 예정
	}
	void Start()
	{
		GSC.Instance.uIManger.InitializeUI(player);
	}

	private void OnEnable()
	{
		isPaused = false;
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


	public ItemDataSO Get_ItemDataSO()
	{ return itemData; }

	public PopUpDataSO Get_PopUpData()
	{ return popUpData; }


	public void PauseGame()
	{
		isPaused = true;
	}

	public void ResumeGame()
	{
		isPaused = false;
	}

}
