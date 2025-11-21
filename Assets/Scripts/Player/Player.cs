using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public abstract class Player : MonoBehaviour, IDamageable
{
	[SerializeField] protected string playerName;
	[SerializeField] protected PlayerStruct playerStruct;
	[SerializeField] protected Type playerType = Type.Player;
	public float CurrentHP => playerStruct.currentHP;
	public float MaxHP => playerStruct.maxHP;

	public event Action<int, Vector3, Type> OnDamaged;

	Dictionary<string, AsyncOperationHandle<GameObject>> attackObjectPrefabHandles = new Dictionary<string, AsyncOperationHandle<GameObject>>();

	protected virtual void Start()
	{
		if (GSC.Instance != null && GSC.Instance.statManager != null)
		{
			playerStruct = GSC.Instance.statManager.Get_PlayerData(playerName);
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

	private void Handle_AttackStatsChanged()
	{
		playerStruct = GSC.Instance.statManager.Get_PlayerData(playerName);
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
