using UnityEngine;
using UnityEngine.UI;
public class SettingMenulUI : MonoBehaviour
{
	[SerializeField] private GameObject popUp;
	public bool IsOpen => popUp.gameObject.activeSelf;

	[SerializeField] Slider masterSlider;
	[SerializeField] Slider bgmSlider;
	[SerializeField] Slider sfxSlider;

	private void Awake()
	{
		GlobalGSC.Instance.RegisterSettingMenulUI(this);
		popUp = transform.GetChild(0).gameObject;
	}
	private void OnEnable()
	{
		popUp.gameObject.SetActive(false);
	}

	private void Start()
	{
		Initialize_Slider();
		if (GlobalGSC.Instance.audioManager != null)
		{
			Setting_Slider();
		}
	}

	private void Initialize_Slider()
	{
		masterSlider = popUp.transform.GetChild(0).GetChild(1).GetComponent<Slider>();
		bgmSlider = popUp.transform.GetChild(0).GetChild(2).GetComponent<Slider>();
		sfxSlider = popUp.transform.GetChild(0).GetChild(3).GetComponent<Slider>();

		masterSlider.onValueChanged.AddListener(GlobalGSC.Instance.audioManager.Set_MasterVolume);
		bgmSlider.onValueChanged.AddListener(GlobalGSC.Instance.audioManager.Set_BGMVolume);
		sfxSlider.onValueChanged.AddListener(GlobalGSC.Instance.audioManager.Set_SFXVolume);
	}

	private void Setting_Slider()
	{
		masterSlider.value = GlobalGSC.Instance.audioManager.masterVolume;
		bgmSlider.value = GlobalGSC.Instance.audioManager.bgmVolume;
		sfxSlider.value = GlobalGSC.Instance.audioManager.sfxVolume;
	}

	public void Show_PopUp()
	{
		popUp.gameObject.SetActive(true);
	}
	public void Hide_PopUp()
	{
		popUp.gameObject.SetActive(false);
	}
}
