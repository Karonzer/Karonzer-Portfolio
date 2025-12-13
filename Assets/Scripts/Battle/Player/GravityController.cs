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
		groundMask = LayerMask.GetMask("Ground");
	}

	private bool CheckGrounded(CharacterController controller)
	{
		Vector3 origin = controller.transform.position;
		bool hit = Physics.Raycast(origin,Vector3.down,1.1f, groundMask);
		return hit;
	}



	public Vector3 GetGravityDelta(CharacterController controller)
	{
		bool controllerGround = controller.isGrounded;
		bool rayGround = CheckGrounded(controller);

		isGrounded = controllerGround || rayGround;

		if (isGrounded && velocityY < 0f)
			velocityY = -2f;

		velocityY += gravity * gravityScale * Time.deltaTime;
		velocityY = Mathf.Max(velocityY, terminalVelocity);

		return new Vector3(0f, velocityY * Time.deltaTime, 0f);
	}

	public bool Jump()
	{
		if (!isGrounded) return false;
		velocityY = Mathf.Sqrt(jumpHeight * -2f * gravity * gravityScale);
		return true;
	}



}
