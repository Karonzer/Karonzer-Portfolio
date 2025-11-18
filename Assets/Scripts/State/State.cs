using UnityEngine;

public abstract class State<T> : MonoBehaviour
{
	public abstract StateID StateID { get; }
	public abstract void OnEnter(T target);
	public abstract void OnExit(T target);

	public abstract void UpdateState(T target);
}
