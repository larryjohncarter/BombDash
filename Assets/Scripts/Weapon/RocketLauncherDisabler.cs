using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLauncherDisabler : MonoBehaviour
{

    private MeshCollider mc;
    private MeshRenderer mr;
    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        mc = GetComponent<MeshCollider>();
    }

    public void DisableRocketHead()
    {
        mc.enabled = false;
        mr.enabled = false;
    }
}
