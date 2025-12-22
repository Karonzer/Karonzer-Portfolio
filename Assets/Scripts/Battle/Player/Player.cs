using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary>
/// Player (추상 클래스)
/// 
/// 플레이어 캐릭터가 공통으로 가져야 하는 기능을 모아둔 베이스 클래스.
/// - 체력(HP) 관리
/// - 데미지 받기 / 사망 처리 이벤트
/// - 스탯 변경(버프 등) 반영
/// - 피격 시 번쩍이는 효과(히트 플래시)
/// - 공격 오브젝트(AttackRoot) 추가 생성 및 SkillManager 등록
/// </summary>
public abstract class Player : MonoBehaviour, IDamageable, IHealthChanged
{
	// 플레이어 기본 데이터(SO)
	[SerializeField] private PlayerDataSO playerDataSO;

	// 이 플레이어를 구분하는 key (StatManager에서 스탯 찾을 때 사용)
	public string PlayerKey => playerDataSO.playerStruct.key;

	// 시작 공격(기본 스킬) key
	public string StartAttackKey => playerDataSO.playerStruct.startAttackObj;
	// 시작 공격 오브젝트 key (동일 값)
	public string StartAttackObj => playerDataSO.playerStruct.startAttackObj;

	// 현재 적용된 플레이어 스탯(기본 + 버프 반영 결과)
	[SerializeField] protected PlayerStruct playerStruct;

	// 공격 주체 타입 (플레이어)
	[SerializeField] protected Type playerType = Type.Player;

	// 체력 변동 이벤트 (UI가 구독해서 HP바 갱신)
	public event Action<float, float> OnHealthChanged;
	// 데미지 받음 이벤트 (피격 이펙트/사운드 등)
	public event Action<DamageInfo> OnDamaged;
	// 사망 이벤트 (GameManager가 구독해서 GameOver 처리)
	public event Action<IDamageable> OnDead;

	// 생성한 공격 오브젝트(AttackRoot)의 Addressables 핸들 캐싱
	// 씬 종료 시 Release 하기 위함
	Dictionary<string, AsyncOperationHandle<GameObject>> attackObjectPrefabHandles = new Dictionary<string, AsyncOperationHandle<GameObject>>();

	// 플레이어 메쉬 렌더러(피격 시 머티리얼 emission을 번쩍이게 할 때 사용)
	[SerializeField] protected SkinnedMeshRenderer meshRenderer;
	// 피격 시 번쩍임에 사용할 머티리얼 인스턴스
	[SerializeField] protected Material hitMatInstance;
	// 피격 번쩍임 코루틴
	protected Coroutine hitFlashRoutine;

	public float CurrentHPHealth => playerStruct.currentHP;
	public float MaxHPHealth => playerStruct.maxHP;
	public GameObject CurrentObj => this.gameObject;
	public float CurrentHPDamege => playerStruct.currentHP;
	public float MaxHPDamege => playerStruct.maxHP;

	/// <summary>
	/// - StatManager에서 현재 플레이어 스탯을 가져와 초기 playerStruct를 만든다.
	/// </summary>
	protected virtual void Awake()
	{
		if (BattleGSC.Instance != null && BattleGSC.Instance.statManager != null)
		{
			playerStruct = BattleGSC.Instance.statManager.Get_PlayerData(PlayerKey);
		}
	}

	/// <summary>
	/// - 플레이어 스탯이 바뀌었을 때(버프/업그레이드 등) 자동 반영하기 위해
	///   StatManager 이벤트를 구독한다.
	/// </summary>
	protected virtual void Start()
	{
		if (BattleGSC.Instance != null && BattleGSC.Instance.statManager != null)
		{
			BattleGSC.Instance.statManager.onChangePlayerStruct += Handle_AttackStatsChanged;
		}
	}

	/// <summary>
	/// - 이벤트 구독 해제 (중복 호출/메모리 누수 방지)
	/// - Addressables로 생성한 공격 오브젝트 인스턴스를 Release
	/// </summary>
	protected virtual void OnDestroy()
	{
		// 씬 전환/오브젝트 삭제 시 이벤트 해제
		if (BattleGSC.Instance != null && BattleGSC.Instance.statManager != null)
			BattleGSC.Instance.statManager.onChangePlayerStruct -= Handle_AttackStatsChanged;

		if (attackObjectPrefabHandles != null)
		{
			foreach (var kvp in attackObjectPrefabHandles)
			{
				var handle = kvp.Value;
				if (handle.IsValid())
				{
					Addressables.Release(handle);
				}
			}

			attackObjectPrefabHandles.Clear();
		}
	}

