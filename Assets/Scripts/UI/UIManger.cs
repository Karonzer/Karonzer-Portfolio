using System.Collections.Generic;

using UnityEngine;

public class UIManger : MonoBehaviour
{
	private Dictionary<UIType, IUIHandler> handlers = new Dictionary<UIType, IUIHandler>();

	private void Awake()
	{
		GSC.Instance.RegisterUIManger(this);

		var handlersInChildren = GetComponentsInChildren<IUIHandler>(true);
		foreach (var h in handlersInChildren)
			RegisterHandler(h);
	}

	public void RegisterHandler(IUIHandler handler)
	{
		handlers[handler.Type] = handler;
	}

	public void Show(UIType type)
	{
		if (handlers.TryGetValue(type, out var handler))
			handler.Show();
	}

	public void Show(UIType type, GameObject _obj = null)
	{
		if (handlers.TryGetValue(type, out var handler))
			handler.Show(_obj);
	}

	public void Hide(UIType type)
	{
		if (handlers.TryGetValue(type, out var handler))
			handler.Hide();
	}


}
