
public enum GunType
{
	Minigun,
	RocketLauncher,
	FlameThrower,
	Shuriken,
	Pistol,
	Spear
}


public interface IGunController
{
	float AttackTimeSc { get;}
	float Speed { get;}
	Soldier AttackOwner { get; set; }
	GunType GunType { get; }
	void PerformAttack();
	void SetActive(bool value);
	void Setup(SoldierTypeConfig config);
}