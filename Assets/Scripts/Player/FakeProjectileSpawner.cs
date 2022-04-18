using UnityEngine;
public class FakeProjectileSpawner : MonoBehaviour
{

    public GameObject fakeProjectile;
    public Transform fakeProjectileSpawn;
    public Transform catapultArm;
    public void SpawnFakeProjectile()
    {
        fakeProjectile.SetActive(true);
        fakeProjectile.transform.position = fakeProjectileSpawn.position;
        fakeProjectile.transform.parent = catapultArm;
    }

    public void DeactivateFakeProjectile()
    {
        fakeProjectile.SetActive(false);
    }
    public void FollowCatapultArm()
    {
        fakeProjectile.transform.position = fakeProjectileSpawn.position;
    }
}
