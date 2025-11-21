using UnityEngine;
using System.Collections.Generic;
public class StateMachine<T>
{
	private readonly T owner;
	private readonly Dictionary<StateID, IState<T>> states
		= new Dictionary<StateID, IState<T>>();

	public IState<T> CurrentState { get; private set; }

	public StateMachine(T owner)
	{
		this.owner = owner;
	}

	public void AddState(IState<T> state)
	{
		states[state.ID] = state;
	}

	public void ChangeState(StateID id)
	{
		if (!states.TryGetValue(id, out var next))
			return;

		CurrentState?.OnExit(owner);
		CurrentState = next;
		CurrentState?.OnEnter(owner);
	}

	public void Tick()
	{
		CurrentState?.Tick(owner);
	}
}
