using UnityEngine;

public class AttackShardSystemManager : MonoBehaviour
{
	private void Awake()
	{
		GSC.Instance.RegisterAttackShardSystemManager(this);
	}
}
