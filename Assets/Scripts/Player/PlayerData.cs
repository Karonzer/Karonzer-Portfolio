using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Game/PlayerData")]
public class PlayerData : ScriptableObject
{
	public float moveSpeed = 3.5f;
	public float currentHP = 100;
	public float jump = 1f;
}