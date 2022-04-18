using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMove : MonoBehaviour
{
	private Soldier _attackOwner;
	private Vector3 _direction;
	private float _aliveTimer, _speed;
	private bool _isAlive = false;
	private Vector3 _prevPosition;
	private Ray _ray;
	private RaycastHit _hit;
	private LayerMask _layer;
	[SerializeField] private float _aliveTime = 1f;
	private System.Action<ProjectileMove> PushHandler;
	private GenericPool<ImpactControl> _impacts;
	private void Start()
	{
		_prevPosition = transform.position;
		_ray = new Ray(_prevPosition, _direction);

	}
	private void OnEnable()
	{
		_prevPosition = transform.position;
		_aliveTimer = Time.time;
	}

	private void Update()
	{
		if ((Time.time - _aliveTimer) >= _aliveTime)
		{
			_isAlive = false;
			PushHandler?.Invoke(this);
		}
		if (!_isAlive)
			return;
		CheckCollision(transform.position);
		transform.position += _direction * _speed * Time.deltaTime;
	}

	private void OnDisable()
	{
		_isAlive = false;
	}
	private void CheckCollision(Vector3 position)
	{
		_ray.direction = position - _prevPosition;
		_ray.origin = _prevPosition;
		_prevPosition = position;
		if (Physics.Raycast(_ray, out _hit, _ray.direction.magnitude, _layer))
		{
			var target = _hit.collider.GetComponent<ITarget>();
			if (target != null && _attackOwner != null)
			{
				if (target.Team != _attackOwner.Team)
				{
					if (_isAlive)
						_isAlive = false;
					PushHandler?.Invoke(this);

					var value = target.TakeDamage(_attackOwner.Damage);

					if (value && target.IsBase)
					{
						if (_impacts != null)
						{
							var impact = _impacts.Pop();
							impact.SetupImpact(_hit.point, _hit.normal, (i) => _impacts.Push(i));
							impact.DoImpact();
						}
					}
					if (!value)
					{
						if (!target.IsBase)
							_attackOwner.SelfDemolish();
					}
				}
			}
			//Debug.Log(_hit.collider.gameObject.name);
		}
	}

	public void SetupProjectile(Vector3 position, Vector3 direction, float speed, 
		Soldier soldier, System.Action<ProjectileMove> push, GenericPool<ImpactControl> impacts)
	{
		_attackOwner = soldier;
		_speed = speed;
		_direction = direction.normalized;
		transform.localPosition = position;
		transform.localRotation = Quaternion.LookRotation(_direction);
		_isAlive = true;
		_aliveTimer = Time.time;
		_prevPosition = transform.position;
		_layer = ~(int)System.Math.Log(soldier.SoldierInfo.TargetLayer, 2);
		//Debug.Log(_layer);
		PushHandler = null;
		PushHandler = push;
		_impacts = null;
		_impacts = impacts;
	}

}