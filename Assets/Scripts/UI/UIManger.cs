using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIManger : MonoBehaviour
{
	private Dictionary<string, GameObject> uiTable = new Dictionary<string, GameObject>();
	private List<IUIInitializable> uiInitializables = new List<IUIInitializable>();
	public XPBarUI xpBarUI;
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
		Debug.Log("UIManger");
	}

	public void Initialize_UI(GameObject _player)
	{
		foreach (var ui in uiInitializables)
		{
			ui.Initialize_UI(_player);
		}
	}

	public void Register_UI(string _name, GameObject _uiObject)
	{
		if (!uiTable.ContainsKey(_name))
			uiTable.Add(name, _uiObject);
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


}
