using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_vfxDestroy : MonoBehaviour
{
    public ParticleSystem sys;

    void Start()
    {
        if(sys==null)
            Destroy(this.gameObject, this.GetComponentInChildren<ParticleSystem>().main.duration);
        else
        {
            Destroy(this.gameObject, sys.main.duration);
        }
    }
}
