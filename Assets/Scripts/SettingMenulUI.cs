using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 전역 설정 메뉴 UI (사운드 볼륨 슬라이더).
/// 
/// 역할:
/// - 팝업 열기/닫기
/// - Master/BGM/SFX 슬라이더 값을 GlobalAudioManager에 연결
/// - 닫힐 때 OnClose 이벤트로 외부(Pause 등)에 알림 가능
/// </summary>
public class SettingMenulUI : MonoBehaviour
{
	[SerializeField] private GameObject popUp;
	public bool IsOpen => popUp.gameObject.activeSelf;

	[SerializeField] Slider masterSlider;
	[SerializeField] Slider bgmSlider;
	[SerializeField] Slider sfxSlider;

	public event Action OnClose;

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

	/// <summary>
	/// 슬라이더 참조를 프리팹 구조에서 찾아오고,
	/// 값 변경 이벤트를 GlobalAudioManager의 Set_Volume 함수에 연결.
	/// </summary>
	private void Initialize_Slider()
	{
		masterSlider = popUp.transform.GetChild(0).GetChild(1).GetComponent<Slider>();
		bgmSlider = popUp.transform.GetChild(0).GetChild(2).GetComponent<Slider>();
		sfxSlider = popUp.transform.GetChild(0).GetChild(3).GetComponent<Slider>();

		masterSlider.onValueChanged.AddListener(GlobalGSC.Instance.audioManager.Set_MasterVolume);
		bgmSlider.onValueChanged.AddListener(GlobalGSC.Instance.audioManager.Set_BGMVolume);
		sfxSlider.onValueChanged.AddListener(GlobalGSC.Instance.audioManager.Set_SFXVolume);
	}

	/// <summary>
	/// 오디오 매니저가 가진 현재 볼륨 값을 슬라이더에 반영
	/// </summary>
	private void Setting_Slider()
	{
		masterSlider.value = GlobalGSC.Instance.audioManager.masterVolume;
		bgmSlider.value = GlobalGSC.Instance.audioManager.bgmVolume;
		sfxSlider.value = GlobalGSC.Instance.audioManager.sfxVolume;
	}

	/// <summary>
	/// 설정 팝업 열기
	/// </summary>
	public void Show_PopUp()
	{
		popUp.gameObject.SetActive(true);
	}


	/// <summary>
	/// 설정 팝업 닫기
	/// - 클릭 사운드 재생
	/// - OnClose 이벤트 발행(외부 UI가 다시 열리거나 포커스 복귀 등에 사용 가능)
	/// </summary>
	public void Hide_PopUp()
	{
		GlobalGSC.Instance.audioManager.Play_Click();
		popUp.gameObject.SetActive(false);
		OnClose?.Invoke();
	}
}
