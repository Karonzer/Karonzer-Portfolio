using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public static class Utilities
{
	[SerializeField] private static float gravity = -9.81f;
	public static float Gravity => gravity;

	public static void AddEvent(this Button button, UnityAction action)
	{
		button.onClick.RemoveAllListeners();
		button.onClick.AddListener(action);
	}
}
