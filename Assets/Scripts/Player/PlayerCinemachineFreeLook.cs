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

		if(BattleGSC.Instance.gameManager.isPaused)
		{
			inputProvider.enabled = false;
		}
		else
		{
			inputProvider.enabled = true;
		}	
	}
}
