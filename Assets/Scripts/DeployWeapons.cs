using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class DeployWeapons : SingletonBehaviour<DeployWeapons>
{
    public List<GameObject> weaponList;
    public int maxAmountOfWeaponsInArena;
    public int currentAmountOfWeapons;
    public float playerSideMinX, playerSideMaxX, playerSideMinZ, playerSideMaxZ;
    public float AISideMinX, AISideMaxX, AISideMinZ, AISideMaxZ;
    public float cooldown = 0;
    float randomX;
    float randomZ;
    [Tooltip("Shows how long before the first weapon shows")]
    public float timeToSpawn;
    private bool afterFirstSpawn = false;
    public Collider[] hitCollider;
    int maxSpawnAttempts = 10;
    private void Start()
    {
       StartCoroutine(WaitUntilSpawnWeapon());
    }
    void Update()
    {
        SpawnWeapon();
        CountDown();
    }
    public void PickPosition()
    {
        if(currentAmountOfWeapons < maxAmountOfWeaponsInArena)
        {
            //int sidePick = 5;//Random.Range(1,11);
            //if (sidePick <= 6)
            //{
            //    randomX = Random.Range(playerSideMinX, playerSideMaxX);
            //    randomZ = Random.Range(playerSideMinZ, playerSideMaxZ);
            //}
            //else if (sidePick >= 7)
            //{
            //    randomX = Random.Range(AISideMinX, AISideMaxX);
            //    randomZ = Random.Range(AISideMinZ, AISideMaxZ);
            //}
            WeaponSpawn();
        }
    } 
    public void WeaponSpawn()
    {
        bool validPosition = false;
        int spawnAttempts = 0;
        int randomWeaponSelection = Random.Range(0, weaponList.Count);

        while (!validPosition && spawnAttempts < maxSpawnAttempts && GameManager.Instance.gameStarted)
        {
            int sidePick = Random.Range(1,11);
            if (sidePick <= 6)
            {
                randomX = Random.Range(playerSideMinX, playerSideMaxX);
                randomZ = Random.Range(playerSideMinZ, playerSideMaxZ);
            }
            else if (sidePick >= 7)
            {
                randomX = Random.Range(AISideMinX, AISideMaxX);
                randomZ = Random.Range(AISideMinZ, AISideMaxZ);
            }
            spawnAttempts++;
            validPosition = true;

            hitCollider = Physics.OverlapSphere(new Vector3(randomX, 1.5f, randomZ),2f);
            foreach (Collider hitcollider in hitCollider)
            {
                if (hitcollider.CompareTag(TagControl.Weapon) || hitcollider.CompareTag(TagControl.Enemy) || hitcollider.CompareTag(TagControl.Player))
                {
                    validPosition = false;
                    //cooldown = -1;
                   // Debug.Log($"Weapon Cannot Be Spawned!");
                }
            }

            if (validPosition)
            {
                GameObject go = Instantiate(weaponList[randomWeaponSelection]);
                go.transform.position = new Vector3(randomX, 1.5f, randomZ);
                currentAmountOfWeapons++;
            }
        }
    }

    private IEnumerator WaitUntilSpawnWeapon()
    {
        yield return new WaitForSeconds(timeToSpawn);
        PickPosition();
        cooldown = 5;
        afterFirstSpawn = true;
    }
    private void SpawnWeapon()
    {
        if (cooldown <= 0 && afterFirstSpawn && !GameManager.Instance.isGameOver)
        {
            PickPosition();
            cooldown = 5;
        }
    }
    private void CountDown()
    {
        if (currentAmountOfWeapons < maxAmountOfWeaponsInArena && cooldown >= 0 )
            cooldown -= Time.deltaTime;
    }

#if UNITY_EDITOR
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(new Vector3(randomX, 1.5f, randomZ), 2);
    //}
#endif
}
