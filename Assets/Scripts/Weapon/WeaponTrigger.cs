using UnityEngine;
using DG.Tweening;
public class WeaponTrigger : MonoBehaviour
{
    private Weapon parent;
    public bool hasBeenPickedUp;
    [SerializeField] private RocketLauncherDisabler rlDisabler;
    private Tweener _tween;
    public float rotationSpeed;
    private float stayInWeaponTimer = .5f;
    public Collider[] hitCollider;
    void Start()
    {
        parent = GetComponentInParent<Weapon>();
        parent.weaponTimer = parent.timeToDestroyWeapon;
        rotationSpeed = 8;
        _tween = parent.transform.DORotate(new Vector3(0, 360, 0), rotationSpeed, RotateMode.FastBeyond360).SetLoops(-1).SetEase(Ease.Linear);
        _tween.Play();
    }

    void Update()
    {
        if (parent.weaponTimer <= 0 && !GameManager.Instance.isGameOver && !hasBeenPickedUp)
        {
            DestroyWeapon();
        }
        if (parent.weaponTimer >= 0)
        {
            parent.weaponTimer -= Time.deltaTime;
        }
        PickUpWeapon();
        SphereAroundWeapon();
    }
    public void SphereAroundWeapon()
    {
        hitCollider = Physics.OverlapSphere(new Vector3(transform.position.x, 1.5f, transform.position.z), 2f);
        foreach (Collider hitcolliders in hitCollider)
        {
           if(hitcolliders.CompareTag(TagControl.PlayerProjectile) || hitcolliders.CompareTag(TagControl.AIProjectile))
            {
                if (stayInWeaponTimer >= 0 && hitcolliders.GetComponent<Rigidbody>().velocity == Vector3.zero)
                {
                    stayInWeaponTimer -= Time.deltaTime;
                }
            }
        }
    }
    private void PickUpWeapon()
    {
        if(stayInWeaponTimer <= 0)
        {
            hasBeenPickedUp = true;
            DestroyWeapon();
        }
    }

    public void DestroyWeapon()
    {
        if(parent.gameObject != null)
        {
            _tween.Kill(parent.gameObject);
            DeployWeapons.Instance.currentAmountOfWeapons--;
            if(rlDisabler != null)
                rlDisabler.DisableRocketHead();
            parent.weaponTimer = parent.timeToDestroyWeapon;
            Destroy(parent.gameObject);
        }    
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(new Vector3(transform.position.x, 1.5f, transform.position.z), 2);
    }
#endif
}
