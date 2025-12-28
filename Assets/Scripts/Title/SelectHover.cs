using UnityEngine;
using UnityEngine.EventSystems;


public class SelectHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public void OnPointerEnter(PointerEventData eventData)
	{
		transform.GetChild(0).gameObject.SetActive(true);

	}

	public void OnPointerExit(PointerEventData eventData)
	{
		transform.GetChild(0).gameObject.SetActive(false);

	}
}
