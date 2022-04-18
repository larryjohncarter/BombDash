using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIProjectile : MonoBehaviour
{
    public int amountOfSoldiers;
    private Rigidbody rb;
    private bool spawnedSoldiers = false;
    private bool allowSpawningSoldiers;
    private string weaponType;
    private WeaponTrigger weaponTrigger;
    private Vector3 _scaleFactor = Vector3.one;
    [SerializeField] private Soldier _prefabSoldier;
    [SerializeField] private SoldierTypeConfig[] _configs;
    [SerializeField] private SoldierInfo[] _infos;
    private Soldier enemySoldier;
    private bool canDestroySoldier = true;
    private Animator anim;
    public List<MeshRenderer> childMR;
    public bool spawnOnce = true;
    GameObject go;
    [SerializeField] private GameObject projectileChild;
    private BoxCollider bc;
    public AreaPlacer areaPlacer;
    private Collider[] hitCollider;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        bc = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        StartCoroutine(SpawnNewProjectile());
        PickUpWeapon();

    }
    private void SpawnSoldiers()
    {
		//amountOfSoldiers = Random.Range(2, 7);
		amountOfSoldiers = Random.Range(2, 5);
		if (rb.velocity == Vector3.zero && weaponType != null && !spawnedSoldiers && allowSpawningSoldiers)
        {
            for (int i = 0; i < amountOfSoldiers; i++)
            {
                if (weaponType == "Gun")
                {
                    StartCoroutine(InstantiateSoldiers(i, 0));
                }
                if (weaponType == "RocketLauncher")
                {
                    StartCoroutine(InstantiateSoldiers(i, 1));
                }
                //if (weaponType == "Spear")
                //{
                //}
                if (weaponType == "FlameThrower")
                {
                    StartCoroutine(InstantiateSoldiers(i, 2));
                }
                if (weaponType == "NinjaStar")
                {
                    StartCoroutine(InstantiateSoldiers(i, 3));
                }
            }
            if (weaponTrigger != null)
                weaponTrigger.DestroyWeapon();
        } // Below is if projectile has not landed on a weapon
        if (rb.velocity == Vector3.zero && weaponType == null && allowSpawningSoldiers && !spawnedSoldiers)
        {
            for (int i = 0; i < amountOfSoldiers; i++)
            {
                StartCoroutine(InstantiateSoldiers(i,4));
            }
            spawnedSoldiers = true;
        }
    }
    private IEnumerator InstantiateSoldiers(int posSold, int soldierConfig)
    {
        var pos = new Vector3(transform.GetChild(posSold).position.x, 1.15f, transform.GetChild(posSold).position.z);
        var soldier = PoolManager.Instance.Soldiers.Pop();
        soldier.Setup(pos, Random.rotation, _scaleFactor, _infos[0], _configs[soldierConfig], PoolManager.Instance.DeadParticules);
        if (soldier != null)
        {
            if (!areaPlacer._targets.ContainsKey(soldier.Team))
                areaPlacer._targets[soldier.Team] = new List<ITarget>();
            soldier.UnBindHandler += areaPlacer.UnBindTarget;
			soldier.SetTarget(null);
            areaPlacer._targets[soldier.Team].Add(soldier);
        }
		soldier.Area = areaPlacer;
		// mr.enabled = false;
		bc.enabled = false;
        soldier.gameObject.SetActive(true);
        soldier.tag = TagControl.Enemy;
        yield return new WaitForSeconds(.2f);
        soldier.CanTargeted = true;
        StartCoroutine(DestroyProjectile());
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(TagControl.Ground))
        {
            canDestroySoldier = false;
        }
        if (collision.collider.CompareTag(TagControl.Player) && !collision.collider.CompareTag(TagControl.PlayerProjectile))
        {
            if (collision.gameObject.GetComponent<Soldier>().Team == Team.Alliance && canDestroySoldier)
            {
                enemySoldier = collision.gameObject.GetComponent<Soldier>();
              //  enemySoldier.SelfDemolish();
                enemySoldier.TakeDamage(10);

            }
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        allowSpawningSoldiers = true;
    }
    private void OnTriggerEnter(Collider other)
    {

        //if (other.CompareTag(TagControl.Weapon))
        //{
        //    weaponType = other.gameObject.GetComponentInParent<Weapon>().Type.ToString();
        //    weaponTrigger = other.gameObject.GetComponent<WeaponTrigger>();
        //}
    }
    private void PickUpWeapon()
    {
        hitCollider = Physics.OverlapSphere(new Vector3(transform.position.x, 1.5f, transform.position.z), 2f);
        foreach (Collider hitcolliders in hitCollider)
        {
            if (hitcolliders.CompareTag(TagControl.Weapon) && hitcolliders.GetComponent<WeaponTrigger>().hasBeenPickedUp)
            {
                weaponType = hitcolliders.gameObject.GetComponentInParent<Weapon>().Type.ToString();
                weaponTrigger = hitcolliders.gameObject.GetComponent<WeaponTrigger>();
            }
        }
    }
    private IEnumerator DestroyProjectile()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
    private IEnumerator SpawnNewProjectile()
    {
        if (rb.velocity == Vector3.zero && allowSpawningSoldiers)
        {
            for (int i = 0; i < childMR.Count; i++)
            {
                childMR[i].enabled = false;
            }
            if (spawnOnce)
            {
                go = Instantiate(projectileChild);
                go.transform.position = new Vector3(transform.position.x, .92f, transform.position.z);
                anim = go.GetComponent<Animator>();
                spawnOnce = false;
            }
            yield return new WaitForSeconds(.2f);
            anim.enabled = true;
            yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length / 2);
            SpawnSoldiers();
            yield return new WaitForSeconds(.5f);
          //  Destroy(go);
            go.SetActive(false);

        }
    }
    public void ArenaFinder(AreaPlacer AreaPlace)
    {
        areaPlacer = null;
        areaPlacer = AreaPlace;
        //if(areaPlacer == null)
        //    Debug.LogError($"Areaplace is empty");
    }
    private void OnDestroy()
    {
        areaPlacer = null;
    }
}
