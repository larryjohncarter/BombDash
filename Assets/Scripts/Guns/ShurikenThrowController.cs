﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShurikenThrowController : MonoBehaviour, IGunController
{
	private float _attackTimeCounter = 0;
	private float _attackTimeSc = 0.1f;
	private float _speed = 30f;
	[SerializeField] private Transform _projectilePosition;
	//[SerializeField] private ParticleSystem _muzzleParticule;
	[SerializeField] private GunType _gunType = GunType.Shuriken;
	public GunType GunType => _gunType;
	public float AttackTimeSc => _attackTimeSc;
	public float Speed => _speed;
	public Soldier AttackOwner { get; set; }

	private void OnEnable()
	{
		//_muzzleParticule.gameObject.SetActive(true);
	}
	private void OnDisable()
	{
		//_muzzleParticule.gameObject.SetActive(false);
	}
	public void Setup(SoldierTypeConfig config)
	{
		_speed = config.MissileSpeed;
		_attackTimeSc = config.AttackTimeSc;
		_attackTimeCounter = AttackTimeSc;
	}
	public void PerformAttack()
	{
		_attackTimeCounter -= Time.deltaTime;
		if (_attackTimeCounter <= 0)
		{
			Attack();
			_attackTimeCounter = AttackTimeSc;
		}
	}
	public void SetActive(bool value)
	{
		gameObject.SetActive(value);
	}

	private void Attack()
	{
		var projectile = PoolManager.Instance.Shurinkens.Pop();
		projectile.SetupProjectile(_projectilePosition.position,
			_projectilePosition.forward, Speed,
			AttackOwner, (v) => PoolManager.Instance.Shurinkens.Push(v),
			PoolManager.Instance.BulletImpacts);
		// _muzzleParticule.time = 0;
		// _muzzleParticule.Play();
	}
}
