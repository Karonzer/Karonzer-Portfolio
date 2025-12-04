using UnityEngine;
[CreateAssetMenu(fileName = "UpgradeOption", menuName = "Skill/Upgrade Option")]
public class UpgradeOptionSO : ScriptableObject
{


	[Header("UI 표시용")]
	public string title;
	[TextArea] public string description;
	public Sprite icon;

	[Header("대상 스킬 정보")]
	public UpgradeOptionType optionType;
	public string numKey;
	public string targetKey;        

	[Header("효과")]
	public UpgradeEffectType effectType;
	public float value;              
}