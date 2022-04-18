using UnityEngine;

[CreateAssetMenu(fileName = "SoldierInfo", menuName = "Flamingo/SoldierInfo")]
public class SoldierInfo : ScriptableObject
{
	public Team Team;
	public LayerMask OwnLayer;
	public LayerMask TargetLayer;
	public Material Material;
}