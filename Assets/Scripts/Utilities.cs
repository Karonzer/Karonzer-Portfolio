using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public static class Utilities
{
	public static void AddEvent(this Button button, UnityAction action)
	{
		button.onClick.RemoveAllListeners();
		button.onClick.AddListener(action);
	}
}
