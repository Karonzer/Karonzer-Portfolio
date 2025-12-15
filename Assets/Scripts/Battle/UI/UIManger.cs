using System.Collections.Generic;
using UnityEngine;


public class UIManger : MonoBehaviour
{
	private Dictionary<UIType, IUIHandler> handlers = new Dictionary<UIType, IUIHandler>();

	private void Awake()
	{
		BattleGSC.Instance.RegisterUIManger(this);

		var handlersInChildren = GetComponentsInChildren<IUIHandler>(true);
		foreach (var h in handlersInChildren)
			RegisterHandler(h);
	}

	private void OnEnable()
	{
		
	}

	public void RegisterHandler(IUIHandler _handler)
	{
		handlers[_handler.Type] = _handler;
	}

	public void Show(UIType type)
	{
		if (handlers.TryGetValue(type, out var _handler))
			_handler.Show();
	}

	public void Show(UIType type, GameObject _obj = null)
	{
		if (handlers.TryGetValue(type, out var _handler))
			_handler.ShowAndInitialie(_obj);
	}

	public void Hide(UIType _uIType)
	{
		if (handlers.TryGetValue(_uIType, out var _handler))
			_handler.Hide();
	}

	public bool IsUIOpen(UIType _uIType)
	{
		if (handlers.TryGetValue(_uIType, out var handler))
		{
			return handler.IsOpen;
		}
		else
		{
			return false;
		}
	}



}
