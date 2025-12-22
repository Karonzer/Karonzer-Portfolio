using UnityEngine;
/// <summary>
/// GravityController
/// 
/// CharacterController 기반 캐릭터의
/// - 중력 적용
/// - 바닥 판정
/// - 점프 계산
/// 을 전담하는 컴포넌트.
///
/// PlayerMoveController에서
///  - GetGravityDelta() : 매 프레임 중력 이동량 요청
///  - Jump() : 점프 시도
/// 형태로 사용된다.
/// </summary>
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

	// 바닥 체크용 설정
	[SerializeField] private float groundCheckDistance;
	[SerializeField] private LayerMask groundMask;

	private void OnEnable()
	{
		// Ground 레이어만 바닥으로 인식
		groundMask = LayerMask.GetMask("Ground");
		groundCheckDistance = 1.1f;
	}

	/// <summary>
	/// Raycast를 사용한 바닥 판정
	/// - CharacterController.isGrounded가 불안정할 때 보조용
	/// </summary>
	private bool CheckGrounded(CharacterController controller)
	{
		Vector3 origin = controller.transform.position;
		bool hit = Physics.Raycast(origin,Vector3.down, groundCheckDistance, groundMask);
		return hit;
	}


	/// <summary>
	/// 중력에 의해 이동해야 할 Y축 이동량을 계산해서 반환
	/// PlayerMoveController.Update()에서 매 프레임 호출된다.
	/// </summary>
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

	/// <summary>
	/// 점프 시도
	/// - 바닥에 있을 때만 점프 가능
	/// - 점프 높이를 기준으로 초기 Y 속도 계산
	/// </summary>
	public bool Jump()
	{
		if (!isGrounded) return false;
		velocityY = Mathf.Sqrt(jumpHeight * -2f * gravity * gravityScale);
		return true;
	}



}
