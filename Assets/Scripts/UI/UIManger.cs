using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIManger : MonoBehaviour
{
	private Dictionary<string, GameObject> uiTable = new Dictionary<string, GameObject>();
	private List<IUIInitializable> uiInitializables = new List<IUIInitializable>();
	private void Awake()
	{
		GSC.Instance.RegisterUIManger(this);
		foreach (Transform child in transform)
		{
			uiTable.Add(child.name, child.gameObject);
		}

		uiInitializables = GetComponentsInChildren<IUIInitializable>(true).ToList();
	}

	private void Start()
	{
	}

	public void Initialize_UI(GameObject _player)
	{
		foreach (var ui in uiInitializables)
		{
			ui.Initialize_UI(_player);
		}
	}

	public void Start_UI()
	{

	}

	public void Register_UI(string _name, GameObject _uiObject)
	{
		uiTable.Add(_name, _uiObject);
	}

	public void Show_UI(string _name)
	{
		if (uiTable.TryGetValue(_name, out var ui))
			ui.SetActive(true);
	}

	public void Hide_UI(string _name)
	{
		if (uiTable.TryGetValue(_name, out var ui))
			ui.SetActive(false);
	}

	[SerializeField] private BossHPHUD bossHPHUD;

	public void Show_BossHPUI(IHealthChanged boss)
	{
		bossHPHUD.gameObject.SetActive(true);
		bossHPHUD.Setting_BossUIBar(boss);
	}


}
