using UnityEngine;

public class ProjectileKiller : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TagControl.Player) || other.CompareTag(TagControl.Enemy))
        {
            Destroy(other.gameObject);
        }
    }
}
