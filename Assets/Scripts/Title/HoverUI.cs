using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
public class HoverUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	[SerializeField] private GameObject originalBtn;
	private Coroutine hoverCoroutine;

	private void Awake()
	{
		originalBtn = transform.gameObject;
	}
	private void OnEnable()
	{
		originalBtn.gameObject.SetActive(true);
		originalBtn.transform.localScale = Vector3.one;
		Setting_HoverCoroutine();
	}

	private void Setting_HoverCoroutine()
	{
		if (hoverCoroutine != null)
		{
			StopCoroutine(hoverCoroutine);
			hoverCoroutine = null;
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		Debug.Log("Hover On");
		Setting_HoverCoroutine();
		hoverCoroutine = StartCoroutine(HoverOnEffect());
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		Debug.Log("Hover Off");
		Setting_HoverCoroutine();
		hoverCoroutine = StartCoroutine(HoverOffEffect());
	}

	private IEnumerator HoverOnEffect()
	{
		while (true)
		{
			originalBtn.transform.localScale = Vector3.Lerp(originalBtn.transform.localScale, Vector3.one * 1.1f, Time.deltaTime * 5);
			yield return null;
		}
	}

	private IEnumerator HoverOffEffect()
	{
		while (true)
		{
			originalBtn.transform.localScale = Vector3.Lerp(originalBtn.transform.localScale, Vector3.one, Time.deltaTime * 5);
			yield return null;
		}
	}


}
