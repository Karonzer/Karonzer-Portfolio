using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextPopUp : MonoBehaviour, IUIHandler
{
	public UIType Type => UIType.TextPopUp;

	public bool IsOpen => area.activeSelf;

	[SerializeField] private GameObject area;


	private void Awake()
	{
		area = transform.GetChild(0).gameObject;
	}

	private void OnEnable()
	{
		area.SetActive(false);
	}
	public void Hide()
	{
		area.SetActive(false);
	}

	public void Show()
	{
		area.SetActive(true);
		StartCoroutine("time_Hide");
	}

	public void ShowAndInitialie(GameObject _obj = null)
	{
		area.SetActive(true);
		StartCoroutine("time_Hide");
	}

	IEnumerator time_Hide()
	{
		yield return new WaitForSeconds(1f);
		Hide();
	}



}
