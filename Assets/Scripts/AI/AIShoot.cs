using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIShoot : MonoBehaviour
{
    [Header("Random AI SIDE Position")]
    public float AISideMinX;
    public float AISideMaxX;
    public float AISideMinZ;
    public float AISideMaxZ;
    [Header("Random Player Side Position")]
    public float playerSideMinX;
    public float playerSideMaxX;
    public float playerSideMinZ;
    public float playerSideMaxZ;
    [Header("Variables")]
    public GameObject projectile;
    private float randomX;
    private float randomZ;
    AIProjectileThrow projectileThrow;
    public float coolddown;
    public GameObject shootMgr;
    public bool ShootProjectile;
    public float timer;
    public Animator catapultAnim;
    private bool firstThrow = true;
    private int random;
    private bool aimAtWeapon;
    private bool aimAtPlayerSide;
    [SerializeField] private AreaPlacer _playerSideArea;
    [Tooltip("If the amount of the soldiers in player side surpasses this amount, AI will prioritize player side until the amount is low than this int")]
    public int amountOfSoldiersInPlayerSide;
    [Header("List")]
    public List<Weapon> AvailableWeapons;


    void Start()
    {
        projectileThrow = shootMgr.GetComponent<AIProjectileThrow>();
        coolddown = 3;
    }

    void Update()
    {
        CheckIfThereAreWeaponsAvailable();
        PrioritizePlayerSize();
        if (coolddown <= 0 && !GameManager.Instance.isGameOver && GameManager.Instance.gameStarted)
        {
            StartCoroutine(ShowLine());
            AIShooting();
        }
        if (ShootProjectile)
        {
            StartCoroutine(ShowLine());
            AIShooting();
            ShootProjectile = false;
        }
        if (coolddown <= timer && coolddown >= 0 && GameManager.Instance.gameStarted)
        {
            coolddown -= Time.deltaTime;
        }
    }
    void AIShooting()
    {
        if (AvailableWeapons.Count > 0 && aimAtWeapon && !firstThrow && !aimAtPlayerSide)
        { // Aim at weapon if available
            random = Random.Range(0, AvailableWeapons.Count);
            projectileThrow.ShootObject(projectile, AvailableWeapons[random].transform.position);
        } else if (!aimAtWeapon && !aimAtPlayerSide)
        { // Aim at a random location
            TargetSelection();
            projectileThrow.ShootObject(projectile, new Vector3(randomX, 1.5f, randomZ));
        } else if (aimAtPlayerSide)
        { // prioritize player side if the soldiers have gone more than amountOfSoldiersInPlayerSide
            randomX = Random.Range(playerSideMinX, playerSideMaxX);
            randomZ = Random.Range(playerSideMinZ, playerSideMaxZ);
            projectileThrow.ShootObject(projectile, new Vector3(randomX, 1.5f, randomZ));
        }
        catapultAnim.SetBool("Shoot", true);
        StartCoroutine(AnimatorBoolSetter());
        coolddown = timer;
        RandomChanceOfWeaponAim();
    }
    private void TargetSelection()
    {
        int randomSide;
        if (firstThrow)
        {
            randomX = Random.Range(AISideMinX, AISideMaxX);
            randomZ = Random.Range(AISideMinZ, AISideMaxZ);
            firstThrow = false;
        } else if (!firstThrow)
        {
            randomSide = Random.Range(0, 11);
            if (randomSide > 4f) // AI chooses it's own side
            {
                randomX = Random.Range(AISideMinX, AISideMaxX);
                randomZ = Random.Range(AISideMinZ, AISideMaxZ);
            }
            else if (randomSide < 3f)// AI chooses player side
            {
                randomX = Random.Range(playerSideMinX, playerSideMaxX);
                randomZ = Random.Range(playerSideMinZ, playerSideMaxZ);
            }
        }
    }
    private IEnumerator AnimatorBoolSetter()
    {
        yield return new WaitForSeconds(.8f);
        catapultAnim.SetBool("Shoot", false);
    }
    private IEnumerator ShowLine()
    {
        shootMgr.GetComponent<LineRenderer>().enabled = true;
        yield return new WaitForSeconds(2f);
        shootMgr.GetComponent<LineRenderer>().enabled = false;
    }
    private void CheckIfThereAreWeaponsAvailable()
    {
        GameObject[] go = GameObject.FindGameObjectsWithTag(TagControl.Weapon);
        for (int i = 0; i < go.Length; i++)
        {
            if (!AvailableWeapons.Contains(go[i].gameObject.GetComponent<Weapon>()))
                AvailableWeapons.Add(go[i].gameObject.GetComponent<Weapon>());
        }
        CheckIfWeaponIsGone();
    }
    private void CheckIfWeaponIsGone()
    {
        for (int i = AvailableWeapons.Count - 1; i > -1 ; i--)
        {
            if(AvailableWeapons[i] == null)
            {
                AvailableWeapons.RemoveAt(i);
            }
        }
    }
    private void RandomChanceOfWeaponAim()
    {
        float random = Random.value;
        if(random > .85f)
        {
            aimAtWeapon = true;
        }
        else
        {
            aimAtWeapon = false;
        }
    }
    private void PrioritizePlayerSize()
    {
        int randomChance = Random.Range(0, 10);
        if (_playerSideArea.AllianceCount >= amountOfSoldiersInPlayerSide && randomChance > 7)
        {
            aimAtPlayerSide = true;
        }
        else
            aimAtPlayerSide = false;
    }
}
