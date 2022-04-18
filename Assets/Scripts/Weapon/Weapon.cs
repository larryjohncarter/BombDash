using UnityEngine;
using DG.Tweening;

public class Weapon : MonoBehaviour
{
    public enum WeaponType
    {
        Gun,
        RocketLauncher,
        Spear,
        FlameThrower,
        NinjaStar,
    }
    [SerializeField] private WeaponType type;
    public WeaponType Type { get { return type; } }
    public float timeToDestroyWeapon;
    public float weaponTimer;
}
