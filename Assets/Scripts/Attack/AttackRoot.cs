using UnityEngine;

public abstract class AttackRoot : MonoBehaviour
{
	protected string attackName;
	protected int attackDamage;
	protected float attackRange;
	protected float attackTime;
	protected abstract void Attack();
}
