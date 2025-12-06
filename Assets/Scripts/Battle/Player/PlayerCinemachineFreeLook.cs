using Unity.Cinemachine;
using UnityEngine;
using System.Collections;

public class PlayerCinemachineFreeLook : MonoBehaviour
{
	[SerializeField] private CinemachineInputAxisController inputProvider;
	private CinemachineImpulseSource impulseSource;
	private float timer = 1;
	private Coroutine shakeCinemachine;
	private void Awake()
	{
		inputProvider = GetComponent<CinemachineInputAxisController>();
		impulseSource = GetComponent<CinemachineImpulseSource>();
	}

	private void OnEnable()
	{
		if(shakeCinemachine != null)
		{
			StopCoroutine(shakeCinemachine);
			shakeCinemachine = null;
		}
	}
	public void Shake(float intensity, float time)
	{
		impulseSource.GenerateImpulse(intensity);
		timer = time;
	}


	public void Start_ShakeCinemachine()
	{
		if (shakeCinemachine != null)
		{
			StopCoroutine(shakeCinemachine);
			shakeCinemachine = null;
		}
		shakeCinemachine = StartCoroutine(ShakeCinemachine());
	}

	IEnumerator ShakeCinemachine()
	{
		yield return null;
		//if (timer > 0)
		//{
		//	timer -= Time.deltaTime;
		//	if (timer <= 0)
		//		perlin.AmplitudeGain = 0;
		//}
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
