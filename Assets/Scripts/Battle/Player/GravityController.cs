using UnityEngine;

public class GravityController : MonoBehaviour
{
	[Header("중력 설정 (gravity는 음수)")]
	[SerializeField] private float gravity = -9.81f;
	[SerializeField] private float gravityScale = 1f;
	[SerializeField] private float terminalVelocity = -50f;

	[Header("점프 설정")]
	[SerializeField] private float jumpHeight = 1.5f;


	[SerializeField] private float velocityY = 0f;
	[SerializeField] private bool isGrounded = false;

	public float VelocityY => velocityY;
	public bool IsGrounded => isGrounded;

	[SerializeField] private float groundCheckDistance = 0.2f;
	[SerializeField] private LayerMask groundMask;

	private void OnEnable()
	{
		groundMask = LayerMask.NameToLayer("Ground");
	}

	private bool CheckGrounded(CharacterController controller)
	{
		Vector3 pos = controller.transform.position;
		float radius = controller.radius * 0.9f;

		Vector3 origin = pos + Vector3.up * 0.1f;

		bool hit = Physics.SphereCast(
			origin,
			radius,
			Vector3.down,
			out RaycastHit hitInfo,
			groundCheckDistance,
			groundMask,
			QueryTriggerInteraction.Ignore
		);

		return hit;
	}



	public Vector3 GetGravityDelta(CharacterController controller)
	{
		bool controllerGround = controller.isGrounded;
		bool rayGround = CheckGrounded(controller);

		isGrounded = controllerGround || rayGround;

		// 2) 바닥 접촉 시 Y속도 초기화
		if (isGrounded && velocityY < 0f)
			velocityY = -2f;

		// 3) 중력 적용
		velocityY += gravity * gravityScale * Time.deltaTime;
		velocityY = Mathf.Max(velocityY, terminalVelocity);

		return new Vector3(0f, velocityY * Time.deltaTime, 0f);
	}

	public void Jump()
	{
		if (!isGrounded) return;
		velocityY = Mathf.Sqrt(jumpHeight * -2f * gravity * gravityScale);
	}



}
