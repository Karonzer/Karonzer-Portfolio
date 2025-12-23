using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 화면에 표시되었다가 자동으로 사라지는 텍스트 팝업.
/// 
/// 역할:
/// - Show 호출 시 1초간 표시
/// - 코루틴으로 자동 Hide
/// 
/// 사용 예:
/// - 획득 같은 간단 안내
/// - 짧은 경고 메시지
/// 
/// - UIManger에서 UIType으로 Show/Hide 제어 가능
/// </summary>
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

	/// <summary>
	/// 일정 시간 후 자동으로 숨김
	/// </summary>
	IEnumerator time_Hide()
	{
		yield return new WaitForSeconds(1f);
		Hide();
	}



}
