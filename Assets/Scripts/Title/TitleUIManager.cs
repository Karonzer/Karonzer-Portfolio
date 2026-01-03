using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 타이틀 씬에서 사용되는 UI들을 총괄 관리하는 매니저
/// - 자식 오브젝트에 존재하는 IUIHandler들을 자동 수집
/// - UIType을 키로 하여 Show / Hide / 상태 조회를 제공
/// </summary>
public class TitleUIManager : MonoBehaviour
{
	/// <summary>
	/// 타이틀 관련 로직을 처리하는 외부 핸들러
	/// (타이틀 버튼 클릭, 상태 전환 등)
	/// </summary>
	public ITitleHandler<Title> titleHandler;

	/// <summary>
	/// UIType을 기준으로 각 UI 핸들러를 관리하는 딕셔너리
	/// </summary>
	private Dictionary<UIType, IUIHandler> handlers = new Dictionary<UIType, IUIHandler>();


	private void Awake()
	{
		// 자식 오브젝트들 중 IUIHandler를 구현한 모든 컴포넌트를 수집
		// 비활성화된(GameObject inactive) 상태의 UI도 포함
		var handlersInChildren = GetComponentsInChildren<IUIHandler>(true);
		foreach (var h in handlersInChildren)
		{
			RegisterHandler(h);
			h.Hide();
		}

	}

	private void OnEnable()
	{

	}

	/// <summary>
	/// 외부에서 TitleHandler를 주입받아 설정
	/// (GameManager 또는 TitleController 등에서 호출)
	/// </summary>
	public void Setting_TitleHandler(ITitleHandler<Title> _titleHandler)
	{
		titleHandler = _titleHandler;
	}


	/// <summary>
	/// UI 핸들러 등록
	/// UIType을 키로 하여 딕셔너리에 저장
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

	/// <summary>
	/// 특정 UI 숨김 처리
	/// </summary>
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
