using Unity.VisualScripting;
using UnityEngine;

public class AnimatorComponent : MonoBehaviour
{
	[SerializeField]
	private Animator animator;
	private void Awake()
	{
		if (animator == null)
			animator = GetComponent<Animator>();
	}


	public void SetFloat(string key, float value)
	{
		animator.SetFloat(key, value);
	}

	public void SetInteger(string key, int value)
	{
		animator.SetInteger(key, value);
	}

	public void SetBool(string key, bool value)
	{
		animator.SetBool(key, value);
	}
	public void SetTrigger(string key)
	{
		animator.SetTrigger(key);
	}
}
