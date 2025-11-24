using System;
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

	public event Action<int, Vector3, Type> OnDamaged;

	Dictionary<string, AsyncOperationHandle<GameObject>> attackObjectPrefabHandles = new Dictionary<string, AsyncOperationHandle<GameObject>>();

	public float CurrentHPHealth => playerStruct.currentHP;

	public float MaxHPHealth => playerStruct.maxHP;

	public float CurrentHPDamege => playerStruct.currentHP;
	public float MaxHPDamege => playerStruct.maxHP;


	protected virtual void Start()
	{
		if (GSC.Instance != null && GSC.Instance.statManager != null)
		{
			playerStruct = GSC.Instance.statManager.Get_PlayerData(PlayerKey);
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

	protected void InvokeHealthChanged()
	{
		OnHealthChanged?.Invoke(playerStruct.currentHP, playerStruct.maxHP);
	}

	protected void InvokeDamaged(int damage, Vector3 hitPos, Type _type)
	{
		OnDamaged?.Invoke(damage, hitPos, _type);
	}

	private void Handle_AttackStatsChanged()
	{
		playerStruct = GSC.Instance.statManager.Get_PlayerData(PlayerKey);
	}

	public virtual void Take_Damage(int damageInfo)
	{
	}

	public void Add_AttackObject(string _key)
	{
		Debug.Log(_key);
		AsyncOperationHandle<GameObject> handle =
			Addressables.InstantiateAsync(_key, transform.position, transform.rotation, transform);

		handle.WaitForCompletion();
		attackObjectPrefabHandles[_key] = handle;
		GameObject obj = handle.Result;

		GSC.Instance.skillManager.Add_CurrentAttacks(_key, obj.GetComponent<AttackRoot>());
	}
}
