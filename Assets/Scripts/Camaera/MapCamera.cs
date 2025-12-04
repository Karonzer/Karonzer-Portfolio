using UnityEngine;

public class MapCamera : MonoBehaviour
{
	private Camera m_Camera;
	private Transform m_CameraTarget;
	private void Awake()
	{
		m_Camera = GetComponent<Camera>();
	}
	private void Start()
	{
		m_CameraTarget = GSC.Instance.gameManager.Get_PlayerObject().transform;
	}

	private void Update()
	{
		if (m_CameraTarget != null)
		{
			Vector3 pos = m_CameraTarget.position;
			pos.y += 100f;
			transform.position = pos;
		}
	}
}
