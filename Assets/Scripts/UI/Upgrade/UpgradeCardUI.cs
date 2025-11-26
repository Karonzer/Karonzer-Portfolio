using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UpgradeCardUI : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI titleText;
	[SerializeField] private TextMeshProUGUI descText;
	[SerializeField] private Image iconImage;

	[SerializeField] private UpgradeOptionSO option;
	private void Awake()
	{
		Button button = GetComponent<Button>();
		button.AddEvent(OnClick_Select);
	}

	public void Setup(UpgradeOptionSO _option)
	{
		this.option = _option;
		//titleText.text = _option.title;
		//descText.text = _option.description;
		//if (iconImage != null && _option.icon != null)
		//	iconImage.sprite = _option.icon;
	}

	public void OnClick_Select()
	{
		Debug.Log(option.description);
		GSC.Instance.upgradeManager.Select_Upgrade(option);
		GSC.Instance.uIManger.Hide_UI("UpgradePopUp");
		GSC.Instance.gameManager.ResumeGame();
		GSC.Instance.gameManager.Set_ShowAndHideCursor(false);
		// 여기서 패널 닫고 게임 재개하는 처리
	}
}
