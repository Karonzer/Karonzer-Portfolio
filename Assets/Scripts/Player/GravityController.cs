using UnityEngine;

public class GravityController : MonoBehaviour
{
	[Header("중력 설정 (gravity는 음수)")]
	[SerializeField] private float gravity = -9.81f;
	[SerializeField] private float gravityScale = 1f;
	[SerializeField] private float terminalVelocity = -50f;

	[Header("점프 설정")]
	[SerializeField] private float jumpHeight = 1.5f;

	[Header("바닥 체크 (optional)")]
	[SerializeField] private Transform groundCheck;
	[SerializeField] private float groundDistance = 0.2f;
	[SerializeField] private LayerMask groundMask = 0;

	// 내부 상태
	[SerializeField] private float velocityY = 0f;
	[SerializeField] private bool isGrounded = false;

	// 외부에서 읽거나 디버깅용
	public float VelocityY => velocityY;
	public bool IsGrounded => isGrounded;

	void Reset()
	{
		groundDistance = 0.2f;
	}
	public Vector3 GetGravityDelta(CharacterController controller)
	{
		if (groundCheck != null)
		{
			isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
		}
		else
		{
			isGrounded = controller.isGrounded;
		}

		if (isGrounded && velocityY < 0f)
		{
			velocityY = -2f;
		}

		velocityY += gravity * gravityScale * Time.deltaTime;
		if (velocityY < terminalVelocity) velocityY = terminalVelocity;

		return new Vector3(0f, velocityY * Time.deltaTime, 0f);
	}

	public void Jump()
	{
		if (!isGrounded) return;
		velocityY = Mathf.Sqrt(jumpHeight * -2f * gravity * gravityScale);
	}

	void OnDrawGizmosSelected()
	{
		if (groundCheck != null)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
		}
	}
}
