using System.Collections.Generic;
using UnityEngine;

public enum Team
{
	Alliance,
	Enemy
}

public interface ITarget
{
	GameObject MainObject{get;}
	bool IsBase {get;}
	Team Team { get; }
	bool CanTargeted { get; set; }
	float Health { get; }
	bool TakeDamage(float value);
	System.Action<ITarget> UnBindHandler { get; set; }
	Vector3 GetClosestPosition(Vector3 position);
}