using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="SoldierTypeConfig", menuName="Flamingo/SoldierTypeConfig")]
public class SoldierTypeConfig : ScriptableObject
{
	[Header("Soldier Settings")]
	public float Speed = 5;
	public float Health = 10f;
	[Header("Weapon Settings")]
	public float MissileSpeed = 5;
	public float AttackRange = 2f;
	public float Damage = 1f;
	public float AttackTimeSc = 1f;
	public GunType GunType;
}
