using Unity.Cinemachine;
using UnityEngine;

public class PlayerCinemachineFreeLook : MonoBehaviour
{
	[SerializeField] private CinemachineInputAxisController inputProvider;

	private void Awake()
	{
		inputProvider = GetComponent<CinemachineInputAxisController>();
	}

	private void Update()
	{

		if(GSC.Instance.gameManager.IsPaused)
		{
			inputProvider.enabled = false;
		}
		else
		{
			inputProvider.enabled = true;
		}	
	}
}
