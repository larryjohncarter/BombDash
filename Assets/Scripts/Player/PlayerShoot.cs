using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class PlayerShoot : MonoBehaviour
{
    ProjectileThrow projectileThrow;
    TouchState info;
    private Vector3 markPos;
    public GameObject projectile;
    public float timer;
    [Tooltip("Cooldown is the timer to countdown how many secs before shooting")]
    public int coolDown;
    public GameObject shootMgr;
    public float rotSpeed;
    public GameObject projectileSpawn;
    public GameObject projectileClone;
    private bool shouldSpawnProjectile = true;
    public Transform catapult;
    public Animator catapultAnim;
    public Image cooldownImage;
    public GameObject aimer;
    void Start()
    {
        projectileThrow = shootMgr.GetComponent<ProjectileThrow>();
        timer = 0;
    }

    void Update()
    {
        if (timer <= 0 && !GameManager.Instance.isGameOver /*&& GameManager.Instance.gameStarted*/)
        {
           // SpawnProjectile();
            PlayerShootProjectile();
        }
        if (GameManager.Instance.isGameOver)
        {
            shootMgr.GetComponent<LineRenderer>().enabled = false;
            aimer.SetActive(false);
        }

        TimerDecrement();
    }
    private void TimerDecrement()
    {
        if (timer <= coolDown && timer >= 0 && GameManager.Instance.gameStarted)
        {
            timer -= Time.deltaTime;
           // cooldownImage.fillAmount += timer * Time.deltaTime / timer;
            var percent = timer / coolDown;
            cooldownImage.fillAmount = Mathf.Lerp(1, 0, percent);
            if (cooldownImage.fillAmount >= 1 && timer <= 0)
            {
                cooldownImage.fillAmount = 0;
            }
        }
    }
    void PlayerShootProjectile()
    {
        Vector3 touchPos;
        info = TouchControl.GetTouchState();
        switch (info)
        {
            case TouchState.Start:
                touchPos = TouchControl.GetTouchPosition();
                CheckHitPosition(touchPos);
                aimer.SetActive(true);
                break;

            case TouchState.Moved:
                touchPos = TouchControl.GetTouchPosition();
                CheckHitPosition(touchPos);
                if(!aimer.activeSelf)
                    aimer.SetActive(true);
                if (catapult.rotation.y >= -45 && catapult.rotation.y <= 45 )
                    catapult.DORotate(new Vector3(0, -markPos.x) * 2, .5f);
                break;
            case TouchState.Ended:
                touchPos = TouchControl.GetTouchPosition();
                aimer.SetActive(false);
                catapultAnim.SetBool("Shoot", true);
                StartCoroutine(AnimatorBoolSetter());
                StartCoroutine(ShootObject(touchPos));
                //projectileThrow.ShootObject(projectileClone, touchPos);
                timer = coolDown;
                shouldSpawnProjectile = true;
                if (!GameManager.Instance.gameStarted)
                    LevelManager.Instance.StartGame();
                break;
            default:
                break;
        }
    }
    private IEnumerator AnimatorBoolSetter()
    {
        yield return new WaitForSeconds(.8f);
        catapultAnim.SetBool("Shoot", false);
    }
    private IEnumerator ShootObject(Vector3 touchPos)
    {
        yield return new WaitForSeconds(.3f);
        projectileThrow.ShootObject(projectileClone, touchPos);
        shootMgr.GetComponent<LineRenderer>().enabled = false;
    }
    void CheckHitPosition(Vector3 pos)
    {
        int layerMask = -1;
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(pos);

        if (Physics.Raycast(ray, out hit, 100f, layerMask))
        {
            markPos = Camera.main.ScreenToWorldPoint(new Vector3(pos.x, pos.y, hit.distance));
        }
        projectileThrow.CheckVector(markPos);
    } 
    public void SpawnProjectile()
    {
        if (shouldSpawnProjectile)
        {
            projectileClone = Instantiate(projectile);
            projectileClone.transform.position =  projectileSpawn.transform.position;
            Rigidbody rig = projectileClone.GetComponent<Rigidbody>();
            rig.isKinematic = true;
            rig.useGravity = false;
            shouldSpawnProjectile = false;
        }
    }
}
