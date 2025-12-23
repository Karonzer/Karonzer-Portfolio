using System.Collections;
using UnityEngine;

/// <summary>
/// 플레이어가 획득하는 경험치 아이템.
/// 
/// 특징:
/// - 일정 거리 이내 접근 시 자동 추적
/// - MagnetItem에 의해 강제 추적 가능
/// </summary>
public class XPitem : Item
{
	// 추적 대상 (보통 플레이어)
	private GameObject target;
	// 추적 코루틴
	private Coroutine followRoutine;
	// 획득 시 지급될 경험치
	private int xpAmount;

	// 현재 추적 중인지 여부
	private bool isFollowToPlayer;


	protected override void Start()
	{
		base.Start();
	}

	private void OnEnable()
	{
		// 풀 재사용 대비 상태 초기화
		isFollowToPlayer = false;
		target = null;
		if (followRoutine != null)
		{
			StopCoroutine(followRoutine);
			followRoutine = null;
		}
	}

	/// <summary>
	/// 경험치 값 설정
	/// </summary>
	public void SetXP(int amount)
	{
		xpAmount = amount;
	}

	/// <summary>
	/// 플레이어를 자동 추적 시작
	/// </summary>
	public void Start_FollowToPlayer()
	{

		if (followRoutine == null)
		{
			isFollowToPlayer = true;
			target = BattleGSC.Instance.gameManager.Get_PlayerObject();
			followRoutine = StartCoroutine(Follow_ToPlayer(target));
		}
	}


	/// <summary>
	/// 특정 오브젝트를 추적 시작
	/// (ItemFollowToPlayer에서 호출)
	/// </summary>
	public void Start_FollowToPlayer(GameObject _obj)
	{

		if (followRoutine == null)
		{
			isFollowToPlayer = true;
			target = _obj;
			followRoutine = StartCoroutine(Follow_ToPlayer(target));
		}
	}

	/// <summary>
	/// 플레이어와 충돌 시 경험치 지급
	/// </summary>
	protected override void Fuction_Event(GameObject _obj)
	{
		if (_obj.TryGetComponent<PlayerLevel>(out var levelSystem))
		{
			levelSystem.AddXP(xpAmount);
		}
	}


	/// <summary>
	/// 플레이어 방향으로 이동하는 추적 루프
	/// </summary>
	private IEnumerator Follow_ToPlayer(GameObject _obj)
	{
		while (true)
		{
			Vector3 projectileDir = _obj.Get_TargetDir(transform);
			transform.Translate(projectileDir * 5 * Time.deltaTime);
			yield return null;
		}
	}

	public bool Get_isFollowToPlayer()
	{
		return isFollowToPlayer;
	}
}
