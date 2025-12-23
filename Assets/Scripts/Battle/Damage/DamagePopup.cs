using TMPro;
using UnityEngine;


/// <summary>
/// 월드 좌표에 표시되는 데미지 팝업
/// 
/// 역할:
/// - 텍스트 표시(일반/크리티컬/플레이어 피격/커스텀 텍스트)
/// - 일정 시간 동안 위로 이동
/// - 후반부 FadeOut
/// - 시간이 끝나면 비활성화(풀로 반환)
/// </summary>

public class DamagePopup : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI text;
	// 팝업이 살아있는 시간
	[SerializeField] private float lifeTime = 0.7f;
	// 위로 떠오르는 속도
	[SerializeField] private float moveSpeed = 1.5f;
	// 페이드 속도(알파 감소 속도)
	[SerializeField] private float fadeSpeed = 3f;

	private float timer;
	private CanvasGroup canvasGroup;

	void Awake()
	{
		if (canvasGroup == null)
			canvasGroup = gameObject.AddComponent<CanvasGroup>();

		text = transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
	}

	void OnDisable()
	{
		if (BattleGSC.Instance.damagePopupManager != null)
			BattleGSC.Instance.damagePopupManager.Return_ToDamagePopUpPool(transform.gameObject);
	}

	/// <summary>
	/// 몬스터에게 준 일반 데미지 표시
	/// </summary>
	public void Init_Enemy(int damage)
	{
		text.text = damage.ToString();
		text.color = Color.white;
		text.fontSize = 36;
		timer = lifeTime;
		canvasGroup.alpha = 1f;
	}

	/// <summary>
	/// 몬스터에게 준 크리티컬 데미지 표시
	/// </summary>
	public void Init_CriticalEnemy(int damage)
	{
		text.text = damage.ToString();
		text.color = Color.yellow;
		text.fontSize = 45;
		timer = lifeTime;
		canvasGroup.alpha = 1f;
	}


	/// <summary>
	/// 플레이어가 받은 데미지 표시
	/// </summary>
	public void Init_Player(int damage)
	{
		text.text = $"-{damage.ToString()}";
		text.color = Color.red;
		text.fontSize = 50;
		timer = lifeTime;
		canvasGroup.alpha = 1f;
	}

	/// <summary>
	/// 커스텀 텍스트 표시 (예: 버프/디버프 메시지)
	/// </summary>
	public void Init_Text(string _text)
	{
		string input = _text.Replace("/n", "\n");
		text.text = input;
		text.color = Color.yellow;
		text.fontSize = 50;
		timer = lifeTime;
		canvasGroup.alpha = 1f;
	}


	void Update()
	{
		// 카메라를 바라보도록 빌보드 처리
		transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
				 Camera.main.transform.rotation * Vector3.up);

		// 위로 떠오르는 이동
		transform.position += Vector3.up * moveSpeed * Time.deltaTime;

		// 시간 카운트다운
		timer -= Time.deltaTime;

		// 끝부분에서 서서히 사라지기
		// 수명 후반(절반 이후)부터 페이드 아웃
		if (timer < lifeTime * 0.5f)
		{
			canvasGroup.alpha -= fadeSpeed * Time.deltaTime;
		}

		// 수명 종료 → 비활성화(풀 반환 트리거)
		if (timer <= 0f)
		{
			gameObject.SetActive(false);
		}
	}
}
