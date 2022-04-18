using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : SingletonBehaviour<PoolManager>
{
	[SerializeField] private Soldier _soldierPrefabs;
	private GenericPool<Soldier> _soldiers;
	public GenericPool<Soldier> Soldiers => _soldiers;

	[SerializeField] private ProjectileMove _bulletPrefabs;
	private GenericPool<ProjectileMove> _bullets;
	public GenericPool<ProjectileMove> Bullets => _bullets;

	[SerializeField] private ProjectileMove _rocketPrefabs;
	private GenericPool<ProjectileMove> _rockets;
	public GenericPool<ProjectileMove> Rockets => _rockets;

	[SerializeField] private ProjectileMove _shurinkenPrefab;
	private GenericPool<ProjectileMove> _shurinkens;
	public GenericPool<ProjectileMove> Shurinkens => _shurinkens;

	[SerializeField] private ProjectileMove _flamePrefab;
	private GenericPool<ProjectileMove> _flames;
	public GenericPool<ProjectileMove> Flames => _flames;

	[SerializeField] private ImpactControl _bulletImpact;
	private GenericPool<ImpactControl> _bulletImpacts;
	public GenericPool<ImpactControl> BulletImpacts => _bulletImpacts;

	[SerializeField] private ImpactControl _rocketImpact;
	private GenericPool<ImpactControl> _rocketImpacts;
	public GenericPool<ImpactControl> RokectImpacts => _rocketImpacts;

	[SerializeField] private ImpactControl _deathParticulePref;
	private GenericPool<ImpactControl> _deadParticules;
	public GenericPool<ImpactControl> DeadParticules => _deadParticules;
	private void Start()
	{
		_soldiers = new GenericPool<Soldier>(1, () => 
		{ 
			var a = Instantiate(_soldierPrefabs, Vector3.up, Quaternion.identity); 
			a.gameObject.SetActive(false); 
			return a;
		},
			(v) => v.gameObject.SetActive(false)
		);

		_bullets = new GenericPool<ProjectileMove>(5, () => Instantiate(_bulletPrefabs),
			(v) => v.gameObject.SetActive(false),
			(v) => v.gameObject.SetActive(true)
		);

		_rockets = new GenericPool<ProjectileMove>(5, () => Instantiate(_rocketPrefabs),
			(v) => v.gameObject.SetActive(false),
			(v) => v.gameObject.SetActive(true)
		);

		_shurinkens = new GenericPool<ProjectileMove>(5, () => Instantiate(_shurinkenPrefab),
			(v) => v.gameObject.SetActive(false),
			(v) => v.gameObject.SetActive(true)
		);

		_flames = new GenericPool<ProjectileMove>(5, () => Instantiate(_flamePrefab),
			(v) => v.gameObject.SetActive(false),
			(v) => v.gameObject.SetActive(true)
		);

		_bulletImpacts = new GenericPool<ImpactControl>(5, () => Instantiate(_bulletImpact),
			(v) => v.gameObject.SetActive(false),
			(v) => v.gameObject.SetActive(true)
		);

		_rocketImpacts = new GenericPool<ImpactControl>(5, () => Instantiate(_rocketImpact),
			(v) => v.gameObject.SetActive(false),
			(v) => v.gameObject.SetActive(true)
		);

		_deadParticules = new GenericPool<ImpactControl>(5, () => Instantiate(_deathParticulePref),
			(v) => v.gameObject.SetActive(false),
			(v) => v.gameObject.SetActive(true)
		);

	}
}
