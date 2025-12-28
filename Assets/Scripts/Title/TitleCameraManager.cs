using UnityEngine;
using Unity.Cinemachine;
public class TitleCameraManager : MonoBehaviour
{

	[SerializeField] private ITitleHandler<Title> titleHandler;
	[SerializeField] private CinemachineCamera mainCam;
	[SerializeField] private CinemachineCamera SelectCam;

	[SerializeField] private int basePriority = 10;
	[SerializeField] private int activePriority = 20;

	public void Setting_TitleHandler(ITitleHandler<Title> _titleHandler)
	{
		titleHandler = _titleHandler;
	}

	private void Start()
	{
		To_MainCam();
	}

	public void To_SelectCam()
	{
		mainCam.Priority = basePriority;
		SelectCam.Priority = activePriority;  
	}

	public void To_MainCam()
	{
		SelectCam.Priority = basePriority;
		mainCam.Priority = activePriority;   
	}
}
