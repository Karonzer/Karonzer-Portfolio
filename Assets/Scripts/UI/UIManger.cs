using System.Collections.Generic;
using UnityEngine;

public class UIManger : MonoBehaviour
{
	private Dictionary<string, GameObject> uiTable = new Dictionary<string, GameObject>();
	public XPBarUI xpBarUI;
	private void Awake()
	{
		GSC.Instance.RegisterUIManger(this);
		foreach (Transform child in transform)
		{
			uiTable.Add(child.name, child.gameObject);
		}
	}

	public void InitializeUI(GameObject _player)
	{
		xpBarUI.Initialize(_player.GetComponent<IXPTable>());
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
