using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// StateMachine<T>
/// 
/// 제네릭 상태 머신 클래스.
/// - owner(T)를 기준으로 여러 상태(State)를 관리한다.
/// - 현재 상태를 하나만 유지하고,
///   상태 변경 시 OnExit → OnEnter 순서로 호출한다.
///
/// 주 사용처:
/// - Enemy AI (추적 / 공격 / 사망)
/// - 캐릭터 상태 관리
/// </summary>
public class StateMachine<T>
{
	/// <summary>
	/// 이 상태 머신을 소유한 객체
	/// (예: Enemy, Boss 등)
	/// </summary>
	private readonly T owner;

	/// <summary>
	/// 상태 목록
	/// - StateID를 key로 사용
	/// - 각 상태는 IState<T>를 구현해야 한다
	/// </summary>
	private readonly Dictionary<StateID, IState<T>> states
		= new Dictionary<StateID, IState<T>>();

	public IState<T> CurrentState { get; private set; }

	/// <summary>
	/// 생성자
	/// - 상태 머신의 owner를 지정
	/// </summary>
	public StateMachine(T owner)
	{
		this.owner = owner;
	}

	/// <summary>
	/// 상태 추가
	/// - 같은 StateID가 들어오면 기존 상태를 덮어쓴다
	/// </summary>
	public void AddState(IState<T> state)
	{
		states[state.ID] = state;
	}

	/// <summary>
	/// 상태 변경
	/// 흐름:
	/// 1) 현재 상태가 있으면 OnExit 호출
	/// 2) 새 상태로 교체
	/// 3) 새 상태의 OnEnter 호출
	/// </summary>
	public void ChangeState(StateID id)
	{
		if (!states.TryGetValue(id, out var next))
			return;

		CurrentState?.OnExit(owner);
		CurrentState = next;
		CurrentState?.OnEnter(owner);
	}

	/// <summary>
	/// 현재 상태의 Tick 실행
	/// - 보통 Update()에서 매 프레임 호출된다
	/// </summary>
	public void Tick()
	{
		CurrentState?.Tick(owner);
	}
}
