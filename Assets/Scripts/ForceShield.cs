using System;
using UnityEngine;

public class ForceShield : MonoBehaviour,ITarget
{
	private bool _isDestoried = false;
	private Bounds _bounds;
	[SerializeField] private bool _isBase = true;
	[SerializeField] private Team _team;
	[SerializeField] private float _yOffset = 2f;
	[SerializeField] private float _health = 10f;
	public GameObject MainObject => gameObject;
	public bool IsBase => _isBase;
	public Team Team { get => _team; set => _team = value; }
	public float Health => _health;
	public bool CanTargeted { get; set; } = true;
	public Vector3 Position => transform.position;
	public Action<ITarget> UnBindHandler { get; set; }

	private void Awake()
	{
		_bounds = transform.GetComponent<MeshCollider>().bounds;
	}
	public bool TakeDamage(float value)
	{
		_health -= value;
		if (_health <= 0)
		{
			CanTargeted = false;
			if(!_isDestoried)
			{
				UnBindHandler?.Invoke(this);
				_isDestoried = true;
				Debug.Log("Losed Team : " + Team);
			}
			return false;
		}
		return true;
	}

	public Vector3 GetClosestPosition(Vector3 position)
	{
		var pos = Position;
		pos.y = position.y = _yOffset;
		var ray = new Ray(position, pos - position);
		if (_bounds.IntersectRay(ray, out float distance))
		{
			var p = ray.GetPoint(distance);
			return p;
		}
		return position;
	}
}
