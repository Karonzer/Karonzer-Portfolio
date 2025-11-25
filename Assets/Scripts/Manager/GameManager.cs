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
	private void Start()
	{
		Debug.Log("Initialize_UI");
		GSC.Instance.uIManger.Initialize_UI(player);
		Setting_Cursor();
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



	private void Setting_Cursor()
	{
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Confined;
	}

	public void Set_ShowAndHideCursor(bool _show)
	{
		Cursor.visible = _show;
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


	public void Update_ToPlayerAttackObj()
	{
		GSC.Instance.upgradeManager.Upgrade_TEST();
		//PauseGame();
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