	/// <summary>
	/// 회복 처리
	/// - 현재 체력이 최대 체력보다 낮을 때만 회복
	/// - 회복 후 UI 갱신 이벤트 호출
	/// </summary>
	public void Healing_CurrentHP(int _value)
	{
		if (playerStruct.currentHP < MaxHPHealth)
		{
			playerStruct.currentHP += _value;
			Invoke_HealthChanged();
		}
	}

	/// <summary>
	/// 체력 변경 이벤트 호출(HP UI 갱신)
	/// </summary>
	protected void Invoke_HealthChanged()
	{
		OnHealthChanged?.Invoke(playerStruct.currentHP, playerStruct.maxHP);
	}

	/// <summary>
	/// 피격 이벤트 호출(이펙트/사운드 등)
	/// </summary>
	protected void Invoke_Damaged(DamageInfo _damageInfo)
	{
		OnDamaged?.Invoke(_damageInfo);
	}

	/// <summary>
	/// 플레이어 스탯이 바뀌었을 때 호출되는 함수(버프/업그레이드 등)
	/// - 새 스탯을 다시 가져온다
	/// - HP가 갑자기 튀지 않도록 "현재 HP 비율을 유지하는 느낌"으로 보정한다
	/// </summary>
	private void Handle_AttackStatsChanged()
	{
		float _currentHP = playerStruct.currentHP;
		float _maxHP = playerStruct.maxHP;
		playerStruct = BattleGSC.Instance.statManager.Get_PlayerData(PlayerKey);
		playerStruct.currentHP = _currentHP + playerStruct.maxHP - _maxHP;
		Invoke_HealthChanged();
	}

	/// <summary>
	/// 데미지를 받는다.
	/// 흐름:
	/// 1) 피격 연출(번쩍임)
	/// 2) HP 감소
	/// 3) 피격/체력 이벤트 호출
	/// 4) HP가 0 이하이면 사망 이벤트 호출
	/// </summary>
	public virtual void Take_Damage(DamageInfo _damageInfo)
	{
		if (playerStruct.currentHP > 0)
			HitFlash();

		playerStruct.currentHP -= _damageInfo.damage;
		Invoke_Damaged(_damageInfo);
		Invoke_HealthChanged();
		if (playerStruct.currentHP <= 0)
		{
			OnDead?.Invoke(this);
		}
	}

	/// <summary>
	/// 피격 시 번쩍임 시작
	/// - 기존 코루틴이 있으면 중단하고 새로 시작(중복 번쩍임 방지)
	/// </summary>
	private void HitFlash()
	{
		if (hitFlashRoutine != null)
		{
			StopCoroutine(hitFlashRoutine);
			hitFlashRoutine = null;
		}

		hitFlashRoutine = StartCoroutine(Co_HitFlashEmission());
	}

	/// <summary>
	/// 피격 번쩍임 코루틴
	/// - emission 컬러를 잠깐 올렸다가 다시 내린다
	/// </summary>
	private IEnumerator Co_HitFlashEmission()
	{
		if (hitMatInstance == null)
			yield break;

		Function_HitFlashEmission();
		hitMatInstance.EnableKeyword("_EMISSION");
		hitMatInstance.SetColor("_EmissionColor", Color.red * 2f); // 번쩍

		yield return new WaitForSeconds(0.1f);

		hitMatInstance.SetColor("_EmissionColor", Color.black); // 원래대로
	}

	/// <summary>
	/// 히트 플래시 시점에 자식 클래스가 추가 행동을 할 수 있도록 분리한 함수
	/// </summary>
	public void Function_HitFlashEmission()
	{
		Event_ChildEvnet();
	}

	/// <summary>
	/// 자식 클래스에서 오버라이드해서 추가 피격 연출/사운드 등을 구현할 수 있음
	/// </summary>
	protected virtual void Event_ChildEvnet()
	{

	}

	/// <summary>
	/// 공격 오브젝트(AttackRoot)를 추가한다.
	/// - Addressables로 공격 프리팹을 Instantiate
	/// - 생성된 AttackRoot를 SkillManager에 등록(현재 보유 공격 목록에 추가)
	/// </summary>
	public void Add_AttackObject(string _key)
	{
		AsyncOperationHandle<GameObject> handle =
			Addressables.InstantiateAsync(_key, transform.position, transform.rotation, transform);

		handle.WaitForCompletion();
		attackObjectPrefabHandles[_key] = handle;
		GameObject obj = handle.Result;

		BattleGSC.Instance.skillManager.Add_CurrentAttacks(_key, obj.GetComponent<AttackRoot>());
	}


}
