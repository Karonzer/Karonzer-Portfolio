using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public abstract class Player : MonoBehaviour, IDamageable, IHealthChanged
{
	[SerializeField] private PlayerDataSO playerDataSO;
	public string PlayerKey => playerDataSO.playerStruct.key;
	public string StartAttackKey => playerDataSO.playerStruct.startAttackObj;
	public string StartAttackObj => playerDataSO.playerStruct.startAttackObj;

	[SerializeField] protected PlayerStruct playerStruct;
	[SerializeField] protected Type playerType = Type.Player;

	public event Action<float, float> OnHealthChanged;
	public event Action<DamageInfo> OnDamaged;
	public event Action<IDamageable> OnDead;

	Dictionary<string, AsyncOperationHandle<GameObject>> attackObjectPrefabHandles = new Dictionary<string, AsyncOperationHandle<GameObject>>();

	[SerializeField] protected SkinnedMeshRenderer meshRenderer;
	protected Material hitMatInstance;
	protected Coroutine hitFlashRoutine;

	public float CurrentHPHealth => playerStruct.currentHP;
	public float MaxHPHealth => playerStruct.maxHP;
	public GameObject CurrentObj => this.gameObject;
	public float CurrentHPDamege => playerStruct.currentHP;
	public float MaxHPDamege => playerStruct.maxHP;

	protected virtual void Awake()
	{
		if (GSC.Instance != null && GSC.Instance.statManager != null)
		{
			playerStruct = GSC.Instance.statManager.Get_PlayerData(PlayerKey);
		}
	}

	protected virtual void Start()
	{
		if (GSC.Instance != null && GSC.Instance.statManager != null)
		{
			GSC.Instance.statManager.onChangePlayerStruct += Handle_AttackStatsChanged;
		}
	}

	protected virtual void OnDestroy()
	{
		// 씬 전환/오브젝트 삭제 시 이벤트 해제
		if (GSC.Instance != null && GSC.Instance.statManager != null)
			GSC.Instance.statManager.onChangePlayerStruct -= Handle_AttackStatsChanged;

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

	protected void Invoke_HealthChanged()
	{
		OnHealthChanged?.Invoke(playerStruct.currentHP, playerStruct.maxHP);
	}

	protected void Invoke_Damaged(DamageInfo _damageInfo)
	{
		OnDamaged?.Invoke(_damageInfo);
	}

	private void Handle_AttackStatsChanged()
	{
		float _currentHP = playerStruct.currentHP;
		float _maxHP = playerStruct.maxHP;
		playerStruct = GSC.Instance.statManager.Get_PlayerData(PlayerKey);
		playerStruct.currentHP = _currentHP + playerStruct.maxHP - _maxHP;
		Invoke_HealthChanged();
	}

	public virtual void Take_Damage(DamageInfo _damageInfo)
	{
		if (playerStruct.currentHP > 0)
			HitFlash();

		playerStruct.currentHP -= _damageInfo.damage;
		Invoke_Damaged(_damageInfo);
		if (playerStruct.currentHP <= 0)
		{

		}
		Invoke_HealthChanged();
	}

	private void HitFlash()
	{
		if (hitFlashRoutine != null)
		{
			StopCoroutine(hitFlashRoutine);
			hitFlashRoutine = null;
		}

		hitFlashRoutine = StartCoroutine(Co_HitFlashEmission());
	}

	private IEnumerator Co_HitFlashEmission()
	{
		if (hitMatInstance == null)
			yield break;

		hitMatInstance.EnableKeyword("_EMISSION");
		hitMatInstance.SetColor("_EmissionColor", Color.red * 2f); // 번쩍

		yield return new WaitForSeconds(0.1f);

		hitMatInstance.SetColor("_EmissionColor", Color.black); // 원래대로
	}

	public void Add_AttackObject(string _key)
	{
		AsyncOperationHandle<GameObject> handle =
			Addressables.InstantiateAsync(_key, transform.position, transform.rotation, transform);

		handle.WaitForCompletion();
		attackObjectPrefabHandles[_key] = handle;
		GameObject obj = handle.Result;

		GSC.Instance.skillManager.Add_CurrentAttacks(_key, obj.GetComponent<AttackRoot>());
	}


}
