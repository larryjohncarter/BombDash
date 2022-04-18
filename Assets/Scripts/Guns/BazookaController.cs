using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BazookaController : MonoBehaviour, IGunController
{
	private float _rocketShowTime;
	private float _attackTimeCounter = 0.01f;
	private float _attackTimeSc = 5f;
	private float _speed = 30f;
	[SerializeField] private Transform _projectilePosition;
	[SerializeField] private ParticleSystem _muzzleParticule;
	[SerializeField] private GameObject _rocket;
	
	[SerializeField] private GunType _gunType = GunType.Minigun;
	public GunType GunType => _gunType;
	public float AttackTimeSc => _attackTimeSc;
	public float Speed => _speed;
	public Soldier AttackOwner { get; set; }


	private void OnEnable()
	{
		_muzzleParticule.gameObject.SetActive(true);
		_rocketShowTime = _attackTimeSc * .5f;
	}
	private void OnDisable()
	{
		_muzzleParticule.gameObject.SetActive(false);
	}
	public void Setup(SoldierTypeConfig config)
	{
		_speed = config.MissileSpeed;
		_attackTimeSc = config.AttackTimeSc;
		_attackTimeCounter = AttackTimeSc;
	}
	public void PerformAttack()
	{
		if (_attackTimeCounter <= 0)
		{
			Attack();
			_rocket.SetActive(false);
			_rocketShowTime = _attackTimeSc * .5f;
			_attackTimeCounter = AttackTimeSc;
		}
		if (_rocketShowTime <= 0)
		{
			if (!_rocket.activeSelf)
				_rocket.SetActive(true);
			_rocketShowTime = 0;
		}
		_attackTimeCounter -= Time.deltaTime;
		_rocketShowTime -= Time.deltaTime;
	}
	public void SetActive(bool value)
	{
		gameObject.SetActive(value);
	}

	private void Attack()
	{
		var projectile = PoolManager.Instance.Rockets.Pop();
		projectile.SetupProjectile(_projectilePosition.position, 
			_projectilePosition.forward, Speed
			, AttackOwner, (v) => PoolManager.Instance.Rockets.Push(v),
			PoolManager.Instance.RokectImpacts);
		_muzzleParticule.time = 0;
		_muzzleParticule.Play();
	}
}
