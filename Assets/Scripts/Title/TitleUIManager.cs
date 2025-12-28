using System.Collections.Generic;
using UnityEngine;

public class TitleUIManager : MonoBehaviour
{
	public ITitleHandler<Title> titleHandler;
	private Dictionary<UIType, IUIHandler> handlers = new Dictionary<UIType, IUIHandler>();


	private void Awake()
	{
		// 자식들에서 IUIHandler를 모두 수집하여 등록
		var handlersInChildren = GetComponentsInChildren<IUIHandler>(true);
		foreach (var h in handlersInChildren)
			RegisterHandler(h);
	}

	private void OnEnable()
	{

	}

	public void Setting_TitleHandler(ITitleHandler<Title> _titleHandler)
	{
		titleHandler = _titleHandler;
	}

	/// <summary>
	/// 핸들러 등록
	/// </summary>
	public void RegisterHandler(IUIHandler _handler)
	{
		handlers[_handler.Type] = _handler;
	}

	/// <summary>
	/// 단순 Show
	/// </summary>
	public void Show(UIType type)
	{
		if (handlers.TryGetValue(type, out var _handler))
			_handler.Show();
	}

	/// <summary>
	/// Show + 초기화 데이터 전달 (필요한 UI만 obj를 사용)
	/// </summary>
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

	/// <summary>
	/// 특정 UI가 열려있는지 조회
	/// </summary>
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

	public Title Get_ITitleHandle()
	{
		throw new System.NotImplementedException();
	}
}
