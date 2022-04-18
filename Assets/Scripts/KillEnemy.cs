using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillEnemy : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
       if(other.CompareTag(TagControl.Enemy) || other.CompareTag(TagControl.Player))
        {
            other.GetComponent<Soldier>().TakeDamage(10);
            Debug.Log($"This GameObject: {gameObject.name} Has Killed the enemy. Location of other gameobject: {other.transform.position}");
        }
    }
}
