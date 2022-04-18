using UnityEngine;

public class FlameThrowController : MonoBehaviour, IGunController
{
	private float _attackTimeCounter = 0;
	private float _attackTimeSc = 0.1f;
	private float _speed = 15f;
	[SerializeField] private Transform _projectilePosition;
	[SerializeField] private ParticleSystem _muzzleParticule;
	[SerializeField] private GunType _gunType = GunType.FlameThrower;
	public GunType GunType => _gunType;
	public float AttackTimeSc => _attackTimeSc;
	public float Speed => _speed;
	public Soldier AttackOwner { get; set; }

	private void OnEnable()
	{
		_muzzleParticule.gameObject.SetActive(true);
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
		var projectile = PoolManager.Instance.Flames.Pop();
		projectile.SetupProjectile(_projectilePosition.position,
			_projectilePosition.forward, Speed,
			AttackOwner, (v) => PoolManager.Instance.Flames.Push(v),
			PoolManager.Instance.BulletImpacts);
		_muzzleParticule.time = 0;
		_muzzleParticule.Play();
	}
}