using UnityEngine;

public class UILookCamera : MonoBehaviour
{
	private Camera mainCam;
	private void Awake()
	{

		mainCam = Camera.main;
	}

	void LateUpdate()
	{
		if (mainCam != null)
			transform.forward = mainCam.transform.forward;
	}
}
