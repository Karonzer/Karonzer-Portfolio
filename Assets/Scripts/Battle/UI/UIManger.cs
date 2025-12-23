using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UIType 기반으로 UI 핸들러(IUIHandler)를 등록/관리하는 매니저.
/// 
/// 특징:
/// - 씬 내 모든 IUIHandler를 자동으로 찾아 등록 (비활성 오브젝트 포함)
/// - Show/Hide 호출을 UIType으로 통일
/// 
/// 이 구조의 장점:
/// - 새로운 UI 추가 시, IUIHandler만 구현하면 자동 등록 가능
/// </summary>
public class UIManger : MonoBehaviour
{
	// UIType -> IUIHandler 매핑
	private Dictionary<UIType, IUIHandler> handlers = new Dictionary<UIType, IUIHandler>();

	private void Awake()
	{
		// 전역 접근 등록
		BattleGSC.Instance.RegisterUIManger(this);

		// 자식들에서 IUIHandler를 모두 수집하여 등록
		var handlersInChildren = GetComponentsInChildren<IUIHandler>(true);
		foreach (var h in handlersInChildren)
			RegisterHandler(h);
	}

	private void OnEnable()
	{
		
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



}
