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

		descText  = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
	}

	public void Setup(UpgradeOptionSO _option)
	{
		this.option = _option;
		descText.text = _option.description;
		//titleText.text = _option.title;
		//descText.text = _option.description;
		//if (iconImage != null && _option.icon != null)
		//	iconImage.sprite = _option.icon;
	}

	public void OnClick_Select()
	{
		Debug.Log(option.description);
		BattleGSC.Instance.upgradeManager.Select_Upgrade(option);
		BattleGSC.Instance.uIManger.Hide(UIType.UpgradePopUp);
		BattleGSC.Instance.gameManager.ResumeGame();
		BattleGSC.Instance.gameManager.Set_ShowAndHideCursor(false);
		BattleGSC.Instance.gameManager.Check_PendingLevelUp();
		GlobalGSC.Instance.audioManager.Play_Click();
	}
}
