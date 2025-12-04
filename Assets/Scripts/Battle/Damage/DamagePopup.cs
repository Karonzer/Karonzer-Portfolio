using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI text;
	[SerializeField] private float lifeTime = 0.7f;
	[SerializeField] private float moveSpeed = 1.5f;
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

	public void Init_Enemy(int damage)
	{
		text.text = damage.ToString();
		text.color = Color.white;
		text.fontSize = 36;
		timer = lifeTime;
		canvasGroup.alpha = 1f;
	}

	public void Init_CriticalEnemy(int damage)
	{
		text.text = damage.ToString();
		text.color = Color.yellow;
		text.fontSize = 45;
		timer = lifeTime;
		canvasGroup.alpha = 1f;
	}

	public void Init_Player(int damage)
	{
		text.text = $"-{damage.ToString()}";
		text.color = Color.red;
		text.fontSize = 50;
		timer = lifeTime;
		canvasGroup.alpha = 1f;
	}


	void Update()
	{
		transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
				 Camera.main.transform.rotation * Vector3.up);
		transform.position += Vector3.up * moveSpeed * Time.deltaTime;

		timer -= Time.deltaTime;

		// 끝부분에서 서서히 사라지기
		if (timer < lifeTime * 0.5f)
		{
			canvasGroup.alpha -= fadeSpeed * Time.deltaTime;
		}

		if (timer <= 0f)
		{
			gameObject.SetActive(false);
		}
	}
}
